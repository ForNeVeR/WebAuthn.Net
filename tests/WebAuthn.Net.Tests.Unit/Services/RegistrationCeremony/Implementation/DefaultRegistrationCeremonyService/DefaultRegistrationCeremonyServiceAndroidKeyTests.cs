﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using NUnit.Framework;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Services.Cryptography.Cose.Models.Enums;
using WebAuthn.Net.Services.RegistrationCeremony.Implementation.DefaultRegistrationCeremonyService.Abstractions;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateOptions;

namespace WebAuthn.Net.Services.RegistrationCeremony.Implementation.DefaultRegistrationCeremonyService;

public class DefaultRegistrationCeremonyServiceAndroidKeyTests : AbstractRegistrationCeremonyServiceTests
{
    protected override Uri GetRelyingPartyAddress()
    {
        return new("https://vanbukin-pc.local", UriKind.Absolute);
    }

    [Test]
    public async Task DefaultRegistrationCeremonyService_PerformsCeremonyWithoutErrorsForAndroidKey_WhenAllAlgorithms()
    {
        TimeProvider.Change(DateTimeOffset.Parse("2023-11-02T13:29:06Z", CultureInfo.InvariantCulture));
        var userId = WebEncoders.Base64UrlDecode("AAAAAAAAAAAAAAAAAAAAAQ");
        var beginResult = await RegistrationCeremonyService.BeginCeremonyAsync(
            new DefaultHttpContext(new FeatureCollection()),
            new(
                null,
                null,
                "Test Host",
                new("testuser", userId, "Test User"),
                32,
                new[] { CoseAlgorithm.ES256 },
                60000,
                RegistrationCeremonyExcludeCredentials.AllExisting(),
                new(AuthenticatorAttachment.Platform, null, null, UserVerificationRequirement.Required),
                null,
                AttestationConveyancePreference.Direct,
                null,
                null),
            CancellationToken.None);

        RegistrationCeremonyStorage.ReplaceChallengeForRegistrationCeremonyOptions(
            beginResult.RegistrationCeremonyId,
            WebEncoders.Base64UrlDecode("8jSRetKG3xneEajEfed_vmEb9U7bXZ2yrBcvytlj-cI"));

        var competeResult = await RegistrationCeremonyService.CompleteCeremonyAsync(
            new DefaultHttpContext(new FeatureCollection()),
            new(beginResult.RegistrationCeremonyId, new(
                "OThmNjc3YmUtMDEwNy00Mzg0LWEzMGMtODczMmI5ZDFiOGI0",
                "OThmNjc3YmUtMDEwNy00Mzg0LWEzMGMtODczMmI5ZDFiOGI0",
                new(
                    "eyJjaGFsbGVuZ2UiOiI4alNSZXRLRzN4bmVFYWpFZmVkX3ZtRWI5VTdiWFoyeXJCY3Z5dGxqLWNJIiwib3JpZ2luIjoiaHR0cHM6Ly92YW5idWtpbi1wYy5sb2NhbCIsInR5cGUiOiJ3ZWJhdXRobi5jcmVhdGUifQ",
                    null,
                    null,
                    null,
                    null,
                    "o2NmbXRrYW5kcm9pZC1rZXlnYXR0U3RtdKNjYWxnJmNzaWdYRzBFAiArewfiZGdWlUEWsn0WNicpfvlg4U3OHHR9NNmCPAYO1AIhAMT8dr2gAtLrkW8quEZPbFwkR967BYJcSf9CSxrToIFFY3g1Y4RZAu8wggLrMIICkaADAgECAgEBMAoGCCqGSM49BAMCMDkxDDAKBgNVBAwMA1RFRTEpMCcGA1UEBRMgNWFjOTU5NTI1MTU0ZWE1YzhkNzdkZGMzNDFlYWYxMTgwHhcNMjMxMTAyMTMyOTA1WhcNMjMxMTAyMTMyOTM1WjBXMSYwJAYDVQQLEx1jb20uZ29vZ2xlLmF0dGVzdGF0aW9uZXhhbXBsZTEtMCsGA1UEAxMkOThmNjc3YmUtMDEwNy00Mzg0LWEzMGMtODczMmI5ZDFiOGI0MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEEQGqAhUIkFWY_oWrroIjrFNmaPSDPNNxa9kMaEfGxR7ibAKSWOPp5cfJ7Ypl0SPWGUtuRPLnf8LpoJ7XmoHk1aOCAWowggFmMA4GA1UdDwEB_wQEAwIHgDCCAVIGCisGAQQB1nkCAREEggFCMIIBPgIBZAoBAQIBZAoBAQQgOjrUR3cjXGUK28bAFpBW0xXA5IcKRjGsryfh8PIaoewEADBev4U9CAIGAYuQRdq4v4VFTgRMMEoxJDAiBB1jb20uZ29vZ2xlLmF0dGVzdGF0aW9uZXhhbXBsZQIBATEiBCAPk-Buuh3WefghMWVT5FAXot2thq78D-6GoW5Y9dHCSDCBq6EFMQMCAQKiAwIBA6MEAgIBAKUFMQMCAQSqAwIBAb-DeAMCAQO_g3kFAgMBUYC_hT4DAgEAv4VATDBKBCAHrGWksyShKyYDh1pt6rID5_YqBpdIrguWTegi07S1VgEB_woBAAQgQtYHNM15I0340zIWXLbhfZQ2o06oZYsfvFUliqFP0VK_hUEFAgMB-9C_hUIFAgMDFkS_hU4GAgQBNLKRv4VPBgIEATSykTAKBggqhkjOPQQDAgNIADBFAiEAlu5fmn7cAw39EtmYivXfYoYeusQRtN_0xNWsokeRh_YCIAQxDtH94NCJ3qMEHdjT1a5egfZlQNA9PF5GEB9LPXlvWQH3MIIB8zCCAXmgAwIBAgIQf3bT-DfajG4WzkB4pC14RTAKBggqhkjOPQQDAjA5MQwwCgYDVQQMDANURUUxKTAnBgNVBAUTIGQ4YmZlOTUxYzg0MGEwNGQ1MTcwYjVhZGUwNmQzYTU0MB4XDTIyMDEyNTIzMzQzMloXDTMyMDEyMzIzMzQzMlowOTEMMAoGA1UEDAwDVEVFMSkwJwYDVQQFEyA1YWM5NTk1MjUxNTRlYTVjOGQ3N2RkYzM0MWVhZjExODBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABLXo2lBV5Agqr2ussDTinISSFpj7ZMMtY0G-s0J-j05vbasSlG6m3N8GHGdch2BX7kmHinn1ZgCmt4hSUlQmL5CjYzBhMB0GA1UdDgQWBBQOQhTIQqeEhkTh2hdylNqFZPc5MTAfBgNVHSMEGDAWgBRI9I6OtrDGv8Vaz6nPAL_WqA7rUzAPBgNVHRMBAf8EBTADAQH_MA4GA1UdDwEB_wQEAwICBDAKBggqhkjOPQQDAgNoADBlAjEAgnHN6sr4NMe8KmXSsmrT1Zeh62_haXAsllY_5ILJ2EWn2UcUFedwYSdwF4hdWdj3AjBBmWEaVCC1cajLWAGKQGYbCOl559iUv5i0AwhiW1HGQZBPOZIFbRd26KmzEGMrNtJZA5gwggOUMIIBfKADAgECAhEAlUJNPb0pyjolvcrF7lEYfDANBgkqhkiG9w0BAQsFADAbMRkwFwYDVQQFExBmOTIwMDllODUzYjZiMDQ1MB4XDTIyMDEyNTIzMzI0OFoXDTMyMDEyMzIzMzI0OFowOTEMMAoGA1UEDAwDVEVFMSkwJwYDVQQFEyBkOGJmZTk1MWM4NDBhMDRkNTE3MGI1YWRlMDZkM2E1NDB2MBAGByqGSM49AgEGBSuBBAAiA2IABBbAh_K1cbccKI_kDzqWE0fpjEszmciCeyxPwB6MTAC0X9CHysj377mTKXgKhJNbx2HtzuyxE-nwB6SeA38HMcHcDsy7DpnZXr3PBBXJ7YdpUyauzLQAAe_XlSeaDf_U3KNjMGEwHQYDVR0OBBYEFEj0jo62sMa_xVrPqc8Av9aoDutTMB8GA1UdIwQYMBaAFDZh4QB8iAUJUYtEbEf_GkzJ6k8SMA8GA1UdEwEB_wQFMAMBAf8wDgYDVR0PAQH_BAQDAgIEMA0GCSqGSIb3DQEBCwUAA4ICAQAz2wgqg92Xw54h_Zt-I3Gim-DS79JrG7-p8xMHLDNeuymQGYKBeTQ1UdGdL4r4FiwgzF3sZNw9T3z7r0w4tIEfmoRjUb_W677AiVTcHr-_eT6jIAbMLBmI0EyQgXwBJiP-3ezv2Hr3LngJUAWorBYFdlZ5dlrAZjol1Df10uWQunEGrYlvR8kL_2Z29TeXDuPqpVA-lcMoq6TWXmZOy_vKtIfLFQHIenwi8X5HEhsZ7eo-kyVEHyRJEkq6C0w8-7aDSwBjEj0fDwObVRC-BABaPYon20d2JmeNvdvIeSbd3T7ATNJzON-pDmTkNFI6NZ1CTy9K_dIc55QbbnD_eUq-DUBWKGCKuict1INiMKbpz0cmXwBrqP2h8jBlAhmfuueXx7YAZr8E2Ca-I2dzZmCeTDWeFSxs2Z1-g1vJ_mwsrQ-tJyBn4g2bHtmp-Wderfbm2h2mxbYtiQ9H5Lpw4byEX_0zzuAIrpEcLQYGgOcyEDoy0af_cxOK8QFKIHddUPxr3LAWG546TK8wA37rVUsVINCLccAXGVATcuj-9Vka1C5l_SnF__7ZMD4f6h4lHt-ed_kvCWVQgE3vRd-BWMDWjE-dJh_YRQQXHAbEMaEM8cnL658nVJCK7Olohum4GOrtcc2deSsNtU1NPz_xPysU8X6mnaJpEoTS1WFvHz0sE1kFIDCCBRwwggMEoAMCAQICCQDDa3xEua4YMTANBgkqhkiG9w0BAQsFADAbMRkwFwYDVQQFExBmOTIwMDllODUzYjZiMDQ1MB4XDTIxMTExNzIzMTA0MloXDTM2MTExMzIzMTA0MlowGzEZMBcGA1UEBRMQZjkyMDA5ZTg1M2I2YjA0NTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAK-2x4IrsacB7Cu0LovMVBZjq--YLzLHf3UxAwyXUksbX-gJ-8cqqUUfdDy9mm8TNXRKpV539rasNTXuF8JeY5UX3ZyS5jdKU8v-JY-P-7b9EpN4oipMqZxFLUelnzIB9EGXyhzNfnYvsvUxUbb-sv_9K2_k_lvGvZ7DS_4II52q_OuOtajtKzrNnF46d5DhtRRCeTFZhZgRrZ6yqWu916V8k6kcQfzNJ9Z_1vZxqguBUmGtOE-jeUSGRgTds9jE-SChmxZWwvFK1tA8VuwGCJkEHB7Rpf5tNEC1VrrR0KFSWJxT5V03B2LwEi7vkYYbGw5sTICSdJnA6b7AuD47wfk8csBJYEu9LxNF5iw_jibb7AbJR2bzwSgjnU9DEvrYEjiH4Gvs9WdYO_g1WoH-6rr5moPI3z4qMir8ZyvxILE1FYtoIc6vMJtu7nf5iDOwGNqhDkUfBqN01QeB81kIKWa7d4uTCJQmmOdOC80kYooBwswD5R8LPltKweTfnq-f9qSSp3wUg4gohQFbQizme4C4jJtI4TtgerVFxyP_jET48tNoufZSDTEUXr-ehirXHfajv9JFCVnWU3QNl6EvNosT72bV0KVKbi9dmm_vRGgyvGeERyWGHwk90ObzQF2olkPvD01ptkIAUf25MElnPjaVBYDTzfT70IvFhIOVJgBjAgMBAAGjYzBhMB0GA1UdDgQWBBQ2YeEAfIgFCVGLRGxH_xpMyepPEjAfBgNVHSMEGDAWgBQ2YeEAfIgFCVGLRGxH_xpMyepPEjAPBgNVHRMBAf8EBTADAQH_MA4GA1UdDwEB_wQEAwICBDANBgkqhkiG9w0BAQsFAAOCAgEAUzTWXuXLn_KIqvo1dIrUxs1lYUk4zgRJNhUL4ddSd6N5Z2tKO63fERR5zdNKuIYuk2qRYYeKmsP4hul4PsTmp-t54i1iAuRjjxYD3mFzPfpwW982cwvAAcqWLgrrFgprek59_j4289zE1YURl7k_00B-ChhWOD4b8wMl8HY0zglyA_mh7neES3Eskq9Bavy_kfE1mpbzNcCST4ckY6kQiXqxrXwWoIgC874Z5mO1NahXEtDQpyo6Du6BXnSnVpWc9GAH7t2hgiXeCh09DLBoi2Xs_Vj_NcWEqyjDRLAyvsyuX1c8OowO3MZqV3AEU55gLhlHiO1VQ4Q8ynlTnLX92tKkC8AvndPsaxE2eK9n0RjcNmBLNlvEI-qA3Hz76vScknu6SesHB55eRGdJcHOMR-2OA8fUQNSZX6KCzMN7TnSWR9Hp8T12snXwA92In3maRWlM4nB3i81SS7fXbxgdGx0CxOPhKihYDmb9hKD-vOg0Km1UtbvvZNKdsWzANdOUwSJO56a2mvFTNH560Sou8JWSsHR_mjQMoW10VvcbJzgyfoPHheOds724iip4BCoqyuSxonqFwV-7WfQ9RjQR9jm92yjsMCFnRBZXv2Bf4es1oHXqGjRg6lQay69vtA7VqIgdWgxIy1pfRZsiFMlJu5g_7xQ5MxfsJu3MlqUKQlVoYXV0aERhdGFYqMGxkeySm_95wg0v2W_08clHuGuK7RV2gLBgFAaEEBj1RQAAAAAAAAAAAAAAAAAAAAAAAAAAACQ5OGY2NzdiZS0wMTA3LTQzODQtYTMwYy04NzMyYjlkMWI4YjSlAQIDJiABIVggEQGqAhUIkFWY_oWrroIjrFNmaPSDPNNxa9kMaEfGxR4iWCDibAKSWOPp5cfJ7Ypl0SPWGUtuRPLnf8LpoJ7XmoHk1Q"
                ),
                null,
                null,
                "public-key")),
            CancellationToken.None);
        Assert.That(competeResult.Successful, Is.True);
    }
}
