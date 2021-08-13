using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.UIElements
{
	internal class NewUITextBoxMultiLine : UITextPanel<string>
	{
		internal bool focused = false;
		private int _cursor;
		private int _frameCount;
		//private int _maxLength = 60;
		private int _maxLineLength = 45;
		private string hintText;
		public event Action OnFocus;
		public event Action OnUnfocus;
		public event Action OnTextChanged;
		public event Action OnTabPressed;
		public event Action OnEnterPressed;
		public event Action OnUpPressed;
		internal bool unfocusOnEnter = true;
		internal bool unfocusOnTab = true;


		public NewUITextBoxMultiLine(string text, float textScale = 1, bool large = false) : base("", textScale, large) {
			hintText = text;
			SetPadding(0);
			//			keyBoardInput.newKeyEvent += KeyboardInput_newKeyEvent;
		}

		public override void RightClick(UIMouseEvent evt) {
			base.RightClick(evt);
			SetText("");
		}

		public override void Click(UIMouseEvent evt) {
			Focus();
			base.Click(evt);
		}

		public void SetUnfocusKeys(bool unfocusOnEnter, bool unfocusOnTab) {
			this.unfocusOnEnter = unfocusOnEnter;
			this.unfocusOnTab = unfocusOnTab;
		}

		public override int CompareTo(object obj) {
			NewUITextBoxMultiLine text = obj as NewUITextBoxMultiLine;
			if (text != null) {
				return 0;
			}
			UICodeEntry other = obj as UICodeEntry;
			return other.num.CompareTo(int.MaxValue);
		}

		//void KeyboardInput_newKeyEvent(char obj)
		//{
		//	// Problem: keyBoardInput.newKeyEvent only fires on regular keyboard buttons.

		//	if (!focused) return;
		//	if (obj.Equals((char)Keys.Back)) // '\b'
		//	{
		//		Backspace();
		//	}
		//	else if (obj.Equals((char)Keys.Enter))
		//	{
		//		Unfocus();
		//		Main.chatRelease = false;
		//	}
		//	else if (Char.IsLetterOrDigit(obj))
		//	{
		//		Write(obj.ToString());
		//	}
		//}

		public void Unfocus() {
			if (focused) {
				focused = false;
				Main.blockInput = false;

				OnUnfocus?.Invoke();
			}
		}

		public void Focus() {
			if (!focused) {
				Main.clrInput();
				focused = true;
				Main.blockInput = true;

				OnFocus?.Invoke();
			}
		}

		public override void Update(GameTime gameTime) {
			Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (!ContainsPoint(MousePosition) && Main.mouseLeft) {
				// TODO, figure out how to refocus without triggering unfocus while clicking enable button.
				Unfocus();
			}
			base.Update(gameTime);
		}

		public void Write(string text) {
			base.SetText(base.Text.Insert(this._cursor, text));
			this._cursor += text.Length;
			_cursor = Math.Min(Text.Length, _cursor);

			int lines = text.Split('\n').Length;

			Recalculate();
			this.MinHeight.Set(-4 + lines * 28 + this.PaddingTop + this.PaddingBottom, 0f);

			OnTextChanged?.Invoke();
		}

		public void WriteAll(string text) {
			bool changed = text != Text;
			base.SetText(text);
			this._cursor = text.Length;
			//_cursor = Math.Min(Text.Length, _cursor);
			Recalculate();

			if (changed) {
				OnTextChanged?.Invoke();
			}
		}

		public override void SetText(string text, float textScale, bool large) {
			string lastLine = text.Split('\n').Last();
			if (lastLine.Length > this._maxLineLength) {
				//text = text + "\n";
			}
			//if (text.ToString().Length > this._maxLength)
			//{
			//	text = text.ToString().Substring(0, this._maxLength);
			//}
			base.SetText(text, textScale, large);

			int lines = text.Split('\n').Length;
			this.MinHeight.Set(-4 + lines * 28 + this.PaddingTop + this.PaddingBottom, 0f);

			//this.MinWidth.Set(120, 0f);

			this._cursor = Math.Min(base.Text.Length, this._cursor);

			OnTextChanged?.Invoke();
		}

		//public void SetTextMaxLength(int maxLength)
		//{
		//	this._maxLength = maxLength;
		//}

		public void Backspace() {
			if (this._cursor == 0) {
				return;
			}
			base.SetText(base.Text.Substring(0, base.Text.Length - 1));
			Recalculate();
		}

		public void CursorLeft() {
			if (this._cursor == 0) {
				return;
			}
			this._cursor--;
		}

		public void CursorRight() {
			if (this._cursor < base.Text.Length) {
				this._cursor++;
			}
		}

		private static bool JustPressed(Keys key) {
			return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.LightCyan * 0.6f);

			if (focused) {
				Terraria.GameInput.PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				// This might work.....assuming chat isn't open
				var a = Main.GetInputText(Text);


				var lines = a.Split('\n');
				string lastLine = lines.Last();
				if (lastLine.Length > this._maxLineLength + 1) {
					var orig = a.ToCharArray();
					char last = orig[orig.Length - 1];
					orig[orig.Length - 1] = '\n';
					a = new string(orig) + last;
				}

				WriteAll(a);

				if (JustPressed(Keys.Tab)) {
					if (unfocusOnTab)
						Unfocus();
					//	Main.NewText("Tab");
					OnTabPressed?.Invoke();
				}

				if (JustPressed(Keys.Enter)) {
					//	Main.NewText("Enter");
					if (unfocusOnEnter)
						Unfocus();
					OnEnterPressed?.Invoke();
				}
				if (JustPressed(Keys.Up)) {
					OnUpPressed?.Invoke();
				}

			}
			CalculatedStyle innerDimensions2 = base.GetInnerDimensions();
			Vector2 pos2 = innerDimensions2.Position();
			if (IsLarge) {
				pos2.Y -= 10f * TextScale * TextScale;
			}
			else {
				pos2.Y -= 2f * TextScale;
			}
			//pos2.X += (innerDimensions2.Width - TextSize.X) * 0.5f;
			if (IsLarge) {
				Utils.DrawBorderStringBig(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
				return;
			}
			Utils.DrawBorderString(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);

			this._frameCount++;

			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Vector2 pos = innerDimensions.Position();
			DynamicSpriteFont spriteFont = base.IsLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
			Vector2 vector = new Vector2(spriteFont.MeasureString(base.Text.Substring(0, this._cursor)).X, base.IsLarge ? 32f : 16f) * base.TextScale;
			if (base.IsLarge) {
				pos.Y -= 8f * base.TextScale;
			}
			else {
				pos.Y -= 1f * base.TextScale;
			}
			if (Text.Length == 0) {
				Vector2 hintTextSize = new Vector2(spriteFont.MeasureString(hintText.ToString()).X, IsLarge ? 32f : 16f) * TextScale;
				pos.X += 5;//(hintTextSize.X);
				if (base.IsLarge) {
					Utils.DrawBorderStringBig(spriteBatch, hintText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
					return;
				}
				Utils.DrawBorderString(spriteBatch, hintText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
				pos.X -= 5;
				//pos.X -= (innerDimensions.Width - hintTextSize.X) * 0.5f;
			}

			if (!focused)
				return;

			pos.X += /*(innerDimensions.Width - base.TextSize.X) * 0.5f*/ +vector.X - (base.IsLarge ? 8f : 4f) * base.TextScale + 6f;
			if ((this._frameCount %= 40) > 20) {
				return;
			}
			if (base.IsLarge) {
				Utils.DrawBorderStringBig(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
				return;
			}
			Utils.DrawBorderString(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
		}
	}
}
