﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using NUnit.Framework;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Services.AuthenticationCeremony.Implementation.DefaultAuthenticationCeremonyService.Abstractions;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums;

namespace WebAuthn.Net.Services.AuthenticationCeremony.Implementation.DefaultAuthenticationCeremonyService.AttestationTypes;

public class DefaultAuthenticationCeremonyServiceAndroidSafetynetTests : AbstractAuthenticationCeremonyServiceTests
{
    protected override Uri GetRelyingPartyAddress()
    {
        return new("https://goose-wondrous-overly.ngrok-free.app", UriKind.Absolute);
    }

    [SetUp]
    public async Task SetupRegistrationAsync()
    {
        TimeProvider.Change(DateTimeOffset.Parse("2023-10-17T15:01:40Z", CultureInfo.InvariantCulture));
        var userId = WebEncoders.Base64UrlDecode("AAAAAAAAAAAAAAAAAAAAAQ");
        var beginResult = await RegistrationCeremonyService.BeginCeremonyAsync(
            new DefaultHttpContext(new FeatureCollection()),
            new(
                null,
                null,
                "Test Host",
                new("testuser", userId, "Test User"),
                32,
                new[]
                {
                    CoseAlgorithm.ES256,
                    CoseAlgorithm.ES384,
                    CoseAlgorithm.ES512,
                    CoseAlgorithm.RS256,
                    CoseAlgorithm.RS384,
                    CoseAlgorithm.RS512,
                    CoseAlgorithm.PS256,
                    CoseAlgorithm.PS384,
                    CoseAlgorithm.PS512
                },
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
            WebEncoders.Base64UrlDecode("w4fwKLrrnhKGOzpleXpXbsn6c3B6Mtzdk04-SQDRKgA"));

        var completeResult = await RegistrationCeremonyService.CompleteCeremonyAsync(
            new DefaultHttpContext(new FeatureCollection()),
            new(
                beginResult.RegistrationCeremonyId,
                null,
                new(
                    "AQhGVh4AhYHIa06UqIE7BpSKEAhmdQCRZXkDohn9wtWcHTIV5_Q1o3pC498cM2Y922-3ikbisHI9l3UhX7tSwhc",
                    "AQhGVh4AhYHIa06UqIE7BpSKEAhmdQCRZXkDohn9wtWcHTIV5_Q1o3pC498cM2Y922-3ikbisHI9l3UhX7tSwhc",
                    new(
                        "eyJ0eXBlIjoid2ViYXV0aG4uY3JlYXRlIiwiY2hhbGxlbmdlIjoidzRmd0tMcnJuaEtHT3pwbGVYcFhic242YzNCNk10emRrMDQtU1FEUktnQSIsIm9yaWdpbiI6Imh0dHBzOlwvXC9nb29zZS13b25kcm91cy1vdmVybHkubmdyb2stZnJlZS5hcHAiLCJhbmRyb2lkUGFja2FnZU5hbWUiOiJjb20uYW5kcm9pZC5jaHJvbWUifQ",
                        null,
                        null,
                        null,
                        null,
                        "o2NmbXRxYW5kcm9pZC1zYWZldHluZXRnYXR0U3RtdKJjdmVyaTIzMzMxNjAxOWhyZXNwb25zZVkhS2V5SmhiR2NpT2lKU1V6STFOaUlzSW5nMVl5STZXeUpOU1VsR1ltcERRMEpHWVdkQmQwbENRV2RKVVZaMFZtWmxTbmMxVDFSelMwNXlMelJHUWxKbGVFUkJUa0puYTNGb2EybEhPWGN3UWtGUmMwWkJSRUpIVFZGemQwTlJXVVJXVVZGSFJYZEtWbFY2UldsTlEwRkhRVEZWUlVOb1RWcFNNamwyV2pKNGJFbEdVbmxrV0U0d1NVWk9iR051V25CWk1sWjZTVVY0VFZGNlJWUk5Ra1ZIUVRGVlJVRjRUVXRTTVZKVVNVVk9Ra2xFUmtWT1JFRmxSbmN3ZVUxNlFUTk5la1YzVDFSQk5FMUVSbUZHZHpCNVRYcEZkMDFxYTNkUFZFRTBUVVJDWVUxQ01IaEhla0ZhUW1kT1ZrSkJUVlJGYlVZd1pFZFdlbVJETldoaWJWSjVZakpzYTB4dFRuWmlWRU5EUVZOSmQwUlJXVXBMYjFwSmFIWmpUa0ZSUlVKQ1VVRkVaMmRGVUVGRVEwTkJVVzlEWjJkRlFrRkxhRGN4Y0VrMWVYcDZhWFJLVUVWbFluWlNXRVJEUWtGSFpXNWlaVk14WWtGRWNITkRReTlKYW5SVFdUUk9jMWxMTHpBdk1sZEhMemxPZWtORGMySkJUazl5UjFwNlpqYzNjM0ZvWVdRd0wxVkZXalV4VDJKc1RtdHhTRmw1UkZKWVozbEJiekp2UzA5amNGQnZZMVZIV25CRVUxb3lVRUZsUWpNelVuSTFaSE0zZDAwMVYwUkZhaTk0TWxSeVUwWnlNMFpqUzFKMWF6UkZXbGd4VTBoak1FaEtRekJDWVhvMVVubHJhbTFrTlN0bE5HbDFORVJyYXpRemNVUXJVaXRpYmxoS2MweGlhMnQzYjJGb04wWmtkWE50ZUhodlpuazRWRXhFYlVWUmRrY3lOMjE0UzA1UlpFZGlRbEZTZG1WdlpFWjFXVWxzYTB4d1VFVjNWRzVqYlVNeWNqSkJabkZMTW5kSVdtSjRVM2xEU0hnMWRqaFRNSGxrV1VZM1IxQjNhVUZPVlhZMU5XbFRlbkV5Y1dodWNtbGplVkZOWWxSRFJXMUlRek00VTFGdE1rMVVlaTk2TVZZMU9HRm5TRE5UVTFwNlIwMURRWGRGUVVGaFQwTkJiamgzWjJkS04wMUJORWRCTVZWa1JIZEZRaTkzVVVWQmQwbEdiMFJCVkVKblRsWklVMVZGUkVSQlMwSm5aM0pDWjBWR1FsRmpSRUZVUVUxQ1owNVdTRkpOUWtGbU9FVkJha0ZCVFVJd1IwRXhWV1JFWjFGWFFrSlJhREI0Vm5WV0szQm9SSEZvV20xcFJuaE9aVzRyYWtob05qTjZRV1pDWjA1V1NGTk5SVWRFUVZkblFsRnNOR2huVDNOc1pWSnNRM0pzTVVZeVIydEpVR1ZWTjA4MGEycENOMEpuWjNKQ1owVkdRbEZqUWtGUlVuWk5SekIzVDBGWlNVdDNXVUpDVVZWSVRVRkhSMHhIYURCa1NFRTJUSGs1ZGxrelRuZE1ia0p5WVZNMWJtSXlPVzVNTTAxMldqTlNlazFYVVRCaFZ6VXdURE5zZDJReU1XNU9hemxoWVcwNWRrMUVSVWREUTNOSFFWRlZSa0o2UVVOb2FWWnZaRWhTZDA5cE9IWmpSM1J3VEcxa2RtSXlZM1pqYlZaM1luazVhbHBZU2pCamVUbHVaRWhOZUZwRVVYVmFSMVo1VFVJd1IwRXhWV1JGVVZGWFRVSlRRMFZ0UmpCa1IxWjZaRU0xYUdKdFVubGlNbXhyVEcxT2RtSlVRV2hDWjA1V1NGTkJSVWRxUVZsTlFXZEhRbTFsUWtSQlJVTkJWRUZOUW1kdmNrSm5SVVZCWkZvMVFXZFZSRTFFT0VkQk1WVmtTSGRSTkUxRVdYZE9TMEY1YjBSRFIweHRhREJrU0VFMlRIazVhbU50ZUhwTWJrSnlZVk0xYm1JeU9XNU1NbVF3WTNwR2EwNUhiSFZrUXprd1lqQk9UVkpWT1hoT1JFWkhaSGsxYW1OdGQzZG5aMFZGUW1kdmNrSm5SVVZCWkZvMVFXZFJRMEpKU0RGQ1NVaDVRVkJCUVdSblFqWk5iM2hWTWt4amRIUnBSSEZQVDBKVFNIVnRSVVp1UVhsRk5GWk9UemxKY25kVWNGaHZNVXh5VldkQlFVRlpiWEpoTURSdVFVRkJSVUYzUWtoTlJWVkRTVVZGVDFObGVqQTVSMUEyTVZWak5XeFhObEJ1ZFRFclZEWjVaMkpNYVZoVFZWSklRVTE2VVdkaWJXcEJhVVZCTTIxdGNtWjBZVEJwVGpaWlMwa3phbmhEYzNWNmRrVlRVV2czVUV0V2RrUnNZbXRtYlhsbVZ6RTRRVUZrWjBONll6TmpTRFJaVWxFclIwOUhNV2RYY0ROQ1JVcFRibXQwYzFkalRVTTBabU00UVUxUFpWUmhiRzFuUVVGQldXMXlZVEF6TUVGQlFVVkJkMEpJVFVWVlEwbEJhMlJqY3pkNFIweGpSMmRUTkhGblNuSnhUMFpVYjI5dFZGVnZTakkzWXpoM05tOVRaRzEwZFhkS1FXbEZRWFJtTTBSUFowSmpaamRDYlhadVVFOUdiMHBrVWt0S1JFMDJXbm93VVVsa00wRnZVVXhpVmt0TGRVVjNSRkZaU2t0dldrbG9kbU5PUVZGRlRFSlJRVVJuWjBWQ1FVWXlRMDlCV2psa2RrbDJXVk0xY1VsTlFWRk9URzlGYmpSbk5GVlFWMlV2VldsVGExTk1jbEZoUWtkcGVuQkZlazVGV1dZNGRuYzNka1Z3TmtkTGRVVjRNV0pPVWtocU9GVTFRMjlVUVdGaVpWQlVka1pDTDJaNVdWZHdjblp5YkdGWksxRjNhbGMyT0RGWlYwMVROMGxuTlRkMlMyVjFXWFpKTlZadWNHTlhlbmRpY0c1NVNtbDVRVUpQWkhkSU5tMTFVVGc0VWxSU2VtNUlaVU5vTWtWalltSnlRWEV5VDBreFEzbEdRbUZLU25WbmNIZFhlbU5IWm5jM1ZIZHlXVVZCTTJaaWRVa3hUamRIZDAxUWFTOHdSVlpHYTB3cmExQmpRV1JhY1dzMFJqVTNMMjF6V25KaVluQk9RVXBWTm5SS1RuTkZlWE5CWm1VelJuTnRabEZMY0ZScU9UUkJRbWRoT1hOSU9FRlhaR05XTWtJdlFsbDZObVJOUlN0UVdESjBSVFZGTjA1RWVsRlRUelphTTFOSWNGbFhZbVZGYUZkUEswbHdla2RUV0ZjNFVHVlpWRGhqUzNSeGVVbEhOVU54UWxoRlFUMGlMQ0pOU1VsR2FrUkRRMEV6VTJkQmQwbENRV2RKVGtGblEwOXpaMGw2VG0xWFRGcE5NMkp0ZWtGT1FtZHJjV2hyYVVjNWR6QkNRVkZ6UmtGRVFraE5VWE4zUTFGWlJGWlJVVWRGZDBwV1ZYcEZhVTFEUVVkQk1WVkZRMmhOV2xJeU9YWmFNbmhzU1VaU2VXUllUakJKUms1c1kyNWFjRmt5Vm5wSlJYaE5VWHBGVlUxQ1NVZEJNVlZGUVhoTlRGSXhVbFJKUmtwMllqTlJaMVZxUlhkSWFHTk9UV3BCZDA5RVJYcE5SRUYzVFVSUmVWZG9ZMDVOYW1OM1QxUk5kMDFFUVhkTlJGRjVWMnBDUjAxUmMzZERVVmxFVmxGUlIwVjNTbFpWZWtWcFRVTkJSMEV4VlVWRGFFMWFVakk1ZGxveWVHeEpSbEo1WkZoT01FbEdUbXhqYmxwd1dUSldla2xGZUUxUmVrVlVUVUpGUjBFeFZVVkJlRTFMVWpGU1ZFbEZUa0pKUkVaRlRrUkRRMEZUU1hkRVVWbEtTMjlhU1doMlkwNUJVVVZDUWxGQlJHZG5SVkJCUkVORFFWRnZRMmRuUlVKQlMzWkJjWEZRUTBVeU4yd3dkemw2UXpoa1ZGQkpSVGc1WWtFcmVGUnRSR0ZITjNrM1ZtWlJOR01yYlU5WGFHeFZaV0pWVVhCTE1IbDJNbkkyTnpoU1NrVjRTekJJVjBScVpYRXJia3hKU0U0eFJXMDFhalp5UVZKYWFYaHRlVkpUYW1oSlVqQkxUMUZRUjBKTlZXeGtjMkY2ZEVsSlNqZFBNR2N2T0RKeGFpOTJSMFJzTHk4emREUjBWSEY0YVZKb1RGRnVWRXhZU21SbFFpc3lSR2hyWkZVMlNVbG5lRFozVGpkRk5VNWpWVWd6VW1OelpXcGpjV280Y0RWVGFqRTVka0p0Tm1reFJtaHhURWQ1YldoTlJuSnZWMVpWUjA4emVIUkpTRGt4WkhObmVUUmxSa3RqWmt0V1RGZExNMjh5TVRrd1VUQk1iUzlUYVV0dFRHSlNTalZCZFRSNU1XVjFSa3B0TWtwTk9XVkNPRFJHYTNGaE0ybDJjbGhYVldWV2RIbGxNRU5SWkV0MmMxa3lSbXRoZW5aNGRIaDJkWE5NU25wTVYxbElhelUxZW1OU1FXRmpSRUV5VTJWRmRFSmlVV1pFTVhGelEwRjNSVUZCWVU5RFFWaFpkMmRuUm5sTlFUUkhRVEZWWkVSM1JVSXZkMUZGUVhkSlFtaHFRV1JDWjA1V1NGTlZSVVpxUVZWQ1oyZHlRbWRGUmtKUlkwUkJVVmxKUzNkWlFrSlJWVWhCZDBsM1JXZFpSRlpTTUZSQlVVZ3ZRa0ZuZDBKblJVSXZkMGxDUVVSQlpFSm5UbFpJVVRSRlJtZFJWVXBsU1ZsRWNrcFlhMXBSY1RWa1VtUm9jRU5FTTJ4UGVuVktTWGRJZDFsRVZsSXdha0pDWjNkR2IwRlZOVXM0Y2twdVJXRkxNR2R1YUZNNVUxcHBlblk0U1d0VVkxUTBkMkZCV1VsTGQxbENRbEZWU0VGUlJVVllSRUpoVFVOWlIwTkRjMGRCVVZWR1FucEJRbWhvY0c5a1NGSjNUMms0ZG1JeVRucGpRelYzWVRKcmRWb3lPWFphZVRsdVpFaE9lVTFVUVhkQ1oyZHlRbWRGUmtKUlkzZEJiMWxyWVVoU01HTkViM1pNTTBKeVlWTTFibUl5T1c1TU0wcHNZMGM0ZGxreVZubGtTRTEyV2pOU2VtTnFSWFZhUjFaNVRVUlJSMEV4VldSSWQxRjBUVU56ZDB0aFFXNXZRMWRIU1RKb01HUklRVFpNZVRscVkyMTNkV05IZEhCTWJXUjJZakpqZGxvelVucGpha1YyV2pOU2VtTnFSWFZaTTBwelRVVXdSMEV4VldSSlFWSkhUVVZSZDBOQldVZGFORVZOUVZGSlFrMUVaMGREYVhOSFFWRlJRakZ1YTBOQ1VVMTNTMnBCYjBKblozSkNaMFZHUWxGalEwRlNXV05oU0ZJd1kwaE5Oa3g1T1hkaE1tdDFXakk1ZGxwNU9YbGFXRUoyWXpKc01HSXpTalZNZWtGT1FtZHJjV2hyYVVjNWR6QkNRVkZ6UmtGQlQwTkJaMFZCU1ZaVWIza3lOR3AzV0ZWeU1ISkJVR001TWpSMmRWTldZa3RSZFZsM00yNU1abXhNWmt4b05VRlpWMFZsVm13dlJIVXhPRkZCVjFWTlpHTktObTh2Y1VaYVltaFlhMEpJTUZCT1kzYzVOM1JvWVdZeVFtVnZSRmxaT1VOckwySXJWVWRzZFdoNE1EWjZaRFJGUW1ZM1NEbFFPRFJ1Ym5KM2NGSXJORWRDUkZwTEsxaG9NMGt3ZEhGS2VUSnlaMDl4VGtSbWJISTFTVTFST0ZwVVYwRXplV3gwWVd0NlUwSkxXalpZY0VZd1VIQnhlVU5TZG5BdlRrTkhkakpMV0RKVWRWQkRTblp6WTNBeEwyMHljRlpVZEhsQ2FsbFFVbEVyVVhWRFVVZEJTa3RxZEU0M1VqVkVSbkptVkhGTlYzWlpaMVpzY0VOS1FtdDNiSFUzS3pkTFdUTmpWRWxtZWtVM1kyMUJUSE5yVFV0T1RIVkVlaXRTZWtOamMxbFVjMVpoVlRkV2NETjRURFl3VDFsb2NVWnJkVUZQVDNoRVdqWndTRTlxT1N0UFNtMVpaMUJ0VDFRMFdETXJOMHcxTVdaWVNubFNTRGxMWmt4U1VEWnVWRE14UkRWdWJYTkhRVTluV2pJMkx6aFVPV2h6UWxjeGRXODVhblUxWmxwTVdsaFdWbE0xU0RCSWVVbENUVVZMZVVkTlNWQm9SbGR5YkhRdmFFWlRNamhPTVhwaFMwa3dXa0pIUkRObldXZEVUR0pwUkZRNVprZFljM1J3YXl0R2JXTTBiMnhXYkZkUWVsaGxPREYyWkc5RmJrWmljalZOTWpjeVNHUm5TbGR2SzFkb1ZEbENXVTB3U21rcmQyUldiVzVTWm1aWVoyeHZSVzlzZFZST1kxZDZZelF4WkVad1owcDFPR1pHTTB4SE1HZHNNbWxpVTFscFEyazVZVFpvZGxVd1ZIQndha3A1U1ZkWWFHdEtWR05OU214UWNsZDRNVlo1ZEVWVlIzSllNbXd3U2tSM1VtcFhMelkxTm5Jd1MxWkNNREo0U0ZKTGRtMHlXa3RKTUROVVoyeE1TWEJ0VmtOTE0ydENTMnRMVG5CQ1RtdEdkRGh5YUdGbVkwTkxUMkk1U25ndk9YUndUa1pzVVZSc04wSXpPWEpLYkVwWGExSXhOMUZ1V25GV2NIUkdaVkJHVDFKdldtMUdlazA5SWl3aVRVbEpSbGxxUTBOQ1JYRm5RWGRKUWtGblNWRmtOekJPWWs1ek1pdFNjbkZKVVM5Rk9FWnFWRVJVUVU1Q1oydHhhR3RwUnpsM01FSkJVWE5HUVVSQ1dFMVJjM2REVVZsRVZsRlJSMFYzU2tOU1ZFVmFUVUpqUjBFeFZVVkRhRTFSVWpKNGRsbHRSbk5WTW14dVltbENkV1JwTVhwWlZFVlJUVUUwUjBFeFZVVkRlRTFJVlcwNWRtUkRRa1JSVkVWaVRVSnJSMEV4VlVWQmVFMVRVako0ZGxsdFJuTlZNbXh1WW1sQ1UySXlPVEJKUlU1Q1RVSTBXRVJVU1hkTlJGbDRUMVJCZDAxRVFUQk5iRzlZUkZSSk5FMUVSWGxQUkVGM1RVUkJNRTFzYjNkU2VrVk1UVUZyUjBFeFZVVkNhRTFEVmxaTmVFbHFRV2RDWjA1V1FrRnZWRWRWWkhaaU1tUnpXbE5DVldOdVZucGtRMEpVV2xoS01tRlhUbXhqZVVKTlZFVk5lRVpFUVZOQ1owNVdRa0ZOVkVNd1pGVlZlVUpUWWpJNU1FbEdTWGhOU1VsRFNXcEJUa0puYTNGb2EybEhPWGN3UWtGUlJVWkJRVTlEUVdjNFFVMUpTVU5EWjB0RFFXZEZRWFJvUlVOcGVEZHFiMWhsWWs4NWVTOXNSRFl6YkdGa1FWQkxTRGxuZG13NVRXZGhRMk5tWWpKcVNDODNOazUxT0dGcE5saHNOazlOVXk5cmNqbHlTRFY2YjFGa2MyWnVSbXc1TjNaMVprdHFObUozVTJsV05tNXhiRXR5SzBOTmJuazJVM2h1UjFCaU1UVnNLemhCY0dVMk1tbHRPVTFhWVZKM01VNUZSRkJxVkhKRlZHODRaMWxpUlhaekwwRnRVVE0xTVd0TFUxVnFRalpITURCcU1IVlpUMFJRTUdkdFNIVTRNVWs0UlRORGQyNXhTV2x5ZFRaNk1XdGFNWEVyVUhOQlpYZHVha2g0WjNOSVFUTjVObTFpVjNkYVJISllXV1pwV1dGU1VVMDVjMGh0YTJ4RGFYUkVNemh0TldGblNTOXdZbTlRUjJsVlZTczJSRTl2WjNKR1dsbEtjM1ZDTm1wRE5URXhjSHB5Y0RGYWEybzFXbEJoU3pRNWJEaExSV280UXpoUlRVRk1XRXd6TW1nM1RURmlTM2RaVlVnclJUUkZlazVyZEUxbk5sUlBPRlZ3YlhaTmNsVndjM2xWY1hSRmFqVmpkVWhMV2xCbWJXZG9RMDQyU2pORGFXOXFOazlIWVVzdlIxQTFRV1pzTkM5WWRHTmtMM0F5YUM5eWN6TTNSVTlsV2xaWWRFd3diVGM1V1VJd1pYTlhRM0oxVDBNM1dFWjRXWEJXY1RsUGN6WndSa3hMWTNkYWNFUkpiRlJwY25oYVZWUlJRWE0yY1hwcmJUQTJjRGs0WnpkQ1FXVXJaRVJ4Tm1SemJ6UTVPV2xaU0RaVVMxZ3ZNVmszUkhwcmRtZDBaR2w2YW10WVVHUnpSSFJSUTNZNVZYY3JkM0E1VlRkRVlrZExiMmRRWlUxaE0wMWtLM0IyWlhvM1Z6TTFSV2xGZFdFckszUm5lUzlDUW1wR1JrWjVNMnd6VjBad1R6bExWMmQ2TjNwd2JUZEJaVXRLZERoVU1URmtiR1ZEWm1WWWEydFZRVXRKUVdZMWNXOUpZbUZ3YzFwWGQzQmlhMDVHYUVoaGVESjRTVkJGUkdkbVp6RmhlbFpaT0RCYVkwWjFZM1JNTjFSc1RHNU5VUzh3YkZWVVltbFRkekZ1U0RZNVRVYzJlazh3WWpsbU5rSlJaR2RCYlVRd05ubExOVFp0UkdOWlFscFZRMEYzUlVGQllVOURRVlJuZDJkblJUQk5RVFJIUVRGVlpFUjNSVUl2ZDFGRlFYZEpRbWhxUVZCQ1owNVdTRkpOUWtGbU9FVkNWRUZFUVZGSUwwMUNNRWRCTVZWa1JHZFJWMEpDVkd0eWVYTnRZMUp2Y2xORFpVWk1NVXB0VEU4dmQybFNUbmhRYWtGbVFtZE9Wa2hUVFVWSFJFRlhaMEpTWjJVeVdXRlNVVEpZZVc5c1VVd3pNRVY2VkZOdkx5OTZPVk42UW1kQ1oyZHlRbWRGUmtKUlkwSkJVVkpWVFVaSmQwcFJXVWxMZDFsQ1FsRlZTRTFCUjBkSFYyZ3daRWhCTmt4NU9YWlpNMDUzVEc1Q2NtRlROVzVpTWpsdVRESmtlbU5xUlhkTFVWbEpTM2RaUWtKUlZVaE5RVXRIU0Zkb01HUklRVFpNZVRsM1lUSnJkVm95T1haYWVUbHVZek5KZUV3eVpIcGpha1YxV1ROS01FMUVTVWRCTVZWa1NIZFJjazFEYTNkS05rRnNiME5QUjBsWGFEQmtTRUUyVEhrNWFtTnRkM1ZqUjNSd1RHMWtkbUl5WTNaYU0wNTVUVk01Ym1NelNYaE1iVTU1WWtSQk4wSm5UbFpJVTBGRlRrUkJlVTFCWjBkQ2JXVkNSRUZGUTBGVVFVbENaMXB1WjFGM1FrRm5TWGRFVVZsTVMzZFpRa0pCU0ZkbFVVbEdRWGRKZDBSUldVeExkMWxDUWtGSVYyVlJTVVpCZDAxM1JGRlpTa3R2V2tsb2RtTk9RVkZGVEVKUlFVUm5aMFZDUVVSVGEwaHlSVzl2T1VNd1pHaGxiVTFZYjJnMlpFWlRVSE5xWW1SQ1drSnBUR2M1VGxJemREVlFLMVEwVm5obWNUZDJjV1pOTDJJMVFUTlNhVEZtZVVwdE9XSjJhR1JIWVVwUk0ySXlkRFo1VFVGWlRpOXZiRlZoZW5OaFRDdDVlVVZ1T1Zkd2NrdEJVMDl6YUVsQmNrRnZlVnBzSzNSS1lXOTRNVEU0Wm1WemMyMVliakZvU1ZaM05ERnZaVkZoTVhZeGRtYzBSblkzTkhwUWJEWXZRV2hUY25jNVZUVndRMXBGZERSWGFUUjNVM1I2Tm1SVVdpOURURUZPZURoTVdtZ3hTamRSU2xacU1tWm9UWFJtVkVweU9YYzBlak13V2pJd09XWlBWVEJwVDAxNUszRmtkVUp0Y0haMldYVlNOMmhhVERaRWRYQnplbVp1ZHpCVGEyWjBhSE14T0dSSE9WcExZalU1VldoMmJXRlRSMXBTVm1KT1VYQnpaek5DV214MmFXUXdiRWxMVHpKa01YaHZlbU5zVDNwbmFsaFFXVzkyU2twSmRXeDBlbXROZFRNMGNWRmlPVk42TDNscGJISmlRMmRxT0QwaVhYMC5leUp1YjI1alpTSTZJbTF6V1ZVNFZFSXljMGh5UkVoVFZWSmFiSE5sUVVodFdIVTFUM2N6VW1rd1NsRXlhbTV3TTFwcFdGRTlJaXdpZEdsdFpYTjBZVzF3VFhNaU9qRTJPVGMxTlRRNE9Ea3pPRGNzSW1Gd2ExQmhZMnRoWjJWT1lXMWxJam9pWTI5dExtZHZiMmRzWlM1aGJtUnliMmxrTG1kdGN5SXNJbUZ3YTBScFoyVnpkRk5vWVRJMU5pSTZJbXBQYXpBclkxSTVTakF6YWs1S2IwbEZZV1JXVURoTlkwdDNSRVpMWms1bFFrNXZlVFJ6VFVKdGFUZzlJaXdpWTNSelVISnZabWxzWlUxaGRHTm9JanAwY25WbExDSmhjR3REWlhKMGFXWnBZMkYwWlVScFoyVnpkRk5vWVRJMU5pSTZXeUk0VURGelZ6QkZVRXBqYzJ4M04xVjZVbk5wV0V3Mk5IY3JUelV3UldRclVrSkpRM1JoZVRGbk1qUk5QU0pkTENKaVlYTnBZMGx1ZEdWbmNtbDBlU0k2ZEhKMVpTd2laWFpoYkhWaGRHbHZibFI1Y0dVaU9pSkNRVk5KUXlJc0ltUmxjSEpsWTJGMGFXOXVTVzVtYjNKdFlYUnBiMjRpT2lKVWFHVWdVMkZtWlhSNVRtVjBJRUYwZEdWemRHRjBhVzl1SUVGUVNTQnBjeUJrWlhCeVpXTmhkR1ZrTGlCSmRDQnBjeUJ5WldOdmJXMWxibVJsWkNCMGJ5QjFjMlVnZEdobElGQnNZWGtnU1c1MFpXZHlhWFI1SUVGUVNUb2dhSFIwY0hNNkx5OW5MbU52TDNCc1lYa3ZjMkZtWlhSNWJtVjBMWFJwYldWc2FXNWxMaUo5LkNiTTBWdjFTNFRTS3FPZmtlclhjdDYxYThaeFUtNlE5U2I0dDc4V1R6SzlJVGZLOG9pcnBRX0UtZmpJS1VOWUJnWEFoSlRqTkxCa05QbHIxVkJqYkYtOG9SY0FCMHR3QUh5Mkp6STl6MkZzenI4bHM2SXlJVGNSRVA0QVZMeGNoQTdOaDNaam5ibVFUTTlaT0VMZE1Ob18tWTNuRVc3VEVmcFlwbGtoU2trbTBuUW5LSGFrVnllYmozdGM4ZkFXSl96QmVETkhkZzYwVmhaTFoxaVhsUlhOY3BUSVFxcWdHRFhjcFFGeXE1NzNSX196T0Q4OHVUMkFvNkxpVWZCT0Zlc2NuRWl4Ql96aE9FZ2YxN0s2Vm9ZanVIbVhFbHhRSUU2OUtja3ozZk5nb0hXaUNXYi0zVmpyY3FCcHRha0tuV2hQRzhBOGNnWW5BT1VUeEVEY0djQWhhdXRoRGF0YVjFmiPJOr64Yikjfv8MyyFBLBwA4260CubjCtHmm4ZttsVFAAAAALk_2WHy5kYvsSKCACJH3ngAQQEIRlYeAIWByGtOlKiBOwaUihAIZnUAkWV5A6IZ_cLVnB0yFef0NaN6QuPfHDNmPdtvt4pG4rByPZd1IV-7UsIXpQECAyYgASFYINTAwHTrs3qkmGNPh5X5_PQFZJEe7arfjqvbH9D_JdfIIlggL71h0_v_S2d_jKUrYpG6woY52-ydXy4FuSSGzPZLcS0"
                    ),
                    null,
                    new(),
                    "public-key")),
            CancellationToken.None);
        Assert.That(completeResult.Successful, Is.True);
    }

    [Test]
    public async Task DefaultAuthenticationCeremonyService_PerformsCeremonyWithoutErrorsForAndroidSafetynet_WhenAllAlgorithms()
    {
        TimeProvider.Change(DateTimeOffset.Parse("2023-10-17T15:01:50Z", CultureInfo.InvariantCulture));
        var beginRequest = new BeginAuthenticationCeremonyRequest(
            null,
            null,
            WebEncoders.Base64UrlDecode("AAAAAAAAAAAAAAAAAAAAAQ"),
            32,
            60000,
            AuthenticationCeremonyIncludeCredentials.ManuallySpecified(
                new[]
                {
                    new AuthenticationCeremonyPublicKeyCredentialDescriptor(
                        PublicKeyCredentialType.PublicKey,
                        WebEncoders.Base64UrlDecode("AQhGVh4AhYHIa06UqIE7BpSKEAhmdQCRZXkDohn9wtWcHTIV5_Q1o3pC498cM2Y922-3ikbisHI9l3UhX7tSwhc"))
                }),
            UserVerificationRequirement.Required,
            null,
            null,
            null,
            null);
        var beginResult = await AuthenticationCeremonyService.BeginCeremonyAsync(
            new DefaultHttpContext(new FeatureCollection()),
            beginRequest,
            CancellationToken.None);

        AuthenticationCeremonyStorage.ReplaceChallengeForAuthenticationCeremonyOptions(
            beginResult.AuthenticationCeremonyId,
            WebEncoders.Base64UrlDecode("SdasfQYURj6uzy3bMTTKXgXBeUf7ikAj_7cad8bHX_c"));

        var completeResult = await AuthenticationCeremonyService.CompleteCeremonyAsync(
            new DefaultHttpContext(new FeatureCollection()),
            new(beginResult.AuthenticationCeremonyId,
                new("AQhGVh4AhYHIa06UqIE7BpSKEAhmdQCRZXkDohn9wtWcHTIV5_Q1o3pC498cM2Y922-3ikbisHI9l3UhX7tSwhc",
                    "AQhGVh4AhYHIa06UqIE7BpSKEAhmdQCRZXkDohn9wtWcHTIV5_Q1o3pC498cM2Y922-3ikbisHI9l3UhX7tSwhc",
                    new("eyJ0eXBlIjoid2ViYXV0aG4uZ2V0IiwiY2hhbGxlbmdlIjoiU2Rhc2ZRWVVSajZ1enkzYk1UVEtYZ1hCZVVmN2lrQWpfN2NhZDhiSFhfYyIsIm9yaWdpbiI6Imh0dHBzOlwvXC9nb29zZS13b25kcm91cy1vdmVybHkubmdyb2stZnJlZS5hcHAiLCJhbmRyb2lkUGFja2FnZU5hbWUiOiJjb20uYW5kcm9pZC5jaHJvbWUifQ",
                        "miPJOr64Yikjfv8MyyFBLBwA4260CubjCtHmm4ZttsUFAAAAAQ",
                        "MEUCIQChq7IL-ngSC3l3MV8PJU6_n3e36xeaqFS6f0_mhVc2zgIgTzIBqOJZ8EvGvOVFoLHFx1K7WHm72Qg_qYyUjnzBcJk",
                        "",
                        null),
                    null,
                    new(),
                    "public-key")),
            CancellationToken.None);
        Assert.That(completeResult.Successful, Is.True);
    }
}
