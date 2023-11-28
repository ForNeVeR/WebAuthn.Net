﻿namespace WebAuthn.Net.Services.Common.ChallengeGenerator;

/// <summary>
///     Service that generates a challenge for WebAuthn ceremonies.
/// </summary>
public interface IChallengeGenerator
{
    /// <summary>
    ///     Generates a challenge of the specified size.
    /// </summary>
    /// <param name="size">Challenge size.</param>
    /// <returns>Challenge for WebAuthn ceremony.</returns>
    public byte[] GenerateChallenge(int size);
}
