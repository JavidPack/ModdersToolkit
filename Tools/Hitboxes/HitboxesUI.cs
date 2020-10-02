using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.Hitboxes
{
	class HitboxesUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public HitboxesUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			mainPanel = new UIPanel();
			mainPanel.SetPadding(0);
			mainPanel.Left.Set(-190f, 1f);
			mainPanel.Top.Set(-290f, 1f);
			mainPanel.Width.Set(150f, 0f);
			mainPanel.Height.Set(180f, 0f);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Hitboxes:", 0.85f);
			text.Top.Set(12f, 0f);
			text.Left.Set(12f, 0f);
			mainPanel.Append(text);

			// checkboxes
			int top = 32;
			UICheckbox keepShowingCheckbox = new UICheckbox("Keep Showing", "Show hitboxes after leaving this tool");
			keepShowingCheckbox.Top.Set(top, 0f);
			keepShowingCheckbox.Left.Set(12f, 0f);
			keepShowingCheckbox.OnSelectedChanged += () => HitboxesTool.keepShowingHitboxes = keepShowingCheckbox.Selected;
			mainPanel.Append(keepShowingCheckbox);
			top += 20;

			UICheckbox playerMeleeCheckbox = new UICheckbox("Player Melee", "");
			playerMeleeCheckbox.Top.Set(top, 0f);
			playerMeleeCheckbox.Left.Set(12f, 0f);
			playerMeleeCheckbox.OnSelectedChanged += () => HitboxesTool.showPlayerMeleeHitboxes = playerMeleeCheckbox.Selected;
			mainPanel.Append(playerMeleeCheckbox);
			top += 20;

			UICheckbox npcCheckbox = new UICheckbox("NPC", "");
			npcCheckbox.Top.Set(top, 0f);
			npcCheckbox.Left.Set(12f, 0f);
			npcCheckbox.OnSelectedChanged += () => HitboxesTool.showNPCHitboxes = npcCheckbox.Selected;
			mainPanel.Append(npcCheckbox);
			top += 20;

			UICheckbox projectileCheckbox = new UICheckbox("Projectile C", "Collision: Some projectiles have special collision logic");
			projectileCheckbox.Top.Set(top, 0f);
			projectileCheckbox.Left.Set(12f, 0f);
			projectileCheckbox.OnSelectedChanged += () => HitboxesTool.showProjectileHitboxes = projectileCheckbox.Selected;
			mainPanel.Append(projectileCheckbox);
			top += 20;

			UICheckbox projectileDamageCheckbox = new UICheckbox("Projectile D", "Damage: Hitboxes modified by ModifyDamageHitbox");
			projectileDamageCheckbox.Top.Set(top, 0f);
			projectileDamageCheckbox.Left.Set(12f, 0f);
			projectileDamageCheckbox.OnSelectedChanged += () => HitboxesTool.showProjectileDamageHitboxes = projectileDamageCheckbox.Selected;
			mainPanel.Append(projectileDamageCheckbox);
			top += 20;

			UICheckbox teCheckbox = new UICheckbox("TE Position", "");
			teCheckbox.Top.Set(top, 0f);
			teCheckbox.Left.Set(12f, 0f);
			teCheckbox.OnSelectedChanged += () => HitboxesTool.showTEPositions = teCheckbox.Selected;
			mainPanel.Append(teCheckbox);
			top += 20;

			UICheckbox worldItemCheckbox = new UICheckbox("World Items", "");
			worldItemCheckbox.Top.Set(top, 0f);
			worldItemCheckbox.Left.Set(12f, 0f);
			worldItemCheckbox.OnSelectedChanged += () => HitboxesTool.showWorldItemHitboxes = worldItemCheckbox.Selected;
			mainPanel.Append(worldItemCheckbox);
			top += 20;

			Append(mainPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
