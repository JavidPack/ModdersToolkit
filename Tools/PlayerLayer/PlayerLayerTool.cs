using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.PlayerLayer
{
	internal class PlayerLayerTool : Tool
	{
		internal static PlayerLayerUI playerLayerUI;
		internal static PlayerDrawLayer layerToDraw;

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
			Interface = null;

			playerLayerUI.Deactivate();
			playerLayerUI = null;
		}


		public override void UIDraw() {
			layerToDraw = null;
			if (Visible) {
				playerLayerUI.Draw(Main.spriteBatch);
			}
		}
	}
}
