using Microsoft.Xna.Framework;
using Terraria.UI;

namespace ModdersToolkit.Tools
{
	public class UIToolState : UIState
	{
		public int width;
		public int height;

		public void AppendToAndAdjustWidthHeight(UIElement panel, UIElement element, ref int height, ref int width) {
			element.Top.Set(height, 0);
			panel.Append(element);
			height += (int)element.GetOuterDimensions().Height + 4;
			//height += element.MinHeight.Pixels
			width = (int)MathHelper.Max(width, element.GetOuterDimensions().Width);
		}

		public override void OnInitialize() {
			base.OnInitialize();
			width = 0;
			height = 0;
		}

		public void AdjustMainPanelDimensions(UIElement mainPanel) {
			height += (int)(mainPanel.PaddingBottom + mainPanel.PaddingTop);
			width += (int)(mainPanel.PaddingLeft + mainPanel.PaddingRight);

			mainPanel.Left.Set(-20f - width, 1f);
			mainPanel.Top.Set(-80f - height, 1f);
			mainPanel.Width.Set(width, 0f);
			mainPanel.Height.Set(height, 0f);
		}
	}
}
