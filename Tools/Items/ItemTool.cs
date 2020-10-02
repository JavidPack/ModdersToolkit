using System.Reflection;
using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.Items
{
	class ItemTool : Tool
	{
		internal static ItemUI itemUI;
		internal static FieldInfo globalItemsField;

		internal override void Initialize() {
			toggleTooltip = "Click to toggle Item Tool";
			globalItemsField = typeof(Item).GetField("globalItems", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		internal override void ClientInitialize() {
			userInterface = new UserInterface();
		}

		internal override void UIDraw() {
			if (visible) {
				itemUI.Draw(Main.spriteBatch);
			}
		}

		internal override void PostSetupContent() {
			if (!Main.dedServ) {
				itemUI = new ItemUI(userInterface);
				itemUI.Activate();
				userInterface.SetState(itemUI);
			}
		}
	}
}
