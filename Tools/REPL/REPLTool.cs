using ModdersToolkit.REPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.REPL
{
	class REPLTool : Tool
	{
		internal static REPLBackend replBackend;
		internal static UserInterface ModdersToolkitUserInterface;
		internal static REPLUI moddersToolkitUI;

		internal override void Initialize()
		{
			replBackend = new REPLBackend();
			toggleTooltip = "Click to toggle C# REPL";
		}

		internal override void ClientInitialize()
		{
			ModdersToolkitUserInterface = new UserInterface();
			moddersToolkitUI = new REPLUI(ModdersToolkitUserInterface);
			moddersToolkitUI.Activate();
			ModdersToolkitUserInterface.SetState(moddersToolkitUI);
		}

		internal override void ScreenResolutionChanged()
		{
			ModdersToolkitUserInterface.Recalculate();
		}
		internal override void UIUpdate()
		{
			if (visible)
			{
				ModdersToolkitUserInterface.Update(Main._drawInterfaceGameTime);
			}
		}
		internal override void UIDraw()
		{
			if (visible)
			{
				moddersToolkitUI.Draw(Main.spriteBatch);
			}
		}
		internal override void Toggled()
		{
			Main.drawingPlayerChat = false;
			if (visible)
			{
				Tools.REPL.REPLTool.moddersToolkitUI.codeTextBox.Focus();
			}
			if (!visible)
			{
				Tools.REPL.REPLTool.moddersToolkitUI.codeTextBox.Unfocus();
			}
		}
	}
}
