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
	class HitboxesTool : Tool
	{
		//internal static UserInterface userInterface;
		internal static HitboxesUI hitboxesUI;
		internal static bool keepShowingHitboxes;
		internal static bool showPlayerMeleeHitboxes;
		internal static bool showNPCHitboxes;
		internal static bool showProjectileHitboxes;
		internal static bool showTEPositions;
		internal static bool showWorldItemHitboxes;
		//internal static HitboxesGlobalItem hitboxesGlobalItem;

		internal override void Initialize()
		{
			toggleTooltip = "Click to toggle Hitboxes Tool";
			//hitboxesGlobalItem = (HitboxesGlobalItem)ModdersToolkit.instance.GetGlobalItem("HitboxesGlobalItem");
		}

		internal override void ClientInitialize()
		{
			userInterface = new UserInterface();
			hitboxesUI = new HitboxesUI(userInterface);
			hitboxesUI.Activate();
			userInterface.SetState(hitboxesUI);
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
		internal override void UIDraw()
		{
			if (visible)
			{
				hitboxesUI.Draw(Main.spriteBatch);
			}
			if (visible || keepShowingHitboxes)
			{ 
				if (showPlayerMeleeHitboxes) drawPlayerMeleeHitboxes();
				if (showNPCHitboxes) drawNPCHitboxes();
				if (showProjectileHitboxes) drawProjectileHitboxes();
				if (showTEPositions) drawTileEntityPositions();
				if (showWorldItemHitboxes) drawWorldItemHitboxes();
			}
		}

		private void drawPlayerMeleeHitboxes()
		{
			for (int i = 0; i < 256; i++)
			{
				//if (hitboxesGlobalItem.meleeHitbox[i].HasValue)
				if (HitboxesGlobalItem.meleeHitbox[i].HasValue)
				{
					Rectangle hitbox = HitboxesGlobalItem.meleeHitbox[i].Value;
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.MediumPurple * 0.6f);
					HitboxesGlobalItem.meleeHitbox[i] = null;
				}
			}
		}

		private void drawNPCHitboxes()
		{
			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active)
				{
					Rectangle hitbox = npc.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);
				}
			}
		}
		
		private void drawWorldItemHitboxes()
		{
			for (int i = 0; i < 400; i++)
			{
				Item item = Main.item[i];
				if (item.active)
				{
					Rectangle hitbox = item.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Teal * 0.6f);
				}
			}
		}

		private void drawProjectileHitboxes()
		{
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active)
				{
					Rectangle hitbox = projectile.getRect();
					hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Orange * 0.6f);
				}
			}
		}

		private void drawTileEntityPositions()
		{
			foreach (var pair in TileEntity.ByPosition)
			{
				Rectangle locationRectangle = new Rectangle(pair.Key.X * 16, pair.Key.Y * 16, 16, 16);
				locationRectangle.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				Main.spriteBatch.Draw(Main.magicPixel, locationRectangle, Color.Green * 0.6f);
			}
		}
	}

	class HitboxesGlobalItem : GlobalItem
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
