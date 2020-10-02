using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.Projectiles
{
	internal class ProjectilesTool : Tool
	{
		internal static ProjectilesUI projectileUI;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Projectile Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			projectileUI = new ProjectilesUI(Interface);
			projectileUI.Activate();

			Interface.SetState(projectileUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			projectileUI?.Deactivate();
			projectileUI = null;
		}


		public override void UIDraw() {
			if (Visible) {
				projectileUI.Draw(Main.spriteBatch);
			}
		}
	}
}
