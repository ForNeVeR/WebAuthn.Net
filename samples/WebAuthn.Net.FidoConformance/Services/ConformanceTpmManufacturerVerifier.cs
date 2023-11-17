﻿using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Implementation.Tpm;
using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Implementation.Tpm.Constants;
using WebAuthn.Net.Services.Common.AttestationStatementVerifier.Implementation.Tpm.Models.Manufacturer;

namespace WebAuthn.Net.FidoConformance.Services;

public class ConformanceTpmManufacturerVerifier : DefaultTpmManufacturerVerifier
{
    public override TpmManufacturerVerificationResult IsValid(string tpmManufacturer)
    {
        var baseResult = base.IsValid(tpmManufacturer);
        if (!baseResult.IsValid)
        {
            if (tpmManufacturer == "id:FFFFF1D0")
            {
                return new(true, TpmRoots.Microsoft);
            }
        }

        return baseResult;
    }
}
