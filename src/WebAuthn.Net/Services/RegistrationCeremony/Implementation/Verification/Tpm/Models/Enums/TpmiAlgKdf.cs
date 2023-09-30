namespace WebAuthn.Net.Services.RegistrationCeremony.Implementation.Verification.Tpm.Models.Enums;

/// <summary>
///     TPMI_ALG_KDF, based on TPM_ALG_ID (UINT16) - TPM_ALG_!ALG.HM, +TPM_ALG_NULL
/// </summary>
/// <remarks>
///     <para>
///         <a href="https://trustedcomputinggroup.org/resource/tpm-library-specification/">TPM 2.0 Library</a>
///     </para>
///     <para>
///         <a href="https://trustedcomputinggroup.org/wp-content/uploads/TCG_TPM2_r1p59_Part2_Structures_pub.pdf">TPM 2.0 Library - Part 2: Structures, Family "2.0", Level 00 Revision 01.59, November 8, 2019</a>
///     </para>
///     <para>9.32 TPMI_ALG_KDF (Key and Mask Generation Functions)</para>
/// </remarks>
public enum TpmiAlgKdf : ushort
{
    /// <summary>
    ///     Null algorithm
    /// </summary>
    /// <remarks>TCG TPM 2.0 library specification</remarks>
    TpmAlgNull = 0x0010
}
