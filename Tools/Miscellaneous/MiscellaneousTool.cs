using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;

namespace ModdersToolkit.Tools.Miscellaneous
{
	class MiscellaneousTool : Tool
	{
		internal static MiscellaneousUI miscellaneousUI;
		internal static bool showNPCInfo;
		internal static bool showTileGrid;

		internal override void Initialize()
		{
			toggleTooltip = "Click to toggle Miscellaneous Tool";
		}

		internal override void ClientInitialize()
		{
			userInterface = new UserInterface();
			miscellaneousUI = new MiscellaneousUI(userInterface);
			miscellaneousUI.Activate();
			userInterface.SetState(miscellaneousUI);
		}

		internal override void UIDraw()
		{
			if (visible)
			{
				miscellaneousUI.Draw(Main.spriteBatch);
			}
		}
	}

	class NPCInfoGlobalNPC : GlobalNPC
	{
		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			if (MiscellaneousTool.showNPCInfo)
			{
				if (npc.realLife == -1 || npc.realLife == npc.whoAmI)
				{
					var font = Main.fontMouseText;
					//Main.NewText("Drawing: " + npc.TypeName);
					Rectangle infoRectangle = new Rectangle((int)(npc.BottomRight.X - Main.screenPosition.X), (int)(npc.BottomRight.Y - Main.screenPosition.Y), 300, 140);
					spriteBatch.Draw(Main.magicPixel, infoRectangle, Color.NavajoWhite);

					spriteBatch.DrawString(font, $"ai:", infoRectangle.TopLeft() + new Vector2(5, 5), Color.Black);
					spriteBatch.DrawString(font, $"localAI:", infoRectangle.TopLeft() + new Vector2(5, 25), Color.Black);
					for (int i = 0; i < 4; i++)
					{
						spriteBatch.DrawString(font, $"{npc.ai[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 5), Color.Black);
						spriteBatch.DrawString(font, $"{npc.localAI[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 25), Color.Black);
					}

					spriteBatch.DrawString(font, $"spriteDirection: {npc.spriteDirection}  direction: {npc.direction}", infoRectangle.TopLeft() + new Vector2(5, 45), Color.Black);
					spriteBatch.DrawString(font, $"type: {npc.type} aiStyle: {npc.aiStyle} whoAmI: {npc.whoAmI}", infoRectangle.TopLeft() + new Vector2(5, 65), Color.Black);
					spriteBatch.DrawString(font, $"Buffs:", infoRectangle.TopLeft() + new Vector2(5, 85), Color.Black);
					for (int i = 0; i < 5; i++)
					{
						if (npc.buffType[i] > 0)
						{
							spriteBatch.DrawString(font, $"{Lang.GetBuffName(npc.buffType[i])}({npc.buffType[i]}) {npc.buffTime[i]} ", infoRectangle.TopLeft() + new Vector2(5 + 55, 85 + i * 20), Color.Black);
						}
					}
					//if(npc.realLife != -1)
					//{
					//	Utils.DrawLine(spriteBatch, npc.Center.ToPoint(), Main.npc[npc.realLife].Center.ToPoint(), Color.White);
					//}
				}
			}
		}
	}

	class TileGridModWorld : ModWorld
	{
		public override void PostDrawTiles()
		{
			if (MiscellaneousTool.showTileGrid)
			{
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

				Vector2 center = Main.LocalPlayer.Center.ToTileCoordinates().ToVector2() * 16;

				for (int i = -13; i < 14; i++)
				{
					Utils.DrawLine(Main.spriteBatch, new Vector2(center.X + i * 16, center.Y - 13*16), new Vector2(center.X + i * 16, center.Y + 13 * 16), Color.White, Color.White, 1);
					Utils.DrawLine(Main.spriteBatch, new Vector2(center.X - 13 * 16, center.Y + i * 16), new Vector2(center.X + 13 * 16, center.Y + i * 16), Color.White, Color.White, 1);
				}
				Main.spriteBatch.End();
			}
		}
	}
}
