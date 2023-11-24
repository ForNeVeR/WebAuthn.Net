# WebAuthn.Net

Working implementation of the [WebAuthn Level 3](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/) standard for .NET 6 and 8

## Purpose

Our goal is to create an easy-to-use, expandable, production-ready implementation of the [WebAuthn Relying Party](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#webauthn-relying-party) library for the [latest version of the WebAuthn standard](https://www.w3.org/standards/history/webauthn-3/), which passes the [FIDO conformance test](https://fidoalliance.org/certification/functional-certification/conformance/), and is adapted for [LTS versions of .NET](https://dotnet.microsoft.com/en-us/platform/support/policy) (6 and 8 at the moment).

## Supported features

- ✅ [Passkeys out of the box](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#passkey)
- ✅ Attestation API & verification (Register and verify credentials/authenticators)
- ✅ Assertion API & verification (Authenticate users)
- ✅ 100% completion of the entire [FIDO Conformance Test](https://fidoalliance.org/certification/functional-certification/conformance/), including all optional features
- ✅ Authenticators embedded in the device (FaceID, TouchID, Windows Hello)
- ✅ Roaming aka cross-platform authenticators (USB/NFC/BLE keys, for example Yubico)
- ✅ All current attestation statement formats
    - [packed](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-packed-attestation)
    - [tpm](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-tpm-attestation)
    - [android-key](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-android-key-attestation)
    - [android-safetynet](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-android-safetynet-attestation)
    - [fido-u2f](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-fido-u2f-attestation)
    - [none](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-none-attestation)
    - [apple](https://www.w3.org/TR/2023/WD-webauthn-3-20230927/#sctn-apple-anonymous-attestation)
- ✅ All cryptographic algorithms required to pass the [FIDO Conformance Test](https://fidoalliance.org/certification/functional-certification/conformance/)
    - [RS1](https://www.rfc-editor.org/rfc/rfc8812.html#section-2)
    - [RS256](https://www.rfc-editor.org/rfc/rfc8812.html#section-2)
    - [RS384](https://www.rfc-editor.org/rfc/rfc8812.html#section-2)
    - [RS512](https://www.rfc-editor.org/rfc/rfc8812.html#section-2)
    - [PS256](https://www.rfc-editor.org/rfc/rfc8230.html#section-2)
    - [PS384](https://www.rfc-editor.org/rfc/rfc8230.html#section-2)
    - [PS512](https://www.rfc-editor.org/rfc/rfc8230.html#section-2)
    - [ES256](https://www.rfc-editor.org/rfc/rfc9053.html#section-2.1)
    - [ES384](https://www.rfc-editor.org/rfc/rfc9053.html#section-2.1)
    - [ES512](https://www.rfc-editor.org/rfc/rfc9053.html#section-2.1)
    - [EdDSA](https://www.rfc-editor.org/rfc/rfc9053.html#section-2.2) (via [libsodium](https://github.com/jedisct1/libsodium), because .NET currently [doesn't support Ed25519](https://github.com/dotnet/runtime/issues/14741))
- ✅ Processing of each request with a single transaction
- ✅ Ready-to-use storage implementations:
    - [Microsoft SQL Server](src/WebAuthn.Net.Storage.SqlServer)
    - [PostgreSQL](src/WebAuthn.Net.Storage.PostgreSql)
    - [MySQL](src/WebAuthn.Net.Storage.MySql)
    - [InMemory](src/WebAuthn.Net.Storage.InMemory) (for sample applications)
- ✅ [Sample applications](samples)
- ✅ Exceptionless API (avoid exceptions wherever possible)
- ✅ Designed with extensibility in mind (almost any library component can be overridden)
- ✅ Intellisense documentation

## Local development

### Required dependencies

- [.NET SDK 6.0.417+](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [.NET SDK 8.0.100+](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Releasing

Create release via [Github New Release](https://github.com/dodopizza/WebAuthn.Net/releases/new).
Specify tag by semver (example: 1.3.37) and target branch (main by default) then publish.
Github Actions will trigger on new release and publish NuGet package for a release.

## Tips for Contribution

Any contributions appreciated!
If you have an idea, feature request, or you find a bug – feel free to open new issue.

There are few tips how to make your contribution even better:

- Start with issue.
  Even if you want to write code by yourself to implement some feature or fix a bug,
  please create Issue first and link it in the Pull Request.
  It allows us to better understand what problem you are trying to solve.
- When you create a Pull Request make sure triggered GitHub Actions workflow finished successfully.
- Please use English for everything :)

## Acknowledgements

WebAuthn.Net is built using the following wonderful tools:

* [.NET](https://github.com/dotnet/runtime)
* [ASP.NET Core](https://github.com/dotnet/aspnetcore)
* [FIDO2 .NET Library (WebAuthn)](https://github.com/passwordless-lib/fido2-net-lib)
* [JetBrains Rider](https://www.jetbrains.com/rider)
