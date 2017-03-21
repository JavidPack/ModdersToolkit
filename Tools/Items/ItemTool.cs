using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;
namespace ModdersToolkit.Tools.Items
{
	class ItemTool : Tool
	{
		internal static ItemUI itemUI;
		internal override void Initialize()
		{
			toggleTooltip = "Click to toggle Item Tool";
		}
		internal override void ClientInitialize()
		{
			userInterface = new UserInterface();
		}
		internal override void UIDraw()
		{
			if (visible)
			{
				itemUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent()
		{
			itemUI = new ItemUI(userInterface);
				itemUI.Activate();
				userInterface.SetState(itemUI);
		}
	}
}
