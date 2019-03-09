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
	class BackgroundsUI : UIState
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

		public BackgroundsUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			int width = 350;
			int height = 340;
			mainPanel.Left.Set(-40f - width, 1f);
			mainPanel.Top.Set(-110f - height, 1f);
			mainPanel.Width.Set(width, 0f);
			mainPanel.Height.Set(height, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);
			Append(mainPanel);

			UIText text = new UIText("Backgrounds:", 0.85f);
			text.Top.Set(12f, 0f);
			text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			int nextSurfaceBgStyle = (int)typeof(SurfaceBgStyleLoader).GetField("nextSurfaceBgStyle", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			int nextUndergroundBgStyle = (int)typeof(UgBgStyleLoader).GetField("nextUgBgStyle", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			int nextBackground = (int)typeof(BackgroundTextureLoader).GetField("nextBackground", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

			int top = 12;

			top += 20;
			quickChangeCheckbox = new UICheckbox("Quick Change", "Bypass background fade");
			quickChangeCheckbox.Top.Set(top, 0f);
			mainPanel.Append(quickChangeCheckbox);

			top += 30;
			surfaceBgStyleDataProperty = new UIIntRangedDataValue("SurfaceBgStyle:", -1, -1, nextSurfaceBgStyle - 1, true, true);
			UIElement uiRange = new UIRange<int>(surfaceBgStyleDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			undergroundBgStyleDataProperty = new UIIntRangedDataValue("UgBgStyle:", -1, -1, nextUndergroundBgStyle - 1, true, true);
			uiRange = new UIRange<int>(undergroundBgStyleDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			undergroundTextureDataProperties = new UIIntRangedDataValue[7];
			for (int i = 0; i < 7; i++)
			{
				top += 30;
				// TODO: Show names of Mod added Styles and slots
				undergroundTextureDataProperties[i] = new UIIntRangedDataValue($"Ug Texture {i}:", -1, -1, nextBackground - 1, true, true);
				uiRange = new UIRange<int>(undergroundTextureDataProperties[i]);
				uiRange.Top.Set(top, 0f);
				uiRange.Width.Set(0, 1f);
				mainPanel.Append(uiRange);
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
