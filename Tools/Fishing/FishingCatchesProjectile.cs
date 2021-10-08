using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ModdersToolkit.Tools.Fishing
{
	// This class is used in conjunction with the FishingTool to "trigger" a catch the first tick the bobber gets wet
	// It then gets logged, and will repeat every call of AI until the testing is complete
	internal class FishingCatchesProjectile : GlobalProjectile
	{
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
				projectile.FishingCheck(); // Calls ModPlayer.CatchFish, after that assigns localAI[1] to the catch type
			}

			// Player.ItemCheck then checks if ai[0] is 0, and localAI[1] is above 0, then sets ai[0] to 1f and ai[1] to be localAI[1]
			// Simulate Player.ItemCheck here:
			if (projectile.ai[0] == 0 && projectile.localAI[1] > 0) {
				projectile.ai[0] = 1f;
				projectile.ai[1] = projectile.localAI[1];
			}

			// Prematurely reset it here:
			if (projectile.ai[1] > 0f && projectile.localAI[1] >= 0f) {
				projectile.localAI[1] = -1;
				// ...
			}

			// Projectile.AI then checks if ai[0] >= 1f and will reel in the bobber
			// Once it collides with the player, it will spawn the item assigned to ai[1]

			// The collision is prevented to not spawn the items on the player during testing by 
			// 1. only calling AI (and not Update, which calls HandleMovement)
			// 2. moving the player hitbox out of the way before starting the testing

			int itemType = (int)projectile.ai[1];

			Dictionary<int, int> catches = FishingTool.catches;
			catches.TryGetValue(itemType, out int currentCount);
			catches[itemType] = currentCount + 1;
		}
	}
}
