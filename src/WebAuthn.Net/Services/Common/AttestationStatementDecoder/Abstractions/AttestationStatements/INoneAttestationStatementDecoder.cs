﻿using WebAuthn.Net.Models;
using WebAuthn.Net.Services.Common.AttestationStatementDecoder.Models.AttestationStatements;
using WebAuthn.Net.Services.Serialization.Cbor.Models.Tree;

namespace WebAuthn.Net.Services.Common.AttestationStatementDecoder.Abstractions.AttestationStatements;

public interface INoneAttestationStatementDecoder
{
    Result<NoneAttestationStatement> Decode(CborMap attStmt);
}
