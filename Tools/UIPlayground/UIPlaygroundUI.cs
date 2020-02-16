using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using ModdersToolkit.UIElements;
using ModdersToolkit.Tools.Dusts;

namespace ModdersToolkit.Tools.UIPlayground
{
	class UIPlaygroundUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;

		internal DebugDrawUIPanel playgroundPanel;
		internal UIText playgroundText;
		internal UITextPanel<string> playgroundTextPanel;
		internal UIImageButton playgroundImageButton;

		internal UICheckbox playgroundTextCheckbox;
		internal UICheckbox playgroundTextPanelCheckbox;
		internal UICheckbox playgroundImageButtonCheckbox;

		internal UICheckbox drawInnerDimensionsCheckbox;
		internal UICheckbox drawDimensionsCheckbox;
		internal UICheckbox drawOuterDimensionsCheckbox;

		internal UIIntRangedDataValue panelPaddingData;

		internal UIFloatRangedDataValue textHAlign;
		internal UIFloatRangedDataValue textVAlign;
		internal UIFloatRangedDataValue textLeftPixels;
		internal UIFloatRangedDataValue textLeftPercent;
		internal UIFloatRangedDataValue textTopPixels;
		internal UIFloatRangedDataValue textTopPercent;

		internal NewUITextBox textPanelTextInput;
		internal UIFloatRangedDataValue textPanelTextScale;
		internal UIFloatRangedDataValue textPanelPadding;
		internal UIFloatRangedDataValue textPanelHAlign;
		internal UIFloatRangedDataValue textPanelVAlign;
		internal UIFloatRangedDataValue textPanelLeftPixels;
		internal UIFloatRangedDataValue textPanelLeftPercent;
		internal UIFloatRangedDataValue textPanelTopPixels;
		internal UIFloatRangedDataValue textPanelTopPercent;
		internal UIFloatRangedDataValue textPanelWidthPixels;
		internal UIFloatRangedDataValue textPanelWidthPercent;
		internal UIFloatRangedDataValue textPanelHeightPixels;
		internal UIFloatRangedDataValue textPanelHeightPercent;
		internal UIText textPanelMinWidthHeightDisplay;
		//internal UIFloatRangedDataValue textPanelMinWidthPixels;
		//internal UIFloatRangedDataValue textPanelMinWidthPercent;
		//internal UIFloatRangedDataValue textPanelMinHeightPixels;
		//internal UIFloatRangedDataValue textPanelMinHeightPercent;

		internal UIFloatRangedDataValue imageButtonHAlign;
		internal UIFloatRangedDataValue imageButtonVAlign;
		internal UIFloatRangedDataValue imageButtonLeftPixels;
		internal UIFloatRangedDataValue imageButtonLeftPercent;
		internal UIFloatRangedDataValue imageButtonTopPixels;
		internal UIFloatRangedDataValue imageButtonTopPercent;

		internal UICheckbox drawAllDimensionsCheckbox;
		internal ColorDataRangeProperty colorDataProperty;
		internal UIIntRangedDataValue drawAllDimensionsDepthData;
		internal UICheckbox drawAllParallaxCheckbox;
		internal UIFloatRangedDataValue parallaxXProperty;
		internal UIFloatRangedDataValue parallaxYProperty;

		public UIPlaygroundUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			int width = 350;
			int height = 840;
			mainPanel.Left.Set(-40f - width, 1f);
			mainPanel.Top.Set(-110f - height, 1f);
			mainPanel.Width.Set(width, 0f);
			mainPanel.Height.Set(height, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			int playgroundPanelWidth = 400;
			int playgroundPanelHeight = 150;

			playgroundPanel = new DebugDrawUIPanel();
			playgroundPanel.SetPadding(0);
			playgroundPanel.Left.Set(0, .15f);
			playgroundPanel.Top.Set(0, .5f);
			playgroundPanel.Width.Set(playgroundPanelWidth, 0f);
			playgroundPanel.Height.Set(playgroundPanelHeight, 0f);
			playgroundPanel.BackgroundColor = new Color(108, 226, 108);
			Append(playgroundPanel);

			playgroundText = new UIText("Example UIText");
			playgroundTextPanel = new UITextPanel<string>("Example UITextPanel");
			playgroundImageButton = new UIImageButton(Main.inventoryBack10Texture);

			// checkboxes
			int top = 0;
			int indent = 0;
			UIElement uiRange;

			UIText text = new UIText("UIPlayground UI:", 0.85f);
			//text.Top.Set(12f, 0f);
			//text.Left.Set(12f, 0f);
			mainPanel.Append(text);
			top += 20;

			panelPaddingData = new UIIntRangedDataValue("Panel Padding:", 0, 0, 12);
			uiRange = new UIRange<int>(panelPaddingData);
			panelPaddingData.OnValueChanged += () => UpdatePlaygroundChildren();
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			uiRange.Left.Set(indent, 0);
			mainPanel.Append(uiRange);
			top += 20;

			drawInnerDimensionsCheckbox = new UICheckbox("Draw Inner Dimensions", "Inner Dimensions takes into account Padding of the current UIElement");
			drawInnerDimensionsCheckbox.TextColor = Color.Blue;
			drawInnerDimensionsCheckbox.Top.Set(top, 0f);
			drawInnerDimensionsCheckbox.Left.Set(indent, 0f);
			mainPanel.Append(drawInnerDimensionsCheckbox);
			top += 20;

			drawDimensionsCheckbox = new UICheckbox("Draw Dimensions", "");
			drawDimensionsCheckbox.TextColor = Color.Pink;
			drawDimensionsCheckbox.Top.Set(top, 0f);
			drawDimensionsCheckbox.Left.Set(indent, 0f);
			mainPanel.Append(drawDimensionsCheckbox);
			top += 20;

			drawOuterDimensionsCheckbox = new UICheckbox("Draw Outer Dimensions", "Outer Dimensions takes into account Margin of the current UIElement");
			drawOuterDimensionsCheckbox.TextColor = Color.Yellow;
			drawOuterDimensionsCheckbox.Top.Set(top, 0f);
			drawOuterDimensionsCheckbox.Left.Set(indent, 0f);
			mainPanel.Append(drawOuterDimensionsCheckbox);
			top += 20;

			top += 8; // additional spacing
			playgroundTextCheckbox = new UICheckbox("UIText", "Show UIText");
			playgroundTextCheckbox.Top.Set(top, 0f);
			playgroundTextCheckbox.Left.Set(indent, 0f);
			playgroundTextCheckbox.OnSelectedChanged += () => UpdatePlaygroundChildren();
			mainPanel.Append(playgroundTextCheckbox);
			top += 20;

			indent = 18;

			textHAlign = new UIFloatRangedDataValue("HAlign:", 0, 0, 1);
			textVAlign = new UIFloatRangedDataValue("VAlign:", 0, 0, 1);
			textLeftPixels = new UIFloatRangedDataValue("LeftPixels:", 0, 0, playgroundPanelWidth);
			textLeftPercent = new UIFloatRangedDataValue("LeftPercent:", 0, 0, 1);
			textTopPixels = new UIFloatRangedDataValue("TopPixels:", 0, 0, playgroundPanelHeight);
			textTopPercent = new UIFloatRangedDataValue("TopPercent:", 0, 0, 1);
			AppendRange(ref top, indent, textHAlign);
			AppendRange(ref top, indent, textVAlign);
			AppendRange(ref top, indent, textLeftPixels);
			AppendRange(ref top, indent, textLeftPercent);
			AppendRange(ref top, indent, textTopPixels);
			AppendRange(ref top, indent, textTopPercent);
			textHAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			textVAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			textLeftPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			textLeftPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			textTopPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			textTopPercent.OnValueChanged += () => UpdatePlaygroundChildren();

			indent = 0;

			top += 8; // additional spacing
			playgroundTextPanelCheckbox = new UICheckbox("UITextPanel", "Show UITextPanel");
			playgroundTextPanelCheckbox.Top.Set(top, 0f);
			playgroundTextPanelCheckbox.Left.Set(indent, 0f);
			playgroundTextPanelCheckbox.OnSelectedChanged += () => UpdatePlaygroundChildren();
			mainPanel.Append(playgroundTextPanelCheckbox);
			top += 20;

			textPanelTextInput = new NewUITextBox("UITextPanel Text Here", 0.85f);
			textPanelTextInput.SetText(playgroundTextPanel.Text);
			//textPanelTextInput.SetPadding(0);
			textPanelTextInput.Top.Set(top, 0f);
			textPanelTextInput.Left.Set(indent, 0f);
			textPanelTextInput.Width.Set(-indent, 1f);
			mainPanel.Append(textPanelTextInput);
			textPanelTextInput.OnTextChanged += () => // order matters
			{
				//if (mainPanel.Parent != null) // 
				UpdatePlaygroundChildren(); // playgroundTextPanel.SetText(textPanelTextInput.Text, textPanelTextScale.Data, false);
			};
			top += 20;

			textPanelTextScale = new UIFloatRangedDataValue("TextScale:", playgroundTextPanel.TextScale, .2f, 3f);
			textPanelPadding = new UIFloatRangedDataValue("Padding:", playgroundTextPanel.PaddingBottom, 0, 12);
			textPanelHAlign = new UIFloatRangedDataValue("HAlign:", 0, 0, 1);
			textPanelVAlign = new UIFloatRangedDataValue("VAlign:", 0, 0, 1);
			textPanelLeftPixels = new UIFloatRangedDataValue("LeftPixels:", 0, 0, playgroundPanelWidth);
			textPanelLeftPercent = new UIFloatRangedDataValue("LeftPercent:", 0, 0, 1);
			textPanelTopPixels = new UIFloatRangedDataValue("TopPixels:", 0, 0, playgroundPanelHeight);
			textPanelTopPercent = new UIFloatRangedDataValue("TopPercent:", 0, 0, 1);
			textPanelWidthPixels = new UIFloatRangedDataValue("WidthPixels:", playgroundTextPanel.Width.Pixels, 0, playgroundPanelWidth);
			textPanelWidthPercent = new UIFloatRangedDataValue("WidthPercent:", playgroundTextPanel.Width.Precent, 0, 1);
			textPanelHeightPixels = new UIFloatRangedDataValue("HeightPixels:", playgroundTextPanel.Height.Pixels, 0, playgroundPanelHeight);
			textPanelHeightPercent = new UIFloatRangedDataValue("HeightPercent:", playgroundTextPanel.Height.Precent, 0, 1);
			//textPanelMinWidthPixels = new UIFloatRangedDataValue("MinWidthPixels:", playgroundTextPanel.MinWidth.Pixels, 0, playgroundPanelWidth);
			//textPanelMinWidthPercent = new UIFloatRangedDataValue("MinWidthPercent:", playgroundTextPanel.MinWidth.Precent, 0, 1);
			//textPanelMinHeightPixels = new UIFloatRangedDataValue("MinHeightPixels:", playgroundTextPanel.MinHeight.Pixels, 0, playgroundPanelHeight);
			//textPanelMinHeightPercent = new UIFloatRangedDataValue("MinHeightPercent:", playgroundTextPanel.MinHeight.Precent, 0, 1);
			AppendRange(ref top, indent, textPanelTextScale);
			AppendRange(ref top, indent, textPanelPadding);
			AppendRange(ref top, indent, textPanelHAlign);
			AppendRange(ref top, indent, textPanelVAlign);
			AppendRange(ref top, indent, textPanelLeftPixels);
			AppendRange(ref top, indent, textPanelLeftPercent);
			AppendRange(ref top, indent, textPanelTopPixels);
			AppendRange(ref top, indent, textPanelTopPercent);
			AppendRange(ref top, indent, textPanelWidthPixels);
			AppendRange(ref top, indent, textPanelWidthPercent);
			AppendRange(ref top, indent, textPanelHeightPixels);
			AppendRange(ref top, indent, textPanelHeightPercent);
			//AppendRange(ref top, indent, textPanelMinWidthPixels);
			//AppendRange(ref top, indent, textPanelMinWidthPercent);
			//AppendRange(ref top, indent, textPanelMinHeightPixels);
			//AppendRange(ref top, indent, textPanelMinHeightPercent);
			textPanelTextScale.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelPadding.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelHAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelVAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelLeftPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelLeftPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelTopPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelTopPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelWidthPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelWidthPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelHeightPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			textPanelHeightPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			//textPanelMinWidthPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			//textPanelMinWidthPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			//textPanelMinHeightPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			//textPanelMinHeightPercent.OnValueChanged += () => UpdatePlaygroundChildren();

			textPanelMinWidthHeightDisplay = new UIText("", 0.85f);
			textPanelMinWidthHeightDisplay.Top.Set(top, 0f);
			textPanelMinWidthHeightDisplay.Width.Set(-indent, 1f);
			textPanelMinWidthHeightDisplay.Left.Set(indent, 0);
			mainPanel.Append(textPanelMinWidthHeightDisplay);
			top += 20;

			indent = 0;

			top += 8; // additional spacing
			playgroundImageButtonCheckbox = new UICheckbox("UIImageButton", "Show UIImageButton");
			playgroundImageButtonCheckbox.Top.Set(top, 0f);
			playgroundImageButtonCheckbox.Left.Set(indent, 0f);
			playgroundImageButtonCheckbox.OnSelectedChanged += () => UpdatePlaygroundChildren();
			mainPanel.Append(playgroundImageButtonCheckbox);
			top += 20;

			indent = 18;

			imageButtonHAlign = new UIFloatRangedDataValue("HAlign:", 0, 0, 1);
			imageButtonVAlign = new UIFloatRangedDataValue("VAlign:", 0, 0, 1);
			imageButtonLeftPixels = new UIFloatRangedDataValue("LeftPixels:", 0, 0, playgroundPanelWidth);
			imageButtonLeftPercent = new UIFloatRangedDataValue("LeftPercent:", 0, 0, 1);
			imageButtonTopPixels = new UIFloatRangedDataValue("TopPixels:", 0, 0, playgroundPanelHeight);
			imageButtonTopPercent = new UIFloatRangedDataValue("TopPercent:", 0, 0, 1);
			AppendRange(ref top, indent, imageButtonHAlign);
			AppendRange(ref top, indent, imageButtonVAlign);
			AppendRange(ref top, indent, imageButtonLeftPixels);
			AppendRange(ref top, indent, imageButtonLeftPercent);
			AppendRange(ref top, indent, imageButtonTopPixels);
			AppendRange(ref top, indent, imageButtonTopPercent);
			imageButtonHAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonVAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonLeftPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonLeftPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonTopPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonTopPercent.OnValueChanged += () => UpdatePlaygroundChildren();

			text = new UIText("UIPlayground UI Extra:", 0.85f);
			mainPanel.Append(text);
			top += 20;

			drawAllDimensionsCheckbox = new UICheckbox("Draw All UIElement Dimensions", "This will draw outer dimensions for Elements in your mod.");
			drawAllDimensionsCheckbox.Top.Set(top, 0f);
			//drawAllDimensionsCheckbox.Left.Set(indent, 0f);
			mainPanel.Append(drawAllDimensionsCheckbox);
			top += 20;

			colorDataProperty = new ColorDataRangeProperty("Color:");
			colorDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(colorDataProperty.range);
			top += 20;

			drawAllDimensionsDepthData = new UIIntRangedDataValue("Draw All Depth:", -1, -1, 10);
			uiRange = new UIRange<int>(drawAllDimensionsDepthData);
			//drawAllDimensionsDepthData.OnValueChanged += () => UpdatePlaygroundChildren();
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			//uiRange.Left.Set(indent, 0);
			mainPanel.Append(uiRange);
			top += 30;

			drawAllParallaxCheckbox = new UICheckbox("Draw with Parallax", "This will offset UIElements to help visualize their hierarchy");
			drawAllParallaxCheckbox.Top.Set(top, 0f);
			//drawAllParallaxCheckbox.Left.Set(indent, 0f);
			mainPanel.Append(drawAllParallaxCheckbox);

			parallaxXProperty = new UIFloatRangedDataValue("ParallaxX:", 0, -10, 10);
			parallaxYProperty = new UIFloatRangedDataValue("ParallaxY:", 0, -10, 10);
			var ui2DRange = new UI2DRange<float>(parallaxXProperty, parallaxYProperty);
			ui2DRange.Top.Set(top, 0f);
			ui2DRange.Left.Set(200, 0f);
			mainPanel.Append(ui2DRange);
			top += 30;

			Append(mainPanel);
		}

		private void AppendRange<T>(ref int top, int indent, UIRangedDataValue<T> range) {
			UIElement uiRange = new UIRange<T>(range);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(-indent, 1f);
			uiRange.Left.Set(indent, 0);
			mainPanel.Append(uiRange);
			top += 20;
		}

		private void UpdatePlaygroundChildren() {
			playgroundPanel.RemoveAllChildren();
			if (playgroundTextCheckbox.Selected)
				playgroundPanel.Append(playgroundText);
			if (playgroundTextPanelCheckbox.Selected)
				playgroundPanel.Append(playgroundTextPanel);
			if (playgroundImageButtonCheckbox.Selected)
				playgroundPanel.Append(playgroundImageButton);

			playgroundPanel.SetPadding(panelPaddingData.Data);

			playgroundText.HAlign = textHAlign.Data;
			playgroundText.VAlign = textVAlign.Data;
			playgroundText.Left.Set(textLeftPixels.Data, textLeftPercent.Data);
			playgroundText.Top.Set(textTopPixels.Data, textTopPercent.Data);

			// if changed only?
			playgroundTextPanel.SetText(textPanelTextInput.Text, textPanelTextScale.Data, false);
			playgroundTextPanel.SetPadding(textPanelPadding.Data);
			playgroundTextPanel.HAlign = textPanelHAlign.Data;
			playgroundTextPanel.VAlign = textPanelVAlign.Data;
			playgroundTextPanel.Left.Set(textPanelLeftPixels.Data, textPanelLeftPercent.Data);
			playgroundTextPanel.Top.Set(textPanelTopPixels.Data, textPanelTopPercent.Data);
			playgroundTextPanel.Width.Set(textPanelWidthPixels.Data, textPanelWidthPercent.Data);
			playgroundTextPanel.Height.Set(textPanelHeightPixels.Data, textPanelHeightPercent.Data);
			textPanelMinWidthHeightDisplay.SetText($"MinWidth: {playgroundTextPanel.MinWidth.Pixels}, MinHeight: {playgroundTextPanel.MinHeight.Pixels}");
			//playgroundTextPanel.MinWidth.Set(textPanelMinWidthPixels.Data, textPanelMinWidthPercent.Data);
			//playgroundTextPanel.MinHeight.Set(textPanelMinHeightPixels.Data, textPanelMinHeightPercent.Data);
			// Margin

			playgroundImageButton.HAlign = imageButtonHAlign.Data;
			playgroundImageButton.VAlign = imageButtonVAlign.Data;
			playgroundImageButton.Left.Set(imageButtonLeftPixels.Data, imageButtonLeftPercent.Data);
			playgroundImageButton.Top.Set(imageButtonTopPixels.Data, imageButtonTopPercent.Data);
			//playgroundImageButton.Width.Set(imageButtonWidthPixels.Data, imageButtonWidthPercent.Data);
			//playgroundImageButton.Width.Set(imageButtonWidthPixels.Data, imageButtonWidthPercent.Data);

			playgroundPanel.Recalculate();

			// recalculate?
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}

	class DebugDrawUIPanel : UIPanel
	{
		protected override void DrawChildren(SpriteBatch spriteBatch) {
			base.DrawChildren(spriteBatch);
			if (UIPlaygroundTool.uiPlaygroundUI.drawInnerDimensionsCheckbox.Selected)
				foreach (UIElement current in Elements) {
					Rectangle hitbox = current.GetInnerDimensions().ToRectangle();
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Blue * 0.6f);
				}
			if (UIPlaygroundTool.uiPlaygroundUI.drawDimensionsCheckbox.Selected)
				foreach (UIElement current in Elements) {
					Rectangle hitbox = current.GetDimensions().ToRectangle();
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Pink * 0.6f);
				}
			if (UIPlaygroundTool.uiPlaygroundUI.drawOuterDimensionsCheckbox.Selected)
				foreach (UIElement current in Elements) {
					Rectangle hitbox = current.GetOuterDimensions().ToRectangle();
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Yellow * 0.6f);
				}
		}
	}
}
