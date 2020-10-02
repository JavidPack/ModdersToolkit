using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Dusts
{
    internal class DustTool : Tool
	{
		//internal static UserInterface userInterface;
		internal static DustUI dustUI;
		internal override void Initialize()
		{
			ToggleTooltip = "Click to toggle Dust Tool";
		}

		// should ui even initialize during load? are static members nulled out on reload?
		internal override void ClientInitialize()
		{
			Interface = new UserInterface();
			//dustUI = new DustUI(userInterface);
			//dustUI.Activate();
			//userInterface.SetState(dustUI);
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
		internal override void UIDraw()
		{
			if (Visible)
			{
				dustUI.Draw(Main.spriteBatch);
				//if (showProjectileHitboxes) drawProjectileHitboxes();
			}
		}

		internal static int shaderCount;
		internal static int dustCount;
		internal override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				int count = 0;
				ArmorShaderData shader;
				do
				{
					shader = GameShaders.Armor.GetSecondaryShader(count + 1, Main.LocalPlayer);
					count++;
				} while (shader != null);
				shaderCount = count - 1;

				count = Terraria.ID.DustID.Count;
				ModDust dust;
				while (true)
				{
					dust = ModDust.GetDust(count);
					if (dust == null) break;
					count++;
				}
				dustCount = count;

				dustUI = new DustUI(Interface);
				dustUI.Activate();
				Interface.SetState(dustUI);
			}
		}
	}
}
