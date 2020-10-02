using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Spawns
{
    internal class SpawnTool : Tool
	{
		internal static SpawnUI spawnUI;
        internal static Dictionary<int, int> spawns;

        // This is a clone of vanilla code:
        internal static bool noSpawnCycle;
        internal static int defaultSpawnRate = 600;
        internal static int defaultMaxSpawns = 5;
        internal static int spawnRate = defaultSpawnRate;
        internal static int maxSpawns = defaultMaxSpawns;
        public static int sWidth = 1920;
        public static int sHeight = 1080;
        internal static int spawnRangeX = (int)((double)(NPC.sWidth / 16) * 0.7);
        internal static int spawnRangeY = (int)((double)(NPC.sHeight / 16) * 0.7);
        public static int safeRangeX = (int)((double)(NPC.sWidth / 16) * 0.52);
        public static int safeRangeY = (int)((double)(NPC.sHeight / 16) * 0.52);
        internal static int activeRangeX = (int)((double)NPC.sWidth * 2.1);
        internal static int activeRangeY = (int)((double)NPC.sHeight * 2.1);
        internal static int townRangeX = NPC.sWidth;
        internal static int townRangeY = NPC.sHeight;
        internal static int spawnSpaceX = 3;
        internal static int spawnSpaceY = 3;


		public override void Initialize()
		{
			ToggleTooltip = "Click to toggle NPC Spawn Tool";
			spawns = new Dictionary<int, int>();
		}

		public override void ClientInitialize()
		{
			Interface = new UserInterface();

            spawnUI = new SpawnUI(Interface);
            spawnUI.Activate();

            Interface.SetState(spawnUI);
		}

        public override void ClientTerminate()
        {
            Interface = default;

			spawnUI.Deactivate();
            spawnUI = default;
        }


        public override void UIDraw()
		{
			if (Visible)
			{
				spawnUI.Draw(Main.spriteBatch);
			}
		}

        internal static void CalculateSpawns()
		{
			spawns.Clear();

			for (int i = 0; i < 10000; i++)
			{
				SpawnNPC();
			}
		}

		public static int NewNPC(int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255)
		{
			int currentCount;
			spawns.TryGetValue(Type, out currentCount);
			spawns[Type] = currentCount + 1;

			//Main.NewText("Spawn " + Type);

			return 200;
		}

        public static void SpawnNPC()
		{
			if (noSpawnCycle)
			{
				noSpawnCycle = false;
				return;
			}
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					num4++;
				}
			}
			for (int j = 0; j < 255; j++)
			{
				if (Main.player[j].active && !Main.player[j].dead)
				{
					if (Main.slimeRain)
					{
						NPC.SlimeRainSpawns(j);
					}
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					bool flag6 = false;
					bool flag7 = false;
					bool flag8 = false;
					bool flag9 = false;
					bool flag10 = false;
					bool flag11 = false;
					bool flag12 = NPC.downedPlantBoss && Main.hardMode; //patch file: flag12
					if (Main.player[j].active && Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0 && (double)Main.player[j].position.Y < Main.worldSurface * 16.0 + (double)NPC.sHeight)
					{
						int num5 = 3000;
						if ((double)Main.player[j].position.X > Main.invasionX * 16.0 - (double)num5 && (double)Main.player[j].position.X < Main.invasionX * 16.0 + (double)num5)
						{
							//patch file: flag4
							flag4 = true;
						}
						else if (Main.invasionX >= (double)(Main.maxTilesX / 2 - 5) && Main.invasionX <= (double)(Main.maxTilesX / 2 + 5))
						{
							int k = 0;
							while (k < 200)
							{
								if (Main.npc[k].townNPC && Math.Abs(Main.player[j].position.X - Main.npc[k].Center.X) < (float)num5)
								{
									if (Main.rand.Next(3) == 0)
									{
										flag4 = true;
										break;
									}
									break;
								}
								else
								{
									k++;
								}
							}
						}
					}
					if (Main.player[j].ZoneTowerSolar || Main.player[j].ZoneTowerNebula || Main.player[j].ZoneTowerVortex || Main.player[j].ZoneTowerStardust)
					{
						flag4 = true;
					}
					bool flag13 = false;
					spawnRate = defaultSpawnRate;
					maxSpawns = defaultMaxSpawns;
					if (Main.hardMode)
					{
						spawnRate = (int)((double)defaultSpawnRate * 0.9);
						maxSpawns = defaultMaxSpawns + 1;
					}
					if (Main.player[j].position.Y > (float)((Main.maxTilesY - 200) * 16))
					{
						maxSpawns = (int)((float)maxSpawns * 2f);
					}
					else if ((double)Main.player[j].position.Y > Main.rockLayer * 16.0 + (double)NPC.sHeight)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.9f);
					}
					else if ((double)Main.player[j].position.Y > Main.worldSurface * 16.0 + (double)NPC.sHeight)
					{
						if (Main.hardMode)
						{
							spawnRate = (int)((double)spawnRate * 0.45);
							maxSpawns = (int)((float)maxSpawns * 1.8f);
						}
						else
						{
							spawnRate = (int)((double)spawnRate * 0.5);
							maxSpawns = (int)((float)maxSpawns * 1.7f);
						}
					}
					else if (!Main.dayTime)
					{
						spawnRate = (int)((double)spawnRate * 0.6);
						maxSpawns = (int)((float)maxSpawns * 1.3f);
						if (Main.bloodMoon)
						{
							spawnRate = (int)((double)spawnRate * 0.3);
							maxSpawns = (int)((float)maxSpawns * 1.8f);
						}
						if ((Main.pumpkinMoon || Main.snowMoon) && (double)Main.player[j].position.Y < Main.worldSurface * 16.0)
						{
							spawnRate = (int)((double)spawnRate * 0.2);
							maxSpawns *= 2;
						}
					}
					else if (Main.dayTime && Main.eclipse)
					{
						spawnRate = (int)((double)spawnRate * 0.2);
						maxSpawns = (int)((float)maxSpawns * 1.9f);
					}
					if (Main.player[j].ZoneSnow && (double)(Main.player[j].position.Y / 16f) < Main.worldSurface)
					{
						maxSpawns = (int)((float)maxSpawns + (float)maxSpawns * Main.cloudAlpha);
						spawnRate = (int)((float)spawnRate * (1f - Main.cloudAlpha + 1f) / 2f);
					}
					if (Main.player[j].ZoneDungeon)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.7f);
					}
					else if (Main.player[j].ZoneSandstorm)
					{
						spawnRate = (int)((float)spawnRate * (Main.hardMode ? 0.4f : 0.9f));
						maxSpawns = (int)((float)maxSpawns * (Main.hardMode ? 1.5f : 1.2f));
					}
					else if (Main.player[j].ZoneUndergroundDesert)
					{
						spawnRate = (int)((float)spawnRate * (Main.hardMode ? 0.2f : 0.3f));
						maxSpawns = (int)((float)maxSpawns * 2f);
					}
					else if (Main.player[j].ZoneJungle)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.5f);
					}
					else if (Main.player[j].ZoneCorrupt || Main.player[j].ZoneCrimson)
					{
						spawnRate = (int)((double)spawnRate * 0.65);
						maxSpawns = (int)((float)maxSpawns * 1.3f);
					}
					else if (Main.player[j].ZoneMeteor)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.1f);
					}
					if (Main.player[j].ZoneHoly && (double)Main.player[j].position.Y > Main.rockLayer * 16.0 + (double)NPC.sHeight)
					{
						spawnRate = (int)((double)spawnRate * 0.65);
						maxSpawns = (int)((float)maxSpawns * 1.3f);
					}
					if (Main.wof >= 0 && Main.player[j].position.Y > (float)((Main.maxTilesY - 200) * 16))
					{
						maxSpawns = (int)((float)maxSpawns * 0.3f);
						spawnRate *= 3;
					}
					if ((double)Main.player[j].activeNPCs < (double)maxSpawns * 0.2)
					{
						spawnRate = (int)((float)spawnRate * 0.6f);
					}
					else if ((double)Main.player[j].activeNPCs < (double)maxSpawns * 0.4)
					{
						spawnRate = (int)((float)spawnRate * 0.7f);
					}
					else if ((double)Main.player[j].activeNPCs < (double)maxSpawns * 0.6)
					{
						spawnRate = (int)((float)spawnRate * 0.8f);
					}
					else if ((double)Main.player[j].activeNPCs < (double)maxSpawns * 0.8)
					{
						spawnRate = (int)((float)spawnRate * 0.9f);
					}
					if ((double)(Main.player[j].position.Y / 16f) > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.player[j].ZoneCorrupt || Main.player[j].ZoneCrimson)
					{
						if ((double)Main.player[j].activeNPCs < (double)maxSpawns * 0.2)
						{
							spawnRate = (int)((float)spawnRate * 0.7f);
						}
						else if ((double)Main.player[j].activeNPCs < (double)maxSpawns * 0.4)
						{
							spawnRate = (int)((float)spawnRate * 0.9f);
						}
					}
					if (Main.player[j].calmed)
					{
						spawnRate = (int)((float)spawnRate * 1.3f);
						maxSpawns = (int)((float)maxSpawns * 0.7f);
					}
					if (Main.player[j].sunflower)
					{
						spawnRate = (int)((float)spawnRate * 1.2f);
						maxSpawns = (int)((float)maxSpawns * 0.8f);
					}
					if (Main.player[j].enemySpawns)
					{
						spawnRate = (int)((double)spawnRate * 0.5);
						maxSpawns = (int)((float)maxSpawns * 2f);
					}
					if (Main.player[j].ZoneWaterCandle || Main.player[j].inventory[Main.player[j].selectedItem].type == 148)
					{
						if (!Main.player[j].ZonePeaceCandle && Main.player[j].inventory[Main.player[j].selectedItem].type != 3117)
						{
							spawnRate = (int)((double)spawnRate * 0.75);
							maxSpawns = (int)((float)maxSpawns * 1.5f);
						}
					}
					else if (Main.player[j].ZonePeaceCandle || Main.player[j].inventory[Main.player[j].selectedItem].type == 3117)
					{
						spawnRate = (int)((double)spawnRate * 1.3);
						maxSpawns = (int)((float)maxSpawns * 0.7f);
					}
					if (Main.player[j].ZoneWaterCandle && (double)(Main.player[j].position.Y / 16f) < Main.worldSurface * 0.34999999403953552)
					{
						spawnRate = (int)((double)spawnRate * 0.5);
					}
					if ((double)spawnRate < (double)defaultSpawnRate * 0.1)
					{
						spawnRate = (int)((double)defaultSpawnRate * 0.1);
					}
					if (maxSpawns > defaultMaxSpawns * 3)
					{
						maxSpawns = defaultMaxSpawns * 3;
					}
					if ((Main.pumpkinMoon || Main.snowMoon) && (double)Main.player[j].position.Y < Main.worldSurface * 16.0)
					{
						maxSpawns = (int)((double)defaultMaxSpawns * (2.0 + 0.3 * (double)num4));
						spawnRate = 20;
					}
					if (Terraria.GameContent.Events.DD2Event.Ongoing && Main.player[j].ZoneOldOneArmy)
					{
						maxSpawns = defaultMaxSpawns;
						spawnRate = defaultSpawnRate;
					}
					if (flag4)
					{
						maxSpawns = (int)((double)defaultMaxSpawns * (2.0 + 0.3 * (double)num4));
						spawnRate = 20;
					}
					if (Main.player[j].ZoneDungeon && !NPC.downedBoss3)
					{
						spawnRate = 10;
					}
					if (!flag4 && ((!Main.bloodMoon && !Main.pumpkinMoon && !Main.snowMoon) || Main.dayTime) && (!Main.eclipse || !Main.dayTime) && !Main.player[j].ZoneDungeon && !Main.player[j].ZoneCorrupt && !Main.player[j].ZoneCrimson && !Main.player[j].ZoneMeteor && !Main.player[j].ZoneOldOneArmy)
					{
						if (Main.player[j].Center.Y / 16f > (float)(Main.maxTilesY - 200))
						{
							if (Main.player[j].townNPCs == 1f)
							{
								if (Main.rand.Next(2) == 0)
								{
									flag3 = true;
								}
								if (Main.rand.Next(10) == 0)
								{
									flag10 = true;
									maxSpawns = (int)((double)((float)maxSpawns) * 0.5);
								}
								else
								{
									spawnRate = (int)((double)((float)spawnRate) * 1.25);
								}
							}
							else if (Main.player[j].townNPCs == 2f)
							{
								if (Main.rand.Next(4) != 0)
								{
									flag3 = true;
								}
								if (Main.rand.Next(5) == 0)
								{
									flag10 = true;
									maxSpawns = (int)((double)((float)maxSpawns) * 0.5);
								}
								else
								{
									spawnRate = (int)((double)((float)spawnRate) * 1.5);
								}
							}
							else if (Main.player[j].townNPCs >= 3f)
							{
								if (Main.rand.Next(10) != 0)
								{
									flag3 = true;
								}
								if (Main.rand.Next(3) == 0)
								{
									flag10 = true;
									maxSpawns = (int)((double)((float)maxSpawns) * 0.5);
								}
								else
								{
									spawnRate = (int)((float)spawnRate * 2f);
								}
							}
						}
						else if (Main.player[j].townNPCs == 1f)
						{
							flag3 = true;
							if (Main.rand.Next(3) == 1)
							{
								flag10 = true;
								maxSpawns = (int)((double)((float)maxSpawns) * 0.6);
							}
							else
							{
								spawnRate = (int)((float)spawnRate * 2f);
							}
						}
						else if (Main.player[j].townNPCs == 2f)
						{
							flag3 = true;
							if (Main.rand.Next(3) != 0)
							{
								flag10 = true;
								maxSpawns = (int)((double)((float)maxSpawns) * 0.6);
							}
							else
							{
								spawnRate = (int)((float)spawnRate * 3f);
							}
						}
						else if (Main.player[j].townNPCs >= 3f)
						{
							flag3 = true;
							//patch file: flag3, flag10
							if (!Main.expertMode || Main.rand.Next(30) != 0)
							{
								flag10 = true;
							}
							maxSpawns = (int)((double)((float)maxSpawns) * 0.6);
						}
					}
					NPCLoader.EditSpawnRate(Main.player[j], ref spawnRate, ref maxSpawns);
					int num6 = (int)(Main.player[j].position.X + (float)(Main.player[j].width / 2)) / 16;
					int num7 = (int)(Main.player[j].position.Y + (float)(Main.player[j].height / 2)) / 16;
					if (Main.wallHouse[(int)Main.tile[num6, num7].wall])
					{
						flag3 = true;
					}
					if (Main.tile[num6, num7].wall == 87)
					{
						//patch file: flag2
						flag2 = true;
					}
					bool flag14 = false;
					//if (Main.player[j].active && !Main.player[j].dead && Main.player[j].activeNPCs < (float)maxSpawns && Main.rand.Next(spawnRate) == 0)
					if (Main.player[j].active && !Main.player[j].dead)
					{
						spawnRangeX = (int)((double)(NPC.sWidth / 16) * 0.7);
						spawnRangeY = (int)((double)(NPC.sHeight / 16) * 0.7);
						NPC.safeRangeX = (int)((double)(NPC.sWidth / 16) * 0.52);
						NPC.safeRangeY = (int)((double)(NPC.sHeight / 16) * 0.52);
						if (Main.player[j].inventory[Main.player[j].selectedItem].type == 1254 || Main.player[j].inventory[Main.player[j].selectedItem].type == 1299 || Main.player[j].scope)
						{
							float num8 = 1.5f;
							if (Main.player[j].inventory[Main.player[j].selectedItem].type == 1254 && Main.player[j].scope)
							{
								num8 = 1.25f;
							}
							else if (Main.player[j].inventory[Main.player[j].selectedItem].type == 1254)
							{
								num8 = 1.5f;
							}
							else if (Main.player[j].inventory[Main.player[j].selectedItem].type == 1299)
							{
								num8 = 1.5f;
							}
							else if (Main.player[j].scope)
							{
								num8 = 2f;
							}
							spawnRangeX += (int)((double)(NPC.sWidth / 16) * 0.5 / (double)num8);
							spawnRangeY += (int)((double)(NPC.sHeight / 16) * 0.5 / (double)num8);
							NPC.safeRangeX += (int)((double)(NPC.sWidth / 16) * 0.5 / (double)num8);
							NPC.safeRangeY += (int)((double)(NPC.sHeight / 16) * 0.5 / (double)num8);
						}
						NPCLoader.EditSpawnRange(Main.player[j], ref spawnRangeX, ref spawnRangeY,
							ref NPC.safeRangeX, ref NPC.safeRangeY);
						int num9 = (int)(Main.player[j].position.X / 16f) - spawnRangeX;
						int num10 = (int)(Main.player[j].position.X / 16f) + spawnRangeX;
						int num11 = (int)(Main.player[j].position.Y / 16f) - spawnRangeY;
						int num12 = (int)(Main.player[j].position.Y / 16f) + spawnRangeY;
						int num13 = (int)(Main.player[j].position.X / 16f) - NPC.safeRangeX;
						int num14 = (int)(Main.player[j].position.X / 16f) + NPC.safeRangeX;
						int num15 = (int)(Main.player[j].position.Y / 16f) - NPC.safeRangeY;
						int num16 = (int)(Main.player[j].position.Y / 16f) + NPC.safeRangeY;
						if (num9 < 0)
						{
							num9 = 0;
						}
						if (num10 > Main.maxTilesX)
						{
							num10 = Main.maxTilesX;
						}
						if (num11 < 0)
						{
							num11 = 0;
						}
						if (num12 > Main.maxTilesY)
						{
							num12 = Main.maxTilesY;
						}
						int l = 0;
						while (l < 50)
						{
							int num17 = Main.rand.Next(num9, num10);
							int num18 = Main.rand.Next(num11, num12);
							if (Main.tile[num17, num18].nactive() && Main.tileSolid[(int)Main.tile[num17, num18].type])
							{
								goto IL_1540;
							}
							if (!Main.wallHouse[(int)Main.tile[num17, num18].wall])
							{
								if (!flag4 && (double)num18 < Main.worldSurface * 0.34999999403953552 && !flag10 && ((double)num17 < (double)Main.maxTilesX * 0.45 || (double)num17 > (double)Main.maxTilesX * 0.55 || Main.hardMode))
								{
									num3 = (int)Main.tile[num17, num18].type;
									num = num17;
									num2 = num18;
									flag13 = true;
									flag = true;
								}
								else if (!flag4 && (double)num18 < Main.worldSurface * 0.44999998807907104 && !flag10 && Main.hardMode && Main.rand.Next(10) == 0)
								{
									num3 = (int)Main.tile[num17, num18].type;
									num = num17;
									num2 = num18;
									flag13 = true;
									flag = true;
								}
								else
								{
									int m = num18;
									while (m < Main.maxTilesY)
									{
										if (Main.tile[num17, m].nactive() && Main.tileSolid[(int)Main.tile[num17, m].type])
										{
											if (num17 < num13 || num17 > num14 || m < num15 || m > num16)
											{
												num3 = (int)Main.tile[num17, m].type;
												num = num17;
												num2 = m;
												flag13 = true;
												break;
											}
											break;
										}
										else
										{
											m++;
										}
									}
								}
								if (!flag13)
								{
									goto IL_1540;
								}
								int num19 = num - spawnSpaceX / 2;
								int num20 = num + spawnSpaceX / 2;
								int num21 = num2 - spawnSpaceY;
								int num22 = num2;
								if (num19 < 0)
								{
									flag13 = false;
								}
								if (num20 > Main.maxTilesX)
								{
									flag13 = false;
								}
								if (num21 < 0)
								{
									flag13 = false;
								}
								if (num22 > Main.maxTilesY)
								{
									flag13 = false;
								}
								if (flag13)
								{
									for (int n = num19; n < num20; n++)
									{
										for (int num23 = num21; num23 < num22; num23++)
										{
											if (Main.tile[n, num23].nactive() && Main.tileSolid[(int)Main.tile[n, num23].type])
											{
												flag13 = false;
												break;
											}
											if (Main.tile[n, num23].lava())
											{
												flag13 = false;
												break;
											}
										}
									}
								}
								if (num >= num13 && num <= num14)
								{
									//patch file(?): flag14
									flag14 = true;
									goto IL_1540;
								}
								goto IL_1540;
							}
						IL_1546:
							l++;
							continue;
						IL_1540:
							if (!flag13 && !flag13)
							{
								goto IL_1546;
							}
							break;
						}
					}
					if (flag13)
					{
						Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
						for (int num24 = 0; num24 < 255; num24++)
						{
							if (Main.player[num24].active)
							{
								Rectangle rectangle2 = new Rectangle((int)(Main.player[num24].position.X + (float)(Main.player[num24].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[num24].position.Y + (float)(Main.player[num24].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
								if (rectangle.Intersects(rectangle2))
								{
									flag13 = false;
								}
							}
						}
					}
					NPCSpawnInfo spawnInfo = new NPCSpawnInfo();
					if (flag13)
					{
						if (Main.player[j].ZoneDungeon && (!Main.tileDungeon[(int)Main.tile[num, num2].type] || Main.tile[num, num2 - 1].wall == 0))
						{
							flag13 = false;
						}
						if (Main.tile[num, num2 - 1].liquid > 0 && Main.tile[num, num2 - 2].liquid > 0 && !Main.tile[num, num2 - 1].lava())
						{
							//patch file: flag6
							if (Main.tile[num, num2 - 1].honey())
							{
								flag6 = true;
							}
							//patch file: flag5
							else
							{
								flag5 = true;
							}
						}
						int num25 = (int)Main.player[j].Center.X / 16;
						int num26 = (int)(Main.player[j].Bottom.Y + 8f) / 16;
						spawnInfo.playerFloorX = num25;
						spawnInfo.playerFloorY = num26;
						if (Main.tile[num, num2].type == 367)
						{
							flag8 = true;
						}
						else if (Main.tile[num, num2].type == 368)
						{
							//patch file: flag7
							flag7 = true;
						}
						else if (Main.tile[num25, num26].type == 367)
						{
							flag8 = true;
						}
						else if (Main.tile[num25, num26].type == 368)
						{
							flag7 = true;
						}
						else
						{
							int num27 = Main.rand.Next(20, 31);
							int num28 = Main.rand.Next(1, 4);
							if (num - num27 < 0)
							{
								num27 = num;
							}
							if (num2 - num27 < 0)
							{
								num27 = num2;
							}
							if (num + num27 >= Main.maxTilesX)
							{
								num27 = Main.maxTilesX - num - 1;
							}
							if (num2 + num27 >= Main.maxTilesY)
							{
								num27 = Main.maxTilesY - num2 - 1;
							}
							for (int num29 = num - num27; num29 <= num + num27; num29 += num28)
							{
								int num30 = Main.rand.Next(1, 4);
								for (int num31 = num2 - num27; num31 <= num2 + num27; num31 += num30)
								{
									if (Main.tile[num29, num31].type == 367)
									{
										flag8 = true;
									}
									if (Main.tile[num29, num31].type == 368)
									{
										flag7 = true;
									}
								}
							}
							num27 = Main.rand.Next(30, 61);
							num28 = Main.rand.Next(3, 7);
							if (num25 - num27 < 0)
							{
								num27 = num25;
							}
							if (num26 - num27 < 0)
							{
								num27 = num26;
							}
							if (num25 + num27 >= Main.maxTilesX)
							{
								num27 = Main.maxTilesX - num25 - 2;
							}
							if (num26 + num27 >= Main.maxTilesY)
							{
								num27 = Main.maxTilesY - num26 - 2;
							}
							for (int num32 = num25 - num27; num32 <= num25 + num27; num32 += num28)
							{
								int num33 = Main.rand.Next(3, 7);
								for (int num34 = num26 - num27; num34 <= num26 + num27; num34 += num33)
								{
									if (Main.tile[num32, num34].type == 367)
									{
										flag8 = true;
									}
									if (Main.tile[num32, num34].type == 368)
									{
										flag7 = true;
									}
								}
							}
						}
					}
					if (flag6)
					{
						flag13 = false;
					}
					if (flag13)
					{
						if ((double)num2 > Main.rockLayer && num2 < Main.maxTilesY - 200 && !Main.player[j].ZoneDungeon && !flag4)
						{
							if (Main.rand.Next(3) == 0)
							{
								int num35 = Main.rand.Next(5, 15);
								if (num - num35 >= 0 && num + num35 < Main.maxTilesX)
								{
									for (int num36 = num - num35; num36 < num + num35; num36++)
									{
										for (int num37 = num2 - num35; num37 < num2 + num35; num37++)
										{
											if (Main.tile[num36, num37].wall == 62)
											{
												//patch file: flag9
												flag9 = true;
											}
										}
									}
								}
							}
							else
							{
								int num38 = (int)Main.player[j].position.X / 16;
								int num39 = (int)Main.player[j].position.Y / 16;
								if (Main.tile[num38, num39].wall == 62)
								{
									flag9 = true;
								}
							}
						}
						if ((double)num2 < Main.rockLayer && num2 > 200 && !Main.player[j].ZoneDungeon && !flag4)
						{
							if (Main.rand.Next(3) == 0)
							{
								int num40 = Main.rand.Next(5, 15);
								if (num - num40 >= 0 && num + num40 < Main.maxTilesX)
								{
									for (int num41 = num - num40; num41 < num + num40; num41++)
									{
										for (int num42 = num2 - num40; num42 < num2 + num40; num42++)
										{
											if (WallID.Sets.Conversion.Sandstone[(int)Main.tile[num41, num42].wall] || WallID.Sets.Conversion.HardenedSand[(int)Main.tile[num41, num42].wall])
											{
												//patch file: flag11
												flag11 = true;
											}
										}
									}
								}
							}
							else
							{
								int num43 = (int)Main.player[j].position.X / 16;
								int num44 = (int)Main.player[j].position.Y / 16;
								if (WallID.Sets.Conversion.Sandstone[(int)Main.tile[num43, num44].wall] || WallID.Sets.Conversion.HardenedSand[(int)Main.tile[num43, num44].wall])
								{
									flag11 = true;
								}
							}
						}
						spawnInfo.spawnTileX = num;
						spawnInfo.spawnTileY = num2;
						spawnInfo.spawnTileType = num3;
						spawnInfo.player = Main.player[j];
						spawnInfo.sky = flag;
						spawnInfo.lihzahrd = flag2;
						spawnInfo.playerSafe = flag3;
						spawnInfo.invasion = flag4;
						spawnInfo.water = flag5;
						spawnInfo.granite = flag7;
						spawnInfo.marble = flag8;
						spawnInfo.spiderCave = flag9;
						spawnInfo.playerInTown = flag10;
						spawnInfo.desertCave = flag11;
						spawnInfo.planteraDefeated = flag12;
						spawnInfo.safeRangeX = flag14;
						int num45 = (int)Main.tile[num, num2].type;
						int num46 = 200;
						int? spawnChoice = NPCLoader.ChooseSpawn(spawnInfo);
						if (!spawnChoice.HasValue)
						{
							return;
						}
						int spawn = spawnChoice.Value;
						if (spawn != 0)
						{
							goto endVanillaSpawn;
						}
						if (Main.player[j].ZoneTowerNebula)
						{
							bool flag15 = true;
							int num47 = 0;
							while (flag15)
							{
								num47 = Utils.SelectRandom<int>(Main.rand, new int[]
									{
										424,
										424,
										424,
										423,
										423,
										423,
										421,
										421,
										421,
										421,
										421,
										420
									});
								flag15 = false;
								if (num47 == 424 && NPC.CountNPCS(num47) >= 2)
								{
									flag15 = true;
								}
								if (num47 == 423 && NPC.CountNPCS(num47) >= 3)
								{
									flag15 = true;
								}
								if (num47 == 420 && NPC.CountNPCS(num47) >= 2)
								{
									flag15 = true;
								}
							}
							if (num47 != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, num47, 1, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.player[j].ZoneTowerVortex)
						{
							bool flag16 = true;
							int num48 = 0;
							while (flag16)
							{
								num48 = Utils.SelectRandom<int>(Main.rand, new int[]
									{
										429,
										429,
										429,
										429,
										427,
										427,
										425,
										425,
										426
									});
								flag16 = false;
								if (num48 == 425 && NPC.CountNPCS(num48) >= 3)
								{
									flag16 = true;
								}
								if (num48 == 426 && NPC.CountNPCS(num48) >= 3)
								{
									flag16 = true;
								}
								if (num48 == 429 && NPC.CountNPCS(num48) >= 4)
								{
									flag16 = true;
								}
							}
							if (num48 != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, num48, 1, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.player[j].ZoneTowerStardust)
						{
							int num49 = Utils.SelectRandom<int>(Main.rand, new int[]
								{
									411,
									411,
									411,
									409,
									409,
									407,
									402,
									405
								});
							if (num49 != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, num49, 1, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.player[j].ZoneTowerSolar)
						{
							bool flag17 = true;
							int num50 = 0;
							while (flag17)
							{
								num50 = Utils.SelectRandom<int>(Main.rand, new int[]
									{
										518,
										419,
										418,
										412,
										417,
										416,
										415
									});
								flag17 = false;
								if (num50 == 415 && NPC.CountNPCS(num50) >= 2)
								{
									flag17 = true;
								}
								if (num50 == 416 && NPC.CountNPCS(num50) >= 1)
								{
									flag17 = true;
								}
								if (num50 == 518 && NPC.CountNPCS(num50) >= 2)
								{
									flag17 = true;
								}
								if (num50 == 412 && NPC.CountNPCS(num50) >= 1)
								{
									flag17 = true;
								}
							}
							if (num50 != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, num50, 1, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (flag)
						{
							int maxValue = 8;
							int maxValue2 = 30;
							bool flag18 = (float)Math.Abs(num - Main.maxTilesX / 2) / (float)(Main.maxTilesX / 2) > 0.33f && (Main.wallLight[(int)Main.tile[num6, num7].wall] || Main.tile[num6, num7].wall == 73);
							if (flag18)
							{
								bool flag19 = NPC.AnyDanger();
								if (flag19)
								{
									flag18 = false;
								}
							}
							if (Main.player[j].ZoneWaterCandle)
							{
								maxValue = 3;
								maxValue2 = 10;
							}
							if (flag4 && Main.invasionType == 4)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 388, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag18 && Main.hardMode && NPC.downedGolemBoss && ((!NPC.downedMartians && Main.rand.Next(maxValue) == 0) || Main.rand.Next(maxValue2) == 0) && !NPC.AnyNPCs(399))
							{
								NewNPC(num * 16 + 8, num2 * 16, 399, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag18 && Main.hardMode && NPC.downedGolemBoss && ((!NPC.downedMartians && Main.rand.Next(maxValue) == 0) || Main.rand.Next(maxValue2) == 0) && !NPC.AnyNPCs(399) && (Main.player[j].inventory[Main.player[j].selectedItem].type == 148 || Main.player[j].ZoneWaterCandle))
							{
								NewNPC(num * 16 + 8, num2 * 16, 399, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && !NPC.AnyNPCs(87) && !flag3 && Main.rand.Next(10) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 87, 1, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && !NPC.AnyNPCs(87) && !flag3 && Main.rand.Next(10) == 0 && (Main.player[j].inventory[Main.player[j].selectedItem].type == 148 || Main.player[j].ZoneWaterCandle))
							{
								NewNPC(num * 16 + 8, num2 * 16, 87, 1, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, 48, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (flag4)
						{
							if (Main.invasionType == 1)
							{
								if (Main.hardMode && !NPC.AnyNPCs(471) && Main.rand.Next(30) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 471, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(9) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 29, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 26, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 111, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 27, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 28, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.invasionType == 2)
							{
								if (Main.rand.Next(7) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 145, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 143, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 144, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.invasionType == 3)
							{
								if (Main.invasionSize < Main.invasionSizeStart / 2 && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(491) && !Collision.SolidTiles(num - 20, num + 20, num2 - 40, num2 - 10))
								{
									NewNPC(num * 16 + 8, (num2 - 10) * 16, 491, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(30) == 0 && !NPC.AnyNPCs(216))
								{
									NewNPC(num * 16 + 8, num2 * 16, 216, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(11) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 215, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(9) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 252, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(7) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 214, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 213, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 212, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.invasionType == 4)
							{
								int num51 = 0;
								int num52 = Main.rand.Next(7);
								if (Main.invasionSize <= 100 && Main.rand.Next(10) == 0 && !NPC.AnyNPCs(395))
								{
									num51 = 395;
								}
								else if (num52 >= 6)
								{
									if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(395))
									{
										num51 = 395;
									}
									else
									{
										int num53 = Main.rand.Next(2);
										if (num53 == 0)
										{
											num51 = 390;
										}
										if (num53 == 1)
										{
											num51 = 386;
										}
									}
								}
								else if (num52 >= 4)
								{
									int num54 = Main.rand.Next(5);
									if (num54 < 2)
									{
										num51 = 382;
									}
									else if (num54 < 4)
									{
										num51 = 381;
									}
									else
									{
										num51 = 388;
									}
								}
								else
								{
									int num55 = Main.rand.Next(4);
									if (num55 == 3)
									{
										if (!NPC.AnyNPCs(520))
										{
											num51 = 520;
										}
										else
										{
											num55 = Main.rand.Next(3);
										}
									}
									if (num55 == 0)
									{
										num51 = 385;
									}
									if (num55 == 1)
									{
										num51 = 389;
									}
									if (num55 == 2)
									{
										num51 = 383;
									}
								}
								if (num51 != 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, num51, 1, 0f, 0f, 0f, 0f, 255);
								}
							}
						}
						else if (!NPC.savedBartender && DD2Event.ReadyToFindBartender && !NPC.AnyNPCs(579) && Main.rand.Next(80) == 0 && !flag5)
						{
							NewNPC(num * 16 + 8, num2 * 16, 579, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.tile[num, num2].wall == 62 || flag9)
						{
							if (Main.tile[num, num2].wall == 62 && Main.rand.Next(8) == 0 && !flag5 && (double)num2 >= Main.rockLayer && num2 < Main.maxTilesY - 210 && !NPC.savedStylist && !NPC.AnyNPCs(354))
							{
								NewNPC(num * 16 + 8, num2 * 16, 354, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode)
							{
								NewNPC(num * 16 + 8, num2 * 16, 163, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, 164, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if ((WallID.Sets.Conversion.HardenedSand[(int)Main.tile[num, num2].wall] || WallID.Sets.Conversion.Sandstone[(int)Main.tile[num, num2].wall] || flag11) && WorldGen.checkUnderground(num, num2))
						{
							if (Main.hardMode && Main.rand.Next(33) == 0 && !flag3 && (double)num2 > Main.worldSurface + 100.0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 510, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(22) == 0 && !flag3 && (double)num2 > Main.worldSurface + 100.0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 513, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && Main.rand.Next(5) != 0)
							{
								List<int> list = new List<int>();
								if (Main.player[j].ZoneCorrupt)
								{
									list.Add(525);
									list.Add(525);
								}
								if (Main.player[j].ZoneCrimson)
								{
									list.Add(526);
									list.Add(526);
								}
								if (Main.player[j].ZoneHoly)
								{
									list.Add(527);
									list.Add(527);
								}
								if (list.Count == 0)
								{
									list.Add(524);
									list.Add(524);
								}
								if (Main.player[j].ZoneCorrupt || Main.player[j].ZoneCrimson)
								{
									list.Add(533);
									list.Add(529);
								}
								else
								{
									list.Add(530);
									list.Add(528);
								}
								list.Add(532);
								int num56 = Utils.SelectRandom<int>(Main.rand, list.ToArray());
								NewNPC(num * 16 + 8, num2 * 16, num56, 0, 0f, 0f, 0f, 0f, 255);
								list.Clear();
							}
							else
							{
								int num57 = Utils.SelectRandom<int>(Main.rand, new int[]
									{
										69,
										508,
										508,
										508,
										509
									});
								NewNPC(num * 16 + 8, num2 * 16, num57, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.hardMode && flag5 && Main.player[j].ZoneJungle && Main.rand.Next(3) != 0)
						{
							NewNPC(num * 16 + 8, num2 * 16, 157, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && flag5 && Main.player[j].ZoneCrimson && Main.rand.Next(3) != 0)
						{
							NewNPC(num * 16 + 8, num2 * 16, 242, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && flag5 && Main.player[j].ZoneCrimson && Main.rand.Next(3) != 0)
						{
							NewNPC(num * 16 + 8, num2 * 16, 241, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (flag5 && (num < 250 || num > Main.maxTilesX - 250) && Main.tileSand[num45] && (double)num2 < Main.rockLayer)
						{
							bool flag20 = false;
							if (!NPC.savedAngler && !NPC.AnyNPCs(376))
							{
								int num58 = -1;
								for (int num59 = num2 - 1; num59 > num2 - 50; num59--)
								{
									if (Main.tile[num, num59].liquid == 0 && !WorldGen.SolidTile(num, num59) && !WorldGen.SolidTile(num, num59 + 1) && !WorldGen.SolidTile(num, num59 + 2))
									{
										num58 = num59 + 2;
										break;
									}
								}
								if (num58 > num2)
								{
									num58 = num2;
								}
								if (num58 > 0 && !flag14)
								{
									NewNPC(num * 16 + 8, num58 * 16, 376, 0, 0f, 0f, 0f, 0f, 255);
									flag20 = true;
								}
							}
							if (!flag20)
							{
								if (Main.rand.Next(60) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 220, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(25) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 221, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 65, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 67, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 64, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
						}
						else if (!flag5 && !NPC.savedAngler && !NPC.AnyNPCs(376) && (num < 340 || num > Main.maxTilesX - 340) && Main.tileSand[num45] && (double)num2 < Main.worldSurface)
						{
							NewNPC(num * 16 + 8, num2 * 16, 376, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (flag5 && (((double)num2 > Main.rockLayer && Main.rand.Next(2) == 0) || num45 == 60))
						{
							if (Main.hardMode && Main.rand.Next(3) > 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 102, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, 58, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (flag5 && (double)num2 > Main.worldSurface && Main.rand.Next(3) == 0)
						{
							if (Main.hardMode)
							{
								NewNPC(num * 16 + 8, num2 * 16, 103, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, 63, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (flag5 && Main.rand.Next(4) == 0)
						{
							if (Main.player[j].ZoneCorrupt)
							{
								NewNPC(num * 16 + 8, num2 * 16, 57, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((double)num2 < Main.worldSurface && num2 > 50 && Main.rand.Next(3) != 0 && Main.dayTime)
							{
								int num60 = -1;
								for (int num61 = num2 - 1; num61 > num2 - 50; num61--)
								{
									if (Main.tile[num, num61].liquid == 0 && !WorldGen.SolidTile(num, num61) && !WorldGen.SolidTile(num, num61 + 1) && !WorldGen.SolidTile(num, num61 + 2))
									{
										num60 = num61 + 2;
										break;
									}
								}
								if (num60 > num2)
								{
									num60 = num2;
								}
								if (num60 > 0 && !flag14)
								{
									if (Main.rand.Next(2) == 0)
									{
										NewNPC(num * 16 + 8, num60 * 16, 362, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num60 * 16, 364, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 55, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, 55, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (NPC.downedGoblins && Main.rand.Next(20) == 0 && !flag5 && (double)num2 >= Main.rockLayer && num2 < Main.maxTilesY - 210 && !NPC.savedGoblin && !NPC.AnyNPCs(105))
						{
							NewNPC(num * 16 + 8, num2 * 16, 105, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && Main.rand.Next(20) == 0 && !flag5 && (double)num2 >= Main.rockLayer && num2 < Main.maxTilesY - 210 && !NPC.savedWizard && !NPC.AnyNPCs(106))
						{
							NewNPC(num * 16 + 8, num2 * 16, 106, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (flag10)
						{
							if (flag5)
							{
								if ((double)num2 < Main.worldSurface && num2 > 50 && Main.rand.Next(3) != 0 && Main.dayTime)
								{
									int num62 = -1;
									for (int num63 = num2 - 1; num63 > num2 - 50; num63--)
									{
										if (Main.tile[num, num63].liquid == 0 && !WorldGen.SolidTile(num, num63) && !WorldGen.SolidTile(num, num63 + 1) && !WorldGen.SolidTile(num, num63 + 2))
										{
											num62 = num63 + 2;
											break;
										}
									}
									if (num62 > num2)
									{
										num62 = num2;
									}
									if (num62 > 0 && !flag14)
									{
										if (Main.rand.Next(2) == 0)
										{
											NewNPC(num * 16 + 8, num62 * 16, 362, 0, 0f, 0f, 0f, 0f, 255);
										}
										else
										{
											NewNPC(num * 16 + 8, num62 * 16, 364, 0, 0f, 0f, 0f, 0f, 255);
										}
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 55, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 55, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num45 == 147 || num45 == 161)
							{
								if (Main.rand.Next(2) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 148, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 149, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num45 == 60)
							{
								if (Main.rand.Next(NPC.goldCritterChance) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 445, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 361, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else
							{
								if (num45 != 2 && num45 != 109 && (double)num2 <= Main.worldSurface)
								{
									return;
								}
								if (Main.raining)
								{
									if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 448, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (Main.rand.Next(3) != 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 357, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 230, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (!Main.dayTime && Main.rand.Next(NPC.fireFlyFriendly) == 0 && (double)num2 <= Main.worldSurface)
								{
									int num64 = 355;
									if (num45 == 109)
									{
										num64 = 358;
									}
									NewNPC(num * 16 + 8, num2 * 16, num64, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
									{
										NewNPC(num * 16 + 8 - 16, num2 * 16, num64, 0, 0f, 0f, 0f, 0f, 255);
									}
									if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
									{
										NewNPC(num * 16 + 8 + 16, num2 * 16, num64, 0, 0f, 0f, 0f, 0f, 255);
									}
									if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16 - 16, num64, 0, 0f, 0f, 0f, 0f, 255);
									}
									if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16 + 16, num64, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (Main.dayTime && Main.time < 18000.0 && Main.rand.Next(3) != 0 && (double)num2 <= Main.worldSurface)
								{
									int num65 = Main.rand.Next(4);
									if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 442, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num65 == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 297, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num65 == 1)
									{
										NewNPC(num * 16 + 8, num2 * 16, 298, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 74, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (Main.dayTime && Main.rand.Next(NPC.butterflyChance) == 0 && (double)num2 <= Main.worldSurface)
								{
									if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 444, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 356, 0, 0f, 0f, 0f, 0f, 255);
									}
									if (Main.rand.Next(4) == 0)
									{
										NewNPC(num * 16 + 8 - 16, num2 * 16, 356, 0, 0f, 0f, 0f, 0f, 255);
									}
									if (Main.rand.Next(4) == 0)
									{
										NewNPC(num * 16 + 8 + 16, num2 * 16, 356, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (Main.rand.Next(2) == 0 && (double)num2 <= Main.worldSurface)
								{
									int num66 = Main.rand.Next(4);
									if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 442, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num66 == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 297, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num66 == 1)
									{
										NewNPC(num * 16 + 8, num2 * 16, 298, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 74, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (num45 == 53)
								{
									NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(366, 368), 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(NPC.goldCritterChance) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 443, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(NPC.goldCritterChance) == 0 && (double)num2 <= Main.worldSurface)
								{
									NewNPC(num * 16 + 8, num2 * 16, 539, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.halloween && Main.rand.Next(3) != 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 303, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.xMas && Main.rand.Next(3) != 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 337, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (BirthdayParty.PartyIsUp && Main.rand.Next(3) != 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, 540, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0 && (double)num2 <= Main.worldSurface)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)Utils.SelectRandom<short>(Main.rand, new short[]
											{
												299,
												538
											}), 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, 46, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
						}
						else if (Main.player[j].ZoneDungeon)
						{
							int num67 = 0;
							if (Main.tile[num, num2].wall == 94 || Main.tile[num, num2].wall == 96 || Main.tile[num, num2].wall == 98)
							{
								num67 = 1;
							}
							if (Main.tile[num, num2].wall == 95 || Main.tile[num, num2].wall == 97 || Main.tile[num, num2].wall == 99)
							{
								num67 = 2;
							}
							if (Main.rand.Next(7) == 0)
							{
								num67 = Main.rand.Next(3);
							}
							if (!NPC.downedBoss3)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 68, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (!NPC.savedMech && Main.rand.Next(5) == 0 && !flag5 && !NPC.AnyNPCs(123) && (double)num2 > Main.rockLayer)
							{
								NewNPC(num * 16 + 8, num2 * 16, 123, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && Main.rand.Next(30) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 287, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && num67 == 0 && Main.rand.Next(15) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 293, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && num67 == 1 && Main.rand.Next(15) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 291, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && num67 == 2 && Main.rand.Next(15) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 292, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && !NPC.AnyNPCs(290) && num67 == 0 && Main.rand.Next(35) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 290, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && (num67 == 1 || num67 == 2) && Main.rand.Next(30) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 289, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag12 && Main.rand.Next(20) == 0)
							{
								int num68 = 281;
								if (num67 == 0)
								{
									num68 += 2;
								}
								if (num67 == 2)
								{
									num68 += 4;
								}
								num68 += Main.rand.Next(2);
								if (!NPC.AnyNPCs(num68))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, num68, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (flag12 && Main.rand.Next(3) != 0)
							{
								int num69 = 269;
								if (num67 == 0)
								{
									num69 += 4;
								}
								if (num67 == 2)
								{
									num69 += 8;
								}
								num46 = NewNPC(num * 16 + 8, num2 * 16, num69 + Main.rand.Next(4), 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(37) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 71, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (num67 == 1 && Main.rand.Next(4) == 0 && !NPC.NearSpikeBall(num, num2))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 70, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (num67 == 2 && Main.rand.Next(15) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 72, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (num67 == 0 && Main.rand.Next(9) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 34, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(7) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 32, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								int num70 = Main.rand.Next(5);
								if (num70 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 294, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num70 == 1)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 295, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num70 == 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 296, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 31, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-14, -1f);
									}
									else if (Main.rand.Next(5) == 0)
									{
										Main.npc[num46].SetDefaults(-13, -1f);
									}
								}
							}
						}
						else if (Main.player[j].ZoneMeteor)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 23, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (DD2Event.Ongoing && Main.player[j].ZoneOldOneArmy)
						{
							DD2Event.SpawnNPC(ref num46);
						}
						else if ((double)num2 <= Main.worldSurface && !Main.dayTime && Main.snowMoon)
						{
							int num71 = NPC.waveNumber;
							if (Main.rand.Next(30) == 0 && NPC.CountNPCS(341) < 4)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 341, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (num71 >= 20)
							{
								int num72 = Main.rand.Next(3);
								if (num72 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num72 == 1)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 >= 19)
							{
								if (Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 4)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 5)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 7)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 >= 18)
							{
								if (Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 4)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 6)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 351, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 >= 17)
							{
								if (Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 5)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 351, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 >= 16)
							{
								if (Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 4)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 352, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 >= 15)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 14)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 13)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 352, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(6) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 12)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 343, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 11)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 345, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(6) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 352, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 10)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(6) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 351, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 9)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 8)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 351, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 7)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 346, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 6)
							{
								if (Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 347, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 5)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 4)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 344, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 3)
							{
								if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 348, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num71 == 2)
							{
								if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 350, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 342, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(338, 341), 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if ((double)num2 <= Main.worldSurface && !Main.dayTime && Main.pumpkinMoon)
						{
							int num73 = NPC.waveNumber;
							if (NPC.waveNumber >= 15)
							{
								if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 14)
							{
								if (Main.rand.Next(5) == 0 && NPC.CountNPCS(327) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0 && NPC.CountNPCS(325) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 315, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 13)
							{
								if (Main.rand.Next(7) == 0 && NPC.CountNPCS(327) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								if (Main.rand.Next(5) == 0 && NPC.CountNPCS(325) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0 && NPC.CountNPCS(315) < 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 315, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 12)
							{
								if (Main.rand.Next(7) == 0 && NPC.CountNPCS(327) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								if (Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(7) == 0 && NPC.CountNPCS(315) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 315, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(7) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 11)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								if (Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(315))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 315, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(7) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 10)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(325))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(315))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 315, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 9)
							{
								if (Main.rand.Next(8) == 0 && !NPC.AnyNPCs(327))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0 && !NPC.AnyNPCs(325))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(315))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 315, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 8)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 7)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 327, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 6)
							{
								if (Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(6) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 5)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(325))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(8) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 330, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(5) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 4)
							{
								if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(325))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 325, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(10) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 3)
							{
								if (Main.rand.Next(6) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 329, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num73 == 2)
							{
								if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 326, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(305, 315), 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if ((double)num2 <= Main.worldSurface && Main.dayTime && Main.eclipse)
						{
							bool flag21 = false;
							if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
							{
								flag21 = true;
							}
							if (flag21 && Main.rand.Next(80) == 0 && !NPC.AnyNPCs(477))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 477, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(50) == 0 && !NPC.AnyNPCs(251))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 251, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (NPC.downedPlantBoss && Main.rand.Next(5) == 0 && !NPC.AnyNPCs(466))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 466, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (NPC.downedPlantBoss && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(463))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 463, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (NPC.downedPlantBoss && Main.rand.Next(20) == 0 && NPC.CountNPCS(467) < 2)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 467, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(15) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 159, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag21 && Main.rand.Next(13) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 253, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(8) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 469, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (NPC.downedPlantBoss && Main.rand.Next(7) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 468, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (NPC.downedPlantBoss && Main.rand.Next(5) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 460, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(4) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 162, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 461, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 462, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 166, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.hardMode && num3 == 70 && flag5)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 256, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (num3 == 70 && (double)num2 <= Main.worldSurface && Main.rand.Next(3) != 0)
						{
							if ((!Main.hardMode && Main.rand.Next(6) == 0) || Main.rand.Next(12) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 360, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(3) == 0)
							{
								if (Main.rand.Next(4) == 0)
								{
									if (Main.hardMode && Main.rand.Next(3) != 0)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 260, 0, 0f, 0f, 0f, 0f, 255);
										Main.npc[num46].ai[0] = (float)num;
										Main.npc[num46].ai[1] = (float)num2;
										Main.npc[num46].netUpdate = true;
									}
									else
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 259, 0, 0f, 0f, 0f, 0f, 255);
										Main.npc[num46].ai[0] = (float)num;
										Main.npc[num46].ai[1] = (float)num2;
										Main.npc[num46].netUpdate = true;
									}
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 257, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 258, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 254, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 255, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (num3 == 70 && Main.hardMode && (double)num2 >= Main.worldSurface && Main.rand.Next(3) != 0)
						{
							if (Main.hardMode && Main.rand.Next(5) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 374, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((!Main.hardMode && Main.rand.Next(4) == 0) || Main.rand.Next(8) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 360, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(4) == 0)
							{
								if (Main.hardMode && Main.rand.Next(3) != 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 260, 0, 0f, 0f, 0f, 0f, 255);
									Main.npc[num46].ai[0] = (float)num;
									Main.npc[num46].ai[1] = (float)num2;
									Main.npc[num46].netUpdate = true;
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 259, 0, 0f, 0f, 0f, 0f, 255);
									Main.npc[num46].ai[0] = (float)num;
									Main.npc[num46].ai[1] = (float)num2;
									Main.npc[num46].netUpdate = true;
								}
							}
							else if (Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 257, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 258, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.player[j].ZoneCorrupt && Main.rand.Next(65) == 0 && !flag3)
						{
							if (Main.hardMode && Main.rand.Next(4) != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 98, 1, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 7, 1, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.hardMode && (double)num2 > Main.worldSurface && Main.rand.Next(75) == 0)
						{
							if (Main.rand.Next(2) == 0 && Main.player[j].ZoneCorrupt && !NPC.AnyNPCs(473))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 473, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(2) == 0 && Main.player[j].ZoneCrimson && !NPC.AnyNPCs(474))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 474, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(2) == 0 && Main.player[j].ZoneHoly && !NPC.AnyNPCs(475))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 475, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 85, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.hardMode && Main.tile[num, num2 - 1].wall == 2 && Main.rand.Next(20) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 85, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && (double)num2 <= Main.worldSurface && !Main.dayTime && (Main.rand.Next(20) == 0 || (Main.rand.Next(5) == 0 && Main.moonPhase == 4)))
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 82, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && Main.halloween && (double)num2 <= Main.worldSurface && !Main.dayTime && Main.rand.Next(10) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 304, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (num45 == 60 && Main.rand.Next(500) == 0 && !Main.dayTime)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 52, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (num45 == 60 && (double)num2 > Main.worldSurface && Main.rand.Next(60) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 219, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if ((double)num2 > Main.worldSurface && num2 < Main.maxTilesY - 210 && !Main.player[j].ZoneSnow && !Main.player[j].ZoneCrimson && !Main.player[j].ZoneCorrupt && !Main.player[j].ZoneJungle && !Main.player[j].ZoneHoly && Main.rand.Next(8) == 0)
						{
							if (Main.rand.Next(NPC.goldCritterChance) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 448, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 357, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if ((double)num2 > Main.worldSurface && num2 < Main.maxTilesY - 210 && !Main.player[j].ZoneSnow && !Main.player[j].ZoneCrimson && !Main.player[j].ZoneCorrupt && !Main.player[j].ZoneJungle && !Main.player[j].ZoneHoly && Main.rand.Next(13) == 0)
						{
							if (Main.rand.Next(NPC.goldCritterChance) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 447, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 300, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if ((double)num2 > Main.worldSurface && (double)num2 < (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && !Main.player[j].ZoneSnow && !Main.player[j].ZoneCrimson && !Main.player[j].ZoneCorrupt && !Main.player[j].ZoneHoly && Main.rand.Next(13) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 359, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if ((double)num2 < Main.worldSurface && Main.player[j].ZoneJungle && Main.rand.Next(9) == 0)
						{
							if (Main.rand.Next(NPC.goldCritterChance) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 445, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, 361, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (num45 == 60 && Main.hardMode && Main.rand.Next(3) != 0)
						{
							if ((double)num2 < Main.worldSurface && !Main.dayTime && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 152, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((double)num2 < Main.worldSurface && Main.dayTime && Main.rand.Next(4) != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 177, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((double)num2 > Main.worldSurface && Main.rand.Next(100) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 205, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((double)num2 > Main.worldSurface && Main.rand.Next(5) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 236, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((double)num2 > Main.worldSurface && Main.rand.Next(4) != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 176, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(10) == 0)
								{
									Main.npc[num46].SetDefaults(-18, -1f);
								}
								if (Main.rand.Next(10) == 0)
								{
									Main.npc[num46].SetDefaults(-19, -1f);
								}
								if (Main.rand.Next(10) == 0)
								{
									Main.npc[num46].SetDefaults(-20, -1f);
								}
								if (Main.rand.Next(10) == 0)
								{
									Main.npc[num46].SetDefaults(-21, -1f);
								}
							}
							else if (Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 175, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num46].ai[0] = (float)num;
								Main.npc[num46].ai[1] = (float)num2;
								Main.npc[num46].netUpdate = true;
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 153, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (num45 == 226 && flag2)
						{
							if (Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 226, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 198, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (num45 == 60 && (double)num2 > (Main.worldSurface + Main.rockLayer) / 2.0)
						{
							if (Main.rand.Next(4) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 204, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(4) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 43, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num46].ai[0] = (float)num;
								Main.npc[num46].ai[1] = (float)num2;
								Main.npc[num46].netUpdate = true;
							}
							else
							{
								int num74 = Main.rand.Next(8);
								if (num74 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 231, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-56, -1f);
									}
									else if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-57, -1f);
									}
								}
								else if (num74 == 1)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 232, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-58, -1f);
									}
									else if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-59, -1f);
									}
								}
								else if (num74 == 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 233, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-60, -1f);
									}
									else if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-61, -1f);
									}
								}
								else if (num74 == 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 234, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-62, -1f);
									}
									else if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-63, -1f);
									}
								}
								else if (num74 == 4)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 235, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-64, -1f);
									}
									else if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-65, -1f);
									}
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 42, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-16, -1f);
									}
									else if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-17, -1f);
									}
								}
							}
						}
						else if (num45 == 60 && Main.rand.Next(4) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 51, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (num45 == 60 && Main.rand.Next(8) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 56, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num46].ai[0] = (float)num;
							Main.npc[num46].ai[1] = (float)num2;
							Main.npc[num46].netUpdate = true;
						}
						else if (Sandstorm.Happening && Main.player[j].ZoneSandstorm && TileID.Sets.Conversion.Sand[num45] && NPC.Spawning_SandstoneCheck(num, num2))
						{
							if (!NPC.downedBoss1 && !Main.hardMode)
							{
								if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 546, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 508, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 61, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 69, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.hardMode && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(541))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 541, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && !flag3 && Main.rand.Next(3) == 0 && NPC.CountNPCS(510) < 4)
							{
								num46 = NewNPC(num * 16 + 8, (num2 + 10) * 16, 510, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && !flag3 && Main.rand.Next(2) == 0)
							{
								int num75 = 542;
								if (TileID.Sets.Corrupt[num45])
								{
									num75 = 543;
								}
								if (TileID.Sets.Crimson[num45])
								{
									num75 = 544;
								}
								if (TileID.Sets.Hallow[num45])
								{
									num75 = 545;
								}
								num46 = NewNPC(num * 16 + 8, num2 * 16, num75, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && num45 == 53 && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 78, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && (num45 == 112 || num45 == 234) && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 79, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && num45 == 116 && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 80, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 546, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 508, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 509, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.hardMode && num45 == 53 && Main.rand.Next(3) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 78, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && (num45 == 112 || num45 == 234) && Main.rand.Next(2) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 79, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && num45 == 116 && Main.rand.Next(2) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 80, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && !flag5 && (double)num2 < Main.rockLayer && (num45 == 116 || num45 == 117 || num45 == 109 || num45 == 164))
						{
							if (!Main.dayTime && Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 122, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(10) == 0 || (Main.player[j].ZoneWaterCandle && Main.rand.Next(10) == 0))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 86, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 75, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (!flag3 && Main.hardMode && Main.rand.Next(50) == 0 && !flag5 && (double)num2 >= Main.rockLayer && (num45 == 116 || num45 == 117 || num45 == 109 || num45 == 164))
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 84, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if ((num45 == 204 && Main.player[j].ZoneCrimson) || num45 == 199 || num45 == 200 || num45 == 203 || num45 == 234)
						{
							if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(5) == 0 && !flag3)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 182, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 268, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 183, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(3) == 0)
								{
									Main.npc[num46].SetDefaults(-24, -1f);
								}
								else if (Main.rand.Next(3) == 0)
								{
									Main.npc[num46].SetDefaults(-25, -1f);
								}
							}
							else if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(40) == 0 && !flag3)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 179, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && (Main.rand.Next(2) == 0 || (double)num2 > Main.worldSurface))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 174, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((Main.tile[num, num2].wall > 0 && Main.rand.Next(4) != 0) || Main.rand.Next(8) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 239, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 181, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 173, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(3) == 0)
								{
									Main.npc[num46].SetDefaults(-22, -1f);
								}
								else if (Main.rand.Next(3) == 0)
								{
									Main.npc[num46].SetDefaults(-23, -1f);
								}
							}
						}
						else if ((num45 == 22 && Main.player[j].ZoneCorrupt) || num45 == 23 || num45 == 25 || num45 == 112 || num45 == 163)
						{
							if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 101, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num46].ai[0] = (float)num;
								Main.npc[num46].ai[1] = (float)num2;
								Main.npc[num46].netUpdate = true;
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								if (Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 121, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 81, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(40) == 0 && !flag3)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 83, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && (Main.rand.Next(2) == 0 || (double)num2 > Main.rockLayer))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 94, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 6, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(3) == 0)
								{
									Main.npc[num46].SetDefaults(-11, -1f);
								}
								else if (Main.rand.Next(3) == 0)
								{
									Main.npc[num46].SetDefaults(-12, -1f);
								}
							}
						}
						else if ((double)num2 <= Main.worldSurface)
						{
							bool flag22 = (float)Math.Abs(num - Main.maxTilesX / 2) / (float)(Main.maxTilesX / 2) > 0.33f;
							if (flag22)
							{
								bool flag23 = NPC.AnyDanger();
								if (flag23)
								{
									flag22 = false;
								}
							}
							if (Main.player[j].ZoneSnow && Main.hardMode && Main.cloudAlpha > 0f && !NPC.AnyNPCs(243) && Main.rand.Next(20) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 243, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.player[j].ZoneHoly && Main.hardMode && Main.cloudAlpha > 0f && !NPC.AnyNPCs(244) && Main.rand.Next(20) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 244, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (!Main.player[j].ZoneSnow && Main.hardMode && Main.cloudAlpha > 0f && NPC.CountNPCS(250) < 2 && Main.rand.Next(10) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, 250, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag22 && Main.hardMode && NPC.downedGolemBoss && ((!NPC.downedMartians && Main.rand.Next(100) == 0) || Main.rand.Next(400) == 0) && !NPC.AnyNPCs(399))
							{
								NewNPC(num * 16 + 8, num2 * 16, 399, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.dayTime)
							{
								int num76 = Math.Abs(num - Main.spawnTileX);
								if (num76 < Main.maxTilesX / 3 && Main.rand.Next(15) == 0 && (num45 == 2 || num45 == 109 || num45 == 147 || num45 == 161))
								{
									if (num45 == 147 || num45 == 161)
									{
										if (Main.rand.Next(2) == 0)
										{
											NewNPC(num * 16 + 8, num2 * 16, 148, 0, 0f, 0f, 0f, 0f, 255);
										}
										else
										{
											NewNPC(num * 16 + 8, num2 * 16, 149, 0, 0f, 0f, 0f, 0f, 255);
										}
									}
									else if (Main.dayTime && Main.rand.Next(NPC.butterflyChance) == 0 && (double)num2 <= Main.worldSurface)
									{
										if (Main.rand.Next(NPC.goldCritterChance) == 0)
										{
											NewNPC(num * 16 + 8, num2 * 16, 444, 0, 0f, 0f, 0f, 0f, 255);
										}
										else
										{
											NewNPC(num * 16 + 8, num2 * 16, 356, 0, 0f, 0f, 0f, 0f, 255);
										}
										if (Main.rand.Next(4) == 0)
										{
											NewNPC(num * 16 + 8 - 16, num2 * 16, 356, 0, 0f, 0f, 0f, 0f, 255);
										}
										if (Main.rand.Next(4) == 0)
										{
											NewNPC(num * 16 + 8 + 16, num2 * 16, 356, 0, 0f, 0f, 0f, 0f, 255);
										}
									}
									else if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 443, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (Main.rand.Next(NPC.goldCritterChance) == 0 && (double)num2 <= Main.worldSurface)
									{
										NewNPC(num * 16 + 8, num2 * 16, 539, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (Main.halloween && Main.rand.Next(3) != 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 303, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (Main.xMas && Main.rand.Next(3) != 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 337, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (BirthdayParty.PartyIsUp && Main.rand.Next(3) != 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 540, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (Main.rand.Next(3) == 0 && (double)num2 <= Main.worldSurface)
									{
										NewNPC(num * 16 + 8, num2 * 16, (int)Utils.SelectRandom<short>(Main.rand, new short[]
												{
													299,
													538
												}), 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 46, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (num76 < Main.maxTilesX / 3 && Main.rand.Next(15) == 0 && num45 == 53)
								{
									NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(366, 368), 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num76 < Main.maxTilesX / 3 && Main.dayTime && Main.time < 18000.0 && (num45 == 2 || num45 == 109) && Main.rand.Next(4) == 0 && (double)num2 <= Main.worldSurface && NPC.CountNPCS(74) + NPC.CountNPCS(297) + NPC.CountNPCS(298) < 6)
								{
									int num77 = Main.rand.Next(4);
									if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 442, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num77 == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 297, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num77 == 1)
									{
										NewNPC(num * 16 + 8, num2 * 16, 298, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 74, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (num76 < Main.maxTilesX / 3 && Main.rand.Next(15) == 0 && (num45 == 2 || num45 == 109 || num45 == 147))
								{
									int num78 = Main.rand.Next(4);
									if (Main.rand.Next(NPC.goldCritterChance) == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 442, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num78 == 0)
									{
										NewNPC(num * 16 + 8, num2 * 16, 297, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (num78 == 1)
									{
										NewNPC(num * 16 + 8, num2 * 16, 298, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										NewNPC(num * 16 + 8, num2 * 16, 74, 0, 0f, 0f, 0f, 0f, 255);
									}
								}
								else if (num76 > Main.maxTilesX / 3 && num45 == 2 && Main.rand.Next(300) == 0 && !NPC.AnyNPCs(50))
								{
									NPC.SpawnOnPlayer(j, 50);
								}
								else if (num45 == 53 && Main.rand.Next(5) == 0 && NPC.Spawning_SandstoneCheck(num, num2) && !flag5)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 69, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num45 == 53 && Main.rand.Next(3) == 0 && !flag5)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 537, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num45 == 53 && !flag5)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 61, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num76 > Main.maxTilesX / 3 && (Main.rand.Next(15) == 0 || (!NPC.downedGoblins && WorldGen.shadowOrbSmashed && Main.rand.Next(7) == 0)))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 73, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.raining && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 224, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.raining && Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 225, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 1, 0, 0f, 0f, 0f, 0f, 255);
									if (num45 == 60)
									{
										Main.npc[num46].SetDefaults(-10, -1f);
									}
									else if (num45 == 161 || num45 == 147)
									{
										Main.npc[num46].SetDefaults(147, -1f);
									}
									else if (Main.halloween && Main.rand.Next(3) != 0)
									{
										Main.npc[num46].SetDefaults(302, -1f);
									}
									else if (Main.xMas && Main.rand.Next(3) != 0)
									{
										Main.npc[num46].SetDefaults(Main.rand.Next(333, 337), -1f);
									}
									else if (Main.rand.Next(3) == 0 || (num76 < 200 && !Main.expertMode))
									{
										Main.npc[num46].SetDefaults(-3, -1f);
									}
									else if (Main.rand.Next(10) == 0 && (num76 > 400 || Main.expertMode))
									{
										Main.npc[num46].SetDefaults(-7, -1f);
									}
								}
							}
							else if ((num3 == 2 || num3 == 109) && Main.rand.Next(NPC.fireFlyChance) == 0 && (double)num2 <= Main.worldSurface)
							{
								int num79 = 355;
								if (num45 == 109)
								{
									num79 = 358;
								}
								NewNPC(num * 16 + 8, num2 * 16, num79, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
								{
									NewNPC(num * 16 + 8 - 16, num2 * 16, num79, 0, 0f, 0f, 0f, 0f, 255);
								}
								if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
								{
									NewNPC(num * 16 + 8 + 16, num2 * 16, num79, 0, 0f, 0f, 0f, 0f, 255);
								}
								if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16 - 16, num79, 0, 0f, 0f, 0f, 0f, 255);
								}
								if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16 + 16, num79, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.rand.Next(10) == 0 && Main.halloween)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 301, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(6) == 0 || (Main.moonPhase == 4 && Main.rand.Next(2) == 0))
							{
								if (Main.hardMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 133, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.halloween && Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(317, 319), 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 2, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(4) == 0)
									{
										Main.npc[num46].SetDefaults(-43, -1f);
									}
								}
								else
								{
									int num80 = Main.rand.Next(5);
									if (num80 == 0)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 190, 0, 0f, 0f, 0f, 0f, 255);
										if (Main.rand.Next(3) == 0)
										{
											Main.npc[num46].SetDefaults(-38, -1f);
										}
									}
									else if (num80 == 1)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 191, 0, 0f, 0f, 0f, 0f, 255);
										if (Main.rand.Next(3) == 0)
										{
											Main.npc[num46].SetDefaults(-39, -1f);
										}
									}
									else if (num80 == 2)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 192, 0, 0f, 0f, 0f, 0f, 255);
										if (Main.rand.Next(3) == 0)
										{
											Main.npc[num46].SetDefaults(-40, -1f);
										}
									}
									else if (num80 == 3)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 193, 0, 0f, 0f, 0f, 0f, 255);
										if (Main.rand.Next(3) == 0)
										{
											Main.npc[num46].SetDefaults(-41, -1f);
										}
									}
									else if (num80 == 4)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 194, 0, 0f, 0f, 0f, 0f, 255);
										if (Main.rand.Next(3) == 0)
										{
											Main.npc[num46].SetDefaults(-42, -1f);
										}
									}
								}
							}
							else if (Main.hardMode && Main.rand.Next(50) == 0 && Main.bloodMoon && !NPC.AnyNPCs(109))
							{
								NewNPC(num * 16 + 8, num2 * 16, 109, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(250) == 0 && Main.bloodMoon)
							{
								NewNPC(num * 16 + 8, num2 * 16, 53, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(250) == 0 && Main.bloodMoon)
							{
								NewNPC(num * 16 + 8, num2 * 16, 536, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.moonPhase == 0 && Main.hardMode && Main.rand.Next(3) != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 104, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 140, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.bloodMoon && Main.rand.Next(5) < 2)
							{
								if (Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 489, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 490, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (num3 == 147 || num3 == 161 || num3 == 163 || num3 == 164 || num3 == 162)
							{
								if (Main.hardMode && Main.rand.Next(4) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 169, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.hardMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 155, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.expertMode && Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 431, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 161, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.raining && Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 223, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(3) == 0)
								{
									if (Main.rand.Next(2) == 0)
									{
										Main.npc[num46].SetDefaults(-54, -1f);
									}
									else
									{
										Main.npc[num46].SetDefaults(-55, -1f);
									}
								}
							}
							else
							{
								int num81 = Main.rand.Next(7);
								if (Main.halloween && Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(319, 322), 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.xMas && Main.rand.Next(2) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(331, 333), 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 0 && Main.expertMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 430, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 2 && Main.expertMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 432, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 3 && Main.expertMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 433, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 4 && Main.expertMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 434, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 5 && Main.expertMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 435, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 6 && Main.expertMode && Main.rand.Next(3) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 436, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num81 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 3, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-26, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-27, -1f);
										}
									}
								}
								else if (num81 == 1)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 132, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-28, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-29, -1f);
										}
									}
								}
								else if (num81 == 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 186, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-30, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-31, -1f);
										}
									}
								}
								else if (num81 == 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 187, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-32, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-33, -1f);
										}
									}
								}
								else if (num81 == 4)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 188, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-34, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-35, -1f);
										}
									}
								}
								else if (num81 == 5)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 189, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-36, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-37, -1f);
										}
									}
								}
								else if (num81 == 6)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 200, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-44, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-45, -1f);
										}
									}
								}
							}
						}
						else if ((double)num2 <= Main.rockLayer)
						{
							if (!flag3 && Main.rand.Next(50) == 0 && !Main.player[j].ZoneSnow)
							{
								if (Main.hardMode)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 95, 1, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.player[j].ZoneSnow)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 185, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 10, 1, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 140, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && Main.rand.Next(4) != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 141, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (num45 == 147 || num45 == 161 || Main.player[j].ZoneSnow)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 147, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 1, 0, 0f, 0f, 0f, 0f, 255);
								if (Main.rand.Next(5) == 0)
								{
									Main.npc[num46].SetDefaults(-9, -1f);
								}
								else if (Main.rand.Next(2) == 0)
								{
									Main.npc[num46].SetDefaults(1, -1f);
								}
								else
								{
									Main.npc[num46].SetDefaults(-8, -1f);
								}
							}
						}
						else if (num2 > Main.maxTilesY - 190)
						{
							if (Main.hardMode && !NPC.savedTaxCollector && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(534))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 534, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(40) == 0 && !NPC.AnyNPCs(39))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 39, 1, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(14) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 24, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(7) == 0)
							{
								if (Main.rand.Next(7) == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 66, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (Main.hardMode && NPC.downedMechBossAny && Main.rand.Next(5) != 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 156, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 62, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 59, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && NPC.downedMechBossAny && Main.rand.Next(5) != 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 151, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 60, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.rand.Next(60) == 0)
						{
							if (Main.player[j].ZoneSnow)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 218, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 217, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if ((num45 == 116 || num45 == 117 || num45 == 164) && Main.hardMode && !flag3 && Main.rand.Next(8) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 120, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if ((num3 == 147 || num3 == 161 || num3 == 162 || num3 == 163 || num3 == 164) && !flag3 && Main.hardMode && Main.player[j].ZoneCorrupt && Main.rand.Next(30) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 170, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if ((num3 == 147 || num3 == 161 || num3 == 162 || num3 == 163 || num3 == 164) && !flag3 && Main.hardMode && Main.player[j].ZoneHoly && Main.rand.Next(30) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 171, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if ((num3 == 147 || num3 == 161 || num3 == 162 || num3 == 163 || num3 == 164) && !flag3 && Main.hardMode && Main.player[j].ZoneCrimson && Main.rand.Next(30) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 180, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && Main.player[j].ZoneSnow && Main.rand.Next(10) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 154, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (!flag3 && Main.rand.Next(100) == 0 && !Main.player[j].ZoneHoly)
						{
							if (Main.hardMode)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 95, 1, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.player[j].ZoneSnow)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 185, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 10, 1, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (Main.player[j].ZoneSnow && Main.rand.Next(20) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 185, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (!Main.hardMode && Main.rand.Next(10) == 0)
						{
							if (Main.player[j].ZoneSnow)
							{
								Main.npc[num46].SetDefaults(184, -1f);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 16, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else if (!Main.hardMode && Main.rand.Next(4) == 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 1, 0, 0f, 0f, 0f, 0f, 255);
							if (Main.player[j].ZoneJungle)
							{
								Main.npc[num46].SetDefaults(-10, -1f);
							}
							else if (Main.player[j].ZoneSnow)
							{
								Main.npc[num46].SetDefaults(184, -1f);
							}
							else
							{
								Main.npc[num46].SetDefaults(-6, -1f);
							}
						}
						else if (Main.rand.Next(2) == 0)
						{
							if (Main.rand.Next(35) == 0 && NPC.CountNPCS(453) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 453, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((!Main.hardMode && Main.rand.Next(80) == 0) || Main.rand.Next(200) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 195, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.hardMode && (double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(300) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 172, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && (Main.rand.Next(200) == 0 || (Main.rand.Next(50) == 0 && Main.player[j].armor[1].type >= 1282 && Main.player[j].armor[1].type <= 1287 && Main.player[j].armor[0].type != 238)))
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 45, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (flag8 && Main.rand.Next(4) != 0)
							{
								if (Main.rand.Next(6) != 0 && !NPC.AnyNPCs(480) && Main.hardMode)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 480, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 481, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (flag7 && Main.rand.Next(5) != 0)
							{
								if (Main.rand.Next(6) != 0 && !NPC.AnyNPCs(483))
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 483, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 482, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.hardMode && Main.rand.Next(10) != 0)
							{
								if (Main.rand.Next(2) == 0)
								{
									if (Main.player[j].ZoneSnow)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 197, 0, 0f, 0f, 0f, 0f, 255);
									}
									else if (Main.halloween && Main.rand.Next(5) == 0)
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 316, 0, 0f, 0f, 0f, 0f, 255);
									}
									else
									{
										num46 = NewNPC(num * 16 + 8, num2 * 16, 77, 0, 0f, 0f, 0f, 0f, 255);
										if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(5) == 0)
										{
											Main.npc[num46].SetDefaults(-15, -1f);
										}
									}
								}
								else if (Main.player[j].ZoneSnow)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 206, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 110, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else if (Main.rand.Next(20) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 44, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (num3 == 147 || num3 == 161 || num3 == 162)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 167, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.player[j].ZoneSnow)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 185, 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.rand.Next(3) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, NPC.cavernMonsterType[Main.rand.Next(2), Main.rand.Next(3)], 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.halloween && Main.rand.Next(2) == 0)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, Main.rand.Next(322, 325), 0, 0f, 0f, 0f, 0f, 255);
							}
							else if (Main.expertMode && Main.rand.Next(3) == 0)
							{
								int num82 = Main.rand.Next(4);
								if (num82 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 449, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num82 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 450, 0, 0f, 0f, 0f, 0f, 255);
								}
								else if (num82 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 451, 0, 0f, 0f, 0f, 0f, 255);
								}
								else
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 452, 0, 0f, 0f, 0f, 0f, 255);
								}
							}
							else
							{
								int num83 = Main.rand.Next(4);
								if (num83 == 0)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 21, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-47, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-46, -1f);
										}
									}
								}
								else if (num83 == 1)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 201, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-49, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-48, -1f);
										}
									}
								}
								else if (num83 == 2)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 202, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-51, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-50, -1f);
										}
									}
								}
								else if (num83 == 3)
								{
									num46 = NewNPC(num * 16 + 8, num2 * 16, 203, 0, 0f, 0f, 0f, 0f, 255);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[num46].SetDefaults(-53, -1f);
										}
										else
										{
											Main.npc[num46].SetDefaults(-52, -1f);
										}
									}
								}
							}
						}
						else if (Main.hardMode && (Main.player[j].ZoneHoly & Main.rand.Next(2) == 0))
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 138, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.player[j].ZoneJungle)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 51, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && Main.player[j].ZoneHoly)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 137, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (Main.hardMode && Main.rand.Next(6) > 0)
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 93, 0, 0f, 0f, 0f, 0f, 255);
						}
						else if (num3 == 147 || num3 == 161 || num3 == 162)
						{
							if (Main.hardMode)
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 169, 0, 0f, 0f, 0f, 0f, 255);
							}
							else
							{
								num46 = NewNPC(num * 16 + 8, num2 * 16, 150, 0, 0f, 0f, 0f, 0f, 255);
							}
						}
						else
						{
							num46 = NewNPC(num * 16 + 8, num2 * 16, 49, 0, 0f, 0f, 0f, 0f, 255);
						}
					endVanillaSpawn: //this ugly code is just to minimize the diff file
						if (spawn != 0)
						{
							NewNPC(0,0, spawn);
							//num46 = NPCLoader.SpawnNPC(spawn, num, num2);
						}
						if (Main.npc[num46].type == 1 && Main.rand.Next(180) == 0)
						{
							Main.npc[num46].SetDefaults(-4, -1f);
						}
						if (Main.netMode == 2 && num46 < 200)
						{
							//NetMessage.SendData(23, -1, -1, "", num46, 0f, 0f, 0f, 0, 0, 0);
							return;
						}
						break;
					}
				}
			}
		}
	}
}
