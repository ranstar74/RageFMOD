using GTA;
using RageAudio.Memory;
using RageAudio.Memory.Classes;
using RageAudio.Memory.Classes.Entity;
using System;
using System.Linq;
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
            Tick += OnTick;
            Aborted += OnAbort;

            // Background thread that checks if game is paused
            thread = new Thread(() =>
            {
                bool previousPaused = false;
                while (true)
                {
                    bool isPaused = GameSettings.IsGamePaused;

                    if(isPaused != previousPaused)
                    {
                        for(int i = 0; i < AudioPlayer.AudioPlayers.Count; i++)
                        {
                            AudioPlayer.AudioPlayers[i].IsPaused = isPaused;
                        }
                    }

                    previousPaused = isPaused;
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

        private CVehicle CVehicle;
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
            sb.Append($"IsWindowFocused: {GameWindow.IsWindowFocused}\n");
            sb.Append($"IsGamePaused: {GameSettings.IsGamePaused}\n");
            sb.Append($"SoundVolume: {GameSettings.SoundVolume}\n");
            sb.Append($"MusicVolume: {GameSettings.MusicVolume}\n");
            sb.Append($"DialogBoost: {GameSettings.DialogBoost}\n");
            sb.Append($"SoundOutputMode: {GameSettings.SoundOutputMode}\n");
            sb.Append($"MuteSoundOnFocusLost: {GameSettings.MuteSoundOnFocusLost}\n");
            GTA.UI.Screen.ShowHelpText(sb.ToString(), 1, false, false);

            //Vehicle veh = Game.Player.Character.CurrentVehicle;

            //if (Game.Player.Character.IsInVehicle())
            //{
            //    StringBuilder sb = new StringBuilder();

            //    if (CVehicle == null || CVehicle?.MemoryAddress != veh.MemoryAddress)
            //    {
            //        CVehicle = new CVehicle(veh.MemoryAddress);
            //    }

            //    sb.Append($"Current Vehicle: {CVehicle.MemoryAddress.ToString("X")}\n");
            //    //sb.Append($"Veh Audio: {CVehicle.VehicleAudio.MemoryAddress.ToString("X")}\n");
            //    //sb.Append($"Audio Env Group: {CVehicle.VehicleAudio.EnvironmentGroup.MemoryAddress.ToString("X")}\n");
            //    //sb.Append($"Reverb Level: {CVehicle.VehicleAudio.EnvironmentGroup.Reverb}\n");
            //    ////sb.Append($"Environment Reverb Level: {CVehicle.VehicleAudio.EnvironmentGroup.EnvironmentReverb}\n");
            //    //sb.Append($"Echo Level: {CVehicle.VehicleAudio.EnvironmentGroup.Echo}\n");
            //    //sb.Append($"Cam Pos: {GameWorld.RenderingCamPosition}\n");
            //    sb.Append($"Cam Pos: {GameplayCamera.Position}\n");

            //    ////sb.Append($"Listener velocity: {AudioListener.Velocity.Length():0}\n");
            //    ////sb.Append($"Vehicle velocity: {veh.Velocity.Length():0}\n");
            //    sb.Append($"Front Vec: {GameplayCamera.ForwardVector}\n");
            //    sb.Append($"Up Vec : {GameplayCamera.UpVector}\n");
            //    //sb.Append($"{GameplayCamera.MemoryAddress.ToString("X")}");

            //    //GTA.UI.Screen.ShowHelpText(sb.ToString(), 1, false, false);

            //    //GTA.UI.Screen.ShowSubtitle(Test.ToString("X"));
            //}
            //if (NativeMemory.CViewportGameAddr != IntPtr.Zero)
            //{
            //    unsafe
            //    {
            //        var pos = NativeMemory.CViewportGame->Position;
            //        GTA.UI.Screen.ShowSubtitle($"Cam Pos: {pos.X:0.00} {pos.Y:0.00} {pos.Z:0.00}");
            //    }
            //}
        }
    }
}
