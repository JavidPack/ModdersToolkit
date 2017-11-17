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
using Terraria.Graphics.Shaders;
using ReLogic.OS;

namespace ModdersToolkit.Tools.Dusts
{
	class DustUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public DustUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		internal DustChooserUI dustChooserUI;
		private bool showDustChooser;
		public bool ShowDustChooser
		{
			get { return showDustChooser; }
			set
			{
				if (value)
				{
					if (!HasChild(dustChooserUI)) Append(dustChooserUI);
				}
				else
				{
					if (HasChild(dustChooserUI)) RemoveChild(dustChooserUI);
				}
				showDustChooser = value;
			}
		}

		UICheckbox noGravityCheckbox;
		UICheckbox noLightCheckbox;
		UICheckbox showSpawnRectangleCheckbox;
		UICheckbox useCustomColorCheckbox;

		UIRadioButton NewDustRadioButton;
		UIRadioButton NewDustPerfectRadioButton;
		UIRadioButton NewDustDirectRadioButton;

		UIFloatRangedDataValue scaleDataProperty;
		UIIntRangedDataValue widthDataProperty;
		UIIntRangedDataValue heightDataProperty;
		internal IntDataRangeProperty typeDataProperty;
		UIIntRangedDataValue alphaDataProperty;
		UIIntRangedDataValue shaderDataProperty;
		UIFloatRangedDataValue speedXDataProperty;
		UIFloatRangedDataValue speedYDataProperty;
		UIFloatRangedDataValue fadeInDataProperty;
		UIFloatRangedDataValue spawnChanceDataProperty;
		ColorDataRangeProperty colorDataProperty;
		// Color slider
		// customdata?
		// random jitter on all floats?

		// TODO, browser and Eyedropper, and Intents-Open other tool to select from it.
		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			//mainPanel.SetPadding(0);
			mainPanel.Left.Set(-290f, 1f);
			mainPanel.Top.Set(-620f, 1f);
			mainPanel.Width.Set(240f, 0f);
			mainPanel.Height.Set(520f, 0f);
			mainPanel.SetPadding(12);
			mainPanel.BackgroundColor = new Color(173, 194, 171);

			int top = 0;

			UIText text = new UIText("Dust:", 0.85f);
			text.Top.Set(top, 0f);
			//text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			UITextPanel<string> resetButton = new UITextPanel<string>("Reset Tool");
			resetButton.SetPadding(4);
			resetButton.Width.Set(-10, 0.5f);
			resetButton.Left.Set(0, 0.5f);
			resetButton.Top.Set(top, 0f);
			resetButton.OnClick += ResetButton_OnClick;
			mainPanel.Append(resetButton);

			top += 20;
			noGravityCheckbox = new UICheckbox("No Gravity", "");
			noGravityCheckbox.Top.Set(top, 0f);
			//gravityCheckbox.Left.Set(12f, 0f);
			mainPanel.Append(noGravityCheckbox);

			top += 20;
			noLightCheckbox = new UICheckbox("No Light", "");
			noLightCheckbox.Top.Set(top, 0f);
			mainPanel.Append(noLightCheckbox);

			top += 20;
			showSpawnRectangleCheckbox = new UICheckbox("Show Spawn Rectangle", "");
			showSpawnRectangleCheckbox.Top.Set(top, 0f);
			mainPanel.Append(showSpawnRectangleCheckbox);

			top += 30;
			scaleDataProperty = new UIFloatRangedDataValue("Scale:", 1f, 0, 5f);
			UIElement uiRange = new UIRange<float>(scaleDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			widthDataProperty = new UIIntRangedDataValue("Width:", 30, 0, 400);
			uiRange = new UIRange<int>(widthDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			heightDataProperty = new UIIntRangedDataValue("Height:", 30, 0, 400);
			uiRange = new UIRange<int>(heightDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			UIImageButton b = new UIImageButton(ModdersToolkit.instance.GetTexture("UIElements/searchIcon"));
			b.OnClick += (s, e) => { ShowDustChooser = !ShowDustChooser; Recalculate(); };
			b.Top.Set(top, 0f);
			mainPanel.Append(b);

			typeDataProperty = new IntDataRangeProperty("Type:", 1, DustTool.dustCount, true); //TODO fix.
			typeDataProperty.range.Top.Set(top, 0f);
			typeDataProperty.range.Left.Set(20, 0f);
			typeDataProperty.range.Width.Set(-20, 1f);
			mainPanel.Append(typeDataProperty.range);

			//top += 30;
			//UIImageButton b = new UIImageButton(ModdersToolkit.instance.GetTexture("UIElements/eyedropper"));

			dustChooserUI = new DustChooserUI(userInterface);

			top += 30;
			useCustomColorCheckbox = new UICheckbox("Use Custom Color", "");
			useCustomColorCheckbox.Top.Set(top, 0f);
			mainPanel.Append(useCustomColorCheckbox);

			top += 20;
			colorDataProperty = new ColorDataRangeProperty("Color:");
			colorDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(colorDataProperty.range);

			top += 30;
			alphaDataProperty = new UIIntRangedDataValue("Alpha:", 0, 0, 255);
			uiRange = new UIRange<int>(alphaDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			shaderDataProperty = new UIIntRangedDataValue("Shader:", 0, 0, DustTool.shaderCount);
			uiRange = new UIRange<int>(shaderDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			speedXDataProperty = new UIFloatRangedDataValue("SpeedX:", 0, -10, 10);
			uiRange = new UIRange<float>(speedXDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			speedYDataProperty = new UIFloatRangedDataValue("SpeedY:", 0, -10, 10);
			uiRange = new UIRange<float>(speedYDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			//todo, position this better? displays as well?
			//		top += 30;
			//		var ui2DRange = new UI2DRange<float>(speedXDataProperty, speedYDataProperty);
			//		ui2DRange.Top.Set(top, 0f);
			//		//ui2DRange.Width.Set(0, 1f);
			//		mainPanel.Append(ui2DRange);

			top += 30;
			fadeInDataProperty = new UIFloatRangedDataValue("FadeIn:", 0, 0, 3);
			uiRange = new UIRange<float>(fadeInDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			spawnChanceDataProperty = new UIFloatRangedDataValue("Spawn%:", 1f, 0, 1, true, true);
			uiRange = new UIRange<float>(spawnChanceDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 24;
			UIRadioButtonGroup g = new UIRadioButtonGroup();
			NewDustRadioButton = new UIRadioButton("NewDust", "Dust.NewDust method");
			NewDustPerfectRadioButton = new UIRadioButton("NewDustPerfect", "Dust.NewDust method");
			NewDustDirectRadioButton = new UIRadioButton("NewDustDirect", "Dust.NewDust method");
			g.Add(NewDustRadioButton);
			g.Add(NewDustPerfectRadioButton);
			g.Add(NewDustDirectRadioButton);
			g.Top.Pixels = top;
			g.Width.Set(0, .75f);
			mainPanel.Append(g);
			NewDustRadioButton.Selected = true;

			UIHoverImageButton copyCodeButton = new UIHoverImageButton(ModdersToolkit.instance.GetTexture("UIElements/CopyCodeButton"), "Copy code to clipboard");
			copyCodeButton.OnClick += CopyCodeButton_OnClick;
			copyCodeButton.Top.Set(-20, 1f);
			copyCodeButton.Left.Set(-20, 1f);
			mainPanel.Append(copyCodeButton);

			Append(mainPanel);
		}

		private void CopyCodeButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			int width = widthDataProperty.Data;
			int height = heightDataProperty.Data;
			int type = typeDataProperty.Data;
			float speedX = speedXDataProperty.Data;
			float speedY = speedYDataProperty.Data;
			int alpha = alphaDataProperty.Data;
			Color color = useCustomColorCheckbox.Selected ? colorDataProperty.Data : Color.White;
			float scale = scaleDataProperty.Data;

			StringBuilder s = new StringBuilder();
			s.Append($"if (Main.rand.NextFloat() < {spawnChanceDataProperty.Data}f)" + Environment.NewLine);
			s.Append($"{{" + Environment.NewLine);
			s.Append($"\tDust dust;" + Environment.NewLine);

			s.Append($"\t// You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle." + Environment.NewLine);
			s.Append($"\tVector2 position = Main.LocalPlayer.Center;" + Environment.NewLine);
			if (NewDustRadioButton.Selected)
			{
				s.Append($"\tdust = Main.dust[Terraria.Dust.NewDust(position, {width}, {height}, {type}, {speedX}f, {speedY}f, {alpha}, new Color({color.R},{color.G},{color.B}), {scale}f)];" + Environment.NewLine);
			}
			else if (NewDustPerfectRadioButton.Selected)
			{
				s.Append($"\tdust = Terraria.Dust.NewDustPerfect(position, {type}, new Vector2({speedX}f, {speedY}f), {alpha}, new Color({color.R},{color.G},{color.B}), {scale}f);" + Environment.NewLine);
			}
			else
			{
				s.Append($"\tdust = Terraria.Dust.NewDustDirect(position, {width}, {height}, {type}, {speedX}f, {speedY}f, {alpha}, new Color({color.R},{color.G},{color.B}), {scale}f);" + Environment.NewLine);
			}

			if (noGravityCheckbox.Selected)
				s.Append($"\tdust.noGravity = true;" + Environment.NewLine);

			if (noLightCheckbox.Selected)
				s.Append($"\tdust.noLight = true;" + Environment.NewLine);

			if(shaderDataProperty.Data > 0)
				s.Append($"\tdust.shader = GameShaders.Armor.GetSecondaryShader({shaderDataProperty.Data}, Main.LocalPlayer);" + Environment.NewLine);

			if (fadeInDataProperty.Data > 0)
				s.Append($"\tdust.fadeIn = {fadeInDataProperty.Data}f;" + Environment.NewLine);

			s.Append($"}}" + Environment.NewLine);

			Platform.Current.Clipboard = s.ToString();

			Main.NewText("Copied Dust spawning code to clipboard");
		}

		private void ResetButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			noGravityCheckbox.Selected = false;
			noLightCheckbox.Selected = false;
			showSpawnRectangleCheckbox.Selected = false;
			useCustomColorCheckbox.Selected = false;

			NewDustRadioButton.Selected = true;
			NewDustPerfectRadioButton.Selected = false;
			NewDustDirectRadioButton.Selected = false;

			scaleDataProperty.ResetToDefaultValue();
			widthDataProperty.ResetToDefaultValue();
			heightDataProperty.ResetToDefaultValue();
			//typeDataProperty.ResetToDefaultValue();
			alphaDataProperty.ResetToDefaultValue();
			shaderDataProperty.ResetToDefaultValue();
			speedXDataProperty.ResetToDefaultValue();
			speedYDataProperty.ResetToDefaultValue();
			fadeInDataProperty.ResetToDefaultValue();
			spawnChanceDataProperty.ResetToDefaultValue();
			colorDataProperty.Data = Color.White;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			SpawnDusts();
			if (mainPanel.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
			DrawSpawnRectangle();
		}

		private void DrawSpawnRectangle()
		{
			if (showSpawnRectangleCheckbox.Selected)
			{
				int width = widthDataProperty.Data;
				int height = heightDataProperty.Data;
				Vector2 position = Main.LocalPlayer.Center + new Vector2(0, -30) - new Vector2(width / 2, height / 2);

				Rectangle hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
				hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightCyan * 0.6f);
			}
		}

		//realtype for moddust?
		private void SpawnDusts()
		{
			if (Main.rand.NextFloat() >= spawnChanceDataProperty.Data) return;

			int width = widthDataProperty.Data;
			int height = heightDataProperty.Data;
			int type = typeDataProperty.Data;
			float speedX = speedXDataProperty.Data;
			float speedY = speedYDataProperty.Data;
			int alpha = alphaDataProperty.Data;
			Color color = useCustomColorCheckbox.Selected ? colorDataProperty.Data : Color.White;
			float scale = scaleDataProperty.Data;
			Vector2 position = Main.LocalPlayer.Center + new Vector2(0, -30) - new Vector2(width / 2, height / 2);
			Dust dust;
			if (NewDustRadioButton.Selected)
			{
				dust = Main.dust[Terraria.Dust.NewDust(position, width, height, type, speedX, speedY, alpha, color, scale)];
			}
			else if (NewDustPerfectRadioButton.Selected)
			{
				position = Main.LocalPlayer.Center + new Vector2(0, -30);
				dust = Terraria.Dust.NewDustPerfect(position, type, new Vector2(speedX, speedY), alpha, color, scale);
			}
			else
			{
				dust = Terraria.Dust.NewDustDirect(position, width, height, type, speedX, speedY, alpha, color, scale);
			}
			if (noGravityCheckbox.Selected) dust.noGravity = true;
			if (noLightCheckbox.Selected) dust.noLight = true;
			dust.shader = GameShaders.Armor.GetSecondaryShader(shaderDataProperty.Data, Main.LocalPlayer);
			dust.fadeIn = fadeInDataProperty.Data;

			//if (gravityCheckbox.Selected) dust.shader = true;
			//Main.dust[dust].noGravity;
			// shader?
			// fadein
			// gravity
		}
	}
}


//scaleInput = new NewUITextBox("input:", 0.85f);
//scaleInput.Top.Set(top, 0f);
//scaleInput.PaddingLeft = 12;
//scaleInput.PaddingRight = 12;
////scaleInput.Left.Set(0, 0f);
//scaleInput.HAlign = 1f;
//mainPanel.Append(scaleInput);

//top += 20;
//NewUITextBox ee = new NewUITextBox("1.0");
//ee.Top.Set(top, 0f);
//ee.PaddingLeft = 12;
//ee.PaddingRight = 12;
//ee.OnUnfocus += Ee_OnUnfocus;
////scaleInput.Left.Set(0, 0f);
//ee.HAlign = 1f;
//mainPanel.Append(ee);

//top += 60;
//UISlider slider = new UISlider(null, () => scale / scaleMax, (s) => { scale = s * scaleMax; }, null, 0, Color.AliceBlue);
//slider.Top.Set(top, 0f);
//slider.Height.Set(16, 0f);
//slider.Width.Set(0, .5f);
//slider.PaddingLeft = 12;
//slider.PaddingRight = 12;
//mainPanel.Append(slider);

//UICheckbox npcCheckbox = new UICheckbox("NPC", "");
//npcCheckbox.Top.Set(52f, 0f);
//npcCheckbox.Left.Set(12f, 0f);
//mainPanel.Append(npcCheckbox);
//
//UICheckbox projectileCheckbox = new UICheckbox("Projectile", "Note: Some projectiles have special collision logic");
//projectileCheckbox.Top.Set(72f, 0f);
//projectileCheckbox.Left.Set(12f, 0f);
//mainPanel.Append(projectileCheckbox);