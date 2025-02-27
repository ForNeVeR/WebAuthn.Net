﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAuthn.Net.Models.Protocol.Json.AuthenticationCeremony.VerifyAssertion;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.VerifyAssertion;

namespace WebAuthn.Net.Sample.Mvc.Models.Passwordless;

public class ServerPublicKeyCredential
{
    [JsonConstructor]
    public ServerPublicKeyCredential(
        string id,
        string type,
        ServerAuthenticatorAssertionResponse response,
        string username)
    {
        Id = id;
        Type = type;
        Response = response;
        Username = username;
    }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public string Id { get; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public string Type { get; }

    [JsonPropertyName("username")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public string Username { get; }


    [JsonPropertyName("response")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Required]
    public ServerAuthenticatorAssertionResponse Response { get; }

    public CompleteAuthenticationCeremonyRequest ToCompleteCeremonyRequest(string authenticationCeremonyId)
    {
        var result = ToAuthenticationResponseJson();
        return new(authenticationCeremonyId, result);
    }

    private AuthenticationResponseJSON ToAuthenticationResponseJson()
    {
        var response = ParseResponse(Response);
        return new(
            Id,
            Id,
            response,
            null,
            new(),
            Type);
    }

    private static AuthenticatorAssertionResponseJSON ParseResponse(ServerAuthenticatorAssertionResponse input)
    {
        return new(
            input.ClientDataJson,
            input.AuthenticatorData,
            input.Signature,
            input.UserHandle,
            input.AttestationObject);
    }
}
