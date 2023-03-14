using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.Spawns
{
	internal class SpawnTool : Tool
	{
		internal static SpawnUI spawnUI;
		internal static Dictionary<int, int> spawns;
		internal static bool calculatingSpawns;
		internal static FieldInfo spawnRateFieldInfo;
		internal static FieldInfo maxSpawnsFieldInfo;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle NPC Spawn Tool";
			spawns = new Dictionary<int, int>();
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			spawnUI = new SpawnUI(Interface);
			spawnUI.Activate();

			Interface.SetState(spawnUI);

			Terraria.On_NPC.NewNPC += NPC_NewNPC;
			spawnRateFieldInfo = typeof(NPC).GetField("spawnRate", BindingFlags.Static | BindingFlags.NonPublic);
			maxSpawnsFieldInfo = typeof(NPC).GetField("maxSpawns", BindingFlags.Static | BindingFlags.NonPublic);
		}

		public override void ClientTerminate() {
			Interface = null;

			spawnUI.Deactivate();
			spawnUI = null;
		}

		public override void UIDraw() {
			if (Visible) {
				spawnUI.Draw(Main.spriteBatch);
			}
		}

		internal static void CalculateSpawns() {
			spawns.Clear();

			calculatingSpawns = true;
			for (int i = 0; i < 10000; i++) {
				NPC.SpawnNPC();
			}
			calculatingSpawns = false;
		}

		private int NPC_NewNPC(Terraria.On_NPC.orig_NewNPC orig, Terraria.DataStructures.IEntitySource source, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target) {
			if (calculatingSpawns) {
				int currentCount;
				spawns.TryGetValue(Type, out currentCount);
				spawns[Type] = currentCount + 1;
				
				//var npc = Terraria.ID.ContentSamples.NpcsByNetId[Type];
				//Dust.QuickBox(new Vector2(X, Y), npc.Size, 5, Color.White, null);
				//Utils.DrawLine(Main.spriteBatch, Main.LocalPlayer.Center, new Vector2(X, Y), Color.Red, Color.Red, 4);
				
				return 200;
			}

			int result = orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
			return result;
		}
	}
}
