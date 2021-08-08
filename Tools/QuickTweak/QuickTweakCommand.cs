using System;
using System.Reflection;
using Terraria.ModLoader;

namespace ModdersToolkit.Tools.QuickTweak
{
	class QuickTweakCommand : ModCommand
	{
		public override string Command => "tweak";

		public override string Description => "Adds a static field to the Quick Tweak menu";

		public override string Usage => "/tweak className fieldName";

		public override CommandType Type => CommandType.Chat;

		public override void Action(CommandCaller caller, string input, string[] args) {
			try {
				string className = args[0];
				string fieldName = args[1];

				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					foreach (var type in assembly.GetTypes()) {
						if(type.Name == className) {
							foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)) {
								if(field.Name == fieldName) {
									QuickTweakTool.AddTweak(field, $"{className}.{fieldName}");
									caller.Reply($"Quick tweak found and added for {className}.{fieldName}");
									return;
								}
							}
						}
					}
				}
				caller.Reply($"Static field {className}.{fieldName} not found");
			}
			catch (Exception) {
				throw;
			}
		}
	}
}
