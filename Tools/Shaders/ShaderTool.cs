using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.Shaders
{
    internal class ShaderTool : Tool
    {
        internal static ShaderUI shaderUI;

        internal override void Initialize()
        {
            toggleTooltip = "Click to toggle Shader Tool";
        }

        internal override void ClientInitialize()
        {
            userInterface = new UserInterface();
        }

        internal override void UIDraw()
        {
            if (visible)
            {
                shaderUI.Draw(Main.spriteBatch);
            }
        }

        internal override void PostSetupContent()
        {
            if (Main.dedServ) 
                return;

            shaderUI = new ShaderUI(userInterface);
            shaderUI.Activate();
            userInterface.SetState(shaderUI);
        }
    }
}
