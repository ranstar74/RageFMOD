using FMOD;
using FMOD.Studio;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using RageAudio.Helpers;
using RageAudio.Memory.Classes;
using RageAudio.Memory.Classes.Audio;
using RageAudio.Memory.Classes.Entity;
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
        /// <see cref="CVehicle"/> instance of <see cref="SourceEntity"/> 
        /// if it is the <see cref="Vehicle"/>.
        /// </summary>
        private readonly CVehicle SourceCVehicle;

        /// <summary>
        /// Velocity vector of the <see cref="SourceEntity"/> per second.
        /// </summary>
        public Vector3 SourceVelocity { get; private set; }

        /// <summary>
        /// Previous frame SourceEntity.Position.
        /// </summary>
        private Vector3 previousPosition;

        /// <summary>
        /// <see cref="ATTRIBUTES_3D"/> of the <see cref="SourceEntity"/>.
        /// </summary>
        public ATTRIBUTES_3D Fmod3dAttributes { get; private set; }

        /// <summary>
        /// Mixing <see cref="ChannelGroup"/> of the <see cref="AudioSource"/>.
        /// </summary>
        public readonly ChannelGroup ChannelGroup;

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

            ChannelGroup = new ChannelGroup();

            if (entity is Vehicle)
            {
                SourceCVehicle = new CVehicle(entity.MemoryAddress);
            }
        }

        float currentReverb = 0f;
        float currentEcho = 0f;

        /// <summary>
        /// Updates entity position for spartial sound. Needs to be called every tick.
        /// </summary>
        internal void Update()
        {
            //GTA.UI.Screen.ShowSubtitle(currentReverb.ToString("0.0"));
            // TODO: Use these settings for distant sound
            // TODO: Figure it out how to use cinematic camera
            // TODO: Add rev sound?
            // TODO: Working Blow Off

            //DspReverb.setParameterFloat((int)DSP_SFXREVERB.DECAYTIME, 10000);
            //DspReverb.setParameterFloat((int)DSP_SFXREVERB.HFDECAYRATIO, 100);
            //DspReverb.setParameterFloat((int)DSP_SFXREVERB.DENSITY, 100);
            //DspReverb.setParameterFloat((int)DSP_SFXREVERB.DIFFUSION, 100)
            //DspReverb.setParameterFloat((int)DSP_SFXREVERB.WETLEVEL, currentReverb.Remap(0, 1, -80, 20));
            //DspReverb.setParameterFloat((int)DSP_SFXREVERB.DRYLEVEL, currentReverb.Remap(0, 1, -80, 20));

            UpdateAttributes();

            // TODO: Make reverb work also with sound that are played on player,
            // for example - gun shots
            // Probably need to look for environment group in player class, it must be there,
            // as well as in entity
            EnvironmentGroup vehEnvGroup = SourceCVehicle?.VehicleAudio.EnvironmentGroup;

            currentReverb = MathExtensions.Lerp(currentReverb, vehEnvGroup.Reverb, 0.55f);
            currentEcho = MathExtensions.Lerp(currentEcho, vehEnvGroup.Echo, 0.55f);

            float echoFeedback = currentEcho.Remap(0, 1, 0, 40);
            float echoDelay = currentEcho.Remap(0, 1, 0, 200);
            float echoWetLevel = currentEcho.Remap(0, 0.5f, -80, 40);
            float reverbWetLevel = currentReverb.Remap(0, 0.5f, -80, 10);
            for (int i = 0; i < AudioEvents.Count; i++)
            {
                AudioEvent audioEvent = AudioEvents[i];
                audioEvent.Update();

                if (!audioEvent.IsLoaded)
                    continue;

                // Update audio source position
                audioEvent.EventInstance.set3DAttributes(Fmod3dAttributes);

                // FIXME: Property seems to be broken...
                audioEvent.EventInstance.setPitch(Game.TimeScale);

                // Update Volume and IsMuted
                audioEvent.ChannelGroup.setVolume(AudioPlayer.GameVolume);
                audioEvent.ChannelGroup.setMute(AudioPlayer.IsMuted);

                // Update DSP

                audioEvent.DspEcho.setParameterFloat((int)DSP_ECHO.FEEDBACK, echoFeedback);
                audioEvent.DspEcho.setParameterFloat((int)DSP_ECHO.DELAY, echoDelay);
                audioEvent.DspEcho.setParameterFloat((int)DSP_ECHO.WETLEVEL, echoWetLevel);
                audioEvent.DspEcho.setParameterFloat((int)DSP_ECHO.DRYLEVEL, 0);

                audioEvent.DspReverb.setParameterFloat((int)DSP_SFXREVERB.DECAYTIME, 5000);
                audioEvent.DspReverb.setParameterFloat((int)DSP_SFXREVERB.HFDECAYRATIO, 100);
                audioEvent.DspReverb.setParameterFloat((int)DSP_SFXREVERB.DENSITY, 100);
                audioEvent.DspReverb.setParameterFloat((int)DSP_SFXREVERB.DIFFUSION, 100);
                audioEvent.DspReverb.setParameterFloat((int)DSP_SFXREVERB.WETLEVEL, reverbWetLevel);
                audioEvent.DspReverb.setParameterFloat((int)DSP_SFXREVERB.DRYLEVEL, 0);
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
            RESULT getEvent = System.getEvent(eventName, out EventDescription eventDescription);
            LogHelper.Log(AudioPlayer, $"Get Event: {eventName} Valid: {eventDescription.isValid()}", getEvent);

            RESULT createInstance = eventDescription.createInstance(out EventInstance eventInstance);
            LogHelper.Log(AudioPlayer, $"Create Instance: {eventName}", createInstance);

            AudioEvent newEvent = new AudioEvent(this, eventInstance);

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
            for (int i = 0; i < AudioEvents.Count; i++)
            {
                AudioEvents[i].Dispose();
            }

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}