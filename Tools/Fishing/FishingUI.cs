using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.Fishing
{
	// code based on SpawnUI
	internal class FishingUI : UIToolState
	{
		internal UIPanel mainPanel;
		public UIList checklistList;
		private UserInterface userInterface;

		public FishingUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();

			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			height = 350;
			width = 200;
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Fishing Catches:", 0.85f);
			//	text.Top.Set(12f, 0f);
			//	text.Left.Set(12f, 0f);
			mainPanel.Append(text);

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
			Player player = Main.LocalPlayer;
			if (!FishingTool.ValidateSpawnPosition(player, out Vector2 spawnPosition)) {
				return;
			}

			Main.NewText("Calculating....");

			FishingTool.CalculateCatches(spawnPosition, player.HeldItem.shoot, out int poolSize, out bool lava, out bool honey);

			checklistList.Clear();

			float total = 0;
			foreach (var catchPair in FishingTool.catches) {
				total += catchPair.Value;
			}

			if (total > 0) {
				foreach (var catchPair in FishingTool.catches) {
					UIFishingCatchesInfo spawnInfo = new UIFishingCatchesInfo(catchPair.Key, catchPair.Value / total);
					checklistList.Add(spawnInfo);
				}
			}

			// Helpful message
			string liquidName = "water";
			if (lava) {
				liquidName = "lava";
			}
			else if (honey) {
				liquidName = "honey";
			}

			int fishingPower = player.FishingLevel();

			Main.NewText($"Tested {total} fishing cases with {fishingPower} fishing power in a pool of {liquidName} with size {poolSize}");
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
