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
		internal static FieldInfo globalItemsField;

		internal override void Initialize()
		{
			ToggleTooltip = "Click to toggle Item Tool";
			globalItemsField = typeof(Item).GetField("globalItems", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		internal override void ClientInitialize()
		{
			Interface = new UserInterface();
		}

		internal override void UIDraw()
		{
			if (Visible)
			{
				itemUI.Draw(Main.spriteBatch);
			}
		}

		internal override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				itemUI = new ItemUI(Interface);
				itemUI.Activate();
				Interface.SetState(itemUI);
			}
		}
	}
}
