using System;
using System.Linq;
using System.Text;
using Terraria.ModLoader;

namespace ModdersToolkit.Tools.REPL
{
	internal class REPLConsoleCommand : ModCommand
	{
		public override CommandType Type => CommandType.Console;

		public override string Command => "REPL";

		public override string Description => "Enter the C# REPL";

		public override void Action(CommandCaller caller, string input, string[] args) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Type c# code. Empty line to finish.");

			string buffer = "";
			while (true) {
				//Console.ForegroundColor = ConsoleColor.Cyan;
				//var key = Console.ReadKey(false);
				//if (key.Key == ConsoleKey.Tab)
				//{
				//	TabAction(buffer);
				//}
				//else if (key.Key == ConsoleKey.Escape)
				//{
				//	break;
				//}
				//else if (key.Key == ConsoleKey.LeftArrow)
				//{

				//}
				//else if (key.Key == ConsoleKey.Backspace)
				//{
				//	Console.Write("\b \b");
				//}
				//else
				//{
				////	Console.Write(key.KeyChar);
				//	buffer += key.KeyChar;
				//}
				//if (key.Key == ConsoleKey.Enter)
				//{
				//	Console.WriteLine(buffer);
				//	Console.ForegroundColor = ConsoleColor.Yellow;
				//	if (buffer == "")
				//		break;
				//	REPLTool.replBackend.Action(buffer);
				//	buffer = "";
				//	//break;
				//}
				Console.ForegroundColor = ConsoleColor.Cyan;
				string readline = Console.ReadLine();
				if (readline == "")
					break;
				Console.ForegroundColor = ConsoleColor.Yellow;

				REPLTool.replBackend.Action(readline);
			}
			Console.ResetColor();
		}

		public void TabAction(string buffer) {
			string[] completions = REPLTool.replBackend.GetCompletions(buffer);
			if (buffer.StartsWith("using ")) {
				string pre = buffer.Substring(6);
				completions = REPLTool.replBackend.namespaces.ToArray().Where(x => x.StartsWith(pre)).Select(x => x.Substring(pre.Length)).ToArray();
			}
			if (completions == null) {
				return;
			}

			string prefix = LongestCommonPrefix2(completions);
			if (prefix == "") {
				if (completions.Length > 0 && completions[0] != "") {
					string hint = "{" + string.Join(", ", completions) + "}";
					if (hint.Length > 400) {
						hint = hint.Substring(0, 400) + "...";
					}
					//AddChunkedLine(hint, CodeType.Hint);
				}
			}
			else {
				Console.WriteLine(completions);
				//
				//codeTextBox.Write(prefix);
			}
		}

		public string LongestCommonPrefix2(string[] strs) {
			if (strs.Length == 0)
				return String.Empty;
			Array.Sort(strs);

			var first = strs[0];
			var last = strs[strs.Length - 1];
			var strbuilder = new StringBuilder();
			for (int i = 0; i < first.Length; i++) {
				if (first[i] != last[i]) {
					break;
				}
				strbuilder.Append(first[i]);
			}

			return strbuilder.ToString();
		}

	}
}

