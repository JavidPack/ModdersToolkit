using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Items
{
	internal class ItemUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public ItemUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		private UIIntRangedDataValue damageData;
		internal static UIIntRangedDataValue holdoutXData;
		internal static UIIntRangedDataValue holdoutYData;
		internal static UICheckbox playerMeleeCheckbox;

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mouseAndScrollBlockers.Add(mainPanel);
			width = 240;
			height = 520;
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = Color.Azure * 0.7f;

			int top = 0;

			UIText text = new UIText("Items:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);
			top += 20;

			UITabControl tabControl = new UITabControl();
			tabControl.Top.Set(top, 0f);
			tabControl.Height.Set(-top, 1f);
			tabControl.mainPanel.BackgroundColor = Color.Magenta * 0.7f;

			tabControl.AddTab("Fields", makeMainPanel());
			tabControl.AddTab("Other", makeOtherPanel());
			mainPanel.Append(tabControl);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		private UIPanel makeOtherPanel() {
			UIPanel panel = new UIPanel();
			//panel.Width = StyleDimension.Fill;
			//panel.Height = StyleDimension.Fill;
			//	panel.Top.Pixels = top;
			panel.BackgroundColor = Color.LightCoral * 0.7f;

			int top = 0;

			playerMeleeCheckbox = new UICheckbox("Override HoldoutOffset", "Affects non-staff useStyle 5 weapons. (Guns)");
			playerMeleeCheckbox.Top.Set(top, 0f);
			//playerMeleeCheckbox.Left.Set(0, 0f);
			panel.Append(playerMeleeCheckbox);

			top += 20;

			holdoutXData = new UIIntRangedDataValue("Holdout X:", 0, -30, 30);
			UIElement uiRange = new UIRange<int>(holdoutXData);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			panel.Append(uiRange);

			top += 20;

			holdoutYData = new UIIntRangedDataValue("Holdout Y:", 0, -30, 30);
			uiRange = new UIRange<int>(holdoutYData);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			panel.Append(uiRange);

			top += 20;

			var data = new UIFloatRangedDataValue("Scale:", 1f, 0f, 5f);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.scale;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.scale = value;
			uiRange = new UIRange<float>(data);
			uiRange.Top.Set(top, 0f);
			uiRange.Width.Set(0, 1f);
			panel.Append(uiRange);

			top += 20;

			UIText printItemInfoText = new UIText("Print ItemInfo", 0.85f);
			printItemInfoText.Top.Set(top, 0f);
			printItemInfoText.OnClick += PrintItemInfo_OnClick;
			panel.Append(printItemInfoText);

			return panel;
		}

		private UIPanel makeMainPanel() {
			UIPanel mainPanel = new UIPanel();
			mainPanel.BackgroundColor = Color.Green * 0.7f;

			int top = 0;
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

			data = new UIIntRangedDataValue("Shoot:", 0, 0, TextureAssets.Projectile.Length - 1);
			data.DataGetter = () => Main.LocalPlayer.HeldItem.shoot;
			data.DataSetter = (value) => Main.LocalPlayer.HeldItem.shoot = value;
			uiRanges.Add(new UIRange<int>(data));

			var floatdata = new UIFloatRangedDataValue("ShootSpeed:", 0, 0, 32f);
			floatdata.DataGetter = () => Main.LocalPlayer.HeldItem.shootSpeed;
			floatdata.DataSetter = (value) => Main.LocalPlayer.HeldItem.shootSpeed = value;
			uiRanges.Add(new UIRange<float>(floatdata));

			floatdata = new UIFloatRangedDataValue("KnockBack:", 0, 0, 12f);
			floatdata.DataGetter = () => Main.LocalPlayer.HeldItem.knockBack;
			floatdata.DataSetter = (value) => Main.LocalPlayer.HeldItem.knockBack = value;
			uiRanges.Add(new UIRange<float>(floatdata));

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


			foreach (var uiRange in uiRanges) {
				uiRange.Top.Set(top, 0f);
				uiRange.Width.Set(0, 1f);
				mainPanel.Append(uiRange);
				top += 22;
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

			return mainPanel;
		}

		private void PrefixButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.LocalPlayer.HeldItem.SetDefaults(Main.LocalPlayer.HeldItem.type);
			Main.LocalPlayer.HeldItem.Prefix(-2);
			Main.LocalPlayer.HeldItem.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(Main.LocalPlayer.HeldItem.width / 2);
			Main.LocalPlayer.HeldItem.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(Main.LocalPlayer.HeldItem.height / 2);
			PopupText.NewText(PopupTextContext.ItemReforge, Main.LocalPlayer.HeldItem, Main.LocalPlayer.HeldItem.stack, true, false);
		}

		private void SetDefaultsButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.LocalPlayer.HeldItem.SetDefaults(Main.LocalPlayer.HeldItem.type);
		}

		private void PrintItemInfo_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			// TODO: Double check that this enumeration is correct with AppliesToEntity
			Instanced<GlobalItem>[] globalItems = ((Instanced<GlobalItem>[])(ItemTool.globalItemsField.GetValue(Main.LocalPlayer.HeldItem)));

			for (int i = 0; i < globalItems.Length; i++) {
				GlobalItem param = globalItems[i].Instance;
				if (param.Name == "MysteryGlobalItem")
					continue;
				Main.NewText("Object type: " + param.GetType());
				foreach (PropertyInfo property in param.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
					Main.NewText("PROPERTY " + property.Name + " = " + property.GetValue(param, null) + "\n");
				}

				foreach (FieldInfo field in param.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
					Main.NewText("FIELD " + field.Name + " = " + (field.GetValue(param).ToString() != "" ? field.GetValue(param) : "(Field value not found)") + "\n");
				}
			}
		}
	}

	internal class ItemUIGlobalItem : GlobalItem
	{
		public override Vector2? HoldoutOffset(int type) {
			if (ItemUI.playerMeleeCheckbox.Selected) {
				return new Vector2(ItemUI.holdoutXData.Data, ItemUI.holdoutYData.Data);
			}
			return base.HoldoutOffset(type);
		}
	}
}
