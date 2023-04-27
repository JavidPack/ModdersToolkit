using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Miscellaneous
{
	internal class MiscellaneousTool : Tool
	{
		internal static MiscellaneousUI miscellaneousUI;
		internal static bool showNPCInfo;
		internal static bool showProjectileInfo;
		internal static bool lockProjectileInfo;
		internal static bool showTileGrid;
		internal static bool showCollisionCircle;
		internal static bool showCanHitLine;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Miscellaneous Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			miscellaneousUI = new MiscellaneousUI(Interface);
			miscellaneousUI.Activate();

			Interface.SetState(miscellaneousUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			miscellaneousUI?.Deactivate();
			miscellaneousUI = null;
		}

		public override void UIDraw() {
			if (Visible) {
				miscellaneousUI.Draw(Main.spriteBatch);
			}
		}
	}

	internal class ProjectileInfoGlobalProjectile : GlobalProjectile
	{
		public override void PostDraw(Projectile projectile, Color lightColor) {
			if (MiscellaneousTool.showProjectileInfo) {
				var spriteBatch = Main.spriteBatch;
				var font = FontAssets.MouseText.Value;
				Rectangle infoRectangle = new Rectangle((int)(projectile.BottomRight.X - Main.screenPosition.X), (int)(projectile.BottomRight.Y - Main.screenPosition.Y), 300, 160);
				if (MiscellaneousTool.lockProjectileInfo) {
					infoRectangle.X = Main.screenWidth / 2 - 150;
					infoRectangle.Y = Main.screenHeight / 2 + 30;
				}

				spriteBatch.Draw(TextureAssets.MagicPixel.Value, infoRectangle, Color.NavajoWhite);

				int y = 5;
				spriteBatch.DrawString(font, $"ai:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
				y += 20;
				spriteBatch.DrawString(font, $"localAI:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
				y += 20;
				for (int i = 0; i < Projectile.maxAI; i++) {
					spriteBatch.DrawString(font, $"{projectile.ai[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 5), Color.Black);
					spriteBatch.DrawString(font, $"{projectile.localAI[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 25), Color.Black);
				}

				spriteBatch.DrawString(font, $"spriteDirection: {projectile.spriteDirection}  direction: {projectile.direction}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
				y += 20;
				spriteBatch.DrawString(font, $"damage: {projectile.damage}  owner: {projectile.owner}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
				y += 20;
				spriteBatch.DrawString(font, $"type: {projectile.type} aiStyle: {projectile.aiStyle} whoAmI: {projectile.whoAmI}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
				y += 20;
			}
		}
	}

	internal class NPCInfoGlobalNPC : GlobalNPC
	{
		//public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {
		//	if (MiscellaneousTool.showNPCInfo) {
		//		Main.NewText($"Damage: {damage}   knockback: {knockback}");
		//	}
		//}

		//public override bool PreAI(NPC npc) {
		//	if (MiscellaneousTool.showNPCInfo)
		//		return false;
		//	return base.PreAI(npc);
		//}

		// TODO: What info do we want, is there duplicate info here? Should show this in a tool like damageclass tool.
		/*
		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
			if (MiscellaneousTool.showNPCInfo) {
				Main.NewText($"Damage: {damage,5:##0.0}   knockback: {knockback,5:##0.0}   hitDirection: {hitDirection}");
			}
			return base.ModifyIncomingHit(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
		}
		*/

		public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) {
			if (MiscellaneousTool.showNPCInfo) {
				Main.NewText($"SourceDamage: {modifiers.SourceDamage.Additive,5:##0.0} {modifiers.SourceDamage.Multiplicative,5:##0.0} {modifiers.SourceDamage.Base,5:##0.0} {modifiers.SourceDamage.Flat,5:##0.0}");
				Main.NewText($"Defense: {modifiers.Defense.Additive,5:##0.0} {modifiers.Defense.Multiplicative,5:##0.0} {modifiers.Defense.Base,5:##0.0} {modifiers.Defense.Flat,5:##0.0}");
				Main.NewText($"ArmorPenetration: {modifiers.ArmorPenetration.Value,5:##0.0}");
			}
		}

		public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
			if (MiscellaneousTool.showNPCInfo) {
				Main.NewText($"Damage: {hit.Damage,5:##0.0}   knockback: {hit.Knockback,5:##0.0}   hitDirection: {hit.HitDirection}");
			}
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
			if (MiscellaneousTool.showNPCInfo) {
				Main.NewText($"Damage: {hit.Damage,5:##0.0}   knockback: {hit.Knockback,5:##0.0}   hitDirection: {hit.HitDirection}");
			}
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			if (MiscellaneousTool.showNPCInfo) {
				if (npc.realLife == -1 || npc.realLife == npc.whoAmI) {
					var font = FontAssets.MouseText.Value;
					//Main.NewText("Drawing: " + npc.TypeName);
					Rectangle infoRectangle = new Rectangle((int)(npc.BottomRight.X - Main.screenPosition.X), (int)(npc.BottomRight.Y - Main.screenPosition.Y), 300, 160);
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, infoRectangle, Color.NavajoWhite);

					/*
					var d = npc.Hitbox;
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, d, Color.Red);

					spriteBatch.DrawString(font, $"X {npc.position.X,5:##0.0}", infoRectangle.TopLeft() + new Vector2(5, 5), Color.Black);
					spriteBatch.DrawString(font, $"Y {npc.position.Y,5:##0.0}", infoRectangle.TopLeft() + new Vector2(5, 25), Color.Black);
					return;
					*/

					// TODO: customize display info somehow? Toggles? Separate tab/panel?
					// TODO: Draw modNPC or GlobalNPC field values similar to PrintItemInfo_OnClick.
					int y = 5;
					spriteBatch.DrawString(font, $"ai:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					y += 20;
					spriteBatch.DrawString(font, $"localAI:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					y += 20;
					for (int i = 0; i < NPC.maxAI; i++) {
						spriteBatch.DrawString(font, $"{npc.ai[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 5), Color.Black);
						spriteBatch.DrawString(font, $"{npc.localAI[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 25), Color.Black);
					}

					spriteBatch.DrawString(font, $"npc.immune[]: {npc.immune[Main.myPlayer]}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					y += 20;
					spriteBatch.DrawString(font, $"spriteDirection: {npc.spriteDirection}  direction: {npc.direction}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					y += 20;
					spriteBatch.DrawString(font, $"defense: {npc.defense}  defDefense: {npc.defDefense}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					y += 20;
					spriteBatch.DrawString(font, $"type: {npc.type} aiStyle: {npc.aiStyle} whoAmI: {npc.whoAmI}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					y += 20;
					spriteBatch.DrawString(font, $"Buffs:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
					bool anyBuffs = false;
					for (int i = 0; i < 5; i++) {
						if (npc.buffType[i] > 0) {
							spriteBatch.DrawString(font, $"{Lang.GetBuffName(npc.buffType[i])}({npc.buffType[i]}) {npc.buffTime[i]} ", infoRectangle.TopLeft() + new Vector2(5 + 65, y), Color.Black);
							y += 20;
							anyBuffs = true;
						}
					}
					if (!anyBuffs)
						y += 20;
					//if(npc.realLife != -1)
					//{
					//	Utils.DrawLine(spriteBatch, npc.Center.ToPoint(), Main.npc[npc.realLife].Center.ToPoint(), Color.White);
					//}
				}
			}
		}
	}

	internal class TileGridModSystem : ModSystem
	{
		public override void PostDrawTiles() {
			if (MiscellaneousTool.showTileGrid) {
				//Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				Vector2 center = Main.LocalPlayer.Center.ToTileCoordinates().ToVector2() * 16;

				for (int i = -13; i < 14; i++) {
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(center.X + i * 16 - Main.screenPosition.X), (int)(center.Y - 13 * 16 - Main.screenPosition.Y), 2, 416), new Rectangle(0, 0, 1, 1), Color.White);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(center.X - 13 * 16 - Main.screenPosition.X), (int)(center.Y - i * 16 - Main.screenPosition.Y), 416, 2), new Rectangle(0, 0, 1, 1), Color.White);
					//Utils.DrawLine(Main.spriteBatch, new Vector2(center.X + i * 16, center.Y - 13 * 16), new Vector2(center.X + i * 16, center.Y + 13 * 16), Color.White, Color.White, 1);
					//Utils.DrawLine(Main.spriteBatch, new Vector2(center.X - 13 * 16, center.Y + i * 16), new Vector2(center.X + 13 * 16, center.Y + i * 16), Color.White, Color.White, 1);
				}
				/* Shows some coordinates for making guides on the wiki
				var font = Main.fontMouseText;
				var b = Main.LocalPlayer.Center.ToTileCoordinates();
				Main.spriteBatch.DrawString(font, $"{b.X - 3}, {b.Y - 3}", center - Main.screenPosition + new Vector2(-3*16)+ new Vector2(0,-20), Color.Orange);
				Main.spriteBatch.DrawString(font, $"{b.X + 2}, {b.Y + 2}", center - Main.screenPosition + new Vector2(3*16, 2*16), Color.Orange);

				var c = center - Main.screenPosition + new Vector2(-3 * 16);
				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)c.X, (int)c.Y, 4, 4), Color.Red);

				var d = center - Main.screenPosition + new Vector2(2 * 16);
				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)d.X, (int)d.Y, 4, 4), Color.Red);
				*/
				Main.spriteBatch.End();

				// Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			}
			if (MiscellaneousTool.showCollisionCircle) {
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
				float max = 360f; // TODO: use a real circle drawing algorithm and adjustable radius
				bool showTileCorners = Main.GameUpdateCount / 60 % 2 == 0;
				showTileCorners = Main.LocalPlayer.direction == -1;
				for (int i = 0; i < max; i++) {
					float radians = MathHelper.TwoPi * (i / max);
					Vector2 end = new Vector2(100, 0).RotatedBy(radians) + Main.LocalPlayer.Center;
					bool noCollision = Collision.CanHit(Main.LocalPlayer.Center, 0, 0, end, 0, 0);
					//Dust.QuickDust(end, noCollision ? Color.Green : Color.Red);

					if (!showTileCorners)
						Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(end.X - Main.screenPosition.X), (int)(end.Y - Main.screenPosition.Y), 2, 2), noCollision ? Color.Green : Color.Red);

					var Position1 = Main.LocalPlayer.Center;
					var Position2 = end;
					var Width1 = 0;
					var Width2 = 0;
					var Height1 = 0;
					var Height2 = 0;
					int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
					int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
					int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
					int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);

					Vector2 actualCheck = new Vector2(num3, num4) * 16;
					if (showTileCorners) {
						Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 2, (int)(actualCheck.Y - Main.screenPosition.Y) + 2, 1, 1), noCollision ? Color.Green : Color.Red);
						Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 2, (int)(actualCheck.Y - Main.screenPosition.Y) + 14, 1, 1), noCollision ? Color.Green : Color.Red);
						Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 14, (int)(actualCheck.Y - Main.screenPosition.Y) + 2, 1, 1), noCollision ? Color.Green : Color.Red);
						Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 14, (int)(actualCheck.Y - Main.screenPosition.Y) + 14, 1, 1), noCollision ? Color.Green : Color.Red);
					}
				}
				Main.spriteBatch.End();
			}
			if (MiscellaneousTool.showCanHitLine) {
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				bool can = Collision.CanHitLine(Main.LocalPlayer.Center, 0, 0, Main.MouseWorld, 0, 0);
				Color color = can ? Color.Green : Color.Red;
				Utils.DrawLine(Main.spriteBatch, Main.LocalPlayer.Center, Main.MouseWorld, color, color, 1);
				Main.spriteBatch.End();
			}
		}
	}
}
