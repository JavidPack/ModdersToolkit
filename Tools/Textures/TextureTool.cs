using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Textures
{
    internal class TextureTool: Tool
	{
		internal static TextureUI textureUI;

		public override void Initialize()
		{
			ToggleTooltip = "Click to toggle Texture Tool";
		}

        public override void ClientInitialize()
		{
			Interface = new UserInterface();

            textureUI = new TextureUI(Interface);
            textureUI.Activate();

            Interface.SetState(textureUI);
		}

        public override void ClientTerminate()
        {
            Interface = default;

			textureUI?.Deactivate();
            textureUI = default;
        }


        public override void UIDraw()
        {
			if (Visible)
			{
				textureUI.Draw(Main.spriteBatch);
			}
		}
    }
}
