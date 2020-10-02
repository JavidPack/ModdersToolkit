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
    internal class ItemTool : Tool
	{
		internal static ItemUI itemUI;
		internal static FieldInfo globalItemsField;

        public override void Initialize()
		{
			ToggleTooltip = "Click to toggle Item Tool";
			globalItemsField = typeof(Item).GetField("globalItems", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public override void ClientInitialize()
		{
			Interface = new UserInterface();

            itemUI = new ItemUI(Interface);
            itemUI.Activate();

            Interface.SetState(itemUI);
		}

        public override void ClientTerminate()
        {
            Interface = default;

			itemUI?.Deactivate();
            itemUI = default;
        }


        public override void UIDraw()
		{
			if (Visible)
			{
				itemUI.Draw(Main.spriteBatch);
			}
		}
    }
}
