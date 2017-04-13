using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		internal static FieldInfo itemInfoField;

		internal override void Initialize()
		{
			toggleTooltip = "Click to toggle Item Tool";
			itemInfoField = typeof(Item).GetField("itemInfo", BindingFlags.NonPublic | BindingFlags.Instance);
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
			if (!Main.dedServ)
			{
				itemUI = new ItemUI(userInterface);
				itemUI.Activate();
				userInterface.SetState(itemUI);
			}
		}
	}
}
