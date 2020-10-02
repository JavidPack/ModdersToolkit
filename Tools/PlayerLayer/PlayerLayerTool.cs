using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.PlayerLayer
{
    internal class PlayerLayerTool : Tool
	{
		internal static PlayerLayerUI playerLayerUI;
		internal override void Initialize()
		{
			ToggleTooltip = "Click to toggle Player Layer Tool";
		}
		internal override void ClientInitialize()
		{
			Interface = new UserInterface();
		}
		internal override void UIDraw()
		{
			if (Visible)
			{
				playerLayerUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				playerLayerUI = new PlayerLayerUI(Interface);
				playerLayerUI.Activate();
				Interface.SetState(playerLayerUI);
			}
		}
	}
}
