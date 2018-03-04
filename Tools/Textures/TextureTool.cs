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
	class TextureTool: Tool
	{
		internal static TextureUI textureUI;
		internal override void Initialize()
		{
			toggleTooltip = "Click to toggle Texture Tool";
		}
		internal override void ClientInitialize()
		{
			userInterface = new UserInterface();
		}
		internal override void UIDraw()
		{
			if (visible)
			{
				textureUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent()
		{
			if (!Main.dedServ)
			{
				textureUI = new TextureUI(userInterface);
				textureUI.Activate();
				userInterface.SetState(textureUI);
			}
		}
	}
}
