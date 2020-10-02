using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.GameContent.UI.Chat;
using Terraria;
using Terraria.Graphics;

namespace ModdersToolkit.UIElements
{
    internal class UIRadioButton : UIText
	{
		private Texture2D _toggleTexture;
		public event Action OnSelectedChanged;

        private string tooltip = "";
		private bool selected = false;
		internal bool partOfGroup;
		internal int groupID;

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

		public override void Click(UIMouseEvent evt)
		{
			if (!partOfGroup)
			{
				Selected = !Selected;
				Recalculate();
			}
			else
			{
				(Parent as UIRadioButtonGroup).ButtonClicked(groupID);
			}
		}

		public UIRadioButton(string text, string tooltip, float textScale = 1, bool large = false) : base(text, textScale, large)
		{
			this.tooltip = tooltip;
			this._toggleTexture = TextureManager.Load("Images/UI/Settings_Toggle");
			text = "   " + text;
			SetText(text);
			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Vector2 pos = new Vector2(innerDimensions.X, innerDimensions.Y);

			//spriteBatch.Draw(checkboxTexture, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);



			Rectangle value = new Rectangle(Selected ? ((_toggleTexture.Width - 2) / 2 + 2) : 0, 0, (_toggleTexture.Width - 2) / 2, this._toggleTexture.Height);
			//Vector2 vector2 = new Vector2((float)value.Width, 0f);
			//position = new Vector2(dimensions.X + dimensions.Width - vector2.X - 10f, dimensions.Y + 2f + num);
			spriteBatch.Draw(this._toggleTexture, pos, new Rectangle?(value), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			if (IsMouseHovering && tooltip.Length > 0) {
				Main.HoverItem = new Item();
				Main.hoverItemName = tooltip;
			}
		}
	}
}
