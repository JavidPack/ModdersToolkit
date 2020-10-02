using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.InterfaceLayer
{
	internal class InterfaceLayerUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public InterfaceLayerUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public UIList interfaceLayerList;

		public override void OnInitialize() {
			mainPanel = new UIPanel();
			mainPanel.Left.Set(-350f, 1f);
			mainPanel.Top.Set(-620f, 1f);
			mainPanel.Width.Set(310f, 0f);
			mainPanel.Height.Set(520f, 0f);
			mainPanel.SetPadding(12);
			mainPanel.BackgroundColor = Color.Yellow * 0.8f;

			int top = 0;
			UIText text = new UIText("Interface Layer:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);
			top += 20;

			interfaceLayerList = new UIList();
			interfaceLayerList.Top.Pixels = top;
			//autoTrashGrid.Left.Pixels = spacing;
			interfaceLayerList.Width.Set(-25f, 1f); // leave space for scroll?
			interfaceLayerList.Height.Set(-top, 1f);
			interfaceLayerList.ListPadding = 6f;
			mainPanel.Append(interfaceLayerList);

			// this will initialize List
			updateneeded = true;

			var interfaceLayerListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			interfaceLayerListScrollbar.SetView(100f, 1000f);
			interfaceLayerListScrollbar.Top.Pixels = top;// + spacing;
			interfaceLayerListScrollbar.Height.Set(-top /*- spacing*/, 1f);
			interfaceLayerListScrollbar.HAlign = 1f;
			mainPanel.Append(interfaceLayerListScrollbar);
			interfaceLayerList.SetScrollbar(interfaceLayerListScrollbar);

			Append(mainPanel);
		}

		public List<string> interfaceLayers = new List<string>();
		public List<UICheckbox> interfaceLayersCheckboxes = new List<UICheckbox>();
		public bool updateneeded;
		internal void UpdateList() {
			if (!updateneeded) { return; }
			updateneeded = false;

			interfaceLayerList.Clear();
			interfaceLayersCheckboxes.Clear();
			int order = 0;
			foreach (var item in interfaceLayers) {
				var box = new UICheckbox(item, "");
				box.order = order++;
				box.Selected = true;
				interfaceLayersCheckboxes.Add(box);
			}
			interfaceLayerList.AddRange(interfaceLayersCheckboxes);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateList();
		}


		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			InformLayers(layers);

			if (updateneeded)
				return;
			foreach (var layer in layers) {
				if (interfaceLayers.Contains(layer.Name) && layer.Name != "ModdersToolkit: Tools") {
					var layerIndex = interfaceLayers.IndexOf(layer.Name);
					layer.Active = interfaceLayersCheckboxes[layerIndex].Selected;
				}
				else {
				}
			}
		}

		internal void InformLayers(List<GameInterfaceLayer> layers) {
			foreach (var layer in layers) {
				if (!interfaceLayers.Contains(layer.Name)) {
					updateneeded = true;
					break;
				}
			}
			if (updateneeded) {
				interfaceLayers.Clear();
				interfaceLayers.AddRange(layers.Select(x => x.Name));
			}
		}
	}
}
