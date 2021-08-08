using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace ModdersToolkit.Tools.QuickTweak
{
	class TweakEntry
	{
		public object item;
		public string label;
		//public Func<string> label;

		public TweakEntry(object item, string label) {
			this.item = item;
			this.label = label;
		}
	}

	public class QuickTweakTool : Tool
	{
		internal static QuickTweakUI QuickTweakUI;
		internal static List<TweakEntry> tweaks = new List<TweakEntry>();

		internal static bool showPrivateFields = false;

		public override void Initialize() {
			ToggleTooltip = "Click to toggle Quick Tweak Tool";
		}

		public override void ClientInitialize() {
			Interface = new UserInterface();

			QuickTweakUI = new QuickTweakUI(Interface);
			QuickTweakUI.Activate();

			Interface.SetState(QuickTweakUI);
		}

		public override void ClientTerminate() {
			Interface = null;

			QuickTweakUI?.Deactivate();
			QuickTweakUI = null;
		}

		public override void UIDraw() {
			if (Visible) {
				QuickTweakUI.Draw(Main.spriteBatch);
			}
		}

		internal static void Call(object[] args) {
			// Pass in object instance -> all fields shown
			// Pass in just fieldinfo -> static field
			// pass in static class somehow? Pass in Type?
			// Pass in ()=> annonomous functions.
			// --> View only usage? () => Main.LocalPlayer.Center.X, ModPLayer bool, etc.
			// check RangeAttribute automatically somehow? pass in range?
			// Pass in object "stringFieldName"

			string label = "";
			if (args.Length >= 3 && args[2] is string _label)
				label = _label;

			//Type inputType = args[1].GetType();
			//if (inputType.IsGenericType) {
			//	var openType = inputType.GetGenericTypeDefinition();
			//	if (openType == typeof(ValueTuple<,>)) {

			//		//object myTuple = ((object)instance, new object());

			//		if (typeof(object).IsAssignableFrom(inputType.GenericTypeArguments[0]) && typeof(FieldInfo).IsAssignableFrom(inputType.GenericTypeArguments[1])){
			//			var tuple = (ValueTuple <object, string>)args[1];
			//			var result = new ValueTuple<object, string>();
			//			tweaks.Add(new TweakEntry(args[1], label));
			//		}
			//	}
			//}

			tweaks.Add(new TweakEntry(args[1], label));
		}

		internal static void AddTweak(FieldInfo field, string label) {
			tweaks.Add(new TweakEntry(field, label));
			QuickTweakUI.updateNeeded = true;
		}

		public static void AddTweak(object field, string label) {
			tweaks.Add(new TweakEntry(field, label));
			QuickTweakUI.updateNeeded = true;
		}

		internal static void AddTweak(ValueTuple<object, FieldInfo> tuple) {
			tweaks.Add(new TweakEntry(tuple, ""));
			QuickTweakUI.updateNeeded = true;
		}

		internal static void AddTweakUIElement(UIElement uIElement) {
			tweaks.Add(new TweakEntry(uIElement, ""));
			QuickTweakUI.updateNeeded = true;
		}
	}
}
