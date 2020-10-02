using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
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

		public override void Initialize()
		{
			ToggleTooltip = "Click to toggle Hitboxes Tool";
			//hitboxesGlobalItem = (HitboxesGlobalItem)ModdersToolkit.instance.GetGlobalItem("HitboxesGlobalItem");
		}

        public override void ClientInitialize()
		{
			Interface = new UserInterface();

			hitboxesUI = new HitboxesUI(Interface);
			hitboxesUI.Activate();

			Interface.SetState(hitboxesUI);
		}

        public override void ClientTerminate()
        {
            Interface = default;

			hitboxesUI?.Deactivate();
            hitboxesUI = default;
        }


        public override void UIDraw()
		{
			if (Visible)
			{
				hitboxesUI.Draw(Main.spriteBatch);
			}
		}

        public override void WorldDraw()
		{
			if (Visible || keepShowingHitboxes)
			{ 
				if (showPlayerMeleeHitboxes) DrawPlayerMeleeHitboxes();
				if (showNPCHitboxes) DrawNPCHitboxes();
				if (showProjectileHitboxes) DrawProjectileHitboxes();
				// TODO: TileCollideStyle hitboxes? different?
				if (showProjectileDamageHitboxes) DrawProjectileDamageHitboxes();
				if (showTEPositions) DrawTileEntityPositions();
				if (showWorldItemHitboxes) DrawWorldItemHitboxes();
			}
		}

		private void DrawPlayerMeleeHitboxes()
		{
			for (int i = 0; i < 256; i++)
			{
				//if (hitboxesGlobalItem.meleeHitbox[i].HasValue)
				if (HitboxesGlobalItem.meleeHitbox[i].HasValue)
				{
					Rectangle hitbox = HitboxesGlobalItem.meleeHitbox[i].Value;
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.MediumPurple * 0.6f);
					HitboxesGlobalItem.meleeHitbox[i] = null;
				}
			}
		}

		private void DrawNPCHitboxes()
		{
			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active)
				{
					Rectangle hitbox = npc.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);
				}
			}
		}
		
		private void DrawWorldItemHitboxes()
		{
			for (int i = 0; i < 400; i++)
			{
				Item item = Main.item[i];
				if (item.active)
				{
					Rectangle hitbox = item.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Teal * 0.6f);
				}
			}
		}

		private void DrawProjectileHitboxes()
		{
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active)
				{
					Rectangle hitbox = projectile.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Orange * 0.6f);
				}
			}
		}

		private void DrawProjectileDamageHitboxes()
		{
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active)
				{
					Rectangle hitbox = projectile.getRect();
					ProjectileLoader.ModifyDamageHitbox(projectile, ref hitbox);
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					hitbox = Main.ReverseGravitySupport(hitbox);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.OrangeRed * 0.6f);
				}
			}
		}

		private void DrawTileEntityPositions()
		{
			foreach (var pair in TileEntity.ByPosition)
			{
				Rectangle locationRectangle = new Rectangle(pair.Key.X * 16, pair.Key.Y * 16, 16, 16);
				locationRectangle.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				locationRectangle = Main.ReverseGravitySupport(locationRectangle);
				Main.spriteBatch.Draw(Main.magicPixel, locationRectangle, Color.Green * 0.6f);
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
		public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
		{
			meleeHitbox[player.whoAmI] = hitbox;
		}

		public override void PostUpdate(Item item)
		{
			base.PostUpdate(item);
		}
	}
}
