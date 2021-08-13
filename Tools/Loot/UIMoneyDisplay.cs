using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace ModdersToolkit.Tools.Loot
{
	internal class UIMoneyDisplay : UIElement
	{
		public long coins;
		public UIMoneyDisplay() {
			Width.Set(100, 0f);
			Height.Set(26, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Red * 0.6f);

			CalculatedStyle innerDimensions = base.GetInnerDimensions();

			float shopx = innerDimensions.X;
			float shopy = innerDimensions.Y;

			int[] coinsArray = Utils.CoinsSplit(coins);
			for (int j = 0; j < 4; j++) {
				int num = (j == 0 && coinsArray[3 - j] > 99) ? -6 : 0;
				spriteBatch.Draw(TextureAssets.Item[74 - j].Value, new Vector2(shopx + 16f + (float)(24 * j), shopy + 12f), null, Color.White, 0f, TextureAssets.Item[74 - j].Size() / 2f, 1f, SpriteEffects.None, 0f);
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, coinsArray[3 - j].ToString(), shopx + 5 + (float)(24 * j) + (float)num, shopy + 12, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
			}
		}
	}
}
