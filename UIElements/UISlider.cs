using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.UI.Chat;


/*
 * 		public static Texture2D colorSliderTexture;
		public static Texture2D colorBarTexture;
		public static Texture2D colorBlipTexture;
		Slider_Highlight?
		*/

namespace ModdersToolkit.UIElements
{
	class UISlider : UIElement
	{
		private Color _color;
		private Func<string> _TextDisplayFunction;
		private Func<float> _GetStatusFunction;
		private Action<float> _SlideKeyboardAction;
		private Action _SlideGamepadAction;
		private int _sliderIDInPage;
		private Texture2D _toggleTexture;
		private static UISlider currentDrag;
		private bool colorRange;

		public UISlider(Func<string> getText, Func<float> getStatus, Action<float> setStatusKeyboard, Action setStatusGamepad, int sliderIDInPage, Color color)
		{
			this._color = color;
			this._toggleTexture = TextureManager.Load("Images/UI/Settings_Toggle");
			this._TextDisplayFunction = getText ?? (() => "???");
			this._GetStatusFunction = getStatus ?? (() => 0f);
			this._SlideKeyboardAction = setStatusKeyboard ?? ((s) => { });
			this._SlideGamepadAction = setStatusGamepad ?? (() => { });
			this._sliderIDInPage = sliderIDInPage;
		}

		public override void Recalculate()
		{
			base.Recalculate();
		}

		public void SetHueMode(bool isHue)
		{
			colorRange = isHue;
		}

		int paddingY = 4;
		int paddingX = 5;
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetInnerDimensions();
			Rectangle rectangle = dimensions.ToRectangle();
			spriteBatch.Draw(Main.colorBarTexture, rectangle, Color.White);
			if (IsMouseHovering) spriteBatch.Draw(Main.colorHighlightTexture, rectangle, Main.OurFavoriteColor);
			rectangle.Inflate(-paddingX, -paddingY);
			//int num = 167;
			float scale = 1f;
			float x = (float)rectangle.X + 5f * scale;
			float y = (float)rectangle.Y + 4f * scale;

			x = rectangle.X;
			y = rectangle.Y;
			for (float i = 0f; i < rectangle.Width; i += 1f)
			{
				float amount = i / rectangle.Width;
				Color color = colorRange ? Main.hslToRgb(amount, 1f, 0.5f) : Color.Lerp(Color.Black, Color.White, amount);
				spriteBatch.Draw(Main.colorBlipTexture, new Vector2(x + i * scale, y),
					null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}

			float percent = _GetStatusFunction();
			percent = Utils.Clamp<float>(percent, 0f, 1f);

			spriteBatch.Draw(Main.colorSliderTexture,
				new Vector2(x + rectangle.Width * scale * percent, y + 4f * scale),
				null, Color.White, 0f,
				new Vector2(0.5f * (float)Main.colorSliderTexture.Width, 0.5f * (float)Main.colorSliderTexture.Height),
				scale, SpriteEffects.None, 0f);

			// LOGIC
			if (IsMouseHovering && Main.mouseLeft)
			{
				float newPerc = (Main.mouseX - rectangle.X) / (float)rectangle.Width;
				newPerc = Utils.Clamp<float>(newPerc, 0f, 1f);
				this._SlideKeyboardAction(newPerc);
			}

			return;

			float num = 6f;
			base.DrawSelf(spriteBatch);
			int lockstate = 0;
			int rightHover = -1;
			if (!Main.mouseLeft)
			{
				IngameOptions.rightLock = -1;
			}
			if (IngameOptions.rightLock == this._sliderIDInPage)
			{
				lockstate = 1;
			}
			else if (IngameOptions.rightLock != -1)
			{
				lockstate = 2;
			}
			//CalculatedStyle dimensions = base.GetDimensions();
			//float num3 = dimensions.Width + 1f;
			//Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
			//bool flag = false;
			bool flag2 = IsMouseHovering;
			if (lockstate == 1)
			{
				flag2 = true;
			}
			if (lockstate == 2)
			{
				flag2 = false;
			}
			//Vector2 baseScale = new Vector2(0.8f);
			//Color color = flag ? Color.Gold : (flag2 ? Color.White : Color.Silver);
			//color = Color.Lerp(color, Color.White, flag2 ? 0.5f : 0f);
			//Color color2 = flag2 ? this._color : this._color.MultiplyRGBA(new Color(180, 180, 180));
			Vector2 vector2 = new Vector2(dimensions.X, dimensions.Y);
			//Utils.DrawSettingsPanel(spriteBatch, vector2, num3, color2);
			vector2.X += 8f;
			vector2.Y += 2f + num;
			//		ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, this._TextDisplayFunction(), vector2, color, 0f, Vector2.Zero, baseScale, num3, 2f);
			vector2.X -= 17f;
			//Main.colorBarTexture.Frame(1, 1, 0, 0);
			vector2 = new Vector2(dimensions.X + dimensions.Width - 10f, dimensions.Y + 10f + num);
			IngameOptions.valuePosition = vector2;
			float newPercent = IngameOptions.DrawValueBar(spriteBatch, 1f, this._GetStatusFunction(), lockstate);
			if (IngameOptions.inBar || IngameOptions.rightLock == this._sliderIDInPage)
			{
				rightHover = this._sliderIDInPage;
				if (PlayerInput.Triggers.Current.MouseLeft /*&& !PlayerInput.UsingGamepad*/ && IngameOptions.rightLock == this._sliderIDInPage)
				{
					this._SlideKeyboardAction(newPercent);
				}
			}
			if (rightHover != -1 && IngameOptions.rightLock == -1)
			{
				IngameOptions.rightLock = rightHover;
			}
			//if (base.IsMouseHovering)
			//{
			//	this._SlideGamepadAction();
			//}
		}
	}
}
