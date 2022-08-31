using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.DamageClasses
{
	class DamageClassesUI : UIToolState
	{
		internal UIPanel mainPanel;
		public UIList damageClassList;
		public bool updateNeeded;

		private UserInterface userInterface;
		public DamageClassesUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public override void OnInitialize() {
			base.OnInitialize();
			width = 560;
			mainPanel = new UIPanel();
			mouseAndScrollBlockers.Add(mainPanel);
			mainPanel.SetPadding(6);
			mainPanel.MinWidth.Set(width, 0);
			mainPanel.BackgroundColor = Color.Honeydew;

			UIText text = new UIText("Damage Classes:", 0.85f);
			AppendToAndAdjustWidthHeight(mainPanel, text, ref height, ref width);
			//mainPanel.Append(text);
			
			damageClassList = new UIList();
			damageClassList.MinHeight.Set(500, 0f);
			damageClassList.Width.Set(-25, 1f);
			damageClassList.ListPadding = 6f;

			updateNeeded = true;

			var damageClassListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			damageClassListScrollbar.SetView(100f, 1000f);
			damageClassListScrollbar.Top.Pixels = height;// + spacing;
			damageClassListScrollbar.Height.Set(-height /*- spacing*/, 1f);
			damageClassListScrollbar.HAlign = 1f;
			mainPanel.Append(damageClassListScrollbar);
			damageClassList.SetScrollbar(damageClassListScrollbar);
			AppendToAndAdjustWidthHeight(mainPanel, damageClassList, ref height, ref width);
			// minheight and minwidth have to be set for this to work.
			// If adjusting dynamically, mainpanel size not determined yet.

			AdjustMainPanelDimensions(mainPanel);
			mainPanel.Left.Pixels -= 180;
			Append(mainPanel);
			//AppendToAndAdjustWidthHeight(mainPanel, damageClassList, ref height, ref width);
		}

		internal void UpdateList() {
			if (!updateNeeded) { return; }
			updateNeeded = false;

			damageClassList.Clear();
			// var damageClasses = ModContent.GetContent<DamageClass>(); only retrieves modded damage classes...

			List<DamageClass> damageClasses = (List<DamageClass>)typeof(DamageClassLoader).GetField("DamageClasses", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

			foreach (var damageClass in damageClasses) {
				var inheritors = new List<(DamageClass, StatInheritanceData)>();
				foreach (var otherDamageClass in damageClasses) {
					if (damageClass != otherDamageClass) {
						StatInheritanceData inheritanceData = damageClass.GetModifierInheritance(otherDamageClass);
						if (!inheritanceData.Equals(StatInheritanceData.None))
							inheritors.Add((otherDamageClass, inheritanceData));
					}
				}

				damageClassList.Add(new UIDamageClass(damageClass, inheritors));
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			UpdateList();
		}
	}
}
