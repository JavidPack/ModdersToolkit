using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;

namespace ModdersToolkit.Tools.Loot
{
	class UIMoneyDisplay : UIElement
	{
		public long coins;
		public UIMoneyDisplay()
		{
			Width.Set(100, 0f);
			Height.Set(26, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);

			CalculatedStyle innerDimensions = base.GetInnerDimensions();

			float shopx = innerDimensions.X;
			float shopy = innerDimensions.Y;

			int[] coinsArray = Utils.CoinsSplit(coins);
			for (int j = 0; j < 4; j++)
			{
				int num = (j == 0 && coinsArray[3 - j] > 99) ? -6 : 0;
				spriteBatch.Draw(Main.itemTexture[74 - j], new Vector2(shopx + 16f + (float)(24 * j), shopy + 12f), null, Color.White, 0f, Main.itemTexture[74 - j].Size() / 2f, 1f, SpriteEffects.None, 0f);
				Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, coinsArray[3 - j].ToString(), shopx + 5 + (float)(24 * j) + (float)num, shopy + 12, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
			}
		}
	}
}
