using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using ReLogic.Content;
using System;
using Terraria;
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
