using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using System.Text;
using ModdersToolkit.UIElements;
using ModdersToolkit.Tools;
using System.IO;
using Terraria.DataStructures;

namespace ModdersToolkit.Tools.Miscellaneous
{
	class MiscellaneousUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public MiscellaneousUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			mainPanel.SetPadding(0);
			int width = 150;
			int height = 180;
			mainPanel.Left.Set(-40f - width, 1f);
			mainPanel.Top.Set(-110f - height, 1f);
			mainPanel.Width.Set(width, 0f);
			mainPanel.Height.Set(height, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Miscellaneous:", 0.85f);
			text.Top.Set(12f, 0f);
			text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			// checkboxes
			int top = 32;
			UICheckbox npcInfoCheckbox = new UICheckbox("NPC Info", "Show NPC ai values and other variables.");
			npcInfoCheckbox.Top.Set(top, 0f);
			npcInfoCheckbox.Left.Set(12f, 0f);
			npcInfoCheckbox.OnSelectedChanged += () => MiscellaneousTool.showNPCInfo = npcInfoCheckbox.Selected;
			mainPanel.Append(npcInfoCheckbox);
			top += 20;

			UICheckbox tileGridCheckbox = new UICheckbox("Tile Grid", "Show grid lines between tiles.");
			tileGridCheckbox.Top.Set(top, 0f);
			tileGridCheckbox.Left.Set(12f, 0f);
			tileGridCheckbox.OnSelectedChanged += () => MiscellaneousTool.showTileGrid = tileGridCheckbox.Selected;
			mainPanel.Append(tileGridCheckbox);
			top += 20;

			UITextPanel<string> calculateChunkData = new UITextPanel<string>("Calculate Chunk Size");
			calculateChunkData.SetPadding(4);
			calculateChunkData.Width.Set(-10, 0.5f);
			calculateChunkData.Top.Set(top, 0f);
			calculateChunkData.OnClick += CalculateButton_OnClick;
			mainPanel.Append(calculateChunkData);
			top += 30;

			
			UITextPanel<string> generateTownSprite = new UITextPanel<string>("Generate Town Sprite");
			generateTownSprite.SetPadding(4);
			generateTownSprite.Width.Set(-10, 0.5f);
			generateTownSprite.Top.Set(top, 0f);
			generateTownSprite.OnClick += GenerateTownSprite_OnClick;
			mainPanel.Append(generateTownSprite);
			top += 30;
			

			UICheckbox collisionCircleCheckbox = new UICheckbox("Collision Circle", "Show a circle of Collision.CanHit");
			collisionCircleCheckbox.Top.Set(top, 0f);
			collisionCircleCheckbox.Left.Set(12f, 0f);
			collisionCircleCheckbox.OnSelectedChanged += () => MiscellaneousTool.showCollisionCircle = collisionCircleCheckbox.Selected;
			mainPanel.Append(collisionCircleCheckbox);
			top += 20;

			UICheckbox logSoundsCheckbox = new UICheckbox("Log Sounds", "Log Sound Styles and Types");
			logSoundsCheckbox.Top.Set(top, 0f);
			logSoundsCheckbox.Left.Set(12f, 0f);
			logSoundsCheckbox.OnSelectedChanged += () => MiscellaneousTool.logSounds = logSoundsCheckbox.Selected;
			mainPanel.Append(logSoundsCheckbox);
			top += 20;

			Append(mainPanel);
		}


		static string folder = Path.Combine(Main.SavePath, "Mods", "Cache");
		private void GenerateTownSprite_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
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

				Main.instance.DrawPlayer(Main.LocalPlayer, Main.screenPosition + /*Main.LocalPlayer.position +*/ new Vector2(10, i * 58), 0f, Vector2.Zero, 0f); // add Main.screenPosition since DrawPlayer will subtract it
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

		private void CalculateButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
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

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					long position = memoryStream.Position;
					foreach (var item in TileEntity.ByPosition)
					{
						Point16 pos = item.Key;
						if (pos.X >= x && pos.X < x + 200 && pos.Y >= y && pos.Y < y + 150)
						{
							if (item.Value.type > 2)
							{
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

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (mainPanel.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
