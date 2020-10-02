using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.PlayerLayer
{
	class PlayerLayerTool : Tool
	{
		internal static PlayerLayerUI playerLayerUI;
		internal override void Initialize() {
			toggleTooltip = "Click to toggle Player Layer Tool";
		}
		internal override void ClientInitialize() {
			userInterface = new UserInterface();
		}
		internal override void UIDraw() {
			if (visible) {
				playerLayerUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent() {
			if (!Main.dedServ) {
				playerLayerUI = new PlayerLayerUI(userInterface);
				playerLayerUI.Activate();
				userInterface.SetState(playerLayerUI);
			}
		}
	}
}
