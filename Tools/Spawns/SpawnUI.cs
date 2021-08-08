using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.Spawns
{
	internal class SpawnUI : UIToolState
	{
		internal UIPanel mainPanel;
		public UIList checklistList;
		private UserInterface userInterface;

		public SpawnUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			height = 350;
			width = 200;
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("NPC Spawns:", 0.85f);
			//	text.Top.Set(12f, 0f);
			//	text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			/*
			UITextPanel<string> spawnTownNPCButton = new UITextPanel<string>("Spawn\nTownNPC");
			spawnTownNPCButton.OnClick += (a, b) =>
			{
				Main.NewText(Main.checkForSpawns);
				Main.checkForSpawns = 7200;
				WorldGen.spawnDelay = 20;
				//var coords = Main.LocalPlayer.Center.ToPoint();
				//WorldGen.TrySpawningTownNPC(coords.X, coords.Y);
			};
			spawnTownNPCButton.Top.Set(0, 0f);
			spawnTownNPCButton.Width.Set(-10, 0.5f);
			spawnTownNPCButton.HAlign = 1;
			spawnTownNPCButton.SetPadding(4);
			mainPanel.Append(spawnTownNPCButton);
			*/

			int top = 20;

			UITextPanel<string> calculateButton = new UITextPanel<string>("Calculate");
			calculateButton.SetPadding(4);
			calculateButton.Width.Set(-10, 0.5f);
			calculateButton.Top.Set(top, 0f);
			//	calculateButton.Left.Set(6, 0f);
			calculateButton.OnClick += CalculateButton_OnClick;
			mainPanel.Append(calculateButton);

			top += 28;

			checklistList = new UIList();
			//	checklistList.SetPadding(6);
			checklistList.Top.Pixels = top;
			checklistList.Width.Set(-25f, 1f);
			checklistList.Height.Set(-top, 1f);
			checklistList.ListPadding = 6f;
			mainPanel.Append(checklistList);

			var checklistListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			checklistListScrollbar.SetView(100f, 1000f);
			//checklistListScrollbar.Height.Set(0f, 1f);
			checklistListScrollbar.Top.Pixels = top;
			checklistListScrollbar.Height.Set(-top, 1f);
			checklistListScrollbar.Left.Set(-20, 1f);
			//checklistListScrollbar.HAlign = 1f;
			mainPanel.Append(checklistListScrollbar);
			checklistList.SetScrollbar(checklistListScrollbar);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		private void CalculateButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.NewText("Calculating....");

			SpawnTool.CalculateSpawns();

			checklistList.Clear();

			float total = 0;
			foreach (var spawn in SpawnTool.spawns) {
				total += spawn.Value;
			}

			if (total > 0) {
				foreach (var spawn in SpawnTool.spawns) {
					UINPCSpawnInfo spawnInfo = new UINPCSpawnInfo(spawn.Key, spawn.Value / total);
					checklistList.Add(spawnInfo);
				}
			}

			Main.NewText($"spawnRate: {SpawnTool.spawnRate}");
			Main.NewText($"maxSpawns: {SpawnTool.maxSpawns}");
			//Main.NewText($"activeNPCs: {Main.LocalPlayer.activeNPCs}");
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
