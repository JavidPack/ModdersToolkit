using ModdersToolkit.REPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools
{
	abstract class Tool
	{
		internal bool visible;
		internal string toggleTooltip;
		//internal virtual bool visible => _visible;

		/// <summary>
		/// Initializes this Tool. Called during Load.
		/// Useful for initializing data.
		/// </summary>
		internal virtual void Initialize() { }

		/// <summary>
		/// Initializes this Tool. Called during Load after Initialize only on SP and Clients.
		/// Useful for initializing UI.
		/// </summary>
		internal virtual void ClientInitialize() { }

		internal virtual void ScreenResolutionChanged() { }

		internal virtual void UIUpdate() { }

		internal virtual void UIDraw() { }

		internal virtual void DrawUpdateToggle() { }

		internal virtual void Toggled() { }

		//	internal virtual void UIDraw() { }
	}

	//class ToolTest : Tool
	//{
	//	//private bool _a;
	//	//internal override bool a => _a;
	//}
}
