namespace RageAudio.Memory.Enums
{
    /// <summary>
    /// PauseMenu->Audio->FrontSpeakers
    /// </summary>
    /// <remarks>
    /// Remains unchanged even if <see cref="SoundOutput"/> isn't set to <see cref="SoundOutput.SurroundSpeakers"/>.
    /// </remarks>
    internal enum FrontSpeakers
    {
        Wide = 0,
        Middle = 1,
        Tall = 2
    }
}
