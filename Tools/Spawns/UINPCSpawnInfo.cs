using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.Spawns
{
	class UINPCSpawnInfo : UIElement
	{
		internal UINPCSlot npcSlot;
		internal UIText information;
		internal UIText percentText;
		internal int npcid;
		internal float percent;

		public UINPCSpawnInfo(int npcid, float percent) {
			this.npcid = npcid;
			this.percent = 100 * percent;
			Width = StyleDimension.Fill;
			Height.Pixels = 32;

			NPC npc = new NPC();
			npc.SetDefaults(npcid);

			npcSlot = new UINPCSlot(npc);
			Append(npcSlot);

			string name = Lang.GetNPCNameValue(npc.type) + (npc.modNPC != null ? " [" + npc.modNPC.mod.Name + "]" : "");

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
			UINPCSpawnInfo other = obj as UINPCSpawnInfo;
			return -1 * percent.CompareTo(other.percent);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightBlue * 0.6f);
		}
	}
}
