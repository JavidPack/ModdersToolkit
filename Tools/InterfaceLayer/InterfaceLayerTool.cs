using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.InterfaceLayer
{
	class InterfaceLayerTool : Tool
	{
		internal static InterfaceLayerUI interfaceLayerUI;
		internal override void Initialize() {
			toggleTooltip = "Click to toggle Interface Layer Tool";
		}
		internal override void ClientInitialize() {
			userInterface = new UserInterface();
		}
		internal override void UIDraw() {
			if (visible) {
				interfaceLayerUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent() {
			if (!Main.dedServ) {
				interfaceLayerUI = new InterfaceLayerUI(userInterface);
				interfaceLayerUI.Activate();
				userInterface.SetState(interfaceLayerUI);
			}
		}
	}
}
