using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.DataStructures;

namespace ModdersToolkit.Tools.Fishing
{
	// code based on SpawnUI
	internal class UIFishingCatchesSlot : UIElement
	{
		public static Texture2D backgroundTexture = Main.inventoryBack9Texture;
		private float scale = .6f;
		public int itemType;
		public Item item;

		public UIFishingCatchesSlot(Item item) {
			this.item = item;
			this.itemType = item.type;
			this.Width.Set(backgroundTexture.Width * scale, 0f);
			this.Height.Set(backgroundTexture.Height * scale, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			//Main.instance.LoadItem(itemType);
			Texture2D itemTexture = Main.itemTexture[itemType];

			int width = itemTexture.Width;
			int height = itemTexture.Height;

			Rectangle itemDrawRectangle;
			DrawAnimation drawAnim = Main.itemAnimations[itemType];
			if (drawAnim != null) {
				itemDrawRectangle = drawAnim.GetFrame(itemTexture);
				height = itemDrawRectangle.Height;
				width = itemDrawRectangle.Width;
			}
			else {
				itemDrawRectangle = itemTexture.Bounds;
			}

			CalculatedStyle dimensions = base.GetInnerDimensions();
			spriteBatch.Draw(backgroundTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

			float drawScale = 2f;
			float availableWidth = (float)backgroundTexture.Width * scale;
			if (width * drawScale > availableWidth || height * drawScale > availableWidth) {
				if (width > height) {
					drawScale = availableWidth / width;
				}
				else {
					drawScale = availableWidth / height;
				}
			}
			Vector2 drawPosition = dimensions.Position();
			drawPosition.X += backgroundTexture.Width * scale / 2f - (float)width * drawScale / 2f;
			drawPosition.Y += backgroundTexture.Height * scale / 2f - (float)height * drawScale / 2f;

			Color color = (item.color != new Color(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue)) ? new Color(item.color.R, item.color.G, item.color.B, 255f) : new Color(1f, 1f, 1f);

			spriteBatch.Draw(itemTexture, drawPosition, itemDrawRectangle, color, 0, Vector2.Zero, drawScale, SpriteEffects.None, 0);

			if (IsMouseHovering) {
				string name;
				if (itemType <= 0) {
					name = "Nothing";
				}
				else {
					name = Lang.GetItemNameValue(item.type) + (item.modItem != null ? " [" + item.modItem.mod.Name + "]" : "");
				}
				Main.hoverItemName = name;
			}
		}

		public override int CompareTo(object obj) {
			UIFishingCatchesSlot other = obj as UIFishingCatchesSlot;
			return /*-1 * */itemType.CompareTo(other.itemType);
		}
	}
}
