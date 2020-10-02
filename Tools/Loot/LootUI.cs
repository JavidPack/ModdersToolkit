using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.Tools.Spawns;
using ModdersToolkit.UIElements;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace ModdersToolkit.Tools.Loot
{
	class LootUI : UIState
	{
		internal UIMoneyDisplay money;
		internal UIPanel mainPanel;
		internal UIGrid npcGrid;
		internal UIList lootItemsList;
		private UserInterface userInterface;
		private bool updateNeededNPC;
		private bool updateNeededItems;

		public LootUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			mainPanel.Left.Set(-300f, 1f);
			mainPanel.Top.Set(-520f, 1f);
			mainPanel.Width.Set(400f, 0f);
			mainPanel.Height.Set(600f, 0f);
			mainPanel.BackgroundColor = new Color(73, 94, 171);

			UIText text = new UIText("NPC Loot:", 0.85f);
			mainPanel.Append(text);

			int top = 20;

			// TODO, search filter instead of button.

			UITextPanel<string> calculateButton = new UITextPanel<string>("Calculate");
			calculateButton.SetPadding(4);
			calculateButton.Width.Set(-10, 0.5f);
			calculateButton.Top.Set(top, 0f);
			//	calculateButton.Left.Set(6, 0f);
			//		calculateButton.OnClick += CaluculateButton_OnClick; ;
			mainPanel.Append(calculateButton);

			top += 30;

			npcGrid = new UIGrid(7);
			npcGrid.Top.Pixels = top;
			npcGrid.Width.Set(-25f, 1f); // leave space for scroll?
			npcGrid.Height.Set(200, 0f);
			npcGrid.ListPadding = 6f;
			mainPanel.Append(npcGrid);

			var npcGridScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			npcGridScrollbar.SetView(100f, 1000f);
			npcGridScrollbar.Top.Pixels = top;// + spacing;
			npcGridScrollbar.Height.Set(200 /*- spacing*/, 0f);
			npcGridScrollbar.HAlign = 1f;
			mainPanel.Append(npcGridScrollbar);
			npcGrid.SetScrollbar(npcGridScrollbar);

			updateNeededNPC = true;

			top += 200;

			money = new UIMoneyDisplay();
			money.Top.Pixels = top;
			mainPanel.Append(money);

			top += 30;

			lootItemsList = new UIList();
			lootItemsList.Top.Pixels = top;
			lootItemsList.Width.Set(-25f, 1f);
			lootItemsList.Height.Set(200, 0f);
			lootItemsList.ListPadding = 6f;
			mainPanel.Append(lootItemsList);

			var lootItemsScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			lootItemsScrollbar.SetView(100f, 1000f);
			lootItemsScrollbar.Top.Pixels = top;
			lootItemsScrollbar.Height.Set(200, 0f);
			lootItemsScrollbar.Left.Set(-20, 1f);
			mainPanel.Append(lootItemsScrollbar);
			lootItemsList.SetScrollbar(lootItemsScrollbar);

			Append(mainPanel);
		}

		internal void UpdateNPCGrid() {
			if (!updateNeededNPC) { return; }
			updateNeededNPC = false;

			npcGrid.Clear();
			for (int i = 0; i < Main.npcTexture.Length; i++) // TODO, right?
			{
				//if (Main.projName[i].ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
				{
					NPC npc = new NPC();
					npc.SetDefaults(i);

					var box = new UINPCSlot(npc);
					box.OnClick += Box_OnClick;
					npcGrid._items.Add(box);
					npcGrid._innerList.Append(box);
				}
			}

			npcGrid.UpdateOrder();
			npcGrid._innerList.Recalculate();
		}

		private void Box_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			var slot = evt.Target as UINPCSlot;

			//Calculate
			LootTool.CalculateLoot(slot.npcType);

			updateNeededItems = true;
			//queue an update
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateNPCGrid();
			UpdateItemsList();
		}


		internal void UpdateItemsList() {
			if (!updateNeededItems) { return; }
			updateNeededItems = false;

			lootItemsList.Clear();

			float total = 0;
			foreach (var spawn in LootTool.loots) {
				total += spawn.Value;
			}
			money.coins = 0;
			if (total > 0) {
				foreach (var spawn in LootTool.loots) {
					if (spawn.Key == ItemID.CopperCoin) {
						money.coins = (int)(spawn.Value / (float)LootTool.NumberLootExperiments);
						continue;
					}
					UILootInfo lootInfo = new UILootInfo(spawn.Key, spawn.Value / (float)LootTool.NumberLootExperiments);
					lootItemsList.Add(lootInfo);
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
