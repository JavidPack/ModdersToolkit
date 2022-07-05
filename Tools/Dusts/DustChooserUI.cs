using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Dusts
{
	// todo, Confirm button?
	internal class DustChooserUI : UIPanel
	{
		private UIGrid dustGrid;

		private UserInterface userInterface;
		public DustChooserUI(UserInterface userInterface) {
			this.userInterface = userInterface;
			//}


			//public override void OnInitialize()
			//{
			Left.Set(-525f, 1f);
			Top.Set(-620f, 1f);
			Width.Set(230f, 0f);
			Height.Set(300f, 0f);
			SetPadding(12);
			BackgroundColor = Color.Aquamarine * 1f;

			int top = 0;

			// close button

			dustGrid = new UIGrid(5);
			dustGrid.Top.Pixels = top;
			dustGrid.Width.Set(-25f, 1f);
			dustGrid.Height.Set(-top, 1f);
			dustGrid.ListPadding = 6f;
			Append(dustGrid);

			// this will initialize grid
			updateneeded = true;

			var dustGridScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			dustGridScrollbar.SetView(100f, 1000f);
			dustGridScrollbar.Top.Pixels = top;// + spacing;
			dustGridScrollbar.Height.Set(-top /*- spacing*/, 1f);
			dustGridScrollbar.HAlign = 1f;
			Append(dustGridScrollbar);
			dustGrid.SetScrollbar(dustGridScrollbar);
		}

		private bool updateneeded;
		internal void UpdateGrid() {
			if (!updateneeded) { return; }
			updateneeded = false;

			dustGrid.Clear();
			for (int i = 0; i < DustTool.dustCount; i++) {
				//if (Main.projName[i].ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) != -1)
				{
					var box = new DustSlot(i);
					dustGrid._items.Add(box);
					dustGrid._innerList.Append(box);
				}
			}

			dustGrid.UpdateOrder();
			dustGrid._innerList.Recalculate();
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateGrid();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);
			if (ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}

	internal class DustSlot : UIElement
	{
		public static Texture2D backgroundTexture = TextureAssets.InventoryBack9.Value;
		private float scale = .6f;
		public int type;
		public DustSlot(int type) {
			this.type = type;
			if (type < Terraria.ID.DustID.Count) {
				// vanilla dust

			}
			//this.Height.Set(20f, 0f);
			//this.Width.Set(20f, 0f);

			this.Width.Set(backgroundTexture.Width * scale, 0f);
			this.Height.Set(backgroundTexture.Height * scale, 0f);
		}

		internal int frameCounter = 0;
		internal int frameTimer = 0;

		private const int frameDelay = 7;
		//internal int frameCounter = 0;
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (++frameTimer > frameDelay) {
				frameCounter = frameCounter + 1;
				frameTimer = 0;
			}

			CalculatedStyle dimensions = base.GetInnerDimensions();
			spriteBatch.Draw(backgroundTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			Rectangle rectangle = dimensions.ToRectangle();

			int frames;
			Texture2D texture;
			int height;
			int width;
			int frame;
			int x;
			int y;

			if (type < Terraria.ID.DustID.Count) {
				frames = 3;
				frame = frameCounter % frames;
				texture = TextureAssets.Dust.Value;
				height = 8;
				width = 8;
				x = (type % 100) * 10;
				y = (type / 100) * 30;
				y += (height + 2) * frame;
			}
			else {
				frames = 1;
				var dust = DustLoader.GetDust(type);
				texture = dust.Texture2D.Value;
				height = texture.Height / frames;
				width = texture.Width;
				frame = frameCounter % frames;
				x = 0;
				y = height * frame; // maybe guess? if rectangle?
			}

			Rectangle sourceRectangle = new Rectangle(x, y, width, height);

			float drawScale = 2f;
			float availableWidth = (float)backgroundTexture.Width * scale;
			if (width * drawScale > availableWidth || height * drawScale > availableWidth) {
				if (width > height) {
					drawScale = availableWidth / width;
				}
				else {
					drawScale = availableWidth / height;
				}
			}
			Vector2 drawPosition = dimensions.Position();
			drawPosition.X += backgroundTexture.Width * scale / 2f - (float)width * drawScale / 2f;
			drawPosition.Y += backgroundTexture.Height * scale / 2f - (float)height * drawScale / 2f;

			Main.spriteBatch.Draw(texture, drawPosition, sourceRectangle, Color.White, 0, Vector2.Zero, drawScale, SpriteEffects.None, 0);
			//if (IsMouseHovering)
			//{
			//	Main.hoverItemName = Main.projName[type];
			//}
		}

		public override void Click(UIMouseEvent evt) {
			Main.NewText("choose dust " + type);
			DustTool.dustUI.typeDataProperty.Data = type;
		}

		public override int CompareTo(object obj) {
			return type.CompareTo((obj as DustSlot).type);
		}
	}
}
