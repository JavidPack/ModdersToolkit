using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace ModdersToolkit.Tools.Fishing
{
	// code based on SpawnTool
	internal class FishingTool : Tool
	{
		internal const int Tries = 1000;
		internal static bool currentlyTesting = false;
		internal static Dictionary<int, int> catches; // keys below 1 are treated as "no catch", special check in UI

		internal static FishingUI fishingUI;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Fishing Tool";
			catches = new Dictionary<int, int>();
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			fishingUI = new FishingUI(Interface);
			fishingUI.Activate();

			Interface.SetState(fishingUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			fishingUI.Deactivate();
			fishingUI = null;

			currentlyTesting = false; // Just in case
			catches = null;
		}

		public override void UIDraw() {
			if (Visible) {
				fishingUI.Draw(Main.spriteBatch);
			}
		}

		internal static int GetPoolStats(Vector2 startPosition, out bool lava, out bool honey) {
			// Vanilla code to determine pool size of a liquid lake
			// Added GetTileSafely just in case, and a visualizer

			// This only works if the start position is already in liquid
			int startX = (int)(startPosition.X / 16f);
			int startY = (int)(startPosition.Y / 16f);
			if (Main.tile[startX, startY].LiquidAmount < 0) {
				// ???
				startY++;
			}

			lava = false;
			honey = false;
			int leftStart = startX;
			int rightEnd = startX;
			while (leftStart > 10 && Framing.GetTileSafely(leftStart, startY).LiquidAmount > 0 && !WorldGen.SolidTile(leftStart, startY)) {
				leftStart--;
			}

			for (; rightEnd < Main.maxTilesX - 10 && Framing.GetTileSafely(rightEnd, startY).LiquidAmount > 0 && !WorldGen.SolidTile(rightEnd, startY); rightEnd++) {
			}

			int poolSize = 0;
			for (int x = leftStart; x <= rightEnd; x++) {
				int y = startY;
				while (Framing.GetTileSafely(x, y).LiquidAmount > 0 && !WorldGen.SolidTile(x, y) && y < Main.maxTilesY - 10) {
					poolSize++;
					y++;
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.LiquidType == LiquidID.Lava) {
						lava = true;
					}
					else if (tile.LiquidType == LiquidID.Honey) {
						honey = true;
					}

					// Added code to visualize pool dimensions
					if (poolSize < 1000) {
						// 1000 is highest value vanilla checks
						Dust.QuickDust(new Point(x, y - 1), Color.Orange);
					}
				}
			}

			if (honey) {
				poolSize = (int)(poolSize * 1.5f);
			}

			return poolSize;
		}

		internal static bool ValidateSpawnPosition(Player player, out Vector2 spawnPosition) {
			spawnPosition = Vector2.Zero;
			// Find suitable spot to spawn a bobber at

			//if (player.wet) {
			//	Main.NewText("Player needs to NOT be in liquids");
			//	return false;
			//}

			if (player.HeldItem.fishingPole <= 0) {
				Main.NewText("Player needs to hold a fishing pole");
				return false;
			}

			bool hasBait = false;
			for (int j = 0; j < Main.InventorySlotsTotal; j++) {
				Item item = player.inventory[j];
				if (!item.IsAir && item.bait > 0 && item.type != ItemID.TruffleWorm) {
					hasBait = true;
					break;
				}
			}

			if (!hasBait) {
				Main.NewText("Player needs to have bait in the inventory");
				return false;
			}

			// Check if any liquid exists below player in a 2 wide pillar (until maxYRange)
			// both directions: If starting tile has liquid: check downwards
			//    If not: check upwards

			int startX = (int)(player.Left.X / 16);
			int endX = startX + 1;
			int startY = (int)(player.Center.Y / 16);
			int x = startX;
			int y = startY;
			int maxYRange = 30;
			bool inLiquidAndAirAbove = false;
			bool inAirAndLiquidBelow = false;

			if (Framing.GetTileSafely(startX, startY).LiquidAmount > 0) {
				//Check upwards for air
				while (WorldGen.InWorld(x, y, 40) && y >= startY - maxYRange) {
					for (x = startX; x <= endX; x++) {
						Tile tile = Framing.GetTileSafely(x, y);
						if (!tile.IsActiveUnactuated && tile.LiquidAmount == 0) {
							inLiquidAndAirAbove = true;
							break;
						}

						if (x == endX) {
							y--;
						}
					}

					if (inLiquidAndAirAbove) {
						break;
					}
				}
			}
			else {
				//Check downwards for liquid
				while (WorldGen.InWorld(x, y, 40) && y < startY + maxYRange) {
					for (x = startX; x <= endX; x++) {
						Tile tile = Framing.GetTileSafely(x, y);
						if (!tile.IsActiveUnactuated && tile.LiquidAmount > 0) {
							inAirAndLiquidBelow = true;
							y--; // -1 so above water, bobbers need to be in air for atleast one tick
							break;
						}

						if (x == endX) {
							y++;
						}
					}

					if (inAirAndLiquidBelow) {
						break;
					}
				}
			}

			if (!inAirAndLiquidBelow && !inLiquidAndAirAbove) {
				string genericPositionTip = $"Tip: Stand/Hover a small distance over the pool or submerged in the pool";
				Main.NewText($"Player needs to be within atleast {maxYRange} tiles below or above the surface of a pool of liquid");

				Main.NewText(genericPositionTip);
				return false;
			}

			spawnPosition = new Vector2(x, y) * 16;
			return true;
		}

		internal static void CalculateCatches(Vector2 spawnPosition, int bobberType, out int poolSize, out bool lava, out bool honey) {
			poolSize = 0;
			lava = false;
			honey = false;
			Player player = Main.LocalPlayer;
			Vector2 originalCenter = player.Center;
			bool originalWet = player.wet;
			bool originalHoneyWet = player.honeyWet;
			bool originalLavaWet = player.lavaWet;
			Projectile bobber = null;
			try {
				// Spawn bobber with downward starting velocity
				int index = Projectile.NewProjectile(null, spawnPosition, Vector2.UnitY * 8, bobberType, 0, 0f, Main.myPlayer);
				if (index >= Main.maxProjectiles) {
					return;
				}

				bobber = Main.projectile[index];

				// Debug: Visualize bobber spawn location
				//for (int i = 0; i < 10; i++)
				//{
				//	Dust.NewDust(bobber.position, 16, 16, DustID.Fire);
				//}

				catches.Clear();

				currentlyTesting = true;
				int failureCount = 0;

				player.Center = spawnPosition - new Vector2(0f, bobber.height * 2); //Temporarily teleport player above the bobber + twice the bobbers height
				player.wet = false;
				player.honeyWet = false;
				player.lavaWet = false;

				for (int i = 0; i < Tries; i++) {
					failureCount++;

					if (!bobber.wet) {
						// If not wet, call Update so it moves into liquid first (handles everything including AI)
						bobber.Update(index);
						i--; // Make sure we actually test Tries times, the bobber does not catch if not wet
					}
					else {
						// Once wet, just call AI. Calling Update will reel the bobber back to the player otherwise, which is not intended
						bobber.AI();

						if (poolSize == 0) {
							poolSize = GetPoolStats(bobber.Center, out lava, out honey);
						}
					}

					if (failureCount >= 2 * Tries) {
						break;
					}
				}
			}
			finally {
				player.Center = originalCenter;
				player.wet = originalWet;
				player.honeyWet = originalHoneyWet;
				player.lavaWet = originalLavaWet;
				currentlyTesting = false;
				if(bobber != null) {
					bobber.ai[1] = 0;
					bobber.Kill();
				}
			}


			// Debug: Visualize bobber final location
			//for (int i = 0; i < 10; i++)
			//{
			//	Dust.NewDust(bobber.position, 16, 16, DustID.Fire);
			//}
		}
	}
}
