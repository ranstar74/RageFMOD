using GTA;
using GTA.Native;
using RageAudio.Memory.Classes;
using System;
using System.Drawing;
using System.Text;
using System.Threading;

namespace RageAudio
{
    /// <summary>
    /// Main class of the <see cref="RageAudio"/>.
    /// </summary>
    public class Main : Script
    {
        /// <summary>
        /// External thread that checks if game is paused.
        /// </summary>
        internal readonly Thread thread;

        /// <summary>
        /// Creates a new instance of <see cref="Main"/>.
        /// </summary>
        public Main()
        {
            // TODO: Reverb in tunnels

            Tick += OnTick;
            Aborted += OnAbort;

            thread = new Thread(() =>
            {
                while (true)
                {
                    AudioPlayer.IsPaused =
                        Memory.Classes.GameHud.IsPlayerSwitching ||
                        Memory.Classes.GameSettings.IsGamePaused;

                    Thread.Sleep(15);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Invokes Dispose for audio players.
        /// </summary>
        private void OnAbort(object sender, EventArgs e)
        {
            for (int i = 0; i < AudioPlayer.AudioPlayers.Count; i++)
            {
                AudioPlayer.AudioPlayers[i].Dispose();
            }
            thread?.Abort();
        }

        /// <summary>
        /// Calls update for audio players.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            AudioListener.Update();
            for (int i = 0; i < AudioPlayer.AudioPlayers.Count; i++)
            {
                AudioPlayer.AudioPlayers[i].Update();
            }

            StringBuilder sb = new StringBuilder();
            //sb.Append($"IsWindowFocused: {GameWindow.IsWindowFocused}\n");
            //sb.Append($"IsGamePaused: {GameSettings.IsGamePaused}\n");
            //sb.Append($"SoundVolume: {GameSettings.SoundVolume}\n");
            //sb.Append($"MusicVolume: {GameSettings.MusicVolume}\n");
            //sb.Append($"DialogBoost: {GameSettings.DialogBoost}\n");
            //sb.Append($"SoundOutputMode: {GameSettings.SoundOutputMode}\n");
            //sb.Append($"MuteSoundOnFocusLost: {GameSettings.MuteSoundOnFocusLost}\n");
            Vehicle veh = Game.Player.Character.CurrentVehicle;
            if (Game.Player.Character.IsInVehicle())
            {
                sb.Append($"Current Vehicle: {veh.MemoryAddress.ToString("X")}\n");
                sb.Append($"Reverb Level: 0.0\n");
                sb.Append($"Echo Level: 0.0\n");
                //sb.Append($"Listener velocity: {AudioListener.Velocity.Length():0}\n");
                //sb.Append($"Vehicle velocity: {veh.Velocity.Length():0}\n");

                GTA.UI.Screen.ShowHelpText(sb.ToString(), 1, false, false);
            }

            // TODO: Reverb in tunnels
        }
    }
}
