using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.Tools.Dusts;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.UIElements
{
	class UIRange<T> : UIElement
	{
		internal UIText label;
		internal UISlider slider;
		internal UIImageButton minus;
		internal UIImageButton plus;
		internal NewUITextBox input;
		private Func<float> _GetProportion;
		private Action<float> _SetProportion;
		internal IntDataRangeProperty intDataRangeProperty;
		//private Action validateInput;
		private UIRangedDataValue<T> data;
		// todo, make a UIRangedDataValue subclass?, make this generic?

		public UIRange(UIRangedDataValue<T> data) {
			this.data = data;
			this.Height.Set(20f, 0f);
			this._GetProportion = data.GetProportion;
			this._SetProportion = data.SetProportion;

			label = new UIText(data.label, 0.85f);
			label.HAlign = 0;
			label.VAlign = 0.5f;
			Append(label);

			slider = new UISlider(null, _GetProportion, _SetProportion, null, 0, Color.AliceBlue);
			slider.Height.Set(16, 0f);
			slider.Width.Set(0, .4f);
			slider.Left.Set(0, .4f);
			slider.VAlign = 0.5f;
			Append(slider);

			input = new NewUITextBox("input:", 0.85f);
			input.SetPadding(0);
			input.OnUnfocus += () => data.ParseValue(input.Text);
			input.Width.Set(0, .16f);
			input.HAlign = 1f;
			input.VAlign = 0.5f;
			Append(input);

			data.OnValueChanged += Data_OnValueChanged;
			data.SetValue(data.Data);
		}

		bool debugDraw = false;
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (debugDraw) {
				Rectangle hitbox = GetInnerDimensions().ToRectangle();
				Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);

				hitbox = label.GetInnerDimensions().ToRectangle();
				//hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightCyan * 0.6f);

				hitbox = slider.GetOuterDimensions().ToRectangle();
				Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.AliceBlue * 0.6f);

				hitbox = input.GetOuterDimensions().ToRectangle();
				Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Yellow * 0.6f);

				if (minus != null) {
					hitbox = minus.GetOuterDimensions().ToRectangle();
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Green * 0.6f);
					hitbox = plus.GetOuterDimensions().ToRectangle();
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.BlueViolet * 0.6f);
				}
			}
		}

		private void Data_OnValueChanged() {
			input.SetText(data.GetValueString());
		}

		public UIRange(string labeltext, Func<float> getProportion, Action<float> setProportion, Action validateInput, bool fine = false) {
			this.Height.Set(20f, 0f);
			this._GetProportion = getProportion ?? (() => 0f);
			this._SetProportion = setProportion ?? ((s) => { });

			if (fine) {
				label = new UIText(labeltext, 0.85f);
				label.Width.Set(0, .25f);
				label.VAlign = 0.5f;
				label.HAlign = 0;
				Append(label);

				slider = new UISlider(null, _GetProportion, _SetProportion, null, 0, Color.AliceBlue);
				slider.Height.Set(16, 0f);
				slider.Width.Set(0, .25f);
				slider.Left.Set(0, .25f);
				slider.VAlign = 0.5f;
				//slider.HAlign = .25f;
				Append(slider);

				minus = new UIImageButton(ModdersToolkit.instance.GetTexture("UIElements/ButtonMinus"));
				minus.OnClick += Minus_OnClick;
				//minus.Height.Set(16, 0f);
				minus.Width.Set(0, .125f);
				minus.Left.Set(2, .5f);
				//minus.Left.Set(0, .5f);
				//minus.HAlign = .5f;
				//minus.HAlign = .625f;
				Append(minus);

				plus = new UIImageButton(ModdersToolkit.instance.GetTexture("UIElements/ButtonPlus"));
				plus.OnClick += Plus_OnClick;
				//plus.Height.Set(16, 0f);
				plus.Width.Set(0, .125f);
				plus.Left.Set(2, .625f);
				plus.VAlign = 0.5f;
				//plus.HAlign = .75f;
				Append(plus);

				input = new NewUITextBox("input:", 0.85f);
				input.SetPadding(0);
				input.OnUnfocus += validateInput;
				//input.PaddingLeft = 12;
				input.Width.Set(0, .25f);
				input.Left.Set(0, .75f);
				input.VAlign = 0.5f;
				//input.HAlign = 1f;
				Append(input);
			}
			else {
				label = new UIText(labeltext, 0.85f);
				label.Width.Set(0, .33f);
				label.VAlign = 0.5f;
				//label.HAlign = -0.5f;
				Append(label);

				slider = new UISlider(null, _GetProportion, _SetProportion, null, 0, Color.AliceBlue);
				slider.Height.Set(16, 0f);
				slider.Width.Set(0, .33f);
				//slider.Left.Precent = 0.5f;
				slider.Left.Precent = 0.33f;
				slider.VAlign = 0.5f;
				//slider.HAlign = .5f;
				Append(slider);

				input = new NewUITextBox("input:", 0.85f);
				input.SetPadding(0);
				input.OnUnfocus += validateInput;
				//input.PaddingLeft = 12;
				input.Width.Set(0, .33f);
				//input.HAlign = 1f;
				input.VAlign = 0.5f;
				input.Left.Precent = 0.66f;
				Append(input);
			}
		}

		private void Minus_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			_SetProportion(_GetProportion() - 1f / intDataRangeProperty.max);
		}

		private void Plus_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			_SetProportion(_GetProportion() + 1.1f / intDataRangeProperty.max);
		}
	}
}
