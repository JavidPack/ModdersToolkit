using System.Reflection;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.UI;

namespace ModdersToolkit.Tools.Shaders
{
	internal class ShaderTool : Tool
	{
		internal static ShaderUI shaderUI;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Shader Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();
			Filters.Scene["ModdersToolkit:TestScreenShader"] = new Filter(new ScreenShaderData("FilterInvert"), EffectPriority.VeryHigh);

			shaderUI = new ShaderUI(Interface);
			shaderUI.Activate();

			Interface.SetState(shaderUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			shaderUI.Deactivate();
			shaderUI = null;
		}


		public override void UIDraw() {
			if (Visible) {
				shaderUI.Draw(Main.spriteBatch);
			}
		}
	}
}
