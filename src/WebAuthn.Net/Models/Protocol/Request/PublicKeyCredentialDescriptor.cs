﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using WebAuthn.Net.Models.Protocol.Enums;

namespace WebAuthn.Net.Models.Protocol.Request;

/// <summary>
///     This object contains the attributes that are specified by a caller when referring to a <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a>
///     as an input parameter to the <a href="https://www.w3.org/TR/credential-management-1/#dom-credentialscontainer-create">create()</a> or <a href="https://www.w3.org/TR/credential-management-1/#dom-credentialscontainer-get">get()</a> methods.
///     It mirrors the fields of the PublicKeyCredential object returned by the latter methods.
/// </summary>
/// <remarks>
///     <a href="https://www.w3.org/TR/webauthn-3/#dictdef-publickeycredentialdescriptor">Web Authentication: An API for accessing Public Key Credentials Level 3 - § 5.8.3. Credential Descriptor</a>
/// </remarks>
public class PublicKeyCredentialDescriptor
{
    /// <summary>
    ///     Constructs <see cref="PublicKeyCredentialDescriptor" />.
    /// </summary>
    /// <param name="type">
    ///     This member contains the type of the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a> the caller is referring to.
    ///     The value should be a member of <see cref="PublicKeyCredentialType" /> but client platforms must ignore any <see cref="PublicKeyCredentialDescriptor" /> with an unknown type.
    /// </param>
    /// <param name="id">
    ///     This member contains the <a href="https://www.w3.org/TR/webauthn-3/#credential-id">credential ID</a>
    ///     of the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a> the caller is referring to.
    /// </param>
    /// <param name="transports">
    ///     This optional member contains a hint as to how the <a href="https://www.w3.org/TR/webauthn-3/#client">client</a>
    ///     might communicate with the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential-source-managing-authenticator">managing authenticator</a>
    ///     of the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a> the caller is referring to.
    ///     The values should be members of <see cref="AuthenticatorTransport" /> but client platforms must ignore unknown values.
    /// </param>
    /// <exception cref="ArgumentException">If the parameter <paramref name="type" /> contains an invalid value or if the <paramref name="transports" /> array contains an invalid value.</exception>
    /// <exception cref="ArgumentNullException">If the parameter <paramref name="id" /> is equal to <see langword="null" />.</exception>
    [JsonConstructor]
    public PublicKeyCredentialDescriptor(
        PublicKeyCredentialType type,
        byte[] id,
        AuthenticatorTransport[]? transports)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentException("Incorrect value", nameof(type));
        }

        Type = type;

        ArgumentNullException.ThrowIfNull(id);
        Id = id;

        if (transports?.Length > 0)
        {
            if (transports.Any(static x => !Enum.IsDefined(x)))
            {
                throw new ArgumentException($"One or more objects contained in the {nameof(transports)} enumeration contain an invalid value.", nameof(transports));
            }

            Transports = transports;
        }
    }

    /// <summary>
    ///     This member contains the type of the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a> the caller is referring to.
    ///     The value should be a member of <see cref="PublicKeyCredentialType" /> but client platforms must ignore any <see cref="PublicKeyCredentialDescriptor" /> with an unknown type.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public PublicKeyCredentialType Type { get; }

    /// <summary>
    ///     This member contains the <a href="https://www.w3.org/TR/webauthn-3/#credential-id">credential ID</a>
    ///     of the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a> the caller is referring to.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public byte[] Id { get; }

    /// <summary>
    ///     This optional member contains a hint as to how the <a href="https://www.w3.org/TR/webauthn-3/#client">client</a>
    ///     might communicate with the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential-source-managing-authenticator">managing authenticator</a>
    ///     of the <a href="https://www.w3.org/TR/webauthn-3/#public-key-credential">public key credential</a> the caller is referring to.
    ///     The values should be members of <see cref="AuthenticatorTransport" /> but client platforms must ignore unknown values.
    /// </summary>
    /// <remarks>
    ///     The <a href="https://www.w3.org/TR/webauthn-3/#dom-authenticatorattestationresponse-gettransports">getTransports()</a> operation can provide suitable values for this member.
    ///     When <a href="https://www.w3.org/TR/webauthn-3/#sctn-registering-a-new-credential">registering a new credential</a>, the <a href="https://www.w3.org/TR/webauthn-3/#relying-party">Relying Party</a>
    ///     should store the value returned from <a href="https://www.w3.org/TR/webauthn-3/#dom-authenticatorattestationresponse-gettransports">getTransports()</a>.
    ///     When creating a <see cref="PublicKeyCredentialDescriptor" /> for that credential, the <a href="https://www.w3.org/TR/webauthn-3/#relying-party">Relying Party</a> \
    ///     should retrieve that stored value and set it as the value of the transports member.
    /// </remarks>
    [JsonPropertyName("transports")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AuthenticatorTransport[]? Transports { get; }
}
