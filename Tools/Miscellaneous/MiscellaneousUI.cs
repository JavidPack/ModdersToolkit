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

namespace ModdersToolkit.Tools.Miscellaneous
{
	class MiscellaneousUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public MiscellaneousUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			mainPanel.SetPadding(0);
			int width = 150;
			int height = 180;
			mainPanel.Left.Set(-40f - width, 1f);
			mainPanel.Top.Set(-110f - height, 1f);
			mainPanel.Width.Set(width, 0f);
			mainPanel.Height.Set(height, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Miscellaneous:", 0.85f);
			text.Top.Set(12f, 0f);
			text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			// checkboxes
			int top = 32;
			UICheckbox npcInfoCheckbox = new UICheckbox("NPC Info", "Show NPC ai values and other variables.");
			npcInfoCheckbox.Top.Set(top, 0f);
			npcInfoCheckbox.Left.Set(12f, 0f);
			npcInfoCheckbox.OnSelectedChanged += () => MiscellaneousTool.showNPCInfo = npcInfoCheckbox.Selected;
			mainPanel.Append(npcInfoCheckbox);
			top += 20;

			Append(mainPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (mainPanel.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
