using FMOD;
using FMOD.Studio;
using GTA;
using RageAudio.Helpers;
using RageAudio.Memory.Classes;
using System;
using System.Collections.Generic;
using System.IO;

namespace RageAudio
{
    /// <summary>
    /// Wrapper for Fmod Studio.
    /// </summary>
    public class AudioPlayer : IDisposable
    {
        /// <summary>
        /// Fmod Sudio instance.
        /// </summary>
        /// <remarks>
        /// Use it only if features that are not supported by this played required.
        /// </remarks>
        public readonly FMOD.Studio.System System;

        /// <summary>
        /// Fmod Core instance.
        /// </summary>
        public readonly FMOD.System CoreSystem;

        private static bool isPaused;
        /// <summary>
        /// If True, playback of all events will be paused.
        /// </summary>
        /// <remarks>
        /// Used for pausing sounds during player switch / while in pause menu.
        /// </remarks>
        internal static bool IsPaused
        {
            get => isPaused;
            set
            {
                if (isPaused == value)
                    return;

                // Kind of messy way, not sure
                for(int i = 0; i < AudioPlayers.Count; i++)
                {
                    AudioPlayer player = AudioPlayers[i];
                    for (int k = 0; k < player.AudioSources.Count; k++)
                    {
                        AudioSource source = player.AudioSources[k];
                        for (int g = 0; g < source.AudioEvents.Count; g++)
                        {
                            AudioEvent audioEvent = source.AudioEvents[g];

                            if (value)
                                audioEvent.Pause();
                            else
                                audioEvent.Resume();
                        }
                    }
                    player.System.update();
                }
                isPaused = value;
            }
        }

        /// <summary>
        /// Volume based on game settings. Automatically set to 0 if game is paused.
        /// </summary>
        public float CurrentVolume { get; private set; }

        /// <summary>
        /// List of all created <see cref="AudioSource"/> instances.
        /// </summary>
        public readonly List<AudioSource> AudioSources = new List<AudioSource>();

        /// <summary>
        /// List of all created <see cref="AudioPlayer"/> instances.
        /// </summary>
        public static readonly List<AudioPlayer> AudioPlayers = new List<AudioPlayer>();

        /// <summary>
        /// All loaded audio banks;
        /// </summary>
        private readonly List<Bank> loadedBanks = new List<Bank>();

        /// <summary>
        /// List of all loaded plugins.
        /// </summary>
        private List<uint> loadedPlugins = new List<uint>();

        /// <summary>
        /// Whether this <see cref="AudioPlayer"/> is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Static constructor of <see cref="AudioPlayer"/>.
        /// </summary>
        static AudioPlayer()
        {
            LogHelper.Clear();
            LogHelper.Log("Start", RESULT.OK);
        }

        /// <summary>
        /// Creates a new instance of <see cref="AudioPlayer"/>.
        /// </summary>
        public AudioPlayer()
        {
            RESULT sysCreate = FMOD.Studio.System.create(out System);
            RESULT sysInit = System.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            RESULT getCore = System.getCoreSystem(out CoreSystem);

            LogHelper.Log(this, "System Create", sysCreate);
            LogHelper.Log(this, "System Init", sysInit);
            LogHelper.Log(this, "Get Core", getCore);

            InitializeDebugging();
            InitializeOutput();
            LoadAllPlugins();

            AudioPlayers.Add(this);
        }

        /// <summary>
        /// Initializes output parameters.
        /// </summary>
        private void InitializeOutput()
        {
            SPEAKERMODE speakerMode = SPEAKERMODE.DEFAULT;
            
            // TODO: Find better way handling output mode
            switch(GameSettings.SoundOutputMode)
            {
                case Memory.Enums.SoundOutput.Headphones:
                case Memory.Enums.SoundOutput.TV:
                case Memory.Enums.SoundOutput.StereoSpeakers:
                    {
                        speakerMode = SPEAKERMODE.STEREO;
                        break;
                    }
                case Memory.Enums.SoundOutput.SurroundSpeakers:
                    {
                        speakerMode = SPEAKERMODE.SURROUND;
                        break;
                    }
            }

            CoreSystem.setSoftwareFormat(44100, speakerMode, 2);
            CoreSystem.setOutput(OUTPUTTYPE.AUTODETECT);
        }

        /// <summary>
        /// Initializes FMOD debugging.
        /// </summary>
        private void InitializeDebugging()
        {
            // FIXME:
            /*
             * This function will return FMOD_ERR_UNSUPPORTED 
             *  when using the non-logging (release) versions of FMOD.
             * The logging version of FMOD can be recognized by the 'L' suffix in the library name, 
             *  fmodL.dll or libfmodL.so for instance.
            */

            DEBUG_FLAGS debugFlags = DEBUG_FLAGS.WARNING | DEBUG_FLAGS.ERROR;
            RESULT debugInit = Debug.Initialize(debugFlags, DEBUG_MODE.CALLBACK, (_, __, ___, func, msg) =>
            {
                string action = new StringWrapper(func);
                string result = new StringWrapper(msg);

                LogHelper.Log(action, result);

                return RESULT.OK;
            });
            LogHelper.Log(this, "Debug Init", debugInit);
        }

        /// <summary>
        /// Unloads all plugins loaded in memory.
        /// </summary>
        private void UnloadAllPlugins()
        {
            for(int i = 0; i < loadedPlugins.Count; i++)
            {
                CoreSystem.unloadPlugin(loadedPlugins[i]);
            }
            loadedPlugins.Clear();
        }

        /// <summary>
        /// Loads all plugins into memory.
        /// </summary>
        private void LoadAllPlugins()
        {
            string pluginsPath = "scripts/FMODPlugins/";

            string[] pluginFiles = Directory.GetFiles(pluginsPath, "*.dll");

            for(int i = 0; i < pluginFiles.Length; i++)
            {
                string pluginPath = pluginFiles[i];

                RESULT loadPlugin =  CoreSystem.loadPlugin(pluginPath, out uint handle);
                LogHelper.Log(this, $"Load Plugin: {pluginPath}", loadPlugin);

                loadedPlugins.Add(handle);
            }
        }

        /// <summary>
        /// Updates Fmod system. Needs to be called every tick.
        /// </summary>
        internal void Update()
        {
            /*
             *  It is important to update all orientations before calling Studio::System::update. 
             *  If some orientations are set before Studio::System::update and some are set afterwards, 
             *  then some frames may end up having old positions relative to others. 
             *  This is particularly important when both the listener and the events are moving fast and together - 
             *  if there are frames where the listener moves but the event does not it becomes very noticeable.
            */
            UpdateListener();
            UpdateSources();
            UpdateVolume();

            System.update();
        }

        /// <summary>
        /// Update <see cref="CurrentVolume"/>.
        /// </summary>
        private void UpdateVolume()
        {
            float volume = GameSettings.SoundVolume;
            if (GameSettings.MuteSoundOnFocusLost)
                if (!GameWindow.IsWindowFocused)
                    volume = 0f;

            // TODO: Remove 1f
            volume = 1f;

            CurrentVolume = volume;
        }

        /// <summary>
        /// Calls update for every <see cref="AudioSource"/>.
        /// </summary>
        private void UpdateSources()
        {
            for (int i = 0; i < AudioSources.Count; i++)
            {
                AudioSources[i].Update();
            }
        }

        /// <summary>
        /// Updates listener position for calculating spatial audio.
        /// </summary>
        private void UpdateListener()
        {
            System.setListenerAttributes(0, AudioListener.Fmod3dAttributes);
        }

        /// <summary>
        /// Creates a new instance of <see cref="AudioSource"/> with given bank.
        /// </summary>
        /// <param name="entity">Source of the sound. If set to null, sound plays in 2D.</param>
        /// <returns>A new instance of <see cref="AudioSource"/>.</returns>
        public AudioSource CreateAudioSource(Entity entity)
        {
            AudioSource audioSource = new AudioSource(this, entity);
            AudioSources.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// Loads bank by name.
        /// </summary>
        /// <param name="name">Full path to the bank including extension.</param>
        public void LoadBank(string name)
        {
            bool loaded = false;
            for (int i = 0; i < loadedBanks.Count; i++)
            {
                Bank bank = loadedBanks[i];
                bank.getPath(out string path);

                if (path?.Contains(name) == true)
                {
                    loaded = true;
                    break;
                }
            }

            if (!loaded)
            {
                RESULT bankLoad = System.loadBankFile(name, LOAD_BANK_FLAGS.NORMAL, out Bank bank);
                RESULT bankLoadSample = bank.loadSampleData();

                loadedBanks.Add(bank);

                bank.getEventCount(out int eventCount);

                LogHelper.Log(this, $"Load Bank: {name}", bankLoad);
                LogHelper.Log(this, $"Load Bank Sample Data", bankLoadSample);
                LogHelper.Log(this, $"Found {eventCount} Events", RESULT.OK);
            }
            else
                LogHelper.Log(this, $"Banks already loaded... skipping", RESULT.OK);
        }

        /// <summary>
        /// Disposes this <see cref="AudioPlayer"/> and all <see cref="AudioSource"/> created by it.
        /// </summary>
        public void Dispose()
        {
            for(int i = 0; i < AudioSources.Count; i++)
            {
                AudioSources[i].Dispose();
            }
            for (int i = 0; i < loadedBanks.Count; i++)
            {
                Bank bank = loadedBanks[i];
                bank.unload();
                bank.unloadSampleData();
            }
            UnloadAllPlugins();

            System.unloadAll();
            System.release();

            AudioPlayers.Remove(this);

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}
