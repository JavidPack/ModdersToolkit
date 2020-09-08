using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Shaders
{
	class ShaderTool : Tool
	{
		internal static ShaderUI shaderUI;
        internal override void Initialize() {
			toggleTooltip = "Click to toggle Shader Tool";
		}

        internal override void ClientInitialize() {
			userInterface = new UserInterface();
		}


		internal override void UIDraw() {
			if (visible) {
				shaderUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent() {
			if (!Main.dedServ) {
				shaderUI = new ShaderUI(userInterface);
				shaderUI.Activate();
				userInterface.SetState(shaderUI);
			}
		}
	}
}
