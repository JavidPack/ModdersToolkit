using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Hitboxes
{
	internal class HitboxesTool : Tool
	{
		//internal static UserInterface userInterface;
		internal static HitboxesUI hitboxesUI;
		internal static bool keepShowingHitboxes;
		internal static bool showPlayerMeleeHitboxes;
		internal static bool showNPCHitboxes;
		internal static bool showProjectileHitboxes;
		internal static bool showProjectileDamageHitboxes;
		internal static bool showTEPositions;
		internal static bool showWorldItemHitboxes;
		//internal static HitboxesGlobalItem hitboxesGlobalItem;

		internal static bool showPlayerPosition;
		internal static bool showPlayerVelocity;
		internal static bool showNPCPosition;
		internal static bool showNPCVelocity;
		internal static bool showProjectilePosition;
		internal static bool showProjectileVelocity;


		public override void Initialize() {
			ToggleTooltip = "Click to toggle Hitboxes Tool";
			//hitboxesGlobalItem = (HitboxesGlobalItem)ModdersToolkit.instance.GetGlobalItem("HitboxesGlobalItem");
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			hitboxesUI = new HitboxesUI(Interface);
			hitboxesUI.Activate();

			Interface.SetState(hitboxesUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			hitboxesUI?.Deactivate();
			hitboxesUI = null;
		}


		public override void UIDraw() {
			if (Visible) {
				hitboxesUI.Draw(Main.spriteBatch);
			}
		}

		public override void WorldDraw() {
			if (Visible || keepShowingHitboxes) {

				if (showPlayerPosition)
					DrawPlayerPosition();
				if(showPlayerVelocity)
					DrawPlayerVelocity();
				if (showProjectileVelocity)
					DrawProjectileVelocity();

				if (showPlayerMeleeHitboxes)
					DrawPlayerMeleeHitboxes();
				if (showNPCHitboxes)
					DrawNPCHitboxes();
				if (showProjectileHitboxes)
					DrawProjectileHitboxes();
				// TODO: TileCollideStyle hitboxes? different?
				if (showProjectileDamageHitboxes)
					DrawProjectileDamageHitboxes();
				if (showTEPositions)
					DrawTileEntityPositions();
				if (showWorldItemHitboxes)
					DrawWorldItemHitboxes();
			}
		}

		private void DrawProjectileVelocity() {
			var font = FontAssets.MouseText.Value;
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile p = Main.projectile[i];
				if (p.active) {
					Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, $"velocity: {p.velocity.X:0.0}, {p.velocity.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -40), Color.Red, 0, Vector2.Zero, Vector2.One);
					//Main.spriteBatch.DrawString(font, $"velocity: {p.velocity.X:0.0}, {p.velocity.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -40), Color.Red);
					Vector2 end = p.Center + p.velocity * 30;
					Utils.DrawLine(Main.spriteBatch, p.Center, end, Color.Red, Color.Red, 4);
					var a = Math.Min(24, 4 * p.velocity.Length());
					Vector2 startAdjustment = Vector2.Normalize(p.velocity).RotatedBy(Math.PI / 2) * 4;
					Utils.DrawLine(Main.spriteBatch, startAdjustment + end, startAdjustment + end + Vector2.Normalize(end - p.Center).RotatedBy(0.9f * Math.PI) * a, Color.Red, Color.Red, 4);
					Utils.DrawLine(Main.spriteBatch, startAdjustment + end, startAdjustment + end + Vector2.Normalize(end - p.Center).RotatedBy(-0.9f * Math.PI) * a, Color.Red, Color.Red, 4);

					if(/*p.aiStyle == 2*/ true) {
						// Consistent scale for velocities good for comprehension.
						Vector2 velocityChange = p.velocity - p.oldVelocity;
						Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, $"velocity change: {velocityChange.X:0.0}, {velocityChange.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -20), Color.LightSkyBlue, 0, Vector2.Zero, Vector2.One);
						DrawArrow(p.Center, velocityChange * 3, Color.LightSkyBlue);
					}
				}
			}
		}

		private void DrawArrow(Vector2 center, Vector2 change, Color color, int lineWidth = 4) {
			Vector2 end = center + change * 30; // Global scale for visualizing
			Utils.DrawLine(Main.spriteBatch, center, end, color, color, lineWidth);
			var arrrowHeadScale = Math.Min(24, 4 * change.Length());
			Vector2 startAdjustment = Vector2.Normalize(change).RotatedBy(Math.PI / 2) * lineWidth;
			Utils.DrawLine(Main.spriteBatch, startAdjustment + end, startAdjustment + end + Vector2.Normalize(end - center).RotatedBy(0.9f * Math.PI) * arrrowHeadScale, color, color, lineWidth);
			Utils.DrawLine(Main.spriteBatch, startAdjustment + end, startAdjustment + end + Vector2.Normalize(end - center).RotatedBy(-0.9f * Math.PI) * arrrowHeadScale, color, color, lineWidth);
		}

		private void DrawPlayerVelocity() {
			var font = FontAssets.MouseText.Value;
			for (int i = 0; i < 256; i++) {
				Player p = Main.player[i];
				if (p.active) {
					//Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, $"velocity: {p.velocity.X:0.0}, {p.velocity.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -40), Color.Red, 0, Vector2.Zero, Vector2.One);
					Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, $"velocity: {p.velocity.X:0}, {p.velocity.Y:0}", p.position - Main.screenPosition + new Vector2(5, -40), Color.Red, 0, Vector2.Zero, Vector2.One);
					//Main.spriteBatch.DrawString(font, $"velocity: {p.velocity.X:0.0}, {p.velocity.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -40), Color.Red);
					Vector2 end = p.Center + p.velocity * 30;
					Utils.DrawLine(Main.spriteBatch, p.Center, end, Color.Red, Color.Red, 4);
					var a = Math.Min(24, 4 * p.velocity.Length());
					Vector2 startAdjustment = Vector2.Normalize(p.velocity).RotatedBy(Math.PI / 2) * 4;
					Utils.DrawLine(Main.spriteBatch, startAdjustment + end, startAdjustment + end + Vector2.Normalize(end - p.Center).RotatedBy(0.9f * Math.PI) * a, Color.Red, Color.Red, 4);
					Utils.DrawLine(Main.spriteBatch, startAdjustment + end, startAdjustment + end + Vector2.Normalize(end - p.Center).RotatedBy(-0.9f * Math.PI) * a, Color.Red, Color.Red, 4); 
				}
			}
		}
		private void DrawPlayerPosition() {
			var font = FontAssets.MouseText.Value;
			for (int i = 0; i < 256; i++) {
				Player p = Main.player[i];
				if (p.active) {
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(p.position.X - Main.screenPosition.X), (int)(p.position.Y - Main.screenPosition.Y), 8, 8), Color.Black);
					//Main.spriteBatch.DrawString(font, $"position: {p.position.X:0.0}, {p.position.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -20), Color.Black);
					//Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, $"position: {p.position.X:0.0}, {p.position.Y:0.0}", p.position - Main.screenPosition + new Vector2(5, -20), Color.White, 0, Vector2.Zero, Vector2.One);
					Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, $"position: {p.position.X:0}, {p.position.Y:0}", p.position - Main.screenPosition + new Vector2(5, -20), Color.White, 0, Vector2.Zero, Vector2.One);
				}
			}
		}

		private void DrawPlayerMeleeHitboxes() {
			for (int i = 0; i < 256; i++) {
				//if (hitboxesGlobalItem.meleeHitbox[i].HasValue)
				if (HitboxesGlobalItem.meleeHitbox[i].HasValue) {
					Rectangle hitbox = HitboxesGlobalItem.meleeHitbox[i].Value;
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.MediumPurple * 0.6f);
					HitboxesGlobalItem.meleeHitbox[i] = null;
				}
			}
		}

		private void DrawNPCHitboxes() {
			for (int i = 0; i < 200; i++) {
				NPC npc = Main.npc[i];
				if (npc.active) {
					Rectangle hitbox = npc.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Red * 0.6f);
				}
			}
		}

		private void DrawWorldItemHitboxes() {
			for (int i = 0; i < 400; i++) {
				Item item = Main.item[i];
				if (item.active) {
					Rectangle hitbox = item.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Teal * 0.6f);
				}
			}
		}

		private void DrawProjectileHitboxes() {
			for (int i = 0; i < 1000; i++) {
				Projectile projectile = Main.projectile[i];
				if (projectile.active) {
					Rectangle hitbox = projectile.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Orange * 0.6f);
				}
			}
		}

		private void DrawProjectileDamageHitboxes() {
			for (int i = 0; i < 1000; i++) {
				Projectile projectile = Main.projectile[i];
				if (projectile.active) {
					Rectangle hitbox = projectile.getRect();
					ProjectileLoader.ModifyDamageHitbox(projectile, ref hitbox);
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.OrangeRed * 0.6f);
				}
			}
		}

		private void DrawTileEntityPositions() {
			foreach (var pair in TileEntity.ByPosition) {
				Rectangle locationRectangle = new Rectangle(pair.Key.X * 16, pair.Key.Y * 16, 16, 16);
				locationRectangle.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				locationRectangle = Main.ReverseGravitySupport(locationRectangle);
				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, locationRectangle, Color.Green * 0.6f);
			}
		}

		//internal override void ScreenResolutionChanged()
		//{
		//	userInterface.Recalculate();
		//}
		//internal override void UIUpdate()
		//{
		//	if (visible)
		//	{
		//		userInterface.Update(Main._drawInterfaceGameTime);
		//	}
		//}
	}

	internal class HitboxesGlobalItem : GlobalItem
	{
		internal static Rectangle?[] meleeHitbox = new Rectangle?[256];
		// Is this ok to load in server?
		public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox) {
			meleeHitbox[player.whoAmI] = hitbox;
		}

		public override void PostUpdate(Item item) {
			base.PostUpdate(item);
		}
	}
}
