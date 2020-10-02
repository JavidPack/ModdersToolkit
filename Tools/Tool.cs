using ModdersToolkit.REPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools
{
	public abstract class Tool
	{
        /// <summary>
		/// Initializes this Tool. Called during Load.
		/// Useful for initializing data.
		/// </summary>
		internal virtual void Initialize() { }

        /// <summary>
        /// Initializes this Tool. Called during Unload.
        /// Useful for cleaning and disposing data.
        /// </summary>
		internal virtual void Terminate() { }

		/// <summary>
		/// Initializes this Tool. Called during Load after Initialize only on SP and Clients.
		/// Useful for initializing UI.
		/// </summary>
		internal virtual void ClientInitialize() { }

        /// <summary>
        /// Terminates this Tool. Called during Unload before Terminate only on SP and Clients.
        /// Useful for cleaning and disposing data.
        /// </summary>
		internal virtual void ClientTerminate() { }


        internal virtual void ScreenResolutionChanged()
		{
			Interface?.Recalculate();
		}

		internal virtual void UIUpdate()
		{
			if (Visible)
			{
				Interface?.Update(Main._drawInterfaceGameTime);
			}
		}

		internal virtual void UIDraw() { }

		internal virtual void WorldDraw() { }

		internal virtual void DrawUpdateToggle() { }

		internal virtual void Toggled() { }

		internal virtual void PostSetupContent() { }

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
