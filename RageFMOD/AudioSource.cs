using FMOD;
using FMOD.Studio;
using GTA;
using GTA.Math;
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
        /// Velocity vector of the <see cref="SourceEntity"/> per second.
        /// </summary>
        public Vector3 SourceVelocity { get; private set; }

        /// <summary>
        /// Previous frame <see cref="SourceEntity.Position"/>.
        /// </summary>
        private Vector3 previousPosition;

        /// <summary>
        /// <see cref="ATTRIBUTES_3D"/> of the <see cref="SourceEntity"/>.
        /// </summary>
        public static ATTRIBUTES_3D Fmod3dAttributes { get; private set; }

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
            UpdateAttributes();

            for (int i = 0; i < AudioEvents.Count; i++)
            {
                AudioEvent audioEvent = AudioEvents[i];

                audioEvent.EventInstance.set3DAttributes(Fmod3dAttributes);
                audioEvent.EventInstance.setVolume(AudioPlayer.CurrentVolume);

                if (AudioPlayer.IsPaused)
                    audioEvent.Pause();
                else
                    audioEvent.Resume();
            }

            previousPosition = SourceEntity.Position;
        }

        /// <summary>
        /// Updates <see cref="Fmod3dAttributes"/>.
        /// </summary>
        private void UpdateAttributes()
        {
            SourceVelocity = (SourceEntity.Position - previousPosition) / Game.LastFrameTime;

            Fmod3dAttributes = new ATTRIBUTES_3D()
            {
                // Not sure why but its inverted by default
                forward = (-SourceEntity.ForwardVector).ToFmodVector(),
                position = SourceEntity.Position.ToFmodVector(),
                up = SourceEntity.UpVector.ToFmodVector(),
                velocity = SourceVelocity.ToFmodVector()
            };
        }

        /// <summary>
        /// Creates a new <see cref="AudioEvent"/> that will be played on this <see cref="AudioSource"/>.
        /// </summary>
        /// <remarks>
        /// Path or Guid of the event accorduing to GUIDs.txt file.
        /// </remarks>
        /// <param name="eventName">Event name to create instance of.</param>
        /// <param name="autoPlay">If set to True, playback will be automatically started.</param>
        public AudioEvent CreateEvent(string eventName, bool autoPlay)
        {
            RESULT getEvent = System.getEvent(eventName, out EventDescription _event);

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