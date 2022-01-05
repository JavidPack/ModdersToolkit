using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent;
using ModdersToolkit.Tools.Spawns;

namespace ModdersToolkit.Tools.Fishing
{
	// code based on SpawnUI
	internal class UIFishingCatchesInfo : UIElement
	{
		internal UIText information;
		internal UIText percentText;
		internal int id;
		internal float percent;

		public UIFishingCatchesInfo(int id, float percent) {
			this.id = id;
			this.percent = 100 * percent;
			Width = StyleDimension.Fill;
			Height.Pixels = 32;

			string name;
			if (id == 0) {
				name = "Nothing";
			}
			else if (id > 0) {
				Item item = new Item();
				item.SetDefaults(id);

				var itemSlot = new UIFishingCatchesSlot(item);
				Append(itemSlot);
				name = Lang.GetItemNameValue(item.type) + (item.ModItem != null ? " [" + item.ModItem.Mod.Name + "]" : "");
			}
			else {
				NPC npc = new NPC();
				npc.SetDefaults(-id);

				var npcSlot = new UINPCSlot(npc);
				Append(npcSlot);
				name = Lang.GetNPCNameValue(npc.type) + (npc.ModNPC != null ? " [" + npc.ModNPC.Mod.Name + "]" : "");
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
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, id < 0 ? Color.LightCoral * 0.6f : Color.LightBlue * 0.6f);
		}
	}
}
