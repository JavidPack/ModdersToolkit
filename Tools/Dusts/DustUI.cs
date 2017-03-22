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

		UICheckbox noGravityCheckbox;
		UICheckbox noLightCheckbox;
		UICheckbox showSpawnRectangleCheckbox;
		UICheckbox useCustomColorCheckbox;
		UIText scaleLabel;
		UIText scaleSlider;
		NewUITextBox scaleInput;

		UIRadioButton NewDustRadioButton;
		UIRadioButton NewDustPerfectRadioButton;
		UIRadioButton NewDustDirectRadioButton;

		UIRange<int> widthRange;

		float scaleMax = 5f;
		private float dustScale = 1f;
		internal float DustScale
		{
			get { return dustScale; }
			set
			{
				dustScale = value;
				scaleInput.SetText(dustScale.ToString("0.00"));
			}
		}

		IntDataRangeProperty heightDataProperty;
		IntDataRangeProperty typeDataProperty;
		IntDataRangeProperty alphaDataProperty;
		IntDataRangeProperty shaderDataProperty;
		UIFloatRangedDataValue speedXDataProperty;
		UIFloatRangedDataValue speedYDataProperty;
		FullFloatDataRangeProperty fadeInDataProperty;
		UIFloatRangedDataValue spawnChanceDataProperty;
		ColorDataRangeProperty colorDataProperty;
		// Color slider
		// customdata?
		// random jitter on all floats?

		float widthMax = 200;
		private int dustWidth = 50;
		internal int DustWidth
		{
			get { return dustWidth; }
			set
			{
				dustWidth = value;
				widthRange.input.SetText(dustWidth.ToString());
			}
		}

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
			scaleLabel = new UIText("Scale:", 0.85f);
			//scaleLabel.PaddingLeft = 12;
			//scaleLabel.PaddingRight = 12;
			scaleLabel.Top.Set(top, 0f);
			//scaleLabel.Width.Set(0, .33f);
			//scaleLabel.Left.Set(12f, 0f);
			mainPanel.Append(scaleLabel);

			UISlider slider = new UISlider(null, () => DustScale / scaleMax, (s) => { DustScale = s * scaleMax; }, null, 0, Color.AliceBlue);
			slider.Top.Set(top, 0f);
			slider.Height.Set(16, 0f);
			slider.Width.Set(0, .33f);
			//slider.PaddingLeft = 12;
			//slider.PaddingRight = 12;
			slider.HAlign = .5f;
			mainPanel.Append(slider);

			//scaleSlider = new UIText("slider:", 0.85f);
			//scaleSlider.Top.Set(top, 0f);
			////scaleSlider.Left.Set(0, 0f);
			//scaleSlider.PaddingLeft = 12;
			//scaleSlider.PaddingRight = 12;
			//scaleSlider.HAlign = .5f;
			//mainPanel.Append(scaleSlider);

			scaleInput = new NewUITextBox("input:", 0.85f);
			scaleInput.Top.Set(top, 0f);
			scaleInput.SetPadding(0);
			scaleInput.OnUnfocus += ValidateInput;
			//scaleInput.PaddingLeft = 12;
			//scaleInput.PaddingRight = 12;
			scaleInput.Width.Set(0, .33f);
			scaleInput.HAlign = 1f;
			mainPanel.Append(scaleInput);

			top += 30;
			widthRange = new UIRange<int>("Width", () => DustWidth / widthMax, (s) => { DustWidth = (int)(s * widthMax); }, ValidateWidth);
			widthRange.Top.Set(top, 0f);
			widthRange.Width.Set(0, 1f);
			mainPanel.Append(widthRange);

			top += 30;
			heightDataProperty = new IntDataRangeProperty("Height:", 30, 400);
			heightDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(heightDataProperty.range);

			top += 30;
			typeDataProperty = new IntDataRangeProperty("Type:", 1, DustTool.dustCount, true); //TODO fix.
			typeDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(typeDataProperty.range);


			top += 30;
			useCustomColorCheckbox = new UICheckbox("Use Custom Color", "");
			useCustomColorCheckbox.Top.Set(top, 0f);
			mainPanel.Append(useCustomColorCheckbox);

			top += 20;
			colorDataProperty = new ColorDataRangeProperty("Color:");
			colorDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(colorDataProperty.range);

			top += 30;
			alphaDataProperty = new IntDataRangeProperty("Alpha:", 0, 255);
			alphaDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(alphaDataProperty.range);

			top += 30;
			shaderDataProperty = new IntDataRangeProperty("Shader:", 0, DustTool.shaderCount);
			shaderDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(shaderDataProperty.range);

			//
			//top += 30;
			//speedXDataProperty = new FullFloatDataRangeProperty("SpeedX:", 0, -10, 10);
			//speedXDataProperty.range.Top.Set(top, 0f);
			//mainPanel.Append(speedXDataProperty.range);
			//
			//top += 30;
			//speedYDataProperty = new FullFloatDataRangeProperty("SpeedY:", 0, -10, 10);
			//speedYDataProperty.range.Top.Set(top, 0f);
			//mainPanel.Append(speedYDataProperty.range);
			///

			top += 30;
			speedXDataProperty = new UIFloatRangedDataValue("SpeedX:", -10, 10);
			var uiRange = new UIRange<float>(speedXDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);

			top += 30;
			speedYDataProperty = new UIFloatRangedDataValue("SpeedY:", -10, 10);
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
			fadeInDataProperty = new FullFloatDataRangeProperty("FadeIn:", 0, 0, 3);
			fadeInDataProperty.range.Top.Set(top, 0f);
			mainPanel.Append(fadeInDataProperty.range);

			top += 30;
			spawnChanceDataProperty = new UIFloatRangedDataValue("Spawn%:", 0, 1, true, true);
			spawnChanceDataProperty.SetValue(1f);
			uiRange = new UIRange<float>(spawnChanceDataProperty);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			mainPanel.Append(uiRange);


			//top += 30;
			//var testFloatDataValue = new UIFloatRangedDataValue("Test:", 30f, 30f, false, true);
			//uiRange = new UIRange<float>(testFloatDataValue);
			//uiRange.Top.Set(top, 0f);
			//uiRange.Width.Set(0, 1f);
			//mainPanel.Append(uiRange);

			top += 20;
			UIRadioButtonGroup g = new UIRadioButtonGroup();
			NewDustRadioButton = new UIRadioButton("NewDust", "Dust.NewDust method");
			NewDustPerfectRadioButton = new UIRadioButton("NewDustPerfect", "Dust.NewDust method");
			NewDustDirectRadioButton = new UIRadioButton("NewDustDirect", "Dust.NewDust method");
			g.Add(NewDustRadioButton);
			g.Add(NewDustPerfectRadioButton);
			g.Add(NewDustDirectRadioButton);
			g.Top.Pixels = top;
			mainPanel.Append(g);
			NewDustRadioButton.Selected = true;

			// trigger ui update
			DustScale = dustScale;
			DustWidth = dustWidth;

			Append(mainPanel);
		}

		private void ValidateInput()
		{
			float result;
			if (float.TryParse(scaleInput.Text, out result))
			{
				DustScale = result;
			}
		}

		private void ValidateWidth()
		{
			int result;
			if (int.TryParse(widthRange.input.Text, out result))
			{
				DustWidth = result;
			}
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
				int width = dustWidth;
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

			int width = dustWidth;
			int height = heightDataProperty.Data;
			int type = typeDataProperty.Data;
			float speedX = speedXDataProperty.Data;
			float speedY = speedYDataProperty.Data;
			int alpha = alphaDataProperty.Data;
			Color color = useCustomColorCheckbox.Selected ? colorDataProperty.Data : Color.White;
			float scale = dustScale;
			Vector2 position = Main.LocalPlayer.Center + new Vector2(0, -30) - new Vector2(width / 2, height / 2);
			Dust dust;
			if (NewDustRadioButton.Selected)
			{
				dust = Main.dust[Terraria.Dust.NewDust(position, width, height, type, speedX, speedY, alpha, color, scale)];
			}
			else if (NewDustPerfectRadioButton.Selected)
			{
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