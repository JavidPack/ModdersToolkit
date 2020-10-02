using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.UI.Chat;
using ModdersToolkit.UIElements;
using Terraria.GameContent.UI.Elements;

namespace ModdersToolkit.Tools.Loot
{
    internal class UILootInfo : UIElement
	{
		internal UIItemSlot itemSlot;
		internal UIText information;
		internal UIText percentText;
		internal int itemid;
		internal float percent;

		public UILootInfo(int itemid, float percent)
		{
			this.itemid = itemid;
			this.percent = /*100 * */percent;
			Width = StyleDimension.Fill;
			Height.Pixels = 32;

			Item item = new Item();
			item.SetDefaults(itemid, true);

			itemSlot = new UIItemSlot(item);
			Append(itemSlot);

			string name = item.Name + (item.modItem != null ? " [" + item.modItem.mod.Name + "]" : "");

			information = new UIText(name, 0.8f);
			information.Top.Pixels = 1;
			information.Left.Pixels = 40;
			Append(information);

			percentText = new UIText(this.percent.ToString("0.000") /*+ "%"*/, 0.8f);
			percentText.Left.Pixels = 40;
			percentText.Top.Pixels = 18;
			Append(percentText);
		}

		public override int CompareTo(object obj)
		{
			UILootInfo other = obj as UILootInfo;
			return -1 * percent.CompareTo(other.percent);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightBlue * 0.6f);
		}
	}
}
