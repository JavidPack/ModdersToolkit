using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace ModdersToolkit.Tools.Fishing
{
	// code based on SpawnUI
	internal class UIFishingCatchesInfo : UIElement
	{
		internal UIFishingCatchesSlot npcSlot;
		internal UIText information;
		internal UIText percentText;
		internal int itemid;
		internal float percent;

		public UIFishingCatchesInfo(int itemid, float percent) {
			this.itemid = itemid;
			this.percent = 100 * percent;
			Width = StyleDimension.Fill;
			Height.Pixels = 32;

			Item item = new Item();
			item.SetDefaults(itemid);

			npcSlot = new UIFishingCatchesSlot(item);
			Append(npcSlot);

			string name;
			if (itemid <= 0) {
				name = "Nothing";
			}
			else {
				name = Lang.GetItemNameValue(item.type) + (item.modItem != null ? " [" + item.modItem.mod.Name + "]" : "");
			}

			information = new UIText(name, 0.8f);
			information.Top.Pixels = 1;
			information.Left.Pixels = 40;
			Append(information);

			percentText = new UIText(this.percent.ToString("0.00") + "%", 0.8f);
			percentText.Left.Pixels = 40;
			percentText.Top.Pixels = 18;
			Append(percentText);
		}

		public override int CompareTo(object obj) {
			UIFishingCatchesInfo other = obj as UIFishingCatchesInfo;
			return -1 * percent.CompareTo(other.percent);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightBlue * 0.6f);
		}
	}
}
