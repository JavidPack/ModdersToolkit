using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.GameContent.UI.Chat;
using Terraria;

namespace ModdersToolkit.UIElements
{
	// TODO, tri-state checkbox.
    internal class UICheckbox : UIText
	{
		internal static Texture2D checkboxTexture;
		internal static Texture2D checkmarkTexture;
		public event Action OnSelectedChanged;

		public float order = 0;

		private bool clickable = false;
		public bool Clickable
		{
			get { return clickable; }
			set
			{
				if (value != clickable)
				{
					clickable = value;
				}
				TextColor = clickable ? Color.White : Color.Gray;
			}
		}

        private string tooltip = "";

		private bool selected = false;
		public bool Selected
		{
			get { return selected; }
			set
			{
				if (value != selected)
				{
					selected = value;
					OnSelectedChanged?.Invoke();
				}
			}
		}

		public UICheckbox(string text, string tooltip, bool clickable = true, float textScale = 1, bool large = false) : base(text, textScale, large)
		{
			this.tooltip = tooltip;
			this.Clickable = clickable;
			text = "   " + text;
			SetText(text);
			Recalculate();
		}

		public override void Click(UIMouseEvent evt)
		{
			if (clickable)
			{
				Selected = !Selected;
			}
			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			//Vector2 pos = new Vector2(innerDimensions.X - 20, innerDimensions.Y - 5);
			Vector2 pos = new Vector2(innerDimensions.X, innerDimensions.Y - 5);

			spriteBatch.Draw(checkboxTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			if (Selected)
				spriteBatch.Draw(checkmarkTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			base.DrawSelf(spriteBatch);

			if (IsMouseHovering && tooltip.Length > 0)
			{
				Main.HoverItem = new Item();
				Main.hoverItemName = tooltip;
			}
		}

		public override int CompareTo(object obj)
		{
			UICheckbox other = obj as UICheckbox;
			return order.CompareTo(other.order);
		}
	}


    internal class UICheckbox2<T> : UICheckbox2
	{
		public UICheckbox2(UIBoolDataValue data, float textScale = 1, bool large = false) : base(data, textScale, large)
		{
		}
	}


    internal class UICheckbox2 : UIText
	{
		internal static Texture2D checkboxTexture;
		internal static Texture2D checkmarkTexture;
		public event Action OnSelectedChanged;

		private UIBoolDataValue data;

		public UICheckbox2(UIBoolDataValue data, float textScale = 1, bool large = false) : base(data.label, textScale, large)
		{
			this.data = data;
			SetText("   " + data.label);
			Recalculate();
		}

		public override void Click(UIMouseEvent evt)
		{
			data.SetValue(!data.Data);
			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			//Vector2 pos = new Vector2(innerDimensions.X - 20, innerDimensions.Y - 5);
			Vector2 pos = new Vector2(innerDimensions.X, innerDimensions.Y - 5);

			spriteBatch.Draw(checkboxTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			if (data.Data)
				spriteBatch.Draw(checkmarkTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			base.DrawSelf(spriteBatch);
		}
	}

    internal class UITriStateCheckbox : UIText
	{
		internal static Texture2D checkboxTexture;
		internal static Texture2D checkmarkTexture;
		internal static Texture2D checkXTexture;
		public event Action OnSelectedChanged;

		private UIBoolNDataValue data;

		public UITriStateCheckbox(UIBoolNDataValue data, float textScale = 1, bool large = false) : base(data.label, textScale, large)
		{
			this.data = data;
			SetText("   " + data.label);
			Recalculate();
		}

		public override void Click(UIMouseEvent evt)
		{
			if (data.Data.HasValue && data.Data.Value)
			{
				data.SetValue(false);
			}
			else if (data.Data.HasValue && !data.Data.Value)
			{
				data.SetValue(null);
			}
			else
			{
				data.SetValue(true);
			}

			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			//Vector2 pos = new Vector2(innerDimensions.X - 20, innerDimensions.Y - 5);
			Vector2 pos = new Vector2(innerDimensions.X, innerDimensions.Y - 5);

			spriteBatch.Draw(checkboxTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			if (data.Data.HasValue)
			{
				if (data.Data.Value)
				{
					spriteBatch.Draw(checkmarkTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
				else
				{
					spriteBatch.Draw(checkXTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
			}
			base.DrawSelf(spriteBatch);
		}
	}
}

