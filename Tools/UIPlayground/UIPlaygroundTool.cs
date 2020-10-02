using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;

namespace ModdersToolkit.Tools.UIPlayground
{
    internal class UIPlaygroundTool : Tool
	{
		// TODO: Parallax to teach nesting

		internal static UIPlaygroundUI uiPlaygroundUI;

		internal override void Initialize() {
			ToggleTooltip = "Click to toggle UI Playground Tool";
		}

		internal override void ClientInitialize() {
			Interface = new UserInterface();
			uiPlaygroundUI = new UIPlaygroundUI(Interface);
			uiPlaygroundUI.Activate();
			Interface.SetState(uiPlaygroundUI);

			//On.Terraria.UI.UIElement.DrawSelf += UIElement_DrawSelf;
			On.Terraria.UI.UIElement.Draw += UIElement_Draw;
		}

		private void UIElement_Draw(On.Terraria.UI.UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch) {
			//private void UIElement_DrawSelf(On.Terraria.UI.UIElement.orig_DrawSelf orig, UIElement self, SpriteBatch spriteBatch) {
			bool drawDimensions = true;
			int elementDepth = -1;
			int depth = UIPlaygroundTool.uiPlaygroundUI.drawAllDimensionsDepthData.Data;
			var parent = self;
			bool bypassparallax = false;
			while (parent != null) {
				if (parent == UIPlaygroundTool.uiPlaygroundUI)
					bypassparallax = true;

				elementDepth++;
				parent = parent.Parent;
			}
			//var uiscale = Main.UIScale;
			//var zoom = Main.GameViewMatrix.Zoom;
			float parallaxX = UIPlaygroundTool.uiPlaygroundUI.parallaxXProperty.Data;
			float parallaxY = UIPlaygroundTool.uiPlaygroundUI.parallaxYProperty.Data;
			Rectangle hitbox = self.GetOuterDimensions().ToRectangle();
			if (UIPlaygroundTool.uiPlaygroundUI.drawAllParallaxCheckbox.Selected && !bypassparallax) {
				//Main.GameViewMatrix.Zoom = new Vector2(-UIPlaygroundTool.uiPlaygroundUI.parallaxXProperty.Data, -UIPlaygroundTool.uiPlaygroundUI.parallaxYProperty.Data);
				//Main.UIScale = 0.5f;
				//Main.UIScaleMatrix = Main.UIScaleMatrix * Matrix.CreateTranslation(UIPlaygroundTool.uiPlaygroundUI.parallaxXProperty.Data, UIPlaygroundTool.uiPlaygroundUI.parallaxYProperty.Data, 0f);
				//Main.UIScaleMatrix.Translation *= Matrix.CreateTranslation(UIPlaygroundTool.uiPlaygroundUI.parallaxXProperty.Data, UIPlaygroundTool.uiPlaygroundUI.parallaxYProperty.Data, 0f);
				//var m = Main.UIScaleMatrix * Matrix.CreateScale(10f/(elementDepth + 10)) * Matrix.CreateTranslation(new Vector3(UIPlaygroundTool.uiPlaygroundUI.parallaxXProperty.Data, UIPlaygroundTool.uiPlaygroundUI.parallaxYProperty.Data, 0) * elementDepth * 1);
				//var m = Main.UIScaleMatrix * Matrix.CreateTranslation(new Vector3(parallaxX, parallaxY, 0) * elementDepth * 2) /** Matrix.CreateScale(10f / (elementDepth + 10))*/;
				//var m = Main.UIScaleMatrix * Matrix.CreateTranslation(new Vector3(hitbox.Center.X, hitbox.Center.Y, 0) * -1) * Matrix.CreateScale(30f / (elementDepth + 30)) * Matrix.CreateTranslation(new Vector3(hitbox.Center.X, hitbox.Center.Y, 0) * 1) * Matrix.CreateTranslation(new Vector3(parallaxX, parallaxY, 0) * elementDepth * 2);
				//var m = Main.UIScaleMatrix * Matrix.CreateTranslation(new Vector3(hitbox.Center.X, hitbox.Center.Y, 0) * -1) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(new Vector3(hitbox.Center.X, hitbox.Center.Y, 0) * 1) * Matrix.CreateTranslation(new Vector3(parallaxX, parallaxY, 0) * elementDepth * 2 * scale);
				//m = Main.UIScaleMatrix * Matrix.CreateTranslation(new Vector3(parallaxX, parallaxY, 0) * elementDepth * 2 * scale);
				Main.spriteBatch.End();

				var scale = 30f / (elementDepth + 30);

				int drawDepth = Math.Min(elementDepth, depth);
				if (depth == -1)
					drawDepth = elementDepth;

				var m = Main.UIScaleMatrix * Matrix.CreateTranslation(new Vector3(parallaxX, parallaxY, 0) * drawDepth * 2 * scale);

				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, m);

			}
			if (UIPlaygroundTool.uiPlaygroundUI.drawAllDimensionsCheckbox.Selected) {

				if (depth > -1 && depth != elementDepth || depth == -1 && elementDepth == 0)
					drawDimensions = false;
				if (drawDimensions) {
					spriteBatch.Draw(Main.magicPixel, hitbox, UIPlaygroundTool.uiPlaygroundUI.colorDataProperty.Data * 0.5f);
				}
			}

			//Main.GameViewMatrix = Main.GameViewMatrix.TransformationMatrix * Matrix.CreateTranslation(UIPlaygroundTool.uiPlaygroundUI.parallaxXProperty.Data, UIPlaygroundTool.uiPlaygroundUI.parallaxYProperty.Data, 0f)
			//new Vector2(Main.LocalPlayer.direction * ProjectilesUI.speedXDataProperty.Data, ProjectilesUI.speedYDataProperty.Data),

			orig(self, spriteBatch);
			if (UIPlaygroundTool.uiPlaygroundUI.drawAllDimensionsCheckbox.Selected && !UIPlaygroundTool.uiPlaygroundUI.drawAllParallaxCheckbox.Selected) {
				if (drawDimensions) {
					hitbox.Inflate(-4, -4);
					spriteBatch.Draw(Main.magicPixel, hitbox, UIPlaygroundTool.uiPlaygroundUI.colorDataProperty.Data * 0.4f);
				}
			}
			if (UIPlaygroundTool.uiPlaygroundUI.drawAllParallaxCheckbox.Selected && !bypassparallax) {
				//Main.GameViewMatrix.Zoom = zoom;
				//Main.UIScale = uiscale;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
			}
		}

		internal override void UIDraw() {
			if (Visible) {
				uiPlaygroundUI.Draw(Main.spriteBatch);
			}
		}
	}
}
