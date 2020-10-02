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
    internal class ProjectilesTool : Tool
	{
		internal static ProjectilesUI projectileUI;

		public override void Initialize()
		{
			ToggleTooltip = "Click to toggle Projectile Tool";
		}

		public override void ClientInitialize()
		{
			Interface = new UserInterface();

            projectileUI = new ProjectilesUI(Interface);
            projectileUI.Activate();

            Interface.SetState(projectileUI);
		}

        public override void ClientTerminate()
        {
            Interface = default;

			projectileUI?.Deactivate();
            projectileUI = default;
        }


        public override void UIDraw()
		{
			if (Visible)
			{
				projectileUI.Draw(Main.spriteBatch);
			}
		}
    }
}
