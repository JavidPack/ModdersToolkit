using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Projectiles
{
	class ProjectilesTool : Tool
	{
		internal static ProjectilesUI projectileUI;
		internal override void Initialize()
		{
			ToggleTooltip = "Click to toggle Projectile Tool";
		}
		internal override void ClientInitialize()
		{
			Interface = new UserInterface();
		}
		internal override void UIDraw()
		{
			if (Visible)
			{
				projectileUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				projectileUI = new ProjectilesUI(Interface);
				projectileUI.Activate();
				Interface.SetState(projectileUI);
			}
		}
	}
}
