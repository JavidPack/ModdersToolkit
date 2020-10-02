using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Backgrounds
{
	class BackgroundsTool : Tool
	{
		internal static BackgroundsUI backgroundsUI;
		internal override void Initialize()
		{
			ToggleTooltip = "Click to toggle Background Tool";
		}

		internal override void ClientInitialize()
		{
			Interface = new UserInterface();
			backgroundsUI = new BackgroundsUI(Interface);
			backgroundsUI.Activate();
			Interface.SetState(backgroundsUI);
		}

		internal override void UIDraw()
		{
			if (Visible)
			{
				backgroundsUI.Draw(Main.spriteBatch);
			}
		}
	}

	class BackgroundsToolGlobalBgStyle : GlobalBgStyle
	{
		public override void ChooseSurfaceBgStyle(ref int style)
		{
			if (Main.gameMenu)
				return;
			int choice = BackgroundsTool.backgroundsUI.surfaceBgStyleDataProperty.Data;
			if (choice > -1)
				style = choice;

			if(BackgroundsTool.backgroundsUI.quickChangeCheckbox.Selected)
				Main.quickBG = 10;
		}

		public override void ChooseUgBgStyle(ref int style)
		{
			if (Main.gameMenu)
				return;
			int choice = BackgroundsTool.backgroundsUI.undergroundBgStyleDataProperty.Data;
			if (choice > -1)
				style = choice;

			if (BackgroundsTool.backgroundsUI.quickChangeCheckbox.Selected)
				Main.quickBG = 10;
		}

		public override void FillUgTextureArray(int style, int[] textureSlots)
		{
			if (Main.gameMenu)
				return;
			for (int i = 0; i < 7; i++)
			{
				var data = BackgroundsTool.backgroundsUI.undergroundTextureDataProperties[i].Data;
				if (data > -1)
					textureSlots[i] = data;
			}
		}

		public override void ModifyFarSurfaceFades(int style, float[] fades, float transitionSpeed)
		{
			base.ModifyFarSurfaceFades(style, fades, transitionSpeed);
		}
	}
}
