using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace ModdersToolkit.Tools.Loot
{
	internal class LootTool : Tool
	{
		internal static LootUI lootUI;

		// expert toggle?

		public override void Initialize() {
			ToggleTooltip = "Click to toggle NPC Loot Tool";
			loots = new Dictionary<int, int>();
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			lootUI = new LootUI(Interface);
			lootUI.Activate();

			Interface.SetState(lootUI);
		}

		public override void ClientTerminate() {
			Interface = default;

			lootUI?.Deactivate();
			lootUI = default;
		}

		public override void Terminate() {
			loots?.Clear();
			loots = default;
		}


		public override void UIDraw() {
			if (Visible) {
				lootUI.Draw(Main.spriteBatch);
			}
		}

		// calculate loot. 
		// Prevent downed somehow?
		// automate?
		// prevent kill counts by swapping out npckills data

		public const int NumberLootExperiments = 500;
		internal static Dictionary<int, int> loots;

		internal static void CalculateLoot(int npcid) {
			loots.Clear();

			int[] killCounts = new int[NPC.killCount.Length];
			Utils.Swap(ref killCounts, ref NPC.killCount);

			NPC[] npcs = new NPC[201];

			for (int i = 0; i < npcs.Length; i++) {
				npcs[i] = new NPC();
			}

			Utils.Swap(ref npcs, ref Main.npc);

			Item[] items = new Item[401];
			for (int i = 0; i < items.Length; i++) {
				items[i] = new Item();
			}
			Utils.Swap(ref items, ref Main.item);

			//todo, 401 limit prevents good percentage calculations.
			int currentCount, type, stack;

			for (int i = 0; i < NumberLootExperiments; i++) {
				NPC npc = Main.npc[NPC.NewNPC(0, 0, npcid)];
				npc.NPCLoot();
				npc.active = false;

				foreach (var item in Main.item) {
					if (item.active) {
						type = item.type;
						stack = item.stack;

						if (type == ItemID.PlatinumCoin) {
							type = ItemID.CopperCoin;
							stack = stack * 1000000;
						}
						else if (type == ItemID.GoldCoin) {
							type = ItemID.CopperCoin;
							stack = stack * 10000;
						}
						else if (type == ItemID.SilverCoin) {
							type = ItemID.CopperCoin;
							stack = stack * 100;
						}

						loots.TryGetValue(type, out currentCount);
						loots[type] = currentCount + stack;
						item.active = false;
					}
					else {
						break;
					}
				}
			}

			Utils.Swap(ref items, ref Main.item);
			Utils.Swap(ref npcs, ref Main.npc);
			Utils.Swap(ref killCounts, ref NPC.killCount);


			// calculate dropped items?
		}
	}
}
