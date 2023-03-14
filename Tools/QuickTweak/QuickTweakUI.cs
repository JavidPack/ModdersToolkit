using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace ModdersToolkit.Tools.QuickTweak
{
	class QuickTweakUI : UIToolState
	{
		internal UIPanel mainPanel;
		public UIList tweakList;
		public bool updateNeeded;

		private UserInterface userInterface;
		public QuickTweakUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			width = 500;
			mainPanel = new UIPanel();
			mouseAndScrollBlockers.Add(mainPanel);
			mainPanel.SetPadding(6);
			mainPanel.MinWidth.Set(width, 0);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			//height = 300;
			//mainPanel.Left.Set(-40f - width, 1f);
			//mainPanel.Top.Set(-110f - height, 1f);
			//mainPanel.Width.Set(width, 0f);
			//mainPanel.Height.Set(height, 0f);

			UIText text = new UIText("Quick Tweak:", 0.85f);
			mainPanel.Append(text);

			UICheckbox privateFieldsCheckbox = new UICheckbox("Private", "Show Private fields");
			privateFieldsCheckbox.Selected = QuickTweakTool.showPrivateFields;
			privateFieldsCheckbox.Left.Set(100, 0);
			privateFieldsCheckbox.OnSelectedChanged += () => {
				QuickTweakTool.showPrivateFields = privateFieldsCheckbox.Selected;
				updateNeeded = true;
			};
			mainPanel.Append(privateFieldsCheckbox);

			UIImageButton resetButton = new UIHoverImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Clear Tweaks");
			resetButton.OnLeftClick += (a, b) => {
				QuickTweakTool.tweaks.Clear();
				updateNeeded = true;
			};
			resetButton.Top.Set(height, 0f);
			resetButton.Left.Set(-26, 1f);
			mainPanel.Append(resetButton);

			// Separate page for common things? Save load?
			UIImageButton mountButton = new UIHoverImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Edit current mount");
			mountButton.OnLeftClick += (a, b) => {
				if (Main.LocalPlayer.mount._active) {
					//foreach (var field in typeof(Mount.MountData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)) {
						QuickTweak.QuickTweakTool.AddTweak(Main.LocalPlayer.mount._data, "");
					//}
				}
				updateNeeded = true;
			};
			mountButton.Top.Set(height, 0f);
			mountButton.Left.Set(-52, 1f);
			mainPanel.Append(mountButton);

			UIImageButton modPlayersButton = new UIHoverImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Edit ModPlayers");
			modPlayersButton.OnLeftClick += (a, b) => {
				FieldInfo modPlayersField = typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic);
				ModPlayer[] modPlayers = (ModPlayer[])modPlayersField.GetValue(Main.LocalPlayer);
				foreach (var modPlayer in modPlayers) {
					QuickTweak.QuickTweakTool.AddTweak(modPlayer, "");
				}
				updateNeeded = true;
			};
			modPlayersButton.Top.Set(height, 0f);
			modPlayersButton.Left.Set(-78, 1f);
			mainPanel.Append(modPlayersButton);

			UIImageButton nearestNPCButton = new UIHoverImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Nearest NPC");
			nearestNPCButton.OnLeftClick += (a, b) => {
				NPC closest = null;
				float minDistance = float.MaxValue;
				for (int k = 0; k < 200; k++) {
					if (Main.npc[k].active) {
						float distance = Vector2.DistanceSquared(Main.LocalPlayer.Center, Main.npc[k].Center);
						if (distance < minDistance) {
							closest = Main.npc[k];
							minDistance = distance;
						}
					}
				}
				if (closest != null) {
					QuickTweak.QuickTweakTool.AddTweak(closest, closest.GivenOrTypeName);
					if(closest.ModNPC != null)
						QuickTweak.QuickTweakTool.AddTweak(closest.ModNPC, closest.GivenOrTypeName + " modNPC");
				}
				FieldInfo globalNPCsField = typeof(NPC).GetField("globalNPCs", BindingFlags.Instance | BindingFlags.NonPublic);
				Instanced<GlobalNPC>[] globalNPCs = (Instanced<GlobalNPC>[])globalNPCsField.GetValue(closest);
				// TODO: Double check that this enumeration is correct with AppliesToEntity
				foreach (var globalNPC in globalNPCs) {
					QuickTweak.QuickTweakTool.AddTweak(globalNPC.Instance, "");
				}
				updateNeeded = true;
			};
			nearestNPCButton.Top.Set(height, 0f);
			nearestNPCButton.Left.Set(-104, 1f);
			mainPanel.Append(nearestNPCButton);

			AppendToAndAdjustWidthHeight(mainPanel, text, ref height, ref width);
			height += 14;

			tweakList = new UIList();
			tweakList.MinHeight.Set(500, 0f);
			tweakList.Width.Set(-25, 1f);
			tweakList.ListPadding = 6f;

			updateNeeded = true;

			var tweakListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			tweakListScrollbar.SetView(100f, 1000f);
			tweakListScrollbar.Top.Pixels = height;// + spacing;
			tweakListScrollbar.Height.Set(-height /*- spacing*/, 1f);
			tweakListScrollbar.HAlign = 1f;
			mainPanel.Append(tweakListScrollbar);
			tweakList.SetScrollbar(tweakListScrollbar);
			// minheight and minwidth have to be set for this to work.
			// If adjusting dynamically, mainpanel size not determined yet.
			AppendToAndAdjustWidthHeight(mainPanel, tweakList, ref height, ref width);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		internal void UpdateList() {
			if (!updateNeeded) { return; }
			updateNeeded = false;

			BindingFlags bindingFlags = QuickTweakTool.showPrivateFields ? BindingFlags.Public | BindingFlags.NonPublic : BindingFlags.Public;

			tweakList.Clear();
			foreach (var tweakEntry in QuickTweakTool.tweaks) {
				object tweakItem = tweakEntry.item;
				string label = tweakEntry.label;

				UIPanel panel = MakeUIPanelFromObject(bindingFlags, tweakItem, label, PaddingTop);

				// Additional buttons, separate from the actual data representation.
				// How should this be done in other contexts?
				UIImageButton removeButton = new UIHoverImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Remove");
				removeButton.OnLeftClick += (a, b) => {
					QuickTweakTool.tweaks.Remove(tweakEntry);
					updateNeeded = true;
				};
				removeButton.Top.Set(0, 0f);
				removeButton.Left.Set(-22, 1f);
				panel.Append(removeButton);

				tweakList.Add(panel);
			}
		}

		private static UIPanel MakeUIPanelFromObject(BindingFlags bindingFlags, object tweakItem, string label, float parentPaddingTop) {
			// Ability to pass in dynamic range from Call.
			
			List<(object, FieldInfo)> FieldsToDisplay = new List<(object, FieldInfo)>();
			if (tweakItem is Type type) {
				foreach (var field in type.GetFields(bindingFlags | BindingFlags.Static)) {
					FieldsToDisplay.Add((null, field));
				}
			}
			else if (tweakItem is FieldInfo FieldInfo) {
				FieldsToDisplay.Add((null, FieldInfo));
			}
			else if (tweakItem is ValueTuple<object, FieldInfo> tuple) {
				FieldsToDisplay.Add(((object, FieldInfo))tweakItem);
				//instance = tuple.Item1;
				//item = tuple.Item2;
			}
			else if (tweakItem is ValueTuple<object, string> stringtuple) {
				FieldInfo fieldInfo = stringtuple.Item1.GetType().GetField(stringtuple.Item2, bindingFlags | BindingFlags.Instance);
				if (fieldInfo != null)
					FieldsToDisplay.Add(((object, FieldInfo))(stringtuple.Item1, fieldInfo));
				else
					Main.NewText($"Field not found: {stringtuple.Item2} in {stringtuple.Item1.GetType().Name}");
			}
			else if (tweakItem is object) {
				foreach (FieldInfo field in tweakItem.GetType().GetFields(bindingFlags | BindingFlags.Instance)) {
					FieldsToDisplay.Add((tweakItem, field));
				}
			}

			// Pass in "additionalOnChanged" probably, to handle UI updates?
			// CreateUIElementFromObjectAndField(instance, item);
			// -> object will do recursion.
			// place element on Panel or in List.

			// Need dictionary of expected/known ranges for various field names?

			var panel = new UIPanel();
			panel.SetPadding(6);
			panel.BackgroundColor = Color.LightCoral * 0.7f;
			int top = 0;

			var header = new UIText(label != "" ? label : tweakItem.ToString());
			panel.Append(header);

			top += 20;

			foreach (var fieldToDisplay in FieldsToDisplay) {
				object instance = fieldToDisplay.Item1;
				FieldInfo fieldInfo = fieldToDisplay.Item2;

				if (fieldInfo.FieldType == typeof(bool)) {
					var dataElement = new UIBoolDataValue(fieldInfo.Name);
					dataElement.DataGetter = () => (bool)fieldInfo.GetValue(instance);
					dataElement.DataSetter = (value) => fieldInfo.SetValue(instance, value);
					AppendToAndIncrement(panel, new UICheckbox2(dataElement), ref top);
				}
				else if (fieldInfo.FieldType == typeof(int)) {
					RangeAttribute rangeAttribute = (RangeAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(RangeAttribute));
					int min = 0;
					int max = 100;
					if (rangeAttribute != null && rangeAttribute.Min is int && rangeAttribute.Max is int) {
						min = (int)rangeAttribute.Min;
						max = (int)rangeAttribute.Max;
					}
					var dataElement = new UIIntRangedDataValue(fieldInfo.Name, min: min, max: max);
					dataElement.DataGetter = () => (int)fieldInfo.GetValue(instance);
					dataElement.DataSetter = (value) => fieldInfo.SetValue(instance, value);
					AppendToAndIncrement(panel, new UIRange<int>(dataElement), ref top);
				}
				else if (fieldInfo.FieldType == typeof(float)) {
					RangeAttribute rangeAttribute = (RangeAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(RangeAttribute));
					float min = -1f;
					float max = 1f;
					if (rangeAttribute != null && rangeAttribute.Min is float && rangeAttribute.Max is float) {
						max = (float)rangeAttribute.Max;
						min = (float)rangeAttribute.Min;
					}
					if (rangeAttribute != null && rangeAttribute.Min is int && rangeAttribute.Max is int) {
						min = (int)rangeAttribute.Min;
						max = (int)rangeAttribute.Max;
					}
					var dataElement = new UIFloatRangedDataValue(fieldInfo.Name, .4f, min, max);
					dataElement.DataGetter = () => (float)fieldInfo.GetValue(instance);
					dataElement.DataSetter = (value) => fieldInfo.SetValue(instance, value);
					AppendToAndIncrement(panel, new UIRange<float>(dataElement), ref top);
				}
				else if (fieldInfo.FieldType == typeof(string)) {
					// TODO: editable
					AppendToAndIncrement(panel, new UIText((string)fieldInfo.GetValue(instance)), ref top);
				}
				else {
					//bool hasToString = fieldInfo.FieldType.GetMethod("ToString", new Type[0]).DeclaringType != typeof(object);
					AppendToAndIncrement(panel, new UIText(fieldInfo.Name + ": " + fieldInfo.GetValue(instance)?.ToString() ?? "null"), ref top);
				}
			}
			panel.Height.Set(top + panel.PaddingBottom + parentPaddingTop, 0f);
			panel.Width.Set(0, 1f);
			return panel;
		}

		internal static object GetTweakUIElement(object[] args) {
			// GetTweakUIElement object label
			if (args.Length != 2 && args.Length != 3)
				return "Failure";

			string label = "";
			if (args.Length >= 3 && args[2] is string _label)
				label = _label;

			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;

			UIPanel panel = MakeUIPanelFromObject(bindingFlags, args[1], label, 6f);
			return panel;
		}

		private static void AppendToAndIncrement(UIElement panel, UIElement element, ref int top) {
			element.Width = new StyleDimension(0, 1);
			element.Top = new StyleDimension(top, 0);
			panel.Append(element);
			top += 20;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateList();
		}
	}
}
