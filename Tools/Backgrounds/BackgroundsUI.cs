/*
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Backgrounds
{
	internal class BackgroundsUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;

		internal UICheckbox quickChangeCheckbox;
		internal UIIntRangedDataValue surfaceBgStyleDataProperty;
		internal UIIntRangedDataValue undergroundBgStyleDataProperty;
		internal UIIntRangedDataValue[] undergroundTextureDataProperties;
		//internal UIIntRangedDataValue undergroundTexture0DataProperty;
		//internal UIIntRangedDataValue undergroundTexture1DataProperty;
		//internal UIIntRangedDataValue undergroundTexture2DataProperty;
		//internal UIIntRangedDataValue undergroundTexture3DataProperty;
		//internal UIIntRangedDataValue undergroundTexture4DataProperty;
		//internal UIIntRangedDataValue undergroundTexture5DataProperty;
		//internal UIIntRangedDataValue undergroundTexture6DataProperty;

		public BackgroundsUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Backgrounds:", 0.85f);
			AppendToAndAdjustWidthHeight(mainPanel, text, ref height, ref width);

			int nextSurfaceBgStyle = (int)typeof(SurfaceBgStyleLoader).GetField("nextSurfaceBgStyle", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			int nextUndergroundBgStyle = (int)typeof(UgBgStyleLoader).GetField("nextUgBgStyle", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			int nextBackground = (int)typeof(BackgroundTextureLoader).GetField("nextBackground", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

			width = 350;

			quickChangeCheckbox = new UICheckbox("Quick Change", "Bypass background fade");
			AppendToAndAdjustWidthHeight(mainPanel, quickChangeCheckbox, ref height, ref width);

			surfaceBgStyleDataProperty = new UIIntRangedDataValue("SurfaceBgStyle:", -1, -1, nextSurfaceBgStyle - 1, true, true);
			UIElement uiRange = new UIRange<int>(surfaceBgStyleDataProperty);
			uiRange.Width.Set(0, 1f);
			AppendToAndAdjustWidthHeight(mainPanel, uiRange, ref height, ref width);

			undergroundBgStyleDataProperty = new UIIntRangedDataValue("UgBgStyle:", -1, -1, nextUndergroundBgStyle - 1, true, true);
			uiRange = new UIRange<int>(undergroundBgStyleDataProperty);
			uiRange.Width.Set(0, 1f);
			AppendToAndAdjustWidthHeight(mainPanel, uiRange, ref height, ref width);

			undergroundTextureDataProperties = new UIIntRangedDataValue[7];
			for (int i = 0; i < 7; i++) {
				// TODO: Show names of Mod added Styles and slots
				undergroundTextureDataProperties[i] = new UIIntRangedDataValue($"Ug Texture {i}:", -1, -1, nextBackground - 1, true, true);
				uiRange = new UIRange<int>(undergroundTextureDataProperties[i]);
				uiRange.Width.Set(0, 1f);
				AppendToAndAdjustWidthHeight(mainPanel, uiRange, ref height, ref width);
			}

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
*/