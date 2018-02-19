using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.Tools;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

//todo, tool to make townnpc spritesheet out of current player 
//todo, Main.ignoreErrors = true; -- set to false and report errors to console
// TODO: Trigger spawn TownNPC

namespace ModdersToolkit
{
	public class ModdersToolkit : Mod
	{
		internal static ModdersToolkit instance;
		internal static List<Tool> tools;
		int lastSeenScreenWidth;
		int lastSeenScreenHeight;
		bool visible;

		public override void Load()
		{
			instance = this;

			tools = new List<Tool>();
			if (ModLoader.windows) // REPL tool not working non-windows yet. Is this the attribute problem?
			{
				tools.Add(new Tools.REPL.REPLTool());
			}
			tools.Add(new Tools.Hitboxes.HitboxesTool());
			tools.Add(new Tools.Dusts.DustTool());
			tools.Add(new Tools.Items.ItemTool());
			tools.Add(new Tools.Projectiles.ProjectilesTool());
			tools.Add(new Tools.PlayerLayer.PlayerLayerTool());
			tools.Add(new Tools.InterfaceLayer.InterfaceLayerTool());
			tools.Add(new Tools.Spawns.SpawnTool());
			// Not ready yet tools.Add(new Tools.Loot.LootTool());

			tools.ForEach(tool => tool.Initialize());

			if (!Main.dedServ)
			{
				tools.ForEach(tool => tool.ClientInitialize());

				UIElements.UICheckbox.checkboxTexture = GetTexture("UIElements/checkBox");
				UIElements.UICheckbox.checkmarkTexture = GetTexture("UIElements/checkMark");
				UIElements.UICheckbox2.checkboxTexture = GetTexture("UIElements/checkBox");
				UIElements.UICheckbox2.checkmarkTexture = GetTexture("UIElements/checkMark");
				UIElements.UITriStateCheckbox.checkboxTexture = GetTexture("UIElements/checkBox");
				UIElements.UITriStateCheckbox.checkmarkTexture = GetTexture("UIElements/checkMark");
				UIElements.UITriStateCheckbox.checkXTexture = GetTexture("UIElements/checkX");
			}
		}

		public override void Unload()
		{
			instance = null;
			tools = null;

			UIElements.UICheckbox.checkboxTexture = null;
			UIElements.UICheckbox.checkmarkTexture = null;
			UIElements.UICheckbox2.checkboxTexture = null;
			UIElements.UICheckbox2.checkmarkTexture = null;
			UIElements.UITriStateCheckbox.checkboxTexture = null;
			UIElements.UITriStateCheckbox.checkmarkTexture = null;
			UIElements.UITriStateCheckbox.checkXTexture = null;
		}

		public override void PostSetupContent()
		{
			tools.ForEach(tool => tool.PostSetupContent());
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (inventoryLayerIndex != -1)
			{
				layers.Insert(inventoryLayerIndex, new LegacyGameInterfaceLayer(
					"ModdersToolkit: Tools",
					delegate
					{
						//if (Main.drawingPlayerChat)
						{
							DrawUpdateToggles();
						}
						if (visible)
						{
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight)
							{
								tools.ForEach(tool => tool.ScreenResolutionChanged());
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}
							tools.ForEach(tool => tool.UIUpdate());
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
			if (rulerLayerIndex != -1)
			{
				layers.Insert(rulerLayerIndex, new LegacyGameInterfaceLayer(
					"ModdersToolkit: Tools Game Scale",
					delegate
					{
						if (visible)
						{
							tools.ForEach(tool => tool.WorldDraw());
						}
						return true;
					},
					InterfaceScaleType.Game)
				);
			}
			

			Tools.InterfaceLayer.InterfaceLayerTool.interfaceLayerUI.ModifyInterfaceLayers(layers);
		}

		Tool lastVisibleTool;
		internal void DrawUpdateToggles()
		{
			Point mousePoint = new Point(Main.mouseX, Main.mouseY);
			// calculate?
			int xPosition = Main.screenWidth - 210; //62; //78;
			int yPosition = Main.screenHeight - 36 + 10 - 25;

			// TODO, use UI/Settings_Toggle
			Texture2D toggleTexture = visible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;

			Rectangle toggleToolkitButtonRectangle = new Rectangle(xPosition, yPosition, toggleTexture.Width, toggleTexture.Height);
			bool toggleToolkitButtonHover = false;
			if (toggleToolkitButtonRectangle.Contains(mousePoint))
			{
				Main.LocalPlayer.mouseInterface = true;
				toggleToolkitButtonHover = true;
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					visible = !visible;
					Main.PlaySound(visible ? SoundID.MenuOpen : SoundID.MenuClose);

					if (!visible)
					{
						lastVisibleTool = null;
						foreach (var otherTool in tools)
						{
							if (otherTool.visible)
							{
								lastVisibleTool = otherTool;
								lastVisibleTool.visible = false;
								lastVisibleTool.Toggled();
							}
						}
					}
					if (visible && lastVisibleTool != null)
					{
						lastVisibleTool.visible = true;
						lastVisibleTool.Toggled();
					}
				}
			}
			Main.spriteBatch.Draw(toggleTexture, toggleToolkitButtonRectangle.TopLeft(), Color.White /** 0.7f*/);
			if (toggleToolkitButtonHover)
			{
				Main.HoverItem = new Item();
				Main.hoverItemName = "Click to toggle ModdersToolkit";
			}

			if (visible)
			{
				yPosition -= 18;
				foreach (var tool in tools)
				{
					toggleTexture = tool.visible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;

					toggleToolkitButtonRectangle = new Rectangle(xPosition, yPosition, toggleTexture.Width, toggleTexture.Height);
					toggleToolkitButtonHover = false;
					if (toggleToolkitButtonRectangle.Contains(mousePoint))
					{
						Main.LocalPlayer.mouseInterface = true;
						toggleToolkitButtonHover = true;
						if (Main.mouseLeft && Main.mouseLeftRelease)
						{
							foreach (var otherTool in tools)
							{
								if (tool != otherTool && otherTool.visible)
								{
									otherTool.visible = false;
									otherTool.Toggled();
								}
							}
							tool.visible = !tool.visible;
							Main.PlaySound(tool.visible ? SoundID.MenuOpen : SoundID.MenuClose);
							tool.Toggled();
						}
					}
					Main.spriteBatch.Draw(toggleTexture, toggleToolkitButtonRectangle.TopLeft(), Color.White);
					if (toggleToolkitButtonHover)
					{
						Main.HoverItem = new Item();
						Main.hoverItemName = tool.toggleTooltip;
					}
					xPosition += 18;
				}
			}
		}
	}
}
