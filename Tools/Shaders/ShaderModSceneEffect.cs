using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ModdersToolkit.Tools.Shaders
{
	internal class ShaderModSceneEffect : ModSceneEffect
	{
		public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

		public override bool IsSceneEffectActive(Player player) {
			if (!Platform.IsWindows)
				return false;
			bool forcedShaderActive = ShaderTool.shaderUI.forceShaderCheckbox.Selected == true && ShaderTool.shaderUI.lastShaderIsScreenShader;
			//if (!forcedShaderActive) {
			//	player.ManageSpecialBiomeVisuals("ModdersToolkit:TestScreenShader", forcedShaderActive);
			//}
			// TODO: what is the point of SpecialVisuals if it isn't called when active? The docs say to use it to call player.ManageSpecialBiomeVisuals, but that won't turn it off ever.
			player.ManageSpecialBiomeVisuals("ModdersToolkit:TestScreenShader", forcedShaderActive);
			return forcedShaderActive;
			//return base.IsSceneEffectActive(player);
		}

		public override void SpecialVisuals(Player player) {
			//bool forcedShaderActive = ShaderTool.shaderUI.forceShaderCheckbox.Selected && ShaderTool.shaderUI.lastShaderIsScreenShader;
			//player.ManageSpecialBiomeVisuals("ModdersToolkit:TestScreenShader", forcedShaderActive);
		}

		//public override bool IsBiomeActive(Player player) {

		//}
	}
}
