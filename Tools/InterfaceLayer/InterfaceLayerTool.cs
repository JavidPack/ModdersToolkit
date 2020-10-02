using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.InterfaceLayer
{
    internal class InterfaceLayerTool: Tool
	{
		internal static InterfaceLayerUI interfaceLayerUI;
		internal override void Initialize()
		{
			ToggleTooltip = "Click to toggle Interface Layer Tool";
		}
		internal override void ClientInitialize()
		{
			Interface = new UserInterface();
		}
		internal override void UIDraw()
		{
			if (Visible)
			{
				interfaceLayerUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				interfaceLayerUI = new InterfaceLayerUI(Interface);
				interfaceLayerUI.Activate();
				Interface.SetState(interfaceLayerUI);
			}
		}
	}
}
