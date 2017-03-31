using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using System.Text;
using ModdersToolkit.UIElements;
using ModdersToolkit.Tools;
using ModdersToolkit.Tools.REPL;

namespace ModdersToolkit.REPL
{
	class REPLUI : UIState
	{
		public static bool visible = false;
		public UIElements.FixedUIScrollbar keyboardScrollbar;
		public UIPanel keyboardPanel;
		public UIList replOutput;
		public NewUITextBox codeTextBox;
		float spacing = 8f;
		public static string filterText = "";
		private UserInterface userInterface;

		public REPLUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		public override void OnInitialize()
		{
			keyboardPanel = new UIPanel();
			keyboardPanel.SetPadding(0);
			keyboardPanel.Left.Set(-550f, 1f);
			keyboardPanel.Top.Set(-350f, 1f);
			keyboardPanel.Width.Set(500f, 0f);
			keyboardPanel.Height.Set(300f, 0f);
			keyboardPanel.BackgroundColor = new Color(73, 94, 171);

			codeTextBox = new NewUITextBox("Type code here", 1f);
			codeTextBox.SetUnfocusKeys(false, false);
			codeTextBox.BackgroundColor = Color.Transparent;
			codeTextBox.BorderColor = Color.Transparent;
			codeTextBox.Left.Pixels = 0;
			codeTextBox.Top.Pixels = 0;
			codeTextBox.Width.Set(-20, 1f);
			//filterTextBox.OnTextChanged += () => { filterText = filterTextBox.Text; updateneeded = true; };
			codeTextBox.OnEnterPressed += EnterAction;
			codeTextBox.OnTabPressed += TabAction;
			codeTextBox.OnUpPressed += UpAction;
			keyboardPanel.Append(codeTextBox);

			replOutput = new UIList();
			replOutput.Width.Set(-25f, 1f); // left spacing plus scrollbar
			replOutput.Height.Set(-2 * spacing - codeTextBox.GetDimensions().Height, 1f);
			replOutput.Left.Set(spacing, 0f);
			replOutput.Top.Set(spacing + codeTextBox.GetDimensions().Height, 0f);
			replOutput.ListPadding = 10f;
			keyboardPanel.Append(replOutput);

			keyboardScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			keyboardScrollbar.SetView(100f, 1000f);
			keyboardScrollbar.Top.Pixels = 2 * spacing;
			keyboardScrollbar.Height.Set(-4 * spacing, 1f);
			keyboardScrollbar.Left.Set(-4, 0f);
			keyboardScrollbar.HAlign = 1f;
			keyboardPanel.Append(keyboardScrollbar);

			replOutput.SetScrollbar(keyboardScrollbar);

			Append(keyboardPanel);
		}

		public void EnterAction()
		{
			if (codeTextBox.Text == "clear")
			{
				replOutput.Clear();
				codeTextBox.SetText("");
				Main.drawingPlayerChat = false;
				return;
			}
			if (codeTextBox.Text == "reset")
			{
				replOutput.Clear();
				codeTextBox.SetText("");
				Main.drawingPlayerChat = false;
				REPLTool.replBackend.Reset();
				return;
			}
			AddChunkedLine(codeTextBox.Text, CodeType.Input);
			//replOutput.Add(new UICodeEntry(codeTextBox.Text, CodeType.Input));
			REPLTool.replBackend.Action(codeTextBox.Text);
			codeTextBox.SetText("");
			//Main.chatRelease = false;
			Main.drawingPlayerChat = false;
		}

		public void UpAction()
		{
			foreach (var item in replOutput._items)
			{
				UICodeEntry codeEntry = item as UICodeEntry;
				if (codeEntry.codeType == CodeType.Input)
				{
					codeTextBox.SetText(codeEntry.Text);
					break;
				}
			}
		}

		public void TabAction()
		{
			string[] completions = REPLTool.replBackend.GetCompletions(codeTextBox.Text);
			if (codeTextBox.Text.StartsWith("using "))
			{
				string pre = codeTextBox.Text.Substring(6);
				completions = REPLTool.replBackend.namespaces.ToArray().Where(x => x.StartsWith(pre)).Select(x => x.Substring(pre.Length)).ToArray();
			}
			if (completions == null) return;
			string prefix = LongestCommonPrefix2(completions);
			if (prefix == "")
			{
				if (completions.Length > 0 && completions[0] != "")
				{
					string hint = "{" + string.Join(", ", completions) + "}";
					if (hint.Length > 400)
					{
						hint = hint.Substring(0, 400) + "...";
					}
					AddChunkedLine(hint, CodeType.Hint);
					//replOutput.Add(new UICodeEntry(hint, CodeType.Hint));
				}
			}
			else
			{
				codeTextBox.Write(prefix);
			}
		}

		private bool updateneeded;



		internal void UpdateCheckboxes()
		{
			if (!updateneeded) { return; }
			updateneeded = false;

			//replOutput.Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			UpdateCheckboxes();
			if (keyboardPanel.ContainsPoint(Main.MouseScreen))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}
		}

		public string LongestCommonPrefix2(string[] strs)
		{
			if (strs.Length == 0) return String.Empty;
			Array.Sort(strs);

			var first = strs[0];
			var last = strs[strs.Length - 1];
			var strbuilder = new StringBuilder();
			for (int i = 0; i < first.Length; i++)
			{
				if (first[i] != last[i])
				{
					break;
				}
				strbuilder.Append(first[i]);
			}

			return strbuilder.ToString();
		}

		internal void AddChunkedLine(string buffer, CodeType codeType)
		{
			int chunkSize = 55;
			int stringLength = buffer.Length;
			int chunks = (int)Math.Ceiling((float)stringLength / chunkSize);
			for (int i = chunks - 1; i >= 0; i -= 1)
			{
				int len = chunkSize;
				int start = i * chunkSize;
				if (i == chunks - 1)
				{
					len = stringLength % chunkSize;
				}
				//Console.WriteLine(str.Substring(i, chunkSize));
				replOutput.Add(new UICodeEntry(buffer.Substring(start, len), codeType));
			}
		}
	}
}
