using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.PlayerLayer
{
	internal class PlayerLayerUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface _userInterface;

		public UIList playerLayerList;
		public List<UIPlayerLayerElement> playerLayersElements = new List<UIPlayerLayerElement>();
		public bool updateNeeded;

		public PlayerLayerUI(UserInterface userInterface) {
			this._userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mouseAndScrollBlockers.Add(mainPanel);
			width = 400;
			height = 520;
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = Color.Yellow * 0.8f;

			int top = 0;
			UIText text = new UIText("Player Layer:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);

			UITextPanel<string> resetButton = new UITextPanel<string>("Reset");
			resetButton.SetPadding(4);
			resetButton.HAlign = 1f;
			resetButton.OnLeftClick += (a, b) => {
				foreach (var playerLayer in playerLayersElements) {
					playerLayer.Reset();
				}
			};
			mainPanel.Append(resetButton);
			
			//var playerImagePreview = new UIImage(Terraria.GameContent.TextureAssets.InventoryBack9);
			//playerImagePreview.Top.Set(top, 0f);
			//playerImagePreview.Left.Set(-56, 0);
			//playerImagePreview.HAlign = 1f;
			//mainPanel.Append(playerImagePreview);
			top += 30;
			top += 30;

			playerLayerList = new UIList();
			playerLayerList.Top.Pixels = top;
			//autoTrashGrid.Left.Pixels = spacing;
			playerLayerList.Width.Set(-25f, 1f); // leave space for scroll?
			playerLayerList.Height.Set(-top, 1f);
			playerLayerList.ListPadding = 6f;
			mainPanel.Append(playerLayerList);

			// this will initialize grid
			updateNeeded = true;

			var playerLayerListScrollbar = new UIElements.FixedUIScrollbar(_userInterface);
			playerLayerListScrollbar.SetView(100f, 1000f);
			playerLayerListScrollbar.Top.Pixels = top;// + spacing;
			playerLayerListScrollbar.Height.Set(-top, 1f);
			playerLayerListScrollbar.HAlign = 1f;
			mainPanel.Append(playerLayerListScrollbar);
			playerLayerList.SetScrollbar(playerLayerListScrollbar);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		internal void UpdateList() {
			if (!updateNeeded) { return; }
			updateNeeded = false;

			playerLayerList.Clear();
			playerLayersElements.Clear();
			int order = 0;
			int depth = 0;
			foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.DrawOrder) {
				MakeAndAddCheckbox(layer, ref order, ref depth);
				playerLayersElements[^1].MarginBottom = 10f;
			}
			playerLayerList.AddRange(playerLayersElements);

			void MakeAndAddCheckbox(PlayerDrawLayer layer, ref int index, ref int depth) {
				foreach (var before in layer.ChildrenBefore) {
					depth++;
					MakeAndAddCheckbox(before, ref index, ref depth);
					depth--;
				}

				var box = new UIPlayerLayerElement(layer);
				box.Initialize();
				box.Left.Set(depth * 10, 0f);
				box.Width.Set(-depth * 10, 1f);
				box.order = index++;
				playerLayersElements.Add(box);

				foreach (var after in layer.ChildrenAfter) {
					depth++;
					MakeAndAddCheckbox(after, ref index, ref depth);
					depth--;
				}				
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateList();
		}

		protected override void DrawChildren(SpriteBatch spriteBatch) {
			base.DrawChildren(spriteBatch);
			CalculatedStyle dimensions = mainPanel.GetInnerDimensions();
			Rectangle rectangle = dimensions.ToRectangle();
			spriteBatch.Draw(Terraria.GameContent.TextureAssets.InventoryBack9.Value, dimensions.Position() + new Vector2(200, 0), null, Color.White, 0f, Vector2.Zero, new Vector2(2f, 2f), SpriteEffects.None, 0f);
			if(PlayerLayerTool.layerToDraw != null) {
				PlayerDrawSet drawinfo = default(PlayerDrawSet);
				var dimensionsRec = PlayerLayerTool.playerLayerUI.mainPanel.GetDimensions().ToRectangle();

				List<DrawData> _drawData = new List<DrawData>();
				List<int> _dust = new List<int>();
				List<int> _gore = new List<int>();

				Vector2 drawPosition = new Vector2(dimensionsRec.Right - 30, dimensionsRec.Top - 50) + Main.screenPosition;
				//drawPosition = Main.MouseWorld;
				drawPosition = dimensions.Position() + new Vector2(200, 0) + Main.screenPosition;

				var center = new Vector2(dimensionsRec.Center().X, dimensionsRec.Y);
				drawPosition = dimensions.Position() + new Vector2(240, 30) + Main.screenPosition;

				drawinfo.BoringSetup(Main.LocalPlayer, _drawData, _dust, _gore, drawPosition, 0f, Main.LocalPlayer.fullRotation, Main.LocalPlayer.fullRotationOrigin);
				drawinfo.colorMount = Color.White;

				PlayerLayerTool.layerToDraw.DrawWithTransformationAndChildren(ref drawinfo);
				PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawinfo);
				PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawinfo);
			}
		}
	}

	internal class PlayerLayerModPlayer : ModPlayer
	{
		internal static PlayerDrawSet LocalPlayerDrawSet;
		public override void HideDrawLayers(PlayerDrawSet drawInfo) {
			if (PlayerLayerTool.playerLayerUI.updateNeeded)
				return;

			if(Player.whoAmI == Main.myPlayer)
				LocalPlayerDrawSet = drawInfo;

			int index = 0;
			foreach (var layer in PlayerDrawLayerLoader.DrawOrder) {
				Traverse(layer, ref index);
			}

			void Traverse(PlayerDrawLayer layer, ref int index) {
				foreach (var before in layer.ChildrenBefore) {
					Traverse(before, ref index);
				}

				// TODO: ForceShow support
				if (PlayerLayerTool.playerLayerUI.playerLayersElements[index].ForceHide) {
					layer.Hide();
				}
				index++;

				foreach (var after in layer.ChildrenAfter) {
					Traverse(after, ref index);
				}
			}
		}

		// TODO: Tool for tweaking this data?
		//public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
		//	base.ModifyDrawInfo(ref drawInfo);
		//}
	}
}