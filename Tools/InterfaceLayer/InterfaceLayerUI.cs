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
	internal class InterfaceLayerUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public InterfaceLayerUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public UIList interfaceLayerList;

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = Color.Yellow * 0.8f;

			UIText text = new UIText("Interface Layer:", 0.85f);
			AppendToAndAdjustWidthHeight(mainPanel, text, ref height, ref width);

			interfaceLayerList = new UIList();
			//autoTrashGrid.Left.Pixels = spacing;
			interfaceLayerList.Width.Set(-25f, 1f); // leave space for scroll?
			interfaceLayerList.Height.Set(-height, 1f);

			interfaceLayerList.MinHeight.Set(480, 0f);
			interfaceLayerList.MinWidth.Set(380, 0f);

			interfaceLayerList.ListPadding = 6f;

			// this will initialize List
			updateneeded = true;

			var interfaceLayerListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			interfaceLayerListScrollbar.SetView(100f, 1000f);
			interfaceLayerListScrollbar.Top.Pixels = height;// + spacing;
			interfaceLayerListScrollbar.Height.Set(-height /*- spacing*/, 1f);
			interfaceLayerListScrollbar.HAlign = 1f;
			mainPanel.Append(interfaceLayerListScrollbar);
			interfaceLayerList.SetScrollbar(interfaceLayerListScrollbar);

			AppendToAndAdjustWidthHeight(mainPanel, interfaceLayerList, ref height, ref width);

			AdjustMainPanelDimensions(mainPanel);
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

		private string[] hideWhenAnyToolVisible = new[] { "Vanilla: Mouse Text" };
		public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			if(ModdersToolkit.Instance.LastVisibleTool?.Interface == this.userInterface) {

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
			else {
				foreach (var layer in layers) {
					if (ModdersToolkit.Instance.LastVisibleTool != null && ModdersToolkit.Instance.LastVisibleTool.Visible) {
						hideWhenAnyToolVisible = new string[] {
							"Vanilla: Map / Minimap",
							"Vanilla: Resource Bars",
							"Vanilla: Info Accessories Bar"
						};
						// Inventory still a big issue. Maybe just set map and Main.playerInventory to false?
						if (hideWhenAnyToolVisible.Contains(layer.Name))
							layer.Active = false;
					}
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
