using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.DamageClasses
{
	internal class UIDamageClass : UIElement
	{
		DamageClass damageClass;
		private readonly List<(DamageClass, StatInheritanceData)> inheritors;

		public UIDamageClass(DamageClass damageClass, List<(DamageClass, StatInheritanceData)> inheritors) {
			int bonus = 0;
			foreach (var inheritor in inheritors) {
				bonus += 2;
				//StatInheritanceData inheritanceData = inheritor.Item2;
				//if (inheritanceData.damageInheritance != 0)
				//	bonus++;
				//if (inheritanceData.knockbackInheritance != 0)
				//	bonus++;
				//if (inheritanceData.critChanceInheritance != 0)
				//	bonus++;
				//if (inheritanceData.attackSpeedInheritance != 0)
				//	bonus++;
				//if (inheritanceData.armorPenInheritance != 0)
				//	bonus++;
			}

			Width.Set(0, 1f);
			Height.Set(200 + 20 * bonus, 0f);
			this.damageClass = damageClass;
			this.inheritors = inheritors;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			Rectangle hitbox = GetInnerDimensions().ToRectangle();
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.MediumPurple * 1f);

			CalculatedStyle innerDimensions = base.GetInnerDimensions();

			float shopx = innerDimensions.X + 6;
			float shopy = innerDimensions.Y + 6;

			var font = FontAssets.MouseText.Value;

			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, $"{damageClass.Name}: \"{damageClass.DisplayName}\"", shopx, shopy, Color.White, Color.Black, Vector2.Zero, 1f);
			shopy += 20;

			spriteBatch.DrawString(font, $"-Defined-", new Vector2(shopx + 200, shopy), Color.Black);
			shopy += 20;

			StatModifier statModifier = Main.LocalPlayer.GetDamage(damageClass);
			spriteBatch.DrawString(font, $"Damage:", new Vector2(shopx, shopy), Color.Black);
			DrawStat("Add", statModifier.Additive, 1, 0);
			DrawStat("Mult", statModifier.Multiplicative, 1, 1);
			DrawStat("Base", statModifier.Base, 0, 2);
			DrawStat("Flat", statModifier.Flat, 0, 3);
			shopy += 20;

			statModifier = Main.LocalPlayer.GetKnockback(damageClass);
			spriteBatch.DrawString(font, $"Knockback:", new Vector2(shopx, shopy), Color.Black);
			DrawStat("Add", statModifier.Additive, 1, 0);
			DrawStat("Mult", statModifier.Multiplicative, 1, 1);
			DrawStat("Base", statModifier.Base, 0, 2);
			DrawStat("Flat", statModifier.Flat, 0, 3);
			shopy += 20;

			shopx -= 100;
			DrawStat("Crit", Main.LocalPlayer.GetCritChance(damageClass), 0, 0);
			DrawStat("Attack Speed", Main.LocalPlayer.GetAttackSpeed(damageClass), 1, 1);
			shopx += 100;
			DrawStat("Armor Penetration", Main.LocalPlayer.GetArmorPenetration(damageClass), 0, 2);
			shopy += 20;

			spriteBatch.DrawString(font, $" -Total- ", new Vector2(shopx + 200, shopy), Color.Black);
			shopy += 20;

			statModifier = Main.LocalPlayer.GetTotalDamage(damageClass);
			spriteBatch.DrawString(font, $"Damage:", new Vector2(shopx, shopy), Color.Black);
			DrawStat("Add", statModifier.Additive, 1, 0);
			DrawStat("Mult", statModifier.Multiplicative, 1, 1);
			DrawStat("Base", statModifier.Base, 0, 2);
			DrawStat("Flat", statModifier.Flat, 0, 3);
			shopy += 20;

			statModifier = Main.LocalPlayer.GetTotalKnockback(damageClass);
			spriteBatch.DrawString(font, $"Knockback:", new Vector2(shopx, shopy), Color.Black);
			DrawStat("Add", statModifier.Additive, 1, 0);
			DrawStat("Mult", statModifier.Multiplicative, 1, 1);
			DrawStat("Base", statModifier.Base, 0, 2);
			DrawStat("Flat", statModifier.Flat, 0, 3);
			shopy += 20;

			shopx -= 100;
			DrawStat("Crit", Main.LocalPlayer.GetTotalCritChance(damageClass), 0, 0);
			DrawStat("Attack Speed", Main.LocalPlayer.GetTotalAttackSpeed(damageClass), 1, 1);
			shopx += 100;
			DrawStat("Armor Penetration", Main.LocalPlayer.GetTotalArmorPenetration(damageClass), 0, 2);
			shopy += 30;

			foreach (var inheritor in inheritors) {
				spriteBatch.DrawString(font, $"Inherits from {inheritor.Item1.Name}:", new Vector2(shopx, shopy), Color.Black);
				shopy += 20;
				shopx -= 100;
				StatInheritanceData inheritanceData = inheritor.Item2;
				if (inheritanceData.damageInheritance != 0) {
					DrawStat("Dmg", inheritanceData.damageInheritance, 1, 0);
					//shopy += 20;
				}
				if (inheritanceData.knockbackInheritance != 0) {
					DrawStat("Kb", inheritanceData.knockbackInheritance, 1, 1);
					//shopy += 20;
				}
				if (inheritanceData.critChanceInheritance != 0) {
					DrawStat("Crit", inheritanceData.critChanceInheritance, 1, 2);
					//shopy += 20;
				}
				if (inheritanceData.attackSpeedInheritance != 0) {
					DrawStat("AtSpd", inheritanceData.attackSpeedInheritance, 1, 3);
					//shopy += 20;
				}
				if (inheritanceData.armorPenInheritance != 0) {
					DrawStat("  Pen", inheritanceData.armorPenInheritance, 1, 4);
					//shopy += 20;
				}
				shopy += 20;
				shopx += 100;
			}

			void DrawStat(string name, float value, float defaultValue, int pos) {
				spriteBatch.DrawString(font, $"{name}: {value,5:##0.00}", new Vector2(shopx + 100 + pos * 100, shopy), GetColorFromBonus(value, defaultValue));
			}
		}

		private Color GetColorFromBonus(float additive, float defaultValue) {
			if (additive > defaultValue)
				return Color.Green;
			if (additive < defaultValue)
				return Color.Red;
			return Color.White;
		}
	}
}
