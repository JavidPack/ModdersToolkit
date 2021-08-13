using ModdersToolkit.Tools.REPL;
using ModdersToolkit.UIElements;
//using Mono.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using System.Reflection;

namespace ModdersToolkit.REPL
{
	public class REPLBackend
	{
		// 1.4: Mono.CSharp doesn't work on .net core, so migrated to Microsoft.CodeAnalysis.CSharp.Scripting. 
		// TODO: Reimplement code completion
		internal List<string> namespaces;

		// Make sure state is only initialized when a modder actually uses the tool.
		private ScriptState<object> state;

		public REPLBackend() {
			Reset();
		}

		public void Reset() {
			// TODO: Only call this during 1st initial usage, since 1st RunAsync takes over a second.
			namespaces = new List<string>();
			var opt = ScriptOptions.Default;

			var assemblies = new List<Assembly>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly == null) {
					continue;
				}
				try {
					if (assembly.IsDynamic || assembly.FullName.Contains("Steamworks.NET") || assembly.FullName.Contains("log4net") || assembly.FullName.Contains("mscorlib") || assembly.FullName.Contains("System.Core,") || assembly.FullName.Contains("System,") || assembly.FullName.Contains("System") || (!string.IsNullOrEmpty(assembly.Location) && Path.GetFileName(Path.GetDirectoryName(assembly.Location)) == "ModCompile")) {
					}
					else {
						// TODO: Fix this: Can't create a metadata reference to an assembly without location https://github.com/dotnet/roslyn/issues/2246
						// This prevents all mod dlls from being accessable.
						if (string.IsNullOrWhiteSpace(assembly.Location))
							continue; 
						assemblies.Add(assembly);
						
						var topLevel = assembly.GetTypes()
						   //.Select(t => GetTopLevelNamespace(t))
						   .Select(t => t.Namespace)
						   .Distinct();
						foreach (string ns in topLevel) {
							if (ns != null && !ns.StartsWith("<")) {
								if (!namespaces.Contains(ns)) {
									namespaces.Add(ns);
								}
							}
						}
						//namespaces.AddRange(topLevel);
					}
				}
				catch (NullReferenceException e) {
				}
				catch (ReflectionTypeLoadException e) {
				}
			}

			opt = opt.AddReferences(assemblies);
			opt = opt.AddImports("System", "Terraria", "Terraria.ModLoader", "Microsoft.Xna.Framework");
			state = CSharpScript.RunAsync("", opt).Result;
		}

		private static string GetTopLevelNamespace(Type t) {
			string ns = t.Namespace ?? "";
			int firstDot = ns.IndexOf('.');
			return firstDot == -1 ? ns : ns.Substring(0, firstDot);
		}

		private static string GetNamespace(Type t) {
			string ns = t.Namespace ?? "";
			int firstDot = ns.IndexOf('.');
			return firstDot == -1 ? ns : ns.Substring(0, firstDot);
		}

		public string[] GetCompletions(string input) {
			return new string[0];
			//string prefix;
			//var results = evaluator.GetCompletions(input, out prefix);
			////.NewText("Prefix: " + prefix);
			//return results;
		}

		public void Action(string line) {
			if (line == null)
				return;

			object result;
			bool result_set;

			try {
				state = state.ContinueWithAsync(line).Result;
				if(state.ReturnValue != null) {
					if (Main.dedServ) {
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine(state.ReturnValue.ToString());
					}
					else {
						REPLTool.moddersToolkitUI.AddChunkedLine(state.ReturnValue.ToString(), CodeType.Output);
					}
				}
				// Watch feature maybe?
				// Display all variables?
				// Where to display? Why?
				/*
				foreach (var variable in state.Variables) {
					if (!variable.IsReadOnly) {
						if (variable.Type == typeof(int)) {
							FieldInfo fieldFieldInfo = typeof(ScriptVariable).GetField("_field", BindingFlags.Instance | BindingFlags.NonPublic);
							FieldInfo objectFieldInfo = typeof(ScriptVariable).GetField("_instance", BindingFlags.Instance | BindingFlags.NonPublic);
							object obj = objectFieldInfo.GetValue(variable);
							FieldInfo fieldInfo = (FieldInfo)fieldFieldInfo.GetValue(variable);
							(object, FieldInfo) tweak = (obj, fieldInfo);
							Tools.QuickTweak.QuickTweakTool.AddTweak(tweak);
						}
					}
				}
				*/
			}
			catch (CompilationErrorException e) {
				if (Main.dedServ) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
				}
				else {
					REPLTool.moddersToolkitUI.AddChunkedLine(string.Join(Environment.NewLine, e.Diagnostics), CodeType.Error);
				}
			}
			return;
			// TODO: clean up and re-implement anything below.

			//evaluator.Evaluate(line, out result, out result_set);
			if (result_set) {
				if (result == null) {
					result = "<null>";
				}
				//Console.WriteLine(result);
				//Main.NewText(result.ToString());
				if (Main.dedServ) {
					Console.WriteLine(result.ToString());
				}
				else {
					REPLTool.moddersToolkitUI.AddChunkedLine(result.ToString(), CodeType.Output);
				}
				// TODO: store object for later, if next command is tweak, add to quick tweak menu
				//if(result is object && !(result is ValueType)) {
				//	Tools.QuickTweak.QuickTweakTool.AddTweak(result, "");
				//}
				//ModdersToolkit.instance.ModdersToolkitUI.replOutput.Add(new UICodeEntry(result.ToString(), CodeType.Output));
			}
			else {
				//if (Main.dedServ)
				//{
				//	Console.ForegroundColor = ConsoleColor.Red;
				//	Console.WriteLine(result.ToString());
				//}
				//Main.NewText(line);
			}

			//var mod = ModLoader.GetMod(args[0]);
			//var type = mod == null ? 0 : mod.NPCType(args[1]);
			//caller.Reply(type.ToString(), Color.Yellow);
		}
	}

	// TODO, REPL as ModCommand for server console
	// TODO, REPL as ModCommand for server console
	public class ConsoleTextWriter : TextWriter
	{
		public ConsoleTextWriter() {
		}

		private string buffer = "";
		public override void Write(char value) {
			if (value == '\n') {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(buffer);
				buffer = "";
			}
			else {
				buffer += value;
			}
		}

		public override Encoding Encoding {
			get { return System.Text.Encoding.UTF8; }
		}
	}

	public class MainNewTextTextWriter : TextWriter
	{
		public MainNewTextTextWriter() {
		}

		private string buffer = "";
		public override void Write(char value) {
			if (value == '\n') {
				REPLTool.moddersToolkitUI.AddChunkedLine(buffer, CodeType.Error);
				//int chunkSize = 55;
				//int stringLength = buffer.Length;
				//int chunks = (int)Math.Ceiling((float)stringLength / chunkSize);
				//for (int i = chunks - 1; i >= 0; i -= 1)
				//{
				//	int len = chunkSize;
				//	int start = i*chunkSize;
				//	if (i == chunks - 1)
				//	{
				//		len = stringLength%chunkSize;
				//	}
				//	//Console.WriteLine(str.Substring(i, chunkSize));
				//	ModdersToolkit.instance.ModdersToolkitUI.replOutput.Add(new UICodeEntry(buffer.Substring(start, len), CodeType.Error));
				//}
				//do
				//{
				//	string sub = SafeSubstring(buffer, 0, 50);
				//	ModdersToolkit.instance.ModdersToolkitUI.replOutput.Add(new UICodeEntry(sub, CodeType.Error));
				//	buffer = sub;
				//} while (buffer.Length > 0);
				buffer = "";
			}
			else {
				buffer += value;
			}
		}

		public string SafeSubstring(string text, int start, int length) {
			return text.Length <= start ? ""
				: text.Length - start <= length ? text.Substring(start)
				: text.Substring(start, length);
		}

		public override Encoding Encoding {
			get { return System.Text.Encoding.UTF8; }
		}
	}

}
