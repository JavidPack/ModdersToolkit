using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.Tools;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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

			tools.ForEach(tool => tool.Initialize());

			if (!Main.dedServ)
			{
				tools.ForEach(tool => tool.ClientInitialize());
			}
		}

		public override void PostSetupContent()
		{
			tools.ForEach(tool => tool.PostSetupContent());
		}


		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (inventoryLayerIndex != -1)
			{
				layers.Insert(inventoryLayerIndex, new MethodSequenceListItem(
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
					null)
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
				Main.toolTip = new Item();
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
						Main.toolTip = new Item();
						Main.hoverItemName = tool.toggleTooltip;
					}
					xPosition += 18;
				}
			}
		}
	}
}
