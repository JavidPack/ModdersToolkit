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
using System.Collections.Generic;

namespace ModdersToolkit.Tools.Items
{
	class ItemUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public ItemUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		//todo, iteminfo tab??
		UIIntRangedDataValue damageData;
		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			//mainPanel.SetPadding(0);
			mainPanel.Left.Set(-290f, 1f);
			mainPanel.Top.Set(-620f, 1f);
			mainPanel.Width.Set(240f, 0f);
			mainPanel.Height.Set(520f, 0f);
			mainPanel.SetPadding(12);
			mainPanel.BackgroundColor = Color.Azure * 0.7f;

			int top = 0;

			UIText text = new UIText("Items:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);
			top += 20;
			//var uiRange = new UIRange<int>(damageData);

			var uiRanges = new List<UIElement>();

			damageData = new UIIntRangedDataValue("Damage:", 0, 0, 200);
			damageData.DataGetter = () => Main.LocalPlayer.HeldItem.damage;
			damageData.DataSetter = (value) => Main.LocalPlayer.HeldItem.damage = value;
			uiRanges.Add(new UIRange<int>(damageData));

			//var data = new UIIntRangedDataValue("Width:", 0, 60);
			//data.DataGetter = () => Main.LocalPlayer.HeldItem.width;
			//data.DataSetter = (value) => Main.LocalPlayer.HeldItem.width = value;
			//uiRanges.Add(new UIRange<int>(data));

			//data = new UIIntRangedDataValue("Height:", 0, 60);
			//data.DataGetter = () => Main.LocalPlayer.HeldItem.height;
			//data.DataSetter = (value) => Main.LocalPlayer.HeldItem.height = value;
			//uiRanges.Add(new UIRange<int>(data));

			var data = new UIIntRangedDataValue("UseStyle:", 0, 0, 5);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.useStyle;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.useStyle = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("UseTime:", 2, 2, 80);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.useTime;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.useTime = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("UseAnimation:", 2, 2, 80);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.useAnimation;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.useAnimation = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("ReuseDelay:", 2, 2, 80);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.reuseDelay;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.reuseDelay = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Pick:", 0, 0, 300);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.pick;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.pick = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Axe:", 0, 0, 60);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.axe;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.axe = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Hammer:", 0, 0, 150);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.hammer;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.hammer = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Shoot:", 0, 0, Main.projectileTexture.Length - 1);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.shoot;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.shoot = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("UseAmmo:", 0, 0, Main.itemAnimations.Length - 1);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.useAmmo;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.useAmmo = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Crit:", 0, 0, 100);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.crit;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.crit = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Rare:", 0, 0, 11);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.rare;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.rare = value;
			uiRanges.Add(new UIRange<int>(data));

			data = new UIIntRangedDataValue("Value:", 0, 0, 1000000);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.value;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.value = value;
			uiRanges.Add(new UIRange<int>(data));

			var check = new UIBoolDataValue("AutoReuse");
			check.DataGetter = () => Main.LocalPlayer.HeldItem.autoReuse;
			check.DataSetter = (value) => Main.LocalPlayer.HeldItem.autoReuse = value;
			uiRanges.Add(new UICheckbox2(check));

			check = new UIBoolDataValue("UseTurn");
			check.DataGetter = () => Main.LocalPlayer.HeldItem.useTurn;
			check.DataSetter = (value) => Main.LocalPlayer.HeldItem.useTurn = value;
			uiRanges.Add(new UICheckbox2(check));

			foreach (var uiRange in uiRanges)
			{
				uiRange.Top.Set(top, 0f);
				uiRange.Width.Set(0, 1f);
				mainPanel.Append(uiRange);
				top += 30;
			}

			UITextPanel<string> setDefaultsButton = new UITextPanel<string>("SetDefaults");
			setDefaultsButton.SetPadding(4);
			setDefaultsButton.Width.Set(-10, 0.5f);
			//setDefaultsButton.HAlign = 0.5f;
			setDefaultsButton.Top.Set(top, 0f);
			setDefaultsButton.OnClick += SetDefaultsButton_OnClick;
			mainPanel.Append(setDefaultsButton);

			UITextPanel<string> prefixButton = new UITextPanel<string>("Prefix");
			prefixButton.SetPadding(4);
			prefixButton.Width.Set(-10, 0.5f);
			//prefixButton.Left.Set(0, 0.5f);
			prefixButton.HAlign = 1f;
			prefixButton.Top.Set(top, 0f);
			prefixButton.OnClick += PrefixButton_OnClick;
			mainPanel.Append(prefixButton);

			Append(mainPanel);
		}

		private void PrefixButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.LocalPlayer.HeldItem.SetDefaults(Main.LocalPlayer.HeldItem.type);
			Main.LocalPlayer.HeldItem.Prefix(-2);
			Main.LocalPlayer.HeldItem.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(Main.LocalPlayer.HeldItem.width / 2);
			Main.LocalPlayer.HeldItem.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(Main.LocalPlayer.HeldItem.height / 2);
			ItemText.NewText(Main.LocalPlayer.HeldItem, Main.LocalPlayer.HeldItem.stack, true, false);
		}

		private void SetDefaultsButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.LocalPlayer.HeldItem.SetDefaults(Main.LocalPlayer.HeldItem.type);
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
