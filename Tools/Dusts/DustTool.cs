﻿using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Dusts
{
	internal class DustTool : Tool
	{
		//internal static UserInterface userInterface;
		internal static DustUI dustUI;
		internal static int shaderCount;
		internal static int dustCount;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Dust Tool";
		}

		// should ui even initialize during load? are static members nulled out on reload?
		public override void ClientInitialize() {
			Interface = new UserInterface();

			dustUI = new DustUI(Interface);
			dustUI.Activate();

			Interface.SetState(dustUI);

			//dustUI = new DustUI(userInterface);
			//dustUI.Activate();
			//userInterface.SetState(dustUI);
		}

		public override void PostSetupContent() {
			if (!Main.dedServ) {
				int count = 0;
				ArmorShaderData shader;

				do {
					shader = GameShaders.Armor.GetSecondaryShader(count + 1, Main.LocalPlayer);
					count++;
				}
				while (shader != null);

				shaderCount = count - 1;
				//dustUI.shaderDataProperty.max = shaderCount;

				dustCount = Terraria.GameContent.ChildSafety.SafeDust.Length;
				dustUI.typeDataProperty.max = dustCount; // doesn't work, won't change the slider.
			}
		}

		public override void ClientTerminate() {
			Interface = null;

			dustUI?.Deactivate();
			dustUI = null;
		}


		public override void UIDraw() {
			if (Visible) {
				dustUI.Draw(Main.spriteBatch);
				//if (showProjectileHitboxes) drawProjectileHitboxes();
			}
		}

		//internal override void ScreenResolutionChanged()
		//{
		//	userInterface.Recalculate();
		//}
		//internal override void UIUpdate()
		//{
		//	if (visible)
		//	{
		//		userInterface.Update(Main._drawInterfaceGameTime);
		//	}
		//}
	}
}
