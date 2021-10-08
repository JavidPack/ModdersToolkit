/*
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Backgrounds
{
	internal class BackgroundsTool : Tool
	{
		internal static BackgroundsUI UI { get; private set; }

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Background Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			UI = new BackgroundsUI(Interface);
			UI.Activate();

			Interface.SetState(UI);
		}

		public override void ClientTerminate() {
			Interface = null;

			UI.Deactivate();
			UI = null;
		}


		public override void UIDraw() {
			if (Visible)
				UI.Draw(Main.spriteBatch);
		}
	}

	internal class BackgroundsToolGlobalBgStyle : GlobalBgStyle
	{
		public override void ChooseSurfaceBgStyle(ref int style) {
			if (Main.gameMenu)
				return;

			int choice = BackgroundsTool.UI.surfaceBgStyleDataProperty.Data;

			if (choice > -1)
				style = choice;

			if (BackgroundsTool.UI.quickChangeCheckbox.Selected)
				Main.quickBG = 10;
		}

		public override void ChooseUgBgStyle(ref int style) {
			if (Main.gameMenu)
				return;

			int choice = BackgroundsTool.UI.undergroundBgStyleDataProperty.Data;

			if (choice > -1)
				style = choice;

			if (BackgroundsTool.UI.quickChangeCheckbox.Selected)
				Main.quickBG = 10;
		}

		public override void FillUgTextureArray(int style, int[] textureSlots) {
			if (Main.gameMenu)
				return;
			for (int i = 0; i < 7; i++) {
				var data = BackgroundsTool.UI.undergroundTextureDataProperties[i].Data;
				if (data > -1)
					textureSlots[i] = data;
			}
		}
	}
}
*/