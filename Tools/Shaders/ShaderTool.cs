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

		internal override void Initialize() {
			toggleTooltip = "Click to toggle Shader Tool";
		}

		internal override void ClientInitialize() {
			userInterface = new UserInterface();
			Filters.Scene["ModdersToolkit:TestScreenShader"] = new Filter(new ScreenShaderData("FilterInvert"), EffectPriority.VeryHigh);
		}

		internal override void UIDraw() {
			if (visible) {
				shaderUI.Draw(Main.spriteBatch);
			}
		}

		internal override void PostSetupContent() {
			if (Main.dedServ)
				return;

			shaderUI = new ShaderUI(userInterface);
			shaderUI.Activate();
			userInterface.SetState(shaderUI);
		}

		internal override void Toggled() {
#if DEBUG
			if (!visible) {
				shaderUI.RemoveAllChildren();
				var isInitializedFieldInfo = typeof(Terraria.UI.UIElement).GetField("_isInitialized", BindingFlags.Instance | BindingFlags.NonPublic);
				isInitializedFieldInfo.SetValue(shaderUI, false);
				shaderUI.Activate();
			}
#endif
		}
	}
}
