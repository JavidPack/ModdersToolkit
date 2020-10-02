using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using ModdersToolkit.UIElements;
using System.Collections.Generic;

namespace ModdersToolkit.Tools.PlayerLayer
{
    internal class PlayerLayerUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public PlayerLayerUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		public UIList playerLayerList;

		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			mainPanel.Left.Set(-350f, 1f);
			mainPanel.Top.Set(-620f, 1f);
			mainPanel.Width.Set(310f, 0f);
			mainPanel.Height.Set(520f, 0f);
			mainPanel.SetPadding(12);
			mainPanel.BackgroundColor = Color.Yellow * 0.8f;

			int top = 0;
			UIText text = new UIText("Player Layer:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);
			top += 20;

			playerLayerList = new UIList();
			playerLayerList.Top.Pixels = top;
			//autoTrashGrid.Left.Pixels = spacing;
			playerLayerList.Width.Set(-25f, 1f); // leave space for scroll?
			playerLayerList.Height.Set(-top, 1f);
			playerLayerList.ListPadding = 6f;
			mainPanel.Append(playerLayerList);

			// this will initialize grid
			updateneeded = true;

			var playerLayerListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			playerLayerListScrollbar.SetView(100f, 1000f);
			playerLayerListScrollbar.Top.Pixels = top;// + spacing;
			playerLayerListScrollbar.Height.Set(-top /*- spacing*/, 1f);
			playerLayerListScrollbar.HAlign = 1f;
			mainPanel.Append(playerLayerListScrollbar);
			playerLayerList.SetScrollbar(playerLayerListScrollbar);

			Append(mainPanel);
		}

		public List<Terraria.ModLoader.PlayerLayer> playerLayers = new List<Terraria.ModLoader.PlayerLayer>();
		public List<UICheckbox> playerLayersCheckboxes = new List<UICheckbox>();
		public bool updateneeded;
		internal void UpdateList()
		{
			if (!updateneeded) { return; }
			updateneeded = false;

			playerLayerList.Clear();
			playerLayersCheckboxes.Clear();
			int order = 0;
			foreach (var item in playerLayers)
			{
				var box = new UICheckbox(item.Name, item.mod);
				box.order = order++;
				box.Selected = true;
				playerLayersCheckboxes.Add(box);
			}
			playerLayerList.AddRange(playerLayersCheckboxes);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			UpdateList();
		}


		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (mainPanel.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		internal void InformLayers(List<Terraria.ModLoader.PlayerLayer> layers)
		{
			foreach (var layer in layers)
			{
				if (!PlayerLayerTool.playerLayerUI.playerLayers.Contains(layer))
				{
					updateneeded = true;
					break;
				}
			}
			if (updateneeded)
			{
				playerLayers.Clear();
				playerLayers.AddRange(layers);
			}
		}
	}

    internal class PlayerLayerModPlayer : ModPlayer
	{
		public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
		{
			// TODO
		}

		public override void ModifyDrawLayers(List<Terraria.ModLoader.PlayerLayer> layers)
		{
			PlayerLayerTool.playerLayerUI.InformLayers(layers);

			if (PlayerLayerTool.playerLayerUI.updateneeded) return;
			foreach (var layer in layers)
			{
				if (PlayerLayerTool.playerLayerUI.playerLayers.Contains(layer))
				{
					var layerIndex = PlayerLayerTool.playerLayerUI.playerLayers.IndexOf(layer);
					layer.visible = PlayerLayerTool.playerLayerUI.playerLayersCheckboxes[layerIndex].Selected;
				}
				else
				{
				}
			}
		}

		// Maybe a separate tool or panel/tab	
		public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
		{
			// TODO
		}
	}
}
