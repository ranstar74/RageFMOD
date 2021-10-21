namespace RageAudio.Memory.Enums
{
    /// <summary>
    /// PauseMenu->Audio->RearSpeakers
    /// </summary>
    /// <remarks>
    /// Remains unchanged even if <see cref="SoundOutput"/> isn't set to <see cref="SoundOutput.SurroundSpeakers"/>.
    /// </remarks>
    internal enum RearSpeakers
    {
        Back = 0,
        Middle = 1,
        Side = 2
    }
}
