using FMOD;
using FMOD.Studio;
using GTA;
using RageAudio.Helpers;
using System;
using System.Collections.Generic;

namespace RageAudio
{
    /// <summary>
    /// Wrapper for Fmod Studio.
    /// </summary>
    public class AudioPlayer : IDisposable
    {
        /// <summary>
        /// Path of directory with Fmod banks.
        /// </summary>
        public readonly string BankDirectory;

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

        /// <summary>
        /// Whether this <see cref="AudioPlayer"/> is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

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
        /// <param name="bankDirectory">Path of directory with Fmod banks.</param>
        public AudioPlayer(string bankDirectory)
        {
            BankDirectory = bankDirectory;

            RESULT sysCreate = FMOD.Studio.System.create(out System);
            RESULT sysInit = System.initialize(1024, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS._3D_RIGHTHANDED, IntPtr.Zero);
            RESULT getCore = System.getCoreSystem(out CoreSystem);

            LogHelper.Log(this, "System Create", sysCreate);
            LogHelper.Log(this, "System Init", sysInit);
            LogHelper.Log(this, "Get Core", getCore);

            AudioPlayers.Add(this);
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

            System.update();
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
            ATTRIBUTES_3D listenerAttributes;

            if (World.RenderingCamera.IsActive)
            {
                listenerAttributes = World.RenderingCamera.Get3DAttributes();
            }
            else
            {
                listenerAttributes = FmodHelpers.GetGamePlayer3DAttributes();
            }

            System.setListenerAttributes(0, listenerAttributes);
        }

        /// <summary>
        /// Creates a new instance of <see cref="AudioSource"/> with given bank.
        /// </summary>
        /// <remarks>
        /// Example: CreateAudioSource("AudioSound, AudioSound.strings");
        /// </remarks>
        /// <param name="bankName">Bank name relative to <see cref="BankDirectory"/> without extension.</param>
        /// <param name="stringsBankName">String bank name relative to <see cref="BankDirectory"/> without extension.</param>
        /// <param name="entity">Source of the sound. If set to null, sound plays in 2D.</param>
        /// <returns>A new instance of <see cref="AudioSource"/>.</returns>
        public AudioSource CreateAudioSource(string bankName, string stringsBankName, Entity entity)
        {
            string mainBank = $@"{BankDirectory}\{bankName}.bank";
            string stringsBank = $@"{BankDirectory}\{stringsBankName}.bank";

            bool loaded = false;
            for(int i = 0; i < loadedBanks.Count; i++)
            {
                Bank bank = loadedBanks[i];
                bank.getPath(out string path);

                if(path.Contains(bankName))
                {
                    loaded = true;
                    continue;
                }    
            }

            if(!loaded)
            {
                var mainBankLoad = System.loadBankFile(mainBank, LOAD_BANK_FLAGS.NORMAL, out Bank bankMain);
                var stringBankLoad = System.loadBankFile(stringsBank, LOAD_BANK_FLAGS.NORMAL, out Bank bankString);

                loadedBanks.Add(bankMain);
                loadedBanks.Add(bankString);

                LogHelper.Log(this, $"Load Bank: {bankName}", mainBankLoad);
                LogHelper.Log(this, $"Load String Bank", stringBankLoad);
            }
            else
                LogHelper.Log(this, $"Banks already loaded... skipping", RESULT.OK);

            AudioSource audioSource = new AudioSource(this, entity);
            AudioSources.Add(audioSource);
            return audioSource;
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
            System.unloadAll();
            System.release();

            AudioPlayers.Remove(this);

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
    }
}
