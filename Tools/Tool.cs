using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools
{
	public abstract class Tool
	{
		/// <summary>
		/// Initializes this Tool. Called during Load.
		/// Useful for initializing data.
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// Initializes this Tool. Called during Unload.
		/// Useful for cleaning and disposing data.
		/// </summary>
		public virtual void Terminate() { }

		/// <summary>
		/// Initializes this Tool. Called during Load after Initialize only on SP and Clients.
		/// Useful for initializing UI.
		/// </summary>
		public virtual void ClientInitialize() { }

		/// <summary>
		/// Terminates this Tool. Called during Unload before Terminate only on SP and Clients.
		/// Useful for cleaning and disposing data.
		/// </summary>
		public virtual void ClientTerminate() { }


		public virtual void ScreenResolutionChanged() {
			Interface?.Recalculate();
		}

		internal virtual void UIUpdate() {
			if (Visible) {
				Interface?.Update(Main._drawInterfaceGameTime);
			}
		}

		public virtual void UIDraw() { }

		public virtual void WorldDraw() { }

		public virtual void DrawUpdateToggle() { }

		public virtual void Toggled() { }

		public virtual void PostSetupContent() { }

		//	internal virtual void UIDraw() { }


		public bool Visible { get; internal set; }

		public string ToggleTooltip { get; protected set; }

		public UserInterface Interface { get; internal set; }
	}

	//class ToolTest : Tool
	//{
	//	//private bool _a;
	//	//internal override bool a => _a;
	//}
}
