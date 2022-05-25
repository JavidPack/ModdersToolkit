using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Sounds
{
	internal class SoundTool : Tool
	{
		internal static SoundUI soundUI;
		internal static bool logSounds;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Sound Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			soundUI = new SoundUI(Interface);
			soundUI.Activate();

			Interface.SetState(soundUI);

			On.Terraria.Audio.SoundPlayer.Play += SoundPlayer_Play;
		}

		public override void ClientTerminate() {
			Interface = null;

			soundUI?.Deactivate();
			soundUI = null;
		}


		public override void UIDraw() {
			if (Visible) {
				soundUI.Draw(Main.spriteBatch);
			}
		}

		private ReLogic.Utilities.SlotId SoundPlayer_Play(On.Terraria.Audio.SoundPlayer.orig_Play orig, Terraria.Audio.SoundPlayer self, ref Terraria.Audio.SoundStyle style, Vector2? position) {
			var result = orig(self, ref style, position);

			if (logSounds && Main.soundVolume != 0f) {
				if (style.Type == Terraria.Audio.SoundType.Sound) {
					float soundVolume = Main.soundVolume;
					Main.soundVolume = 0f;
					Main.NewText("");
					// This displays the SoundType data, not the ActiveSound, so pitch variance not taken into account.
					Main.NewText($"SoundPath: {style.SoundPath}{(style.Volume != 1 ? $", Volume: {style.Volume}" : "")}{(style.Pitch != 0 ? $", Pitch: {style.Pitch}" : "")}");
					//Main.NewText($"Type: {type}, Style: {Style}{(x != -1 ? $", x: {x}" : "")}{(y != -1 ? $", y: {y}" : "")}{(volumeScale != 1 ? $", volumeScale: {volumeScale}" : "")}{(pitchOffset != 0 ? $", pitchOffset: {pitchOffset}" : "")}");

					var frames = new StackTrace(true).GetFrames();
					Logging.PrettifyStackTraceSources(frames);
					int index = 2;
					while (frames[index].GetMethod().Name.Contains("PlaySound"))
						index++;
					var frame = frames[index];
					System.Reflection.MethodBase methodBase = frame.GetMethod();
					if (methodBase != null && methodBase.DeclaringType != null) {
						Main.NewText(methodBase.DeclaringType.FullName + "." + methodBase.Name + ":" + frame.GetFileLineNumber());
					}
					Main.soundVolume = soundVolume;
				}
			}

			return result;
		}
	}
}
