using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Localization;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.UI.Chat;

namespace ModdersToolkit.UIElements
{
	class UICodeEntry : UIText
	{
		static int NextNum = 0;
		internal int num;
		internal CodeType codeType;
		public UICodeEntry(string text, CodeType type, float textScale = 1, bool large = false) : base(text, textScale = 1, large = false)
		{
			this.num = NextNum++;
			codeType = type;
			if (type == CodeType.Error)
			{
				TextColor = Color.Red;
			}
			else if (type == CodeType.Output)
			{
				TextColor = Color.Yellow;
			}
			else if (type == CodeType.Hint)
			{
				TextColor = Color.Orange;
			}
			else if (type == CodeType.Input)
			{
				TextColor = Color.LightSeaGreen;
			}

			Recalculate();
		}

		public override int CompareTo(object obj)
		{
			UICodeEntry other = obj as UICodeEntry;
			return other.num.CompareTo(num);
		}
	}

	public enum CodeType
	{
		Input,
		Output,
		Error,
		Hint,
	}
}
