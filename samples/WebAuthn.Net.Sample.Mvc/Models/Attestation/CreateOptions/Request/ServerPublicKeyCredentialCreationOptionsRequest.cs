﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Sample.Mvc.Constants;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.Serialization.Json;

namespace WebAuthn.Net.Sample.Mvc.Models.Attestation.CreateOptions.Request;

public class ServerPublicKeyCredentialCreationOptionsRequest
{
    [JsonConstructor]
    public ServerPublicKeyCredentialCreationOptionsRequest(
        string userName,
        Dictionary<string, JsonElement>? extensions)
    {
        UserName = userName;
        Extensions = extensions;
    }


    [JsonPropertyName("username")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public string UserName { get; }

    [JsonPropertyName("extensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, JsonElement>? Extensions { get; }

    public BeginRegistrationCeremonyRequest ToBeginCeremonyRequest()
    {
        return new(
            null,
            null,
            ExampleConstants.Host.WebAuthnDisplayName,
            new(UserName, WebEncoders.Base64UrlDecode(UserName), UserName),
            16,
            CoseAlgorithms.All,
            120000,
            RegistrationCeremonyExcludeCredentials.AllExisting(),
            null,
            null,
            null,
            null,
            Extensions);
    }
}
