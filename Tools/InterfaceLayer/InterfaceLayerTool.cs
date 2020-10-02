using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.InterfaceLayer
{
	internal class InterfaceLayerTool : Tool
	{
		internal static InterfaceLayerUI interfaceLayerUI;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Interface Layer Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			interfaceLayerUI = new InterfaceLayerUI(Interface);
			interfaceLayerUI.Activate();

			Interface.SetState(interfaceLayerUI);
		}

		public override void ClientTerminate() {
			Interface = default;

			interfaceLayerUI?.Deactivate();
			interfaceLayerUI = default;
		}


		public override void UIDraw() {
			if (Visible) {
				interfaceLayerUI.Draw(Main.spriteBatch);
			}
		}
	}
}
