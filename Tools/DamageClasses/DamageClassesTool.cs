using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.DamageClasses
{
	public class DamageClassesTool : Tool
	{
		internal static DamageClassesUI DamageClassesUI;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Damage Classes Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			DamageClassesUI = new DamageClassesUI(Interface);
			DamageClassesUI.Activate();

			Interface.SetState(DamageClassesUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			DamageClassesUI?.Deactivate();
			DamageClassesUI = null;
		}

		public override void UIDraw() {
			if (Visible) {
				DamageClassesUI.Draw(Main.spriteBatch);
			}
		}
	}
}
