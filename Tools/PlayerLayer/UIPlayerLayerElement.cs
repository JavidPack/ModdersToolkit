using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.PlayerLayer
{
	internal class UIPlayerLayerElement : UIElement
	{
		// Force Off/On - Name - State

		internal PlayerDrawLayer layer;
		internal UIBoolNDataValue selectionData;
		internal UITriStateCheckbox selectionCheckbox;

		public float order = 0;

		public bool ForceHide => selectionData.Data == false;
		public bool ForceShow => selectionData.Data == true;

		public UIPlayerLayerElement(PlayerDrawLayer layer) {
			Width.Set(0, 1f);
			Height.Set(20, 0f);

			this.layer = layer;
		}

		public override void OnInitialize() {
			base.OnInitialize();

			selectionData = new UIBoolNDataValue(layer.Name);
			selectionCheckbox = new UITriStateCheckbox(selectionData);
			Append(selectionCheckbox);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Vector2 pos = new Vector2(innerDimensions.Width + innerDimensions.X, innerDimensions.Y);

			// TODO: Add indicator to show hidden by inherited hiddenness. (e.g. if a parent is hidden, but not this one)
			if (ForceHide) {
				Utils.DrawBorderString(spriteBatch, "(Forced Hidden)", pos, Color.Red, anchorx: 1f);
			}
			else if (ForceShow) {
				Utils.DrawBorderString(spriteBatch, "(Forced Visible)", pos, Color.LightBlue, anchorx: 1f);
			}
			else if (layer.Visible) {
				Utils.DrawBorderString(spriteBatch, "(Visible)" , pos, Color.Green, anchorx:1f);
			}
			else {
				Utils.DrawBorderString(spriteBatch, "(Hidden)", pos, Color.Orange, anchorx: 1f);
			}

			if (IsMouseHovering) {
				// TODO: Detect if the enabled layer is actually drawing anything by counting drawInfo.DrawDataCache
				PlayerLayerTool.layerToDraw = layer;
				//PlayerDrawSet drawinfo = default(PlayerDrawSet);
				//var dimensions = PlayerLayerTool.playerLayerUI.mainPanel.GetDimensions().ToRectangle();

				//List<DrawData> _drawData = new List<DrawData>();
				//List<int> _dust = new List<int>();
				//List<int> _gore = new List<int>();

				//Vector2 drawPosition = new Vector2(dimensions.Right - 30, dimensions.Top - 50) + Main.screenPosition;
				////drawPosition = Main.MouseWorld;
				//drawinfo.BoringSetup(Main.LocalPlayer, _drawData, _dust, _gore, drawPosition, 0f, 0f, Vector2.Zero);

				//Main.NewText(drawPosition.ToString() + " " + Main.MouseWorld.ToString());

				//layer.DrawWithTransformationAndChildren(ref drawinfo);

				//PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawinfo);
				//PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawinfo);


				//spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Rectangle(dimensions.Right - 30, dimensions.Top - 50, 30, 50), Color.MistyRose);

				//spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 30, 50), Color.MistyRose);
			}
		}

		public override int CompareTo(object obj) {
			UIPlayerLayerElement other = obj as UIPlayerLayerElement;
			return order.CompareTo(other.order);
		}

		public void Reset() {
			selectionData.ResetToDefaultValue();
		}
	}
}
