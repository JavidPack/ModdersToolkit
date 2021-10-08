using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace ModdersToolkit.UIElements
{
	//class UI2DRange : UIElement
	//{
	//
	//}

	internal class UI2DRange<T> : UIElement
	{
		private UIRangedDataValue<T> dataX;
		private UIRangedDataValue<T> dataY;

		public UI2DRange(UIRangedDataValue<T> dataX, UIRangedDataValue<T> dataY) {
			this.dataX = dataX;
			this.dataY = dataY;

			this.Height.Set(20f, 0f);
			this.Width.Set(20f, 0f);
		}

		// TODO, choose Y direction, up negative or positive?
		// draw Axis, make circular option, make rotation option(ouside values only.)
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			CalculatedStyle dimensions = base.GetInnerDimensions();
			Rectangle rectangle = dimensions.ToRectangle();
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, Color.Red);

			float x = dataX.GetProportion();
			float y = dataY.GetProportion();

			var position = dimensions.Position();
			position.X += x * dimensions.Width;
			position.Y += y * dimensions.Height;
			var blipRectangle = new Rectangle((int)position.X - 2, (int)position.Y - 2, 4, 4);

			spriteBatch.Draw(TextureAssets.MagicPixel.Value, blipRectangle, Color.Black);

			if (IsMouseHovering && Main.mouseLeft) {
				float newPerc = (Main.mouseX - rectangle.X) / (float)rectangle.Width;
				newPerc = Utils.Clamp<float>(newPerc, 0f, 1f);
				dataX.SetProportion(newPerc);

				newPerc = (Main.mouseY - rectangle.Y) / (float)rectangle.Height;
				newPerc = Utils.Clamp<float>(newPerc, 0f, 1f);
				dataY.SetProportion(newPerc);
			}
		}
	}
}
