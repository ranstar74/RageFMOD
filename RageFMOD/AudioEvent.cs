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
        /// <see cref="ChannelGroup"/> <see cref="EventInstance"/> belongs to.
        /// </summary>
        public ChannelGroup ChannelGroup { get; private set; }

        /// <summary>
        /// Volume of the event in range of 0.0 - 1.0
        /// </summary>
        public float Volume { get; set; } = 1;

        /// <summary>
        /// Echo Dsp for tunnels and caves.
        /// </summary>
        public readonly DSP DspEcho;

        /// <summary>
        /// Reverb Dsp for tunnels.
        /// </summary>
        public readonly DSP DspReverb;

        /// <summary>
        /// Reverb Dsp for the distant sound.
        /// </summary>
        public readonly DSP DSPDistanceReverb;

        /// <summary>
        /// Invokes after event was loaded.
        /// </summary>
        public Action OnLoaded { get; set; }

        /// <summary>
        /// Whether playback event is paused or not.
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// Whether event is loaded or not.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Whether <see cref="AudioEvent"/> is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AudioEvent"/> on given <see cref="AudioSource"/>.
        /// </summary>
        /// <param name="audioSource"><see cref="AudioSource"/> on which <see cref="AudioEvent"/> will be played.</param>
        /// <param name="eventInstance">Event instance to play.</param>
        internal AudioEvent(AudioSource audioSource, EventInstance eventInstance)
        {
            AudioSource = audioSource;
            EventInstance = eventInstance;

            AudioPlayer.CoreSystem.createDSPByType(DSP_TYPE.ECHO, out DspEcho);
            AudioPlayer.CoreSystem.createDSPByType(DSP_TYPE.SFXREVERB, out DspReverb);
            AudioPlayer.CoreSystem.createDSPByType(DSP_TYPE.SFXREVERB, out DSPDistanceReverb);

            DspEcho.setActive(true);
            DspReverb.setActive(true);
            DSPDistanceReverb.setActive(true);
        }

        /// <summary>
        /// Checks event loading state.
        /// </summary>
        internal void Update()
        {
            if (IsLoaded)
                return;

            RESULT getChannelGroup = EventInstance.getChannelGroup(out ChannelGroup channelGroup);

            if (getChannelGroup == RESULT.ERR_STUDIO_NOT_LOADED)
                return;

            LogHelper.Log(AudioPlayer, $"Get Channel Group", getChannelGroup);

            IsLoaded = getChannelGroup == RESULT.OK;

            ChannelGroup = channelGroup;
            OnLoaded?.Invoke();

            ChannelGroup.addDSP(0, DspEcho);
            ChannelGroup.addDSP(0, DspReverb);
            ChannelGroup.addDSP(0, DSPDistanceReverb);

            // TODO: Add proper presets

            // Echo configuration
            DspEcho.setParameterFloat((int)DSP_ECHO.DRYLEVEL, 0);

            // Reverb configuration
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.DECAYTIME, 3000);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.EARLYDELAY, 10);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.HFREFERENCE, 5000);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.HFDECAYRATIO, 100);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.DIFFUSION, 50);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.DENSITY, 50);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.LOWSHELFFREQUENCY, 1000);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.LOWSHELFGAIN, 10);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.HIGHCUT, 3400);
            DspReverb.setParameterFloat((int)DSP_SFXREVERB.EARLYLATEMIX, 72);

            // Distance reverb configuration
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.DECAYTIME, 2700);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.EARLYDELAY, 8);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.LATEDELAY, 12);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.HFREFERENCE, 5000);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.HFDECAYRATIO, 100);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.DIFFUSION, 100);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.DENSITY, 100);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.LOWSHELFFREQUENCY, 250);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.LOWSHELFGAIN, 0);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.HIGHCUT, 10050);
            DSPDistanceReverb.setParameterFloat((int)DSP_SFXREVERB.EARLYLATEMIX, 56);
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
            DspEcho.release();
            DspReverb.release();
            DSPDistanceReverb.release();

            EventInstance.stop(STOP_MODE.IMMEDIATE);
            EventInstance.clearHandle();

            IsDisposed = true;
        }
    }
}
