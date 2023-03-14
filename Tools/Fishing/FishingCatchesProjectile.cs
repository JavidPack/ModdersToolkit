using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModdersToolkit.Tools.Fishing
{
	// This class is used in conjunction with the FishingTool to "trigger" a catch the first tick the bobber gets wet
	// It then gets logged, and will repeat every call of AI until the testing is complete
	internal class FishingCatchesProjectile : GlobalProjectile
	{
		public override void Load() {
			Terraria.On_Projectile.AI_061_FishingBobber_DoASplash += Projectile_AI_061_FishingBobber_DoASplash;
		}

		private void Projectile_AI_061_FishingBobber_DoASplash(Terraria.On_Projectile.orig_AI_061_FishingBobber_DoASplash orig, Projectile self) {
			if (FishingTool.currentlyTesting) {
				// Prevent dust + sound which causes lag in water
				return;
			}

			orig(self);
		}

		public override void AI(Projectile projectile) {
			if (!FishingTool.currentlyTesting) {
				return;
			}
			if (!projectile.bobber) {
				return;
			}
			if (!projectile.wet) {
				return;
			}

			Player player = Main.player[projectile.owner];

			//Simulate pre-FishingCheck preparation code:
			projectile.ai[0] = 0f; // Catchable state
			projectile.localAI[1] = 0f; // Catch timer reached

			if (Main.myPlayer == projectile.owner) {
				projectile.FishingCheck(); // Calls ModPlayer.CatchFish, after that assigns localAI[1] to the item catch type (positive) or NPC catch type (negative)

				// Except when if no catch is found, and player is atleast 2/3 "lava fishing requirements", it advances localAI[1] by 240 (from 0f)
				if (projectile.localAI[1] == 240f) {
					PlayerFishingConditions conditions = player.GetFishingConditions(); // Only iterates inventory twice, nothing else

					int enoughLavaStuff = 0;
					if (ItemID.Sets.IsLavaBait[conditions.BaitItemType]) {
						enoughLavaStuff++;
					}
					if (ItemID.Sets.CanFishInLava[conditions.PoleItemType]) {
						enoughLavaStuff++;
					}
					if (player.accLavaFishing) {
						enoughLavaStuff++;
					}

					if (enoughLavaStuff >= 2) {
						// Only if the vanilla conditions for setting to 240f apply, reset it
						projectile.localAI[1] = 0f;
					}
				}
			}

			// Player.ItemCheck_CheckFishingBobbers then checks if ai[0] is 0, and localAI[1] is not equal to 0, then sets ai[0] to 1f,
			// and if it's negative (an NPC), sets ai[0] to 2f
			// and if it's positive (an item), sets ai[1] to be localAI[1]
			// Simulate it here:
			bool catchNPC = false;
			if (projectile.ai[0] == 0 && projectile.ai[1] < 0f && projectile.localAI[1] != 0) {
				projectile.ai[0] = 1f;

				if (projectile.localAI[1] < 0f) {
					// NPC catches are handled earlier, in ItemCheck_CheckFishingBobber_PullBobber (which is within ItemCheck_CheckFishingBobbers):
					// It reads from bobber.localAI[1], check if its negative, then spawns the NPC, and only sets ai[0] to 2f,
					// Skipping any reel in code used for items
					catchNPC = true;
					projectile.ai[0] = 2f; // This plays a sound if == 2f...
					projectile.ai[0]++; //...so increment it here again, has no impact on the behavior
				}
				else {
					projectile.ai[1] = projectile.localAI[1];
				}
			}

			// Handle negative as NPC type 
			int type = (int)(catchNPC ? projectile.localAI[1] : projectile.ai[1]);

			// DEBUG
			//if (catchNPC) {
			//	var allowed = new int[] { 620, 621, 586, 587, 618, 586, 587 };
			//	if (System.Array.IndexOf(allowed, -type) <= -1) {
			//		int asd = 0;
			//	}
			//}

			// Prematurely reset it here:
			if (projectile.ai[1] > 0f && projectile.localAI[1] >= 0f) {
				projectile.localAI[1] = -1;
				// ...
			}

			if (!catchNPC && type < 0) {
				// If type is negative, but catchNPC is false, do not add, this is just a leftover timer value
				return;
			}

			// Projectile.AI_061_FishingBobber then checks if ai[0] >= 1f and will reel in the bobber
			// Once it collides with the player, it will spawn the item assigned to ai[1] (Projectile.Kill -> Projectile.AI_061_FishingBobber_GiveItemToPlayer)

			// The collision is prevented to not spawn the items on the player during testing by 
			// 1. only calling AI (and not Update, which calls HandleMovement)
			// 2. moving the player hitbox out of the way before starting the testing

			Dictionary<int, int> catches = FishingTool.catches;
			catches.TryGetValue(type, out int currentCount);
			catches[type] = currentCount + 1;
		}
	}
}
