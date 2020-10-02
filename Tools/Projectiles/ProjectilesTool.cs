using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.Projectiles
{
	class ProjectilesTool : Tool
	{
		internal static ProjectilesUI projectileUI;
		internal override void Initialize() {
			toggleTooltip = "Click to toggle Projectile Tool";
		}
		internal override void ClientInitialize() {
			userInterface = new UserInterface();
		}
		internal override void UIDraw() {
			if (visible) {
				projectileUI.Draw(Main.spriteBatch);
			}
		}
		internal override void PostSetupContent() {
			if (!Main.dedServ) {
				projectileUI = new ProjectilesUI(userInterface);
				projectileUI.Activate();
				userInterface.SetState(projectileUI);
			}
		}
	}
}
