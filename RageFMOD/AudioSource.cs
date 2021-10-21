using FMOD;
using FMOD.Studio;
using GTA;
using RageAudio.Helpers;
using RageAudio.Memory.Classes;
using System;
using System.Collections.Generic;

namespace RageAudio
{
    /// <summary>
    /// Audio source in 3D or 2D space.
    /// </summary>
    public class AudioSource : IDisposable
    {
        /// <summary>
        /// <see cref="AudioPlayer"/> this <see cref="AudioSource"/> belongs to.
        /// </summary>
        public readonly AudioPlayer AudioPlayer;

        /// <summary>
        /// Reference to <see cref="AudioPlayer.System"/>.
        /// </summary>
        public FMOD.Studio.System System => AudioPlayer.System;

        /// <summary>
        /// All event instances created by this <see cref="AudioSource"/>.
        /// </summary>
        public readonly List<AudioEvent> AudioEvents = new List<AudioEvent>();

        /// <summary>
        /// Source of the sound. If set to null, sound plays in 2D.
        /// </summary>
        public readonly Entity SourceEntity;

        /// <summary>
        /// Whether <see cref="AudioEvent"/> is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AudioSource"/> from given <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="audioPlayer"><see cref="AudioPlayer"/> context.</param>
        /// <param name="entity">Source of the sound. If set to null, sound plays in 2D.</param>
        internal AudioSource(AudioPlayer audioPlayer, Entity entity)
        {
            AudioPlayer = audioPlayer;
            SourceEntity = entity;
        }

        /// <summary>
        /// Updates entity position for spartial sound. Needs to be called every tick.
        /// </summary>
        internal void Update()
        {
            for(int i = 0; i < AudioEvents.Count; i++)
            {
                AudioEvent audioEvent = AudioEvents[i];

                float volume = GameSettings.SoundVolume;
                if(GameSettings.MuteSoundOnFocusLost)
                    if (!GameWindow.IsWindowFocused)
                        volume = 0f;

                audioEvent.EventInstance.set3DAttributes(SourceEntity.Get3DAttributes());
                audioEvent.EventInstance.setVolume(volume);

                if (AudioPlayer.IsPaused)
                    audioEvent.Pause();
                else
                    audioEvent.Resume();
            }
        }

        /// <summary>
        /// Creates a new <see cref="AudioEvent"/> that will be played on this <see cref="AudioSource"/>.
        /// </summary>
        /// <remarks>
        /// Event name is given according to Fmod Studio hierrarchy, for example: AmbientSounds/Wind
        /// </remarks>
        /// <param name="eventName">Event name to create instance of.</param>
        /// <param name="autoPlay">If set to True, playback will be automatically started.</param>
        public AudioEvent CreateEvent(string eventName, bool autoPlay)
        {
            RESULT getEvent = System.getEvent($"event:/{eventName}", out EventDescription _event);

            LogHelper.Log(AudioPlayer, $"Get Event: {eventName} Valid: {_event.isValid()}", getEvent);

            RESULT createInstance = _event.createInstance(out EventInstance _eventInstance);

            LogHelper.Log(AudioPlayer, $"Create Instance: {eventName}", createInstance);

            AudioEvent newEvent = new AudioEvent(this, _eventInstance);
      
            AudioEvents.Add(newEvent);
            if (autoPlay)
                newEvent.Play();
            
            return newEvent;
        }

        /// <summary>
        /// Disposes all <see cref="AudioEvents"/>.
        /// </summary>
        public void Dispose()
        {
            for(int i = 0; i < AudioEvents.Count; i++)
            {
                AudioEvents[i].Dispose();
            }

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}