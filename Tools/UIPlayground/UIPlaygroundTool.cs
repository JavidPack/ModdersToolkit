using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;

namespace ModdersToolkit.Tools.UIPlayground
{
	class UIPlaygroundTool : Tool
	{
		internal static UIPlaygroundUI uiPlaygroundUI;

		internal override void Initialize()
		{
			toggleTooltip = "Click to toggle UI Playground Tool";
		}

		internal override void ClientInitialize()
		{
			userInterface = new UserInterface();
			uiPlaygroundUI = new UIPlaygroundUI(userInterface);
			uiPlaygroundUI.Activate();
			userInterface.SetState(uiPlaygroundUI);
		}

		internal override void UIDraw()
		{
			if (visible)
			{
				uiPlaygroundUI.Draw(Main.spriteBatch);
			}
		}
	}
}
