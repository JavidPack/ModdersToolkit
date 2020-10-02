using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.PlayerLayer
{
	internal class PlayerLayerTool : Tool
	{
		internal static PlayerLayerUI playerLayerUI;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Player Layer Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			playerLayerUI = new PlayerLayerUI(Interface);
			playerLayerUI.Activate();

			Interface.SetState(playerLayerUI);
		}

		public override void ClientTerminate() {
			Interface = default;

			playerLayerUI.Deactivate();
			playerLayerUI = default;
		}


		public override void UIDraw() {
			if (Visible) {
				playerLayerUI.Draw(Main.spriteBatch);
			}
		}
	}
}
