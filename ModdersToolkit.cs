using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.Tools;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Audio;
using Terraria.GameContent;

//todo, tool to make townnpc spritesheet out of current player 
//todo, Main.ignoreErrors = true; -- set to false and report errors to console
// TODO: Trigger spawn TownNPC

// TODO: Clear Projectiles
// TODO: Reveal Map (Also, highlight changes?)
// TODO: Tooltips/Stats: Player position, block names, block strength (minpick).
// TODO: Moveable Windows
// TODO:

namespace ModdersToolkit
{
	public class ModdersToolkit : Mod
	{
		internal static List<Tool> tools;

		private int lastSeenScreenWidth;
		private int lastSeenScreenHeight;
		private bool visible;

		public override void Load() {
			Instance = this;

			tools = new List<Tool>();

			//if (Platform.IsWindows)
			// TODO: Double check if this works on mac/linux
			AddTool(new Tools.REPL.REPLTool());

			AddTool(new Tools.Hitboxes.HitboxesTool());
			AddTool(new Tools.Dusts.DustTool());
			AddTool(new Tools.Items.ItemTool());
			AddTool(new Tools.Projectiles.ProjectilesTool());
			//	TODO: Reimplement: AddTool(new Tools.PlayerLayer.PlayerLayerTool());
			AddTool(new Tools.InterfaceLayer.InterfaceLayerTool());
			AddTool(new Tools.Spawns.SpawnTool());
			AddTool(new Tools.Textures.TextureTool());
			if (Platform.IsWindows)
				AddTool(new Tools.Shaders.ShaderTool());
			// Not ready yet tools.Add(new Tools.Loot.LootTool());
			AddTool(new Tools.UIPlayground.UIPlaygroundTool());
			//	TODO: Reimplement: AddTool(new Tools.Backgrounds.BackgroundsTool());
			AddTool(new Tools.QuickTweak.QuickTweakTool());
			AddTool(new Tools.Miscellaneous.MiscellaneousTool());
			
			tools.ForEach(tool => tool.Initialize());

			if (!Main.dedServ) {
				tools.ForEach(tool => tool.ClientInitialize());

				UIElements.UICheckbox.checkboxTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkBox");
				UIElements.UICheckbox.checkmarkTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkMark");
				UIElements.UICheckbox2.checkboxTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkBox");
				UIElements.UICheckbox2.checkmarkTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkMark");
				UIElements.UITriStateCheckbox.checkboxTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkBox");
				UIElements.UITriStateCheckbox.checkmarkTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkMark");
				UIElements.UITriStateCheckbox.checkXTexture = ModContent.Request<Texture2D>("ModdersToolkit/UIElements/checkX");
			}
		}

		public override void Unload() {
			if (tools != null) {
				if (!Main.dedServ)
					tools.ForEach(tool => tool.ClientTerminate());

				tools.ForEach(tool => tool.Terminate());
			}

			tools?.Clear();
			tools = null;

			Instance = null;

			UIElements.UICheckbox.checkboxTexture = null;
			UIElements.UICheckbox.checkmarkTexture = null;
			UIElements.UICheckbox2.checkboxTexture = null;
			UIElements.UICheckbox2.checkmarkTexture = null;
			UIElements.UITriStateCheckbox.checkboxTexture = null;
			UIElements.UITriStateCheckbox.checkmarkTexture = null;
			UIElements.UITriStateCheckbox.checkXTexture = null;
		}

		public void AddTool(Tool tool) => tools.Add(tool);

		public override void PostSetupContent() {
			tools.ForEach(tool => tool.PostSetupContent());
		}

		public void UpdateUI(GameTime gameTime) {
			tools?.ForEach(tool => tool.UIUpdate());
		}

		public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (inventoryLayerIndex != -1) {
				layers.Insert(inventoryLayerIndex, new LegacyGameInterfaceLayer(
					"ModdersToolkit: Tools",
					delegate {
						//if (Main.drawingPlayerChat)
						{
							DrawUpdateToggles();
						}
						if (visible) {
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight) {
								tools.ForEach(tool => tool.ScreenResolutionChanged());
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}
							//tools.ForEach(tool => tool.UIUpdate());
							tools.ForEach(tool => tool.UIDraw());
						}
						//else
						//{
						//	if (ModdersToolkitUI.codeTextBox.focused)
						//	{
						//		ModdersToolkitUI.codeTextBox.Unfocus();
						//	}
						//}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}

			int rulerLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
			if (rulerLayerIndex != -1) {
				layers.Insert(rulerLayerIndex, new LegacyGameInterfaceLayer(
					"ModdersToolkit: Tools Game Scale",
					delegate {
						if (visible) {
							tools.ForEach(tool => tool.WorldDraw());
						}
						return true;
					},
					InterfaceScaleType.Game)
				);
			}


			Tools.InterfaceLayer.InterfaceLayerTool.interfaceLayerUI.ModifyInterfaceLayers(layers);
		}

		internal void DrawUpdateToggles() {
			Point mousePoint = new Point(Main.mouseX, Main.mouseY);
			// calculate?
			int xPosition = Main.screenWidth - 8 - tools.Count * 18;
			int yPosition = Main.screenHeight - 36 + 10 - 25;

			// TODO, use UI/Settings_Toggle
			Texture2D toggleTexture = visible ? TextureAssets.InventoryTickOn.Value : TextureAssets.InventoryTickOff.Value;

			Rectangle toggleToolkitButtonRectangle = new Rectangle(xPosition, yPosition, toggleTexture.Width, toggleTexture.Height);
			bool toggleToolkitButtonHover = false;
			if (toggleToolkitButtonRectangle.Contains(mousePoint)) {
				Main.LocalPlayer.mouseInterface = true;
				toggleToolkitButtonHover = true;
				if (Main.mouseLeft && Main.mouseLeftRelease) {
					visible = !visible;
					SoundEngine.PlaySound(visible ? SoundID.MenuOpen : SoundID.MenuClose);

					if (!visible) {
						LastVisibleTool = null;
						foreach (var otherTool in tools) {
							if (otherTool.Visible) {
								LastVisibleTool = otherTool;
								LastVisibleTool.Visible = false;
								LastVisibleTool.Toggled();
							}
						}
					}
					if (visible && LastVisibleTool != null) {
						LastVisibleTool.Visible = true;
						LastVisibleTool.Toggled();
					}
				}
			}
			Main.spriteBatch.Draw(toggleTexture, toggleToolkitButtonRectangle.TopLeft(), Color.White /** 0.7f*/);
			if (toggleToolkitButtonHover) {
				Main.HoverItem = new Item();
				Main.hoverItemName = "Click to toggle ModdersToolkit";
			}

			if (visible) {
				yPosition -= 18;
				foreach (var tool in tools) {
					toggleTexture = tool.Visible ? TextureAssets.InventoryTickOn.Value : TextureAssets.InventoryTickOff.Value;

					toggleToolkitButtonRectangle = new Rectangle(xPosition, yPosition, toggleTexture.Width, toggleTexture.Height);
					toggleToolkitButtonHover = false;
					if (toggleToolkitButtonRectangle.Contains(mousePoint)) {
						Main.LocalPlayer.mouseInterface = true;
						toggleToolkitButtonHover = true;
						if (Main.mouseLeft && Main.mouseLeftRelease) {
							foreach (var otherTool in tools) {
								if (tool != otherTool && otherTool.Visible) {
									otherTool.Visible = false;
									otherTool.Toggled();
								}
							}
							tool.Visible = !tool.Visible;
							SoundEngine.PlaySound(tool.Visible ? SoundID.MenuOpen : SoundID.MenuClose);
							tool.Toggled();
							if (tool.Visible) {
								LastVisibleTool = tool;
								Main.playerInventory = false;
							}
						}
					}
					Main.spriteBatch.Draw(toggleTexture, toggleToolkitButtonRectangle.TopLeft(), Color.White);
					if (toggleToolkitButtonHover) {
						Main.HoverItem = new Item();
						Main.hoverItemName = tool.ToggleTooltip;
					}
					xPosition += 18;
				}
			}
		}

		// v0.16 - add ("AddTweakSimple", object)
		//             - object - all instance fields will be displayed
		//             - Type - all static fields
		//             - FieldInfo - just 1 static field?
		//             - (object, FieldInfo)/ValueTuple<object, FieldInfo> - Only specified instance field.
		//				 nested objects?? pass in depth?
		//			     get range from field itself? [Range(2f, 3f)]
		public override object Call(params object[] args) {
			try {
				string messageType = args[0] as string;
				switch (messageType) {
					case "AddTweakSimple":
						Tools.QuickTweak.QuickTweakTool.Call(args);
						return "Success";
					default:
						Logger.Warn("Unknown Message type: " + messageType);
						return "Failure";
				}
			}
			catch (Exception e) {
				Logger.Warn("Call Error: " + e.StackTrace + e.Message);
			}
			return "Failure";
		}

		public Tool LastVisibleTool { get; private set; }

		public static ModdersToolkit Instance { get; private set; }
	}

	public class ModdersToolkitUISystem : ModSystem
	{
		public override void UpdateUI(GameTime gameTime) => ModdersToolkit.Instance.UpdateUI(gameTime);

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) => ModdersToolkit.Instance.ModifyInterfaceLayers(layers);
	}
}
