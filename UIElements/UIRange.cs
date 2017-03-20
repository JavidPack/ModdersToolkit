using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.UI.Chat;
using Terraria.GameContent.UI.Elements;
using ModdersToolkit.Tools.Dusts;

namespace ModdersToolkit.UIElements
{
	class UIRange : UIElement
	{
		internal UIText label;
		internal UISlider slider;
		internal UIImageButton minus;
		internal UIImageButton plus;
		internal NewUITextBox input;
		private Func<float> _GetStatusFunction;
		private Action<float> _SlideKeyboardAction;
		internal IntDataRangeProperty intDataRangeProperty;
		//private Action validateInput;

		public UIRange(string labeltext, Func<float> getStatus, Action<float> setStatusKeyboard, Action validateInput, bool fine = false)
		{

			this.Height.Set(20f, 0f);
			this._GetStatusFunction = getStatus ?? (() => 0f);
			this._SlideKeyboardAction = setStatusKeyboard ?? ((s) => { });

			if (fine)
			{
				label = new UIText(labeltext, 0.85f);
				//label.Width.Set(0, .33f);
				label.HAlign = 0;
				Append(label);

				slider = new UISlider(null, _GetStatusFunction, _SlideKeyboardAction, null, 0, Color.AliceBlue);
				slider.Height.Set(16, 0f);
				slider.Width.Set(0, .25f);
				slider.HAlign = .25f;
				Append(slider);

				minus = new UIImageButton(ModdersToolkit.instance.GetTexture("UIElements/ButtonMinus"));
				minus.OnClick += Minus_OnClick;
				//minus.Height.Set(16, 0f);
				minus.Width.Set(0, .125f);
				minus.HAlign = .625f;
				Append(minus);

				plus = new UIImageButton(ModdersToolkit.instance.GetTexture("UIElements/ButtonPlus"));
				plus.OnClick += Plus_OnClick;
				//plus.Height.Set(16, 0f);
				plus.Width.Set(0, .125f);
				plus.HAlign = .75f;
				Append(plus);

				input = new NewUITextBox("input:", 0.85f);
				input.SetPadding(0);
				input.OnUnfocus += validateInput;
				input.PaddingLeft = 12;
				input.Width.Set(0, .25f);
				input.HAlign = 1f;
				Append(input);
			}
			else
			{
				label = new UIText(labeltext, 0.85f);
				//label.Width.Set(0, .33f);
				label.HAlign = 0;
				Append(label);

				slider = new UISlider(null, _GetStatusFunction, _SlideKeyboardAction, null, 0, Color.AliceBlue);
				slider.Height.Set(16, 0f);
				slider.Width.Set(0, .33f);
				slider.HAlign = .5f;
				Append(slider);

				input = new NewUITextBox("input:", 0.85f);
				input.SetPadding(0);
				input.OnUnfocus += validateInput;
				input.PaddingLeft = 12;
				input.Width.Set(0, .33f);
				input.HAlign = 1f;
				Append(input);
			}
		}

		private void Minus_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			_SlideKeyboardAction(_GetStatusFunction() - 1f/intDataRangeProperty.max);
		}

		private void Plus_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			_SlideKeyboardAction(_GetStatusFunction() + 1.1f/intDataRangeProperty.max);
		}
	}
}
