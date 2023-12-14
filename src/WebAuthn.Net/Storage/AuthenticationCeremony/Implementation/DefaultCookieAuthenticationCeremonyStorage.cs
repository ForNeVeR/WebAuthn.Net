﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using WebAuthn.Net.Models.Abstractions;
using WebAuthn.Net.Services.Serialization.Json;
using WebAuthn.Net.Services.Static;
using WebAuthn.Net.Storage.AuthenticationCeremony.Implementation.Models;
using WebAuthn.Net.Storage.AuthenticationCeremony.Models;

namespace WebAuthn.Net.Storage.AuthenticationCeremony.Implementation;

/// <summary>
///     Default implementation of <see cref="IAuthenticationCeremonyStorage{TContext}" />.
/// </summary>
/// <typeparam name="TContext">The type of context in which the WebAuthn operation will be performed.</typeparam>
public class DefaultCookieAuthenticationCeremonyStorage<TContext> : IAuthenticationCeremonyStorage<TContext>
    where TContext : class, IWebAuthnContext
{
    /// <summary>
    ///     The purpose of the Data Protector.
    /// </summary>
    public const string DataProtectionPurpose = "WebAuthn.Net.DefaultCookieAuthenticationCeremonyStorage";

    /// <summary>
    ///     Constructs <see cref="DefaultCookieAuthenticationCeremonyStorage{TContext}" />.
    /// </summary>
    /// <param name="options">Accessor for getting the current value of the default authentication ceremony storage options.</param>
    /// <param name="provider">Provider for creating IDataProtectors.</param>
    /// <param name="safeJsonSerializer">Safe (exceptionless) JSON serializer.</param>
    /// <exception cref="ArgumentNullException">Any of the parameters is <see langword="null" /></exception>
    public DefaultCookieAuthenticationCeremonyStorage(
        IOptionsMonitor<DefaultCookieAuthenticationCeremonyStorageOptions> options,
        IDataProtectionProvider provider,
        ISafeJsonSerializer safeJsonSerializer)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(safeJsonSerializer);
        Options = options;
        Protector = provider.CreateProtector(DataProtectionPurpose, "v1");
        CookieManager = new ChunkingCookieManager();
        SafeJsonSerializer = safeJsonSerializer;
    }

    /// <summary>
    ///     Accessor for getting the current value of the default authentication ceremony storage options.
    /// </summary>
    protected IOptionsMonitor<DefaultCookieAuthenticationCeremonyStorageOptions> Options { get; }

    /// <summary>
    ///     Protector for encrypting and decrypting sensitive data.
    /// </summary>
    protected IDataProtector Protector { get; }

    /// <summary>
    ///     Manager for working with Cookies, abstracting away from direct interaction with the Cookie API.
    /// </summary>
    protected ICookieManager CookieManager { get; }

    /// <summary>
    ///     Safe (exceptionless) JSON serializer.
    /// </summary>
    protected ISafeJsonSerializer SafeJsonSerializer { get; }

    /// <inheritdoc />
    public virtual Task<string> SaveAsync(
        TContext context,
        AuthenticationCeremonyParameters authenticationCeremony,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        var options = Options.CurrentValue;
        var id = Guid.NewGuid().ToString("N").ToLowerInvariant();
        var container = new AuthenticationCeremonyParametersCookieContainer(id, authenticationCeremony);
        var jsonBytesResult = SafeJsonSerializer.SerializeToUtf8Bytes(container, options.SerializerOptions);
        if (jsonBytesResult.HasError)
        {
            throw new InvalidOperationException($"Failed to serialize {nameof(AuthenticationCeremonyParametersCookieContainer)} into json");
        }

        var jsonBytes = jsonBytesResult.Ok;
        var protectedJsonBytes = Protector.Protect(jsonBytes);
        var encodedProtectedJsonBytes = Base64Url.Encode(protectedJsonBytes);
        var cookieOptions = options.Cookie.Build(context.HttpContext);
        var cookieName = GetCookieName(options);
        CookieManager.AppendResponseCookie(
            context.HttpContext,
            cookieName,
            encodedProtectedJsonBytes,
            cookieOptions);
        return Task.FromResult(id);
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public Task<AuthenticationCeremonyParameters?> FindAsync(
        TContext context,
        string authenticationCeremonyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        var options = Options.CurrentValue;
        var cookieName = GetCookieName(options);
        try
        {
            var id = new Guid(authenticationCeremonyId).ToString("N").ToLowerInvariant();
            var encodedProtectedJsonBytes = CookieManager.GetRequestCookie(context.HttpContext, cookieName);
            if (encodedProtectedJsonBytes is null)
            {
                return Task.FromResult((AuthenticationCeremonyParameters?) null);
            }

            if (!Base64Url.TryDecode(encodedProtectedJsonBytes, out var protectedJsonBytes))
            {
                return Task.FromResult((AuthenticationCeremonyParameters?) null);
            }

            var jsonBytes = Protector.Unprotect(protectedJsonBytes);
            var containerResult = SafeJsonSerializer.DeserializeNonNullable<AuthenticationCeremonyParametersCookieContainer>(jsonBytes, options.SerializerOptions);
            if (containerResult.HasError)
            {
                return Task.FromResult((AuthenticationCeremonyParameters?) null);
            }

            var container = containerResult.Ok;
            if (container is not null && container.Id == id)
            {
                return Task.FromResult<AuthenticationCeremonyParameters?>(container.AuthenticationCeremonyParameters);
            }

            return Task.FromResult((AuthenticationCeremonyParameters?) null);
        }
        catch (Exception)
        {
            return Task.FromResult((AuthenticationCeremonyParameters?) null);
        }
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public Task RemoveAsync(
        TContext context,
        string authenticationCeremonyId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        var options = Options.CurrentValue;
        var cookieOptions = options.Cookie.Build(context.HttpContext);
        var cookieName = GetCookieName(options);
        try
        {
            var id = new Guid(authenticationCeremonyId).ToString("N").ToLowerInvariant();
            var encodedProtectedJsonBytes = CookieManager.GetRequestCookie(context.HttpContext, cookieName);
            if (encodedProtectedJsonBytes is null)
            {
                return Task.CompletedTask;
            }

            if (!Base64Url.TryDecode(encodedProtectedJsonBytes, out var protectedJsonBytes))
            {
                return Task.FromResult((AuthenticationCeremonyParameters?) null);
            }

            var jsonBytes = Protector.Unprotect(protectedJsonBytes);
            var containerResult = SafeJsonSerializer.DeserializeNonNullable<AuthenticationCeremonyParametersCookieContainer>(jsonBytes, options.SerializerOptions);
            if (containerResult.HasError)
            {
                return Task.FromResult((AuthenticationCeremonyParameters?) null);
            }

            var container = containerResult.Ok;
            if (container is not null && container.Id == id)
            {
                CookieManager.DeleteCookie(context.HttpContext, cookieName, cookieOptions);
            }

            return Task.CompletedTask;
        }
        catch (Exception)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    ///     Returns the name of the cookie that is used to store the authentication ceremony data.
    /// </summary>
    /// <param name="options">Options for the default authentication ceremony store that operates on cookies.</param>
    /// <returns>The name of the cookie used for storing the authentication ceremony data.</returns>
    protected virtual string GetCookieName(DefaultCookieAuthenticationCeremonyStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Cookie.Name ?? DefaultCookieAuthenticationCeremonyStorageOptions.CookieName;
    }
}
