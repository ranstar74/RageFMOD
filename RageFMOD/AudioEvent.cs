using FMOD;
using FMOD.Studio;
using RageAudio.Helpers;
using System;

namespace RageAudio
{
    /// <summary>
    /// Audio that plays on <see cref="AudioSource"/>.
    /// </summary>
    public class AudioEvent : IDisposable
    {
        /// <summary>
        /// <see cref="AudioSource"/> that created this <see cref="AudioEvent"/>.
        /// </summary>
        public readonly AudioSource AudioSource;

        /// <summary>
        /// Fmod Studio Event instance.
        /// </summary>
        public readonly EventInstance EventInstance;

        /// <summary>
        /// Reference to <see cref="AudioPlayer"/>.
        /// </summary>
        public AudioPlayer AudioPlayer => AudioSource.AudioPlayer;

        /// <summary>
        /// Whether <see cref="AudioEvent"/> is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Whether playback event is paused or not.
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// Creates a new instance of <see cref="AudioEvent"/> on given <see cref="AudioSource"/>.
        /// </summary>
        /// <param name="audioSource"><see cref="AudioSource"/> on which <see cref="AudioEvent"/> will be played.</param>
        /// <param name="eventInstance">Event instance to play.</param>
        internal AudioEvent(AudioSource audioSource, EventInstance eventInstance)
        {
            AudioSource = audioSource;
            EventInstance = eventInstance;
        }

        /// <summary>
        /// Starts event playing.
        /// </summary>
        public void Play()
        {
            if (IsDisposed)
                return;

            RESULT start = EventInstance.start();

            EventInstance.getDescription(out EventDescription eventDescription);
            eventDescription.getPath(out string path);

            LogHelper.Log(AudioPlayer, $"Start Event: {path}", start);
        }

        /// <summary>
        /// Pauses event playback.
        /// </summary>
        public void Pause()
        {
            if (IsDisposed || isPaused)
                return;

            RESULT pause = EventInstance.setPaused(true);

            LogHelper.Log(AudioPlayer, $"Pause Event", pause);

            isPaused = true;
        }

        /// <summary>
        /// Resumes event playback.
        /// </summary>
        public void Resume()
        {
            if (IsDisposed || !isPaused)
                return;

            RESULT pause = EventInstance.setPaused(false);

            LogHelper.Log(AudioPlayer, $"Resume Event", pause);

            isPaused = false;
        }

        /// <summary>
        /// Sets param to this event instance.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">Value to set.</param>
        public void SetParameter(string paramName, float value)
        {
            if (IsDisposed)
                return;

            RESULT setParam = EventInstance.setParameterByName(paramName, value, false);
            
            if(setParam != RESULT.OK)
                LogHelper.Log(AudioPlayer, $"Set Param: {paramName}", setParam);
        }

        /// <summary>
        /// Stops playing of the event.
        /// </summary>
        public void Dispose()
        {
            EventInstance.stop(STOP_MODE.IMMEDIATE);
            EventInstance.clearHandle();

            IsDisposed = true;
        }
    }
}
