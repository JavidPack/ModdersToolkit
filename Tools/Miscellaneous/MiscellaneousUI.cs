using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.Miscellaneous
{
	// TODO!!!, this probably needs Liquid Data too!!!
	public readonly record struct TileData(TileTypeData TileTypeData, WallTypeData WallTypeData, TileWallWireStateData TileWallWireStateData);

	internal class MiscellaneousUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public MiscellaneousUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Miscellaneous:", 0.85f);
			AppendToAndAdjustWidthHeight(mainPanel, text, ref height, ref width);

			// checkboxes
			UICheckbox npcInfoCheckbox = new UICheckbox("NPC Info", "Show NPC ai values and other variables.");
			npcInfoCheckbox.OnSelectedChanged += () => MiscellaneousTool.showNPCInfo = npcInfoCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, npcInfoCheckbox, ref height, ref width);

			UICheckbox projectileInfoCheckbox = new UICheckbox("Projectile Info", "Show Projectile ai values and other variables.");
			projectileInfoCheckbox.OnSelectedChanged += () => MiscellaneousTool.showProjectileInfo = projectileInfoCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, projectileInfoCheckbox, ref height, ref width);

			UICheckbox lockProjectileInfoCheckbox = new UICheckbox("Lock Projectile Info", "Lock display to center of screen.");
			lockProjectileInfoCheckbox.OnSelectedChanged += () => MiscellaneousTool.lockProjectileInfo = lockProjectileInfoCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, lockProjectileInfoCheckbox, ref height, ref width);

			UICheckbox tileGridCheckbox = new UICheckbox("Tile Grid", "Show grid lines between tiles.");
			tileGridCheckbox.OnSelectedChanged += () => MiscellaneousTool.showTileGrid = tileGridCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, tileGridCheckbox, ref height, ref width);

			UITextPanel<string> calculateChunkData = new UITextPanel<string>("Calculate Chunk Size");
			calculateChunkData.SetPadding(4);
			//calculateChunkData.Width.Set(-10, 0.5f);
			calculateChunkData.OnClick += CalculateButton_OnClick;
			AppendToAndAdjustWidthHeight(mainPanel, calculateChunkData, ref height, ref width);


			UITextPanel<string> generateTownSprite = new UITextPanel<string>("Generate Town Sprite (WIP)");
			generateTownSprite.SetPadding(4);
			generateTownSprite.OnClick += GenerateTownSprite_OnClick;
			AppendToAndAdjustWidthHeight(mainPanel, generateTownSprite, ref height, ref width);


			UICheckbox collisionCircleCheckbox = new UICheckbox("Collision Circle", "Show a circle of Collision.CanHit");
			collisionCircleCheckbox.OnSelectedChanged += () => MiscellaneousTool.showCollisionCircle = collisionCircleCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, collisionCircleCheckbox, ref height, ref width);

			UICheckbox logSoundsCheckbox = new UICheckbox("Log Sounds", "Log Sound Styles and Types");
			logSoundsCheckbox.OnSelectedChanged += () => MiscellaneousTool.logSounds = logSoundsCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, logSoundsCheckbox, ref height, ref width);

			UITextPanel<string> takeWorldSnapshot = new UITextPanel<string>("Take World Snapshot (WIP)");
			takeWorldSnapshot.SetPadding(4);
			takeWorldSnapshot.OnClick += TakeWorldSnapshot_OnClick;
			;
			AppendToAndAdjustWidthHeight(mainPanel, takeWorldSnapshot, ref height, ref width);

			UITextPanel<string> restoreWorldSnapshot = new UITextPanel<string>("Restore World Snapshot (WIP)");
			restoreWorldSnapshot.SetPadding(4);
			restoreWorldSnapshot.OnClick += RestoreWorldSnapshot_OnClick;
			;
			AppendToAndAdjustWidthHeight(mainPanel, restoreWorldSnapshot, ref height, ref width);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		private TileData[,] snapshot;
		private void TakeWorldSnapshot_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.NewText("Taking Snapshot");
			snapshot = new TileData[Main.maxTilesX, Main.maxTilesY];
			for (int i = 0; i < Main.maxTilesX; i++) {
				for (int j = 0; j < Main.maxTilesY; j++) {
					//snapshot[i, j] = new Tile(Main.tile[i, j]);
					Tile tile = Main.tile[i, j];
					snapshot[i, j] = new (tile.Get<TileTypeData>(), tile.Get<WallTypeData>(), tile.Get<TileWallWireStateData>());
				}
			}
			Main.NewText("Taking Snapshot Complete");
		}

		private void RestoreWorldSnapshot_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if (snapshot == null) {
				Main.NewText("No snapshot available");
				return;
			}
			Main.NewText("Restoring Snapshot");
			for (int i = 0; i < Main.maxTilesX; i++) {
				for (int j = 0; j < Main.maxTilesY; j++) {
					Tile tile = Main.tile[i, j];
					tile.TileType = snapshot[i, j].TileTypeData.Type;
					tile.WallType = snapshot[i, j].WallTypeData.Type;
					tile.Get<TileWallWireStateData>() = snapshot[i, j].TileWallWireStateData;
					//Main.tile[i, j] = new Tile(snapshot[i, j]);
				}
			}
			Main.NewText("Restoring Snapshot Complete");
		}

		private static string folder = Path.Combine(Main.SavePath, "Mods", "Cache");
		private void GenerateTownSprite_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.NewText("Creating TownNPC sprite from current Player");

			int frames = 25;
			RenderTarget2D renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, 40, frames * 58);
			Main.instance.GraphicsDevice.SetRenderTarget(renderTarget);
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

			Main.gameMenu = true;
			// TODO: Figure out how to get all poses.
			for (int i = 0; i < frames; i++) {
				Main.LocalPlayer.PlayerFrame();
				Main.LocalPlayer.direction = -1;

				// Right now this code simply uses the Player animation frames, but they don't match up to TownNPC frames, and there are only 20 instead of ~25
				Main.LocalPlayer.bodyFrame.Y = i * 56;
				Main.LocalPlayer.legFrame.Y = i * 56;

				Main.NewText("This feature unimplemented");
				// TODO: Reimplement with new Camera system?
				//Main.instance.DrawPlayer(Main.LocalPlayer, Main.screenPosition + /*Main.LocalPlayer.position +*/ new Vector2(10, i * 58), 0f, Vector2.Zero, 0f); // add Main.screenPosition since DrawPlayer will subtract it
			}
			Main.gameMenu = false;

			Main.spriteBatch.End();
			Main.instance.GraphicsDevice.SetRenderTarget(null);

			Directory.CreateDirectory(folder);
			string path = Path.Combine(folder, "ModdersToolkit_ExportedTownNPCSprite.png");
			using (Stream stream = File.OpenWrite(path)) {
				renderTarget.SaveAsPng(stream, 40, frames * 58);
			}

			//	Process.Start(folder);
		}

		private void CalculateButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Point point = Main.LocalPlayer.Center.ToTileCoordinates();

			byte[] writeBuffer = new byte[131070];
			int sectionX = point.X / 200;
			int sectionY = point.Y / 150;
			int x = sectionX * 200;
			int y = sectionY * 150;
			// This doesn't return the correct number technically.
			int chunkDataSize = NetMessage.CompressTileBlock(x, y, 200, 150, writeBuffer, 0);

			if (chunkDataSize > 65535) // 65535 131070
				Main.NewText($"[c/FF0000:Bad]: {chunkDataSize} > 65535");
			else
				Main.NewText($"[c/00FF00:OK]: {chunkDataSize} < 65535");

			Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 200, y + 150) * 16, 80, Color.White, null);
			//Dust.QuickBox(new Vector2(x, y) * 16 + new Vector2(3, 3) * 16, new Vector2(x + 200, y + 150) * 16 - new Vector2(3, 3) * 16, 38, Color.LightSkyBlue, null);
			//Dust.QuickBox(new Vector2(x, y) * 16 + new Vector2(6, 6) * 16, new Vector2(x + 200, y + 150) * 16 - new Vector2(6, 6) * 16, 36, Color.LightSteelBlue, null);

			using (MemoryStream memoryStream = new MemoryStream()) {
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {
					long position = memoryStream.Position;
					foreach (var item in TileEntity.ByPosition) {
						Point16 pos = item.Key;
						if (pos.X >= x && pos.X < x + 200 && pos.Y >= y && pos.Y < y + 150) {
							if (item.Value.type > 2) {
								short id = (short)item.Value.ID;
								TileEntity.Write(binaryWriter, TileEntity.ByID[id], false);

								long length = memoryStream.Position - position;
								position = memoryStream.Position;

								Main.NewText($"TE@{pos.X},{pos.Y}: {length}");
							}
						}
					}
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
