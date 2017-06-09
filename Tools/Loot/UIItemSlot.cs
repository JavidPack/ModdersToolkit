using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using System.Text;
using ModdersToolkit.UIElements;
using ModdersToolkit.Tools;
using ReLogic.Graphics;

namespace ModdersToolkit.Tools.Loot
{
	class UIItemSlot : UIElement
	{
		public static Texture2D backgroundTexture = Main.inventoryBack9Texture;
		private float scale = .6f;
		public int itemType;
		public Item item;

		public UIItemSlot(Item item)
		{
			this.item = item;
			this.itemType = item.type;
			this.Width.Set(backgroundTexture.Width * scale, 0f);
			this.Height.Set(backgroundTexture.Height * scale, 0f);
		}

		internal int frameCounter = 0;
		internal int frameTimer = 0;
		const int frameDelay = 7;
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (item != null)
			{
				CalculatedStyle dimensions = base.GetInnerDimensions();
				Rectangle rectangle = dimensions.ToRectangle();
				spriteBatch.Draw(backgroundTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				Texture2D itemTexture = Main.itemTexture[this.item.type];
				

				Rectangle rectangle2;
				if (Main.itemAnimations[item.type] != null)
				{
					rectangle2 = Main.itemAnimations[item.type].GetFrame(itemTexture);
				}
				else
				{
					rectangle2 = itemTexture.Frame(1, 1, 0, 0);
				}
				int height = rectangle2.Height;
				int width = rectangle2.Width;
				float drawScale = 2f;
				float availableWidth = (float)backgroundTexture.Width * scale;
				if (width * drawScale > availableWidth || height * drawScale > availableWidth)
				{
					if (width > height)
					{
						drawScale = availableWidth / width;
					}
					else
					{
						drawScale = availableWidth / height;
					}
				}
				Vector2 drawPosition = dimensions.Position();
				drawPosition.X += backgroundTexture.Width * scale / 2f - (float)width * drawScale / 2f;
				drawPosition.Y += backgroundTexture.Height * scale / 2f - (float)height * drawScale / 2f;

				this.item.GetColor(Color.White);
				spriteBatch.Draw(itemTexture, drawPosition, rectangle2, this.item.GetAlpha(Color.White), 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				if (this.item.color != default(Color))
				{
					spriteBatch.Draw(itemTexture, drawPosition, new Rectangle?(rectangle2), this.item.GetColor(Color.White), 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				}
				if (this.item.stack > 1)
				{
					spriteBatch.DrawString(Main.fontItemStack, this.item.stack.ToString(), new Vector2(drawPosition.X + 10f * scale, drawPosition.Y + 26f * scale), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				}

				if (IsMouseHovering)
				{
					Main.HoverItem = item.Clone();
					Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.mod.Name+"]" : ""));
				}
			}
		}
	}
}
