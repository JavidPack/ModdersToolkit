using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModdersToolkit.Tools.Hitboxes
{
	internal class HitboxesUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public HitboxesUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = new Color(173, 94, 171);

			UIText text = new UIText("Hitboxes:", 0.85f);
			AppendToAndAdjustWidthHeight(mainPanel, text, ref height, ref width);

			// checkboxes
			UICheckbox keepShowingCheckbox = new UICheckbox("Keep Showing", "Show hitboxes after leaving this tool");
			keepShowingCheckbox.OnSelectedChanged += () => HitboxesTool.keepShowingHitboxes = keepShowingCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, keepShowingCheckbox, ref height, ref width);

			var texts = new[] { "Player Position", "Player Velocity", /*"NPC Position", "NPC Velocity", "Projectile Position",*/ "Projectile Velocity" };
			Action<bool>[] actions = new Action<bool>[]
			{
				(bool selected) => HitboxesTool.showPlayerPosition = selected,
				(bool selected) => HitboxesTool.showPlayerVelocity = selected,
				//(bool selected) => HitboxesTool.showNPCPosition = selected,
				//(bool selected) => HitboxesTool.showNPCVelocity = selected,
				//(bool selected) => HitboxesTool.showProjectilePosition = selected,
				(bool selected) => HitboxesTool.showProjectileVelocity = selected,
			};

			foreach (var nw in texts.Zip(actions, Tuple.Create)) {
				Console.WriteLine(nw.Item1 + nw.Item2);

				UICheckbox checkbox = new UICheckbox(nw.Item1, "");
				checkbox.OnSelectedChanged += () => nw.Item2.Invoke(checkbox.Selected);
				AppendToAndAdjustWidthHeight(mainPanel, checkbox, ref height, ref width);
			}

			UICheckbox playerMeleeCheckbox = new UICheckbox("Player Melee", "");
			playerMeleeCheckbox.OnSelectedChanged += () => HitboxesTool.showPlayerMeleeHitboxes = playerMeleeCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, playerMeleeCheckbox, ref height, ref width);

			UICheckbox npcCheckbox = new UICheckbox("NPC", "");
			npcCheckbox.OnSelectedChanged += () => HitboxesTool.showNPCHitboxes = npcCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, npcCheckbox, ref height, ref width);

			UICheckbox projectileCheckbox = new UICheckbox("Projectile C", "Collision: Some projectiles have special collision logic");
			projectileCheckbox.OnSelectedChanged += () => HitboxesTool.showProjectileHitboxes = projectileCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, projectileCheckbox, ref height, ref width);

			UICheckbox projectileDamageCheckbox = new UICheckbox("Projectile D", "Damage: Hitboxes modified by ModifyDamageHitbox");
			projectileDamageCheckbox.OnSelectedChanged += () => HitboxesTool.showProjectileDamageHitboxes = projectileDamageCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, projectileDamageCheckbox, ref height, ref width);

			UICheckbox teCheckbox = new UICheckbox("TE Position", "");
			teCheckbox.OnSelectedChanged += () => HitboxesTool.showTEPositions = teCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, teCheckbox, ref height, ref width);

			UICheckbox worldItemCheckbox = new UICheckbox("World Items", "");
			worldItemCheckbox.OnSelectedChanged += () => HitboxesTool.showWorldItemHitboxes = worldItemCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, worldItemCheckbox, ref height, ref width);

			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
