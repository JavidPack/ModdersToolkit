using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ModdersToolkit.Tools.Shaders
{
    internal class ShaderPlayer : ModPlayer
	{
		public override void UpdateBiomeVisuals() {
			if (Platform.IsWindows) {
				bool forcedShaderActive = ShaderTool.shaderUI.forceShaderCheckbox.Selected && ShaderTool.shaderUI.lastShaderIsScreenShader;
				player.ManageSpecialBiomeVisuals("ModdersToolkit:TestScreenShader", forcedShaderActive);
			}
		}
	}
}
