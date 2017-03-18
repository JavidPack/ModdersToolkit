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

namespace ModdersToolkit.Tools.Hitboxes
{
	class HitboxesUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public HitboxesUI(UserInterface userInterface)
		{
			this.userInterface = userInterface;
		}

		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			mainPanel.SetPadding(0);
			mainPanel.Left.Set(-190f, 1f);
			mainPanel.Top.Set(-450f, 1f);
			mainPanel.Width.Set(140f, 0f);
			mainPanel.Height.Set(100f, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Hitboxes:", 0.85f);
			text.Top.Set(12f, 0f);
			text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			// checkboxes
			UICheckbox playerMeleeCheckbox = new UICheckbox("Player Melee", "");
			playerMeleeCheckbox.Top.Set(32f, 0f);
			playerMeleeCheckbox.Left.Set(12f, 0f);
			playerMeleeCheckbox.OnSelectedChanged += () => HitboxesTool.showPlayerMeleeHitboxes = playerMeleeCheckbox.Selected;
			mainPanel.Append(playerMeleeCheckbox);

			UICheckbox npcCheckbox = new UICheckbox("NPC", "");
			npcCheckbox.Top.Set(52f, 0f);
			npcCheckbox.Left.Set(12f, 0f);
			npcCheckbox.OnSelectedChanged += () => HitboxesTool.showNPCHitboxes = npcCheckbox.Selected;
			mainPanel.Append(npcCheckbox);

			UICheckbox projectileCheckbox = new UICheckbox("Projectile", "Note: Some projectiles have special collision logic");
			projectileCheckbox.Top.Set(72f, 0f);
			projectileCheckbox.Left.Set(12f, 0f);
			projectileCheckbox.OnSelectedChanged += () => HitboxesTool.showProjectileHitboxes = projectileCheckbox.Selected;
			mainPanel.Append(projectileCheckbox);

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
