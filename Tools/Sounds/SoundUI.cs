using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using ReLogic.Content;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Sounds
{
	internal class SoundUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public SoundUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public UIList modList;
		public UIList soundList;
		public UIText currentSound;
		public Mod selectedMod;
		private (string modname, Asset<SoundEffect> sound)? selectedSoundEffectAsset;

		private UIFloatRangedDataValue volumeProperty;
		private UIFloatRangedDataValue pitchProperty;
		private UIFloatRangedDataValue pitchVarianceProperty;

		public bool updateneeded;

		internal NewUITextBox searchFilter;

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			width = 310;
			height = 580;
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = Color.Thistle * 0.8f;

			int top = 0;
			UIText text = new UIText("Sounds:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);
			top += 20;

			modList = new UIList();
			modList.Top.Pixels = top;
			modList.Width.Set(-25f, 1f);
			modList.Height.Set(140, 0f);
			modList.ListPadding = 0f;
			mainPanel.Append(modList);

			var modListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			modListScrollbar.SetView(100f, 1000f);
			modListScrollbar.Top.Pixels = top + 10;// + spacing;
			modListScrollbar.Height.Set(120, 0f);
			modListScrollbar.HAlign = 1f;
			mainPanel.Append(modListScrollbar);
			modList.SetScrollbar(modListScrollbar);

			top += 140;

			UIText text2 = new UIText("Filter:", 0.85f);
			text2.Top.Set(top, 0f);
			mainPanel.Append(text2);

			searchFilter = new NewUITextBox("Search", 0.85f);
			searchFilter.SetPadding(0);
			searchFilter.OnTextChanged += () => { updateneeded = true; };
			searchFilter.Top.Set(top, 0f);
			searchFilter.Left.Set(text2.GetInnerDimensions().Width + 6, 0f);
			searchFilter.Width.Set(-(text2.GetInnerDimensions().Width + 6), 1f);
			searchFilter.Height.Set(20, 0f);
			mainPanel.Append(searchFilter);

			top += 20;

			currentSound = new UIText("Current: None selected", 0.85f);
			currentSound.Top.Set(top, 0f);
			currentSound.Left.Set(0, 0f);
			currentSound.Width.Set(0, 1);
			currentSound.HAlign = 0;
			mainPanel.Append(currentSound);

			top += 20;

			soundList = new UIList();
			soundList.Top.Pixels = top;
			soundList.Width.Set(-25f, 1f);
			soundList.Height.Set(250, 0f);
			soundList.ListPadding = 6f;
			mainPanel.Append(soundList);

			var soundListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			soundListScrollbar.SetView(100f, 1000f);
			soundListScrollbar.Top.Pixels = top + 10;// + spacing;
			soundListScrollbar.Height.Set(230, 0f);
			soundListScrollbar.HAlign = 1f;
			mainPanel.Append(soundListScrollbar);
			soundList.SetScrollbar(soundListScrollbar);

			top += 250;

			var uiRanges = new List<UIElement>();

			volumeProperty = new UIFloatRangedDataValue("Volume:", 1f, 0, 1f);
			uiRanges.Add(new UIRange<float>(volumeProperty));

			pitchProperty = new UIFloatRangedDataValue("Pitch:", 0, -1, 1);
			uiRanges.Add(new UIRange<float>(pitchProperty));

			pitchVarianceProperty = new UIFloatRangedDataValue("Pitch Variance:", 0, 0, 2);
			uiRanges.Add(new UIRange<float>(pitchVarianceProperty));

			foreach (var uiRange in uiRanges) {
				uiRange.Top.Set(top, 0f);
				uiRange.Width.Set(0, 1f);
				mainPanel.Append(uiRange);
				top += 22;
			}

			UIHoverImageButton playSoundButton = new UIHoverImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay", AssetRequestMode.ImmediateLoad), "Play Selected Sound");
			playSoundButton.OnClick += PlaySoundButton_OnClick;
			playSoundButton.Top.Set(top + 5, 0f);
			playSoundButton.Left.Set(0, 0f);
			playSoundButton.SetVisibility(1f, 0.8f);
			mainPanel.Append(playSoundButton);

			UIHoverImageButton copyCodeButton = new UIHoverImageButton(ModdersToolkit.Instance.Assets.Request<Texture2D>("UIElements/CopyCodeButton", AssetRequestMode.ImmediateLoad), "Copy code to clipboard");
			copyCodeButton.OnClick += CopyCodeButton_OnClick;
			copyCodeButton.CopyStyle(playSoundButton);
			copyCodeButton.Left.Set(24, 0f);
			copyCodeButton.SetVisibility(1f, 0.8f);
			mainPanel.Append(copyCodeButton);

			UICheckbox logSoundsCheckbox = new UICheckbox("Log Sounds", "Log Sound Styles and Types");
			logSoundsCheckbox.OnSelectedChanged += () => SoundTool.logSounds = logSoundsCheckbox.Selected;
			AppendToAndAdjustWidthHeight(mainPanel, logSoundsCheckbox, ref height, ref width);

			updateneeded = true;
			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		private void PlaySoundButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if (!selectedSoundEffectAsset.HasValue) {
				Main.NewText("No sound selected");
			}
			else {
				string modname = selectedSoundEffectAsset.Value.modname == "ModLoader" ? "Terraria" : selectedSoundEffectAsset.Value.modname;
				var style = new SoundStyle(modname + "/" + selectedSoundEffectAsset.Value.sound.Name/*.Replace("\\", "/")*/);
				style = style with { Volume = volumeProperty.Data, PitchVariance = pitchVarianceProperty.Data, IsLooped = false, Pitch = pitchProperty.Data , };
				SoundEngine.PlaySound(style);
			}
		}

		private void CopyCodeButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if (!selectedSoundEffectAsset.HasValue) {
				Main.NewText("No sound selected");
				return;
			}
			float PitchVariance = pitchVarianceProperty.Data;
			float Pitch = pitchProperty.Data;
			float Volume = volumeProperty.Data;

			bool needWith = PitchVariance != 0 || Pitch != 0 || Volume != 1;

			StringBuilder s = new StringBuilder();
			string modname = selectedSoundEffectAsset.Value.modname == "ModLoader" ? "Terraria" : selectedSoundEffectAsset.Value.modname;
			s.Append($"SoundStyle style = new SoundStyle(\"{modname + "/" + selectedSoundEffectAsset.Value.sound.Name.Replace("\\","/")}\")");
			if (needWith) {
				s.Append(" with {");
				if (Volume != 1) {
					s.Append($" Volume = {Volume:#.##}f, ");
				}
				if (Pitch != 0) {
					s.Append($" Pitch = {Pitch:#.##}f, ");
				}
				if (PitchVariance != 0) {
					s.Append($" PitchVariance = {PitchVariance:#.##}f, ");
				}
				s.Append("}");
			}
			s.Append(";");
			s.Append(Environment.NewLine);
			s.Append($"SoundEngine.PlaySound(style);");

			Platform.Get<IClipboard>().Value = s.ToString();

			Main.NewText("Copied sound playing code to clipboard");
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (!updateneeded) { return; }
			updateneeded = false;

			if (modList.Count == 0) {
				foreach (var otherMod in ModLoader.Mods) {
					UITextPanel<string> button = new UITextPanel<string>(otherMod.DisplayName);
					button.OnClick += (a, b) => {
						selectedMod = otherMod;
						updateneeded = true;
					};
					modList.Add(button);
				}
			}

			if (selectedMod != null) {
				soundList.Clear();
				// TODO: only loads loaded assets, need button to attempt to load all? show unloaded? ShaderUI shows that approach.
				AssetRepository assets = selectedMod.Assets;
				if (selectedMod.Name == "ModLoader")
					assets = Main.Assets as AssetRepository;
				var sounds = assets.GetLoadedAssets().Where(x => x is Asset<SoundEffect>).Cast<Asset<SoundEffect>>();

				foreach (var soundEntry in sounds) {
					if (searchFilter.Text.Length > 0) {
						if (soundEntry.Name.ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) == -1)
							continue;
					}
					UITextPanel<string> button = new UITextPanel<string>(soundEntry.Name, 0.8f);
					button.OnClick += (a, b) => {
						selectedSoundEffectAsset = (selectedMod.Name, soundEntry);
						currentSound.SetText("Current: " + soundEntry.Name);
						updateneeded = true;
					};
					soundList.Add(button);
				}
				if(soundList.Count == 0) {
					soundList.Add(new UITextPanel<string>($"No sounds found in {selectedMod.DisplayName}", 0.8f));
					soundList.Add(new UITextPanel<string>($"Only already loaded sounds show up", 0.8f));
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
