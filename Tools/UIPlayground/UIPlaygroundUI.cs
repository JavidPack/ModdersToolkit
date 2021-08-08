using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.Tools.Dusts;
using ModdersToolkit.UIElements;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModdersToolkit.Tools.UIPlayground
{
	internal class UIPlaygroundUI : UIToolState
	{
		internal UIPanel mainPanel;
		internal UIPanel learnPanel;
		internal UIPanel tweakPanel;
		internal UITabControl tabControl;
		private UserInterface userInterface;
		public bool updateNeeded;

		internal UIList tweakList;

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
		internal UICheckbox highlightSelectedCheckbox;
		internal UIFloatRangedDataValue parallaxXProperty;
		internal UIFloatRangedDataValue parallaxYProperty;

		public UIPlaygroundUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			width = 350;
			height = 850;
			//mainPanel.Left.Set(-40f - width, 1f);
			//mainPanel.Top.Set(-110f - height, 1f);
			//mainPanel.Width.Set(width, 0f);
			//mainPanel.Height.Set(height, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			int top = 0;
			UIText text = new UIText("UI:", 0.85f);
			mainPanel.Append(text);
			top += 20;

			tabControl = new UITabControl();
			tabControl.Top.Set(top, 0f);
			tabControl.Height.Set(-top, 1f);
			tabControl.mainPanel.BackgroundColor = Color.Magenta * 0.7f;

			tabControl.AddTab("Learn", learnPanel = makeLearnPanel());
			tabControl.AddTab("Tweak", tweakPanel = makeTweakPanel());
			mainPanel.Append(tabControl);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		private UIPanel makeLearnPanel() {
			// TODO: select from all loaded UIElement inheritors

			int playgroundPanelWidth = 400;
			int playgroundPanelHeight = 150;

			playgroundPanel = new DebugDrawUIPanel();
			playgroundPanel.SetPadding(0);
			playgroundPanel.Left.Set(-800, 1f);
			playgroundPanel.Top.Set(-230, 1f);
			playgroundPanel.Width.Set(playgroundPanelWidth, 0f);
			playgroundPanel.Height.Set(playgroundPanelHeight, 0f);
			playgroundPanel.BackgroundColor = new Color(108, 226, 108);
			Append(playgroundPanel);

			playgroundText = new UIText("Example UIText");
			playgroundTextPanel = new UITextPanel<string>("Example UITextPanel");
			playgroundImageButton = new UIImageButton(Main.inventoryBack10Texture);

			UIPanel learnPanel = new UIPanel();
			learnPanel.SetPadding(6);
			int width = 350;
			int height = 850;
			learnPanel.BackgroundColor = new Color(173, 194, 171);

			// checkboxes
			int top = 0;
			int indent = 0;
			UIElement uiRange;

			UIText text = new UIText("UI Playground:", 0.85f);
			//text.Top.Set(12f, 0f);
			//text.Left.Set(12f, 0f);
			learnPanel.Append(text);
			top += 20;

			panelPaddingData = new UIIntRangedDataValue("Panel Padding:", 0, 0, 12);
			uiRange = new UIRange<int>(panelPaddingData);
			panelPaddingData.OnValueChanged += () => UpdatePlaygroundChildren();
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			uiRange.Left.Set(indent, 0);
			learnPanel.Append(uiRange);
			top += 20;

			drawInnerDimensionsCheckbox = new UICheckbox("Draw Inner Dimensions", "Inner Dimensions takes into account Padding of the current UIElement");
			drawInnerDimensionsCheckbox.TextColor = Color.Blue;
			drawInnerDimensionsCheckbox.Top.Set(top, 0f);
			drawInnerDimensionsCheckbox.Left.Set(indent, 0f);
			learnPanel.Append(drawInnerDimensionsCheckbox);
			top += 20;

			drawDimensionsCheckbox = new UICheckbox("Draw Dimensions", "");
			drawDimensionsCheckbox.TextColor = Color.Pink;
			drawDimensionsCheckbox.Top.Set(top, 0f);
			drawDimensionsCheckbox.Left.Set(indent, 0f);
			learnPanel.Append(drawDimensionsCheckbox);
			top += 20;

			drawOuterDimensionsCheckbox = new UICheckbox("Draw Outer Dimensions", "Outer Dimensions takes into account Margin of the current UIElement");
			drawOuterDimensionsCheckbox.TextColor = Color.Yellow;
			drawOuterDimensionsCheckbox.Top.Set(top, 0f);
			drawOuterDimensionsCheckbox.Left.Set(indent, 0f);
			learnPanel.Append(drawOuterDimensionsCheckbox);
			top += 20;

			top += 8; // additional spacing
			playgroundTextCheckbox = new UICheckbox("UIText", "Show UIText");
			playgroundTextCheckbox.Top.Set(top, 0f);
			playgroundTextCheckbox.Left.Set(indent, 0f);
			playgroundTextCheckbox.OnSelectedChanged += () => UpdatePlaygroundChildren();
			learnPanel.Append(playgroundTextCheckbox);
			top += 20;

			indent = 18;

			textHAlign = new UIFloatRangedDataValue("HAlign:", 0, 0, 1);
			textVAlign = new UIFloatRangedDataValue("VAlign:", 0, 0, 1);
			textLeftPixels = new UIFloatRangedDataValue("LeftPixels:", 0, 0, playgroundPanelWidth);
			textLeftPercent = new UIFloatRangedDataValue("LeftPercent:", 0, 0, 1);
			textTopPixels = new UIFloatRangedDataValue("TopPixels:", 0, 0, playgroundPanelHeight);
			textTopPercent = new UIFloatRangedDataValue("TopPercent:", 0, 0, 1);
			AppendRange(learnPanel, ref top, indent, textHAlign);
			AppendRange(learnPanel, ref top, indent, textVAlign);
			AppendRange(learnPanel, ref top, indent, textLeftPixels);
			AppendRange(learnPanel, ref top, indent, textLeftPercent);
			AppendRange(learnPanel, ref top, indent, textTopPixels);
			AppendRange(learnPanel, ref top, indent, textTopPercent);
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
			learnPanel.Append(playgroundTextPanelCheckbox);
			top += 20;

			textPanelTextInput = new NewUITextBox("UITextPanel Text Here", 0.85f);
			textPanelTextInput.SetText(playgroundTextPanel.Text);
			//textPanelTextInput.SetPadding(0);
			textPanelTextInput.Top.Set(top, 0f);
			textPanelTextInput.Left.Set(indent, 0f);
			textPanelTextInput.Width.Set(-indent, 1f);
			learnPanel.Append(textPanelTextInput);
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
			AppendRange(learnPanel, ref top, indent, textPanelTextScale);
			AppendRange(learnPanel, ref top, indent, textPanelPadding);
			AppendRange(learnPanel, ref top, indent, textPanelHAlign);
			AppendRange(learnPanel, ref top, indent, textPanelVAlign);
			AppendRange(learnPanel, ref top, indent, textPanelLeftPixels);
			AppendRange(learnPanel, ref top, indent, textPanelLeftPercent);
			AppendRange(learnPanel, ref top, indent, textPanelTopPixels);
			AppendRange(learnPanel, ref top, indent, textPanelTopPercent);
			AppendRange(learnPanel, ref top, indent, textPanelWidthPixels);
			AppendRange(learnPanel, ref top, indent, textPanelWidthPercent);
			AppendRange(learnPanel, ref top, indent, textPanelHeightPixels);
			AppendRange(learnPanel, ref top, indent, textPanelHeightPercent);
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
			learnPanel.Append(textPanelMinWidthHeightDisplay);
			top += 20;

			indent = 0;

			top += 8; // additional spacing
			playgroundImageButtonCheckbox = new UICheckbox("UIImageButton", "Show UIImageButton");
			playgroundImageButtonCheckbox.Top.Set(top, 0f);
			playgroundImageButtonCheckbox.Left.Set(indent, 0f);
			playgroundImageButtonCheckbox.OnSelectedChanged += () => UpdatePlaygroundChildren();
			learnPanel.Append(playgroundImageButtonCheckbox);
			top += 20;

			indent = 18;

			imageButtonHAlign = new UIFloatRangedDataValue("HAlign:", 0, 0, 1);
			imageButtonVAlign = new UIFloatRangedDataValue("VAlign:", 0, 0, 1);
			imageButtonLeftPixels = new UIFloatRangedDataValue("LeftPixels:", 0, 0, playgroundPanelWidth);
			imageButtonLeftPercent = new UIFloatRangedDataValue("LeftPercent:", 0, 0, 1);
			imageButtonTopPixels = new UIFloatRangedDataValue("TopPixels:", 0, 0, playgroundPanelHeight);
			imageButtonTopPercent = new UIFloatRangedDataValue("TopPercent:", 0, 0, 1);
			AppendRange(learnPanel, ref top, indent, imageButtonHAlign);
			AppendRange(learnPanel, ref top, indent, imageButtonVAlign);
			AppendRange(learnPanel, ref top, indent, imageButtonLeftPixels);
			AppendRange(learnPanel, ref top, indent, imageButtonLeftPercent);
			AppendRange(learnPanel, ref top, indent, imageButtonTopPixels);
			AppendRange(learnPanel, ref top, indent, imageButtonTopPercent);
			imageButtonHAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonVAlign.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonLeftPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonLeftPercent.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonTopPixels.OnValueChanged += () => UpdatePlaygroundChildren();
			imageButtonTopPercent.OnValueChanged += () => UpdatePlaygroundChildren();

			return learnPanel;
		}

		private UIPanel makeTweakPanel() {
			int top = 0;
			UIPanel tweakPanel = new UIPanel();
			tweakPanel.SetPadding(6);

			UIText text = new UIText("Tweak UI:", 0.85f);
			tweakPanel.Append(text);
			top += 20;

			UIImageButton eyeDropperButton = new UIHoverImageButton(ModdersToolkit.Instance.GetTexture("UIElements/eyedropper"), "UI Selector");
			eyeDropperButton.OnClick += EyeDropperButton_OnClick;
			eyeDropperButton.Top.Set(top - 6, 0f);
			eyeDropperButton.Left.Set(0, 0f);
			tweakPanel.Append(eyeDropperButton);

			UIImageButton resetButton = new UIHoverImageButton(ModContent.GetTexture("Terraria/UI/ButtonDelete"), "Clear UI Selections");
			resetButton.OnClick += (a, b) => {
				UIPlaygroundTool.selectedUIElements.Clear();
				updateNeeded = true;
			};
			resetButton.Top.Set(top - 6, 0f);
			resetButton.Left.Set(30, 0f);
			tweakPanel.Append(resetButton);

			UIHoverImageButton copyCodeButton = new UIHoverImageButton(ModdersToolkit.Instance.GetTexture("UIElements/CopyCodeButton"), "Copy code of selected element to clipboard");
			copyCodeButton.OnClick += CopyCodeButton_OnClick;
			copyCodeButton.Top.Set(top - 6, 0f);
			copyCodeButton.Left.Set(60, 0f);
			tweakPanel.Append(copyCodeButton);

			// TODO: Draw move and resize handles
			highlightSelectedCheckbox = new UICheckbox("Highlight Selected", "Controls the highlight drawing for selected UIElements");
			highlightSelectedCheckbox.Selected = true;
			highlightSelectedCheckbox.Top.Set(top - 6, 0f);
			highlightSelectedCheckbox.Left.Set(90, 0f);
			tweakPanel.Append(highlightSelectedCheckbox);

			top += 20;

			(UIPanel tweakListBackPanel, UIList tweakList, UIElements.FixedUIScrollbar tweakListScrollbar) = makeUIListWithBackPanel(top, 400, Color.Green * 0.7f);
			this.tweakList = tweakList;
			tweakPanel.Append(tweakListBackPanel);
			top += 400;

			text = new UIText("UI Playground Extras:", 0.85f);
			text.Top.Set(top, 0f);
			tweakPanel.Append(text);
			top += 20;

			drawAllDimensionsCheckbox = new UICheckbox("Draw All UIElement Dimensions", "This will draw outer dimensions for Elements in your mod.");
			drawAllDimensionsCheckbox.Top.Set(top, 0f);
			//drawAllDimensionsCheckbox.Left.Set(indent, 0f);
			tweakPanel.Append(drawAllDimensionsCheckbox);
			top += 20;

			colorDataProperty = new ColorDataRangeProperty("Color:");
			colorDataProperty.range.Top.Set(top, 0f);
			tweakPanel.Append(colorDataProperty.range);
			top += 20;

			drawAllDimensionsDepthData = new UIIntRangedDataValue("Draw All Depth:", -1, -1, 10);
			UIRange<int> uiRange = new UIRange<int>(drawAllDimensionsDepthData);
			//drawAllDimensionsDepthData.OnValueChanged += () => UpdatePlaygroundChildren();
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			//uiRange.Left.Set(indent, 0);
			tweakPanel.Append(uiRange);
			top += 30;

			drawAllParallaxCheckbox = new UICheckbox("Draw with Parallax", "This will offset UIElements to help visualize their hierarchy");
			drawAllParallaxCheckbox.Top.Set(top, 0f);
			//drawAllParallaxCheckbox.Left.Set(indent, 0f);
			tweakPanel.Append(drawAllParallaxCheckbox);

			parallaxXProperty = new UIFloatRangedDataValue("ParallaxX:", 0, -10, 10);
			parallaxYProperty = new UIFloatRangedDataValue("ParallaxY:", 0, -10, 10);
			var ui2DRange = new UI2DRange<float>(parallaxXProperty, parallaxYProperty);
			ui2DRange.Top.Set(top, 0f);
			ui2DRange.Left.Set(200, 0f);
			tweakPanel.Append(ui2DRange);
			top += 30;

			text = new UIText("Append UIElement to Selected:", 0.85f);
			text.Top.Set(top, 0f);
			tweakPanel.Append(text);
			top += 20;

			(UIPanel toolboxBackPanel, UIList toolboxList, UIElements.FixedUIScrollbar toolboxScrollbar) = makeUIListWithBackPanel(top, 206, Color.Azure * 0.7f);
			tweakPanel.Append(toolboxBackPanel);
			top += 206;

			var modAssemblies = Terraria.ModLoader.ModLoader.Mods.Select(x => x.Code).Where(x => x != null);
			var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			var nonModAssemblies = allAssemblies.Except(modAssemblies);
			var type = typeof(UIElement);
			var modUIElementTypes = modAssemblies
				.SelectMany(s => s.GetTypes())
				.Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract).OrderBy(x=>x.Name);

			//var allUIElementTypes = modUIElementTypes.Concat(typeof(Terraria.Main).Assembly.GetTypes()
			//	.Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract).OrderBy(x => x.Name));

			var selectUIElementTypes = new List<Type>() { typeof(UIPanel), typeof(UIText), typeof(UITextPanel<string>) };

			var allUIElementTypes = selectUIElementTypes.Concat(modUIElementTypes);

			int index = 0;
			foreach (var modUIElementType in allUIElementTypes) {
				if (modUIElementType.GetConstructor(new Type[0]) == null)
					continue;

				var header = new UIText(modUIElementType.Name);
				header.OnClick += (a, b) => InstantiateUIElement(modUIElementType);
				//toolboxList.Add(header);
				header.Recalculate();

				var sortable = new UISortableElement(index);
				sortable.Width = header.MinWidth;
				sortable.Height = header.MinHeight;
				sortable.Append(header);
				toolboxList.Add(sortable);

				index++;
			}

			return tweakPanel;
		}

		private void CopyCodeButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if(UIPlaygroundTool.lastSelectedUIElement == null) {
				Main.NewText("No UIElement Selected");
				return;
			}
			var e = UIPlaygroundTool.lastSelectedUIElement;
			Type type = UIPlaygroundTool.lastSelectedUIElement.GetType();
			StringBuilder s = new StringBuilder();
			s.Append($"{type.Name} element = new {type.Name}();\n");
			if(e.Width.Percent != 0 || e.Width.Pixels != 0)
				s.Append($"element.Width.Set({e.Width.Pixels}f, {e.Width.Percent}f);\n");
			if (e.Height.Percent != 0 || e.Height.Pixels != 0)
				s.Append($"element.Height.Set({e.Height.Pixels}f, {e.Height.Percent}f);\n");
			if (e.Left.Percent != 0 || e.Left.Pixels != 0)
				s.Append($"element.Left.Set({e.Left.Pixels}f, {e.Left.Percent}f);\n");
			if (e.Top.Percent != 0 || e.Top.Pixels != 0)
				s.Append($"element.Top.Set({e.Top.Pixels}f, {e.Top.Percent}f);\n");
			if (e.HAlign != 0)
				s.Append($"element.HAlign = {e.HAlign}f;\n");
			if (e.VAlign != 0)
				s.Append($"element.VAlign = {e.VAlign}f;\n");
			if (e.PaddingBottom != 0)
				s.Append($"element.SetPadding({e.PaddingBottom}f);\n");
			s.Append($"parent.Append(element);\n");
			
			Platform.Current.Clipboard = s.ToString();
			Main.NewText("Copied UIElement spawning code to clipboard");
		}

		private void InstantiateUIElement(Type modUIElementType) {
			Main.NewText(modUIElementType.FullName);

			ConstructorInfo ctor = modUIElementType.GetConstructor(new Type[0]);
			if (ctor != null) {
				object instance = ctor.Invoke(new object[0]);
				if (UIPlaygroundTool.lastSelectedUIElement != null) {
					UIPlaygroundTool.lastSelectedUIElement.Append((UIElement)instance);
				}
				else {
					playgroundPanel.Append((UIElement)instance);
				}
				UIPlaygroundTool.AddTweakUIElement((UIElement)instance);
				UIPlaygroundTool.SelectUIElement((UIElement)instance);
			}
			else {
				Main.NewText($"{modUIElementType.Name} does not have an empty constructor, cannot instantiate.");
			}
		}

		private (UIPanel backPanel, UIList uiList, UIElements.FixedUIScrollbar scrollbar) makeUIListWithBackPanel(int top, int height, Color backgroundColor) {
			var backPanel = new UIPanel();
			backPanel.Top.Set(top, 0);
			backPanel.Width.Set(0, 1f);
			backPanel.Height.Set(height, 0f);
			backPanel.SetPadding(6);
			backPanel.BackgroundColor = backgroundColor;

			UIList uiList = new UIList();
			uiList.Width.Set(-25f, 1f); // leave space for scroll?
			uiList.Height.Set(0, 1f);
			uiList.ListPadding = 6f;
			backPanel.Append(uiList);

			//updateNeeded = true;

			var scrollbar = new UIElements.FixedUIScrollbar(userInterface);
			scrollbar.SetView(100f, 1000f);
			scrollbar.Top.Pixels = 4;
			scrollbar.Height.Set(-8, 1f);
			scrollbar.HAlign = 1f;
			backPanel.Append(scrollbar);
			uiList.SetScrollbar(scrollbar);

			return (backPanel, uiList, scrollbar);
		}

		private void EyeDropperButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			UIPlaygroundTool.EyedropperActive = !UIPlaygroundTool.EyedropperActive;
			if (UIPlaygroundTool.EyedropperActive) {
				Main.NewText("Click on a UIElement. The Element will appear in the Quick Qweak menu");
			}
		}

		private void AppendRange<T>(UIElement panel, ref int top, int indent, UIRangedDataValue<T> range) {
			UIElement uiRange = new UIRange<T>(range);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(-indent, 1f);
			uiRange.Left.Set(indent, 0);
			panel.Append(uiRange);
			top += 20;
		}

		private void AppendToAndIncrement(UIElement panel, UIElement element, ref int top) {
			element.Width = new StyleDimension(0, 1);
			element.Top = new StyleDimension(top, 0);
			panel.Append(element);
			top += 20;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (!updateNeeded) { return; }
			updateNeeded = false;

			tweakList.Clear();
			foreach (var uiElement in UIPlaygroundTool.selectedUIElements) {
				var panel = new UIPanel();
				if (uiElement == UIPlaygroundTool.lastSelectedUIElement)
					panel.BackgroundColor = Terraria.ModLoader.UI.UICommon.DefaultUIBlue;
				int top = 0;

				var header = new UIText($"{(string.IsNullOrEmpty(uiElement.Id) ? "" : uiElement.Id + ": ")}{uiElement.ToString()}:");
				header.OnClick += (a, b) => {
					UIPlaygroundTool.SelectUIElement(uiElement);
					//UIPlaygroundTool.lastSelectedUIElement = uiElement;
					panel.BackgroundColor = Terraria.ModLoader.UI.UICommon.DefaultUIBlue;
				};
				panel.Append(header);
				top += 20;

				var ElementsFieldInfo = typeof(UIElement).GetField("Elements", BindingFlags.Instance | BindingFlags.NonPublic);
				if (uiElement.Parent != null && !(uiElement.Parent is UIState)) {
					var parent = new UIText("->Parent", 0.85f);
					parent.Top.Set(top, 0f);
					parent.OnClick += (a, b) => {
						UIPlaygroundTool.AddTweakUIElement(uiElement.Parent);
						UIPlaygroundTool.lastSelectedUIElement = uiElement.Parent;
						// GOTO happens before parent is added to list, too early.
						tweakList.Goto(delegate (UIElement element)
						{
							if(element is UIPanel panel2) {
								var elements = (List<UIElement>)ElementsFieldInfo.GetValue(panel2);
								if(elements.Count > 0) {
									UIText first = elements[0] as UIText;
									if(first != null) {
										if(first.Text == $"{(string.IsNullOrEmpty(UIPlaygroundTool.lastSelectedUIElement.Id) ? "" : UIPlaygroundTool.lastSelectedUIElement.Id + ": ")}{UIPlaygroundTool.lastSelectedUIElement.ToString()}:") {
											Main.NewText("Found");
											return true;
										}
									}
								}
							}
							//UIRecipeSlot itemSlot = element as UIRecipeSlot;
							//return element == slot;
							return false;
						});
					};
					panel.Append(parent);
					top += 20;
				}

				// TODO: Buttons for each child?
				var elements2 = (List<UIElement>)ElementsFieldInfo.GetValue(uiElement);
				if (elements2.Count > 0) {
					(UIPanel elementsListBackPanel, UIList elementsList, UIElements.FixedUIScrollbar elementsListScrollbar) = makeUIListWithBackPanel(top, 200, Color.Green * 0.7f);
					panel.Append(elementsListBackPanel);
					top += 200;

					foreach (var element in elements2) {
						var child = new UIText(element.ToString(), 0.85f);
						//parent.Top.Set(top, 0f);
						child.OnClick += (a, b) => {
							UIPlaygroundTool.AddTweakUIElement(element);
							UIPlaygroundTool.lastSelectedUIElement = element;
						};
						elementsList.Add(child);
					}
				}

				// Copy button to copy current values.

				var hAlign = new UIFloatRangedDataValue("HAlign:", 0, 0, 1);
				hAlign.DataGetter = () => uiElement.HAlign;
				hAlign.DataSetter = (value) => {
					uiElement.HAlign = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(hAlign) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;

				var vAlign = new UIFloatRangedDataValue("VAlign:", 0, 0, 1);
				vAlign.DataGetter = () => uiElement.VAlign;
				vAlign.DataSetter = (value) => {
					uiElement.VAlign = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(vAlign) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;

				var innerDimensions = uiElement.Parent?.GetInnerDimensions() ?? new CalculatedStyle(0, 0, Main.screenWidth, Main.screenHeight); ;

				var LeftPixels = new UIFloatRangedDataValue("LeftPixels:", 0, 0, (int)innerDimensions.Width);
				LeftPixels.DataGetter = () => uiElement.Left.Pixels;
				LeftPixels.DataSetter = (value) => {
					uiElement.Left.Pixels = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(LeftPixels) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;
				var LeftPercent = new UIFloatRangedDataValue("LeftPercent:");
				LeftPercent.DataGetter = () => uiElement.Left.Percent;
				LeftPercent.DataSetter = (value) => {
					uiElement.Left.Percent = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(LeftPercent) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;

				var TopPixels = new UIFloatRangedDataValue("TopPixels:", 0, 0, (int)innerDimensions.Height);
				TopPixels.DataGetter = () => uiElement.Top.Pixels;
				TopPixels.DataSetter = (value) => {
					uiElement.Top.Pixels = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(TopPixels) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;
				var TopPercent = new UIFloatRangedDataValue("TopPercent:");
				TopPercent.DataGetter = () => uiElement.Top.Percent;
				TopPercent.DataSetter = (value) => {
					uiElement.Top.Percent = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(TopPercent) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;

				var Padding = new UIFloatRangedDataValue("Padding:", 0, 0, 12);
				Padding.DataGetter = () => uiElement.PaddingTop;
				Padding.DataSetter = (value) => {
					uiElement.SetPadding(value);
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(Padding) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;

				var WidthPixels = new UIFloatRangedDataValue("WidthPixels:", 0, 0, (int)innerDimensions.Height);
				WidthPixels.DataGetter = () => uiElement.Width.Pixels;
				WidthPixels.DataSetter = (value) => {
					uiElement.Width.Pixels = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(WidthPixels) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;
				var WidthPercent = new UIFloatRangedDataValue("WidthPercent:");
				WidthPercent.DataGetter = () => uiElement.Width.Percent;
				WidthPercent.DataSetter = (value) => {
					uiElement.Width.Percent = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(WidthPercent) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;

				var HeightPixels = new UIFloatRangedDataValue("HeightPixels:", 0, 0, (int)innerDimensions.Height);
				HeightPixels.DataGetter = () => uiElement.Height.Pixels;
				HeightPixels.DataSetter = (value) => {
					uiElement.Height.Pixels = value;
					uiElement.Recalculate();
				};
				panel.Append(new UIRange<float>(HeightPixels) {
					Width = new StyleDimension(0, 1),
					Top = new StyleDimension(top, 0)
				});
				top += 20;
				var HeightPercent = new UIFloatRangedDataValue("HeightPercent:");
				HeightPercent.DataGetter = () => uiElement.Height.Percent;
				HeightPercent.DataSetter = (value) => {
					uiElement.Height.Percent = value;
					uiElement.Recalculate();
				};
				AppendToAndIncrement(panel, new UIRange<float>(HeightPercent), ref top);
				//panel.Append(new UIRange<float>(HeightPercent) {
				//	Width = new StyleDimension(0, 1),
				//	Top = new StyleDimension(top, 0)
				//});
				//top += 20;

				List<string> skips = new List<string> { "PaddingLeft", "PaddingRight", "PaddingTop", "PaddingBottom" };

				foreach (PropertyFieldWrapper memberInfo in GetFieldsAndProperties(uiElement)) {
					if (skips.Contains(memberInfo.Name))
						continue;

					Type type = memberInfo.Type;
					if (type == typeof(bool))
					{
						var check = new UIBoolDataValue(memberInfo.Name);
						check.DataGetter = () => (bool)memberInfo.GetValue(uiElement);
						check.DataSetter = (value) => memberInfo.SetValue(uiElement, value);
						AppendToAndIncrement(panel, new UICheckbox2(check), ref top);
					}
					else if (type == typeof(int)) {
						var check = new UIIntRangedDataValue(memberInfo.Name);
						check.DataGetter = () => (int)memberInfo.GetValue(uiElement);
						check.DataSetter = (value) => memberInfo.SetValue(uiElement, value);
						AppendToAndIncrement(panel, new UIRange<int>(check), ref top);
					}
					else if (type == typeof(float)) {
						var check = new UIFloatRangedDataValue(memberInfo.Name, .4f, -1f, 1f);
						check.DataGetter = () => (float)memberInfo.GetValue(uiElement);
						check.DataSetter = (value) => memberInfo.SetValue(uiElement, value);
						AppendToAndIncrement(panel, new UIRange<float>(check), ref top);
					}
					else if (type == typeof(Color)) {
						var colorDataProperty = new ColorDataRangeProperty(memberInfo.Name);
						colorDataProperty.Data = (Color)memberInfo.GetValue(uiElement);
						colorDataProperty.OnValueChanged += () => memberInfo.SetValue(uiElement, colorDataProperty.Data);
						AppendToAndIncrement(panel, colorDataProperty.range, ref top);
					}
					// Automatic doesn't have innerdimension limits on pixels
					//else if(type == typeof(StyleDimension)) {
					//	var dataElement = new UIFloatRangedDataValue(memberInfo.Name + "Pixels:");
					//	dataElement.DataGetter = () => ((StyleDimension)memberInfo.GetValue(uiElement)).Pixels;
					//	dataElement.DataSetter = (value) => {
					//		var StyleDimension = (StyleDimension)memberInfo.GetValue(uiElement);
					//		StyleDimension.Pixels = value;
					//		memberInfo.SetValue(uiElement, StyleDimension);
					//		uiElement.Recalculate();
					//	};
					//	panel.Append(new UIRange<float>(dataElement) {
					//		Width = new StyleDimension(0, 1),
					//		Top = new StyleDimension(top, 0)
					//	});
					//	top += 20;
					//	dataElement = new UIFloatRangedDataValue(memberInfo.Name + "Percent:");
					//	dataElement.DataGetter = () => ((StyleDimension)memberInfo.GetValue(uiElement)).Percent;
					//	dataElement.DataSetter = (value) => {
					//		var StyleDimension = (StyleDimension)memberInfo.GetValue(uiElement);
					//		StyleDimension.Percent = value;
					//		memberInfo.SetValue(uiElement, StyleDimension);
					//		uiElement.Recalculate();
					//	};
					//	panel.Append(new UIRange<float>(dataElement) {
					//		Width = new StyleDimension(0, 1),
					//		Top = new StyleDimension(top, 0)
					//	});
					//	top += 20;
					//}
				}

				// other fields here
				//var fields = uiElement.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				//foreach (var fieldInfo in fields) {
				//	if (fieldInfo.FieldType == typeof(bool)) {
				//		var check = new UIBoolDataValue(fieldInfo.Name);
				//		check.DataGetter = () => (bool)fieldInfo.GetValue(uiElement);
				//		check.DataSetter = (value) => fieldInfo.SetValue(uiElement, value);
				//		//panel.Append(new UICheckbox2(check));
				//		AppendToAndIncrement(panel, new UICheckbox2(check), ref top);
				//	}
				//	else if (fieldInfo.FieldType == typeof(int)) {
				//		var check = new UIIntRangedDataValue(fieldInfo.Name);
				//		check.DataGetter = () => (int)fieldInfo.GetValue(uiElement);
				//		check.DataSetter = (value) => fieldInfo.SetValue(uiElement, value);
				//		//panel.Append(new UIRange<int>(check) {
				//		//	Width = new StyleDimension(0, 1)
				//		//});
				//		AppendToAndIncrement(panel, new UIRange<int>(check), ref top);
				//	}
				//	else if (fieldInfo.FieldType == typeof(float)) {
				//		var check = new UIFloatRangedDataValue(fieldInfo.Name, .4f, -1f, 1f);
				//		check.DataGetter = () => (float)fieldInfo.GetValue(uiElement);
				//		check.DataSetter = (value) => fieldInfo.SetValue(uiElement, value);
				//		//panel.Append(new UIRange<float>(check) {
				//		//	Width = new StyleDimension(0, 1)
				//		//});
				//		AppendToAndIncrement(panel, new UIRange<float>(check), ref top);
				//	}
				//	else if (fieldInfo.FieldType == typeof(Color)) {
				//		var colorDataProperty = new ColorDataRangeProperty(fieldInfo.Name);
				//		colorDataProperty.Data = (Color)fieldInfo.GetValue(uiElement);
				//		colorDataProperty.OnValueChanged += () => fieldInfo.SetValue(uiElement, colorDataProperty.Data);
				//		AppendToAndIncrement(panel, colorDataProperty.range, ref top);
				//	}
				//}
				//var properties = uiElement.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				//foreach (var propertyInfo in properties) {
				//	if (propertyInfo.PropertyType == typeof(Color)) {
				//		var colorDataProperty = new ColorDataRangeProperty(propertyInfo.Name);
				//		colorDataProperty.Data = (Color)propertyInfo.GetValue(uiElement);
				//		colorDataProperty.OnValueChanged += () => propertyInfo.SetValue(uiElement, colorDataProperty.Data);
				//		AppendToAndIncrement(panel, colorDataProperty.range, ref top);
				//	}
				//}

				panel.Height.Set(top + panel.PaddingBottom + PaddingTop, 0f);
				panel.Width.Set(0, 1f);
				tweakList.Add(panel);
			}
		}

		public static IEnumerable<PropertyFieldWrapper> GetFieldsAndProperties(object item) {
			PropertyInfo[] properties = item.GetType().GetProperties(
				//BindingFlags.DeclaredOnly |
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance);

			FieldInfo[] fields = item.GetType().GetFields(
				//BindingFlags.DeclaredOnly |
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance);

			return fields.Select(x => new PropertyFieldWrapper(x)).Concat(properties.Select(x => new PropertyFieldWrapper(x)));
		}

		private void UpdatePlaygroundChildren() {
			//Terraria.ModLoader.UI.UICommon.AddOrRemoveChild(playgroundPanel, playgroundPanel, tabControl.GetPanel == learnPanel);

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

	internal class DebugDrawUIPanel : UIPanel
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
