using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Textures
{
	internal class TextureUI : UIToolState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public TextureUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		public UIList modList;
		public UIList textureList;
		public UIText currentTexture;
		public Mod selectedMod;
		private Texture2D selectedTexture2D;

		internal FileSystemWatcher watcher;
		internal const int defaultWatcherCooldown = 15;
		internal int watcherCooldown;
		internal int watcherCooldownSources;
		internal bool watchedFileChanged;
		internal bool watchedFileChangedSources;
		internal string watchedFileChangedSourcesFileName;
		public bool updateneeded;

		internal NewUITextBox searchFilter;
		private UICheckbox watchModSources;

		private static string filename = "ModdersToolkit_Texture.png";
		private static string folder = Path.Combine(Main.SavePath, "Mods", "Cache");
		private static string path = Path.Combine(folder, filename);

		public override void OnInitialize() {
			base.OnInitialize();
			mainPanel = new UIPanel();
			mouseAndScrollBlockers.Add(mainPanel);
			width = 310;
			height = 520;
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = Color.PeachPuff * 0.8f;

			int top = 0;
			UIText text = new UIText("Textures:", 0.85f);
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

			watchModSources = new UICheckbox("Watch ModSources", "Automatically Watch ModSources for Changes", false);
			watchModSources.Top.Set(top, 0);
			watchModSources.OnSelectedChanged += WatchModSources_OnSelectedChanged;
			mainPanel.Append(watchModSources);

			top += 20;

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

			currentTexture = new UIText("Current: None selected", 0.85f);
			currentTexture.Top.Set(top, 0f);
			currentTexture.Left.Set(0, 0f);
			currentTexture.Width.Set(0, 1);
			currentTexture.HAlign = 0;
			mainPanel.Append(currentTexture);

			top += 20;

			textureList = new UIList();
			textureList.Top.Pixels = top;
			textureList.Width.Set(-25f, 1f);
			textureList.Height.Set(250, 0f);
			textureList.ListPadding = 6f;
			mainPanel.Append(textureList);

			var textureListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			textureListScrollbar.SetView(100f, 1000f);
			textureListScrollbar.Top.Pixels = top + 10;// + spacing;
			textureListScrollbar.Height.Set(230, 0f);
			textureListScrollbar.HAlign = 1f;
			mainPanel.Append(textureListScrollbar);
			textureList.SetScrollbar(textureListScrollbar);

			top += 250;

			//UIHoverImageButton exportImageButton = new UIHoverImageButton(ModdersToolkit.instance.GetTexture("UIElements/CopyCodeButton"), "Export Image for Editing");
			//exportImageButton.OnClick += ExportImageButton_OnClick;
			//exportImageButton.Top.Set(top + 5, 0f);
			//exportImageButton.Left.Set(0, 0f);
			//mainPanel.Append(exportImageButton);

			UIHoverImageButton editImageButton = new UIHoverImageButton(ModdersToolkit.Instance.Assets.Request<Texture2D>("UIElements/eyedropper", ReLogic.Content.AssetRequestMode.ImmediateLoad), "Open Exported Image in Default Editor");
			editImageButton.OnLeftClick += EditImageButton_OnClick;
			editImageButton.Top.Set(top + 5, 0f);
			editImageButton.Left.Set(0, 0f);
			mainPanel.Append(editImageButton);

			updateneeded = true;
			AdjustMainPanelDimensions(mainPanel);
			Append(mainPanel);
		}

		internal FileSystemWatcher modSourcesWatcher;
		private void WatchModSources_OnSelectedChanged() {
			if (modSourcesWatcher != null) {
				modSourcesWatcher.Dispose();
			}

			if (watchModSources.Selected) {
				if (!Directory.Exists(ModdersToolkit.ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name)) {
					Main.NewText("Error somehow");
					return;
				}

				modSourcesWatcher = new FileSystemWatcher();
				modSourcesWatcher.Path = ModdersToolkit.ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
				/* Watch for changes in LastAccess and LastWrite times, and
				   the renaming of files or directories. */
				modSourcesWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
				// Only watch text files.
				modSourcesWatcher.Filter = "*.png";
				modSourcesWatcher.IncludeSubdirectories = true;

				// Add event handlers.
				modSourcesWatcher.Changed += (a, b) => {
					watchedFileChangedSources = true;
					watcherCooldownSources = defaultWatcherCooldown;
					watchedFileChangedSourcesFileName = b.FullPath;
				};
				modSourcesWatcher.Renamed += (a, b) => {
					watchedFileChangedSources = true;
					watcherCooldownSources = defaultWatcherCooldown;
					watchedFileChangedSourcesFileName = b.FullPath;
				};

				// Begin watching.
				modSourcesWatcher.EnableRaisingEvents = true;
			}
		}

		//private void ExportImageButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		private void EditImageButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if (selectedTexture2D == null) {
				Main.NewText("No texture selected");
			}
			else {
				//Process.Start(path);
				Process.Start(
					new ProcessStartInfo(path) {
						UseShellExecute = true
					}
				);
			}
		}

		private void SelectedTexture2DChanged() {
			if (selectedTexture2D == null) {
				Main.NewText("No texture selected");
			}
			else {
				Utils.TryCreatingDirectory(folder);
				using (Stream stream = File.OpenWrite(path)) {
					selectedTexture2D.SaveAsPng(stream, selectedTexture2D.Width, selectedTexture2D.Height);
				}
				//if (!File.Exists(path))
				//{
				//	File.WriteAllText(path, "// Write code statements here");
				//}
				if (watcher != null) {
					watcher.Dispose();
				}

				// Create a new FileSystemWatcher and set its properties.
				watcher = new FileSystemWatcher();
				watcher.Path = folder;
				/* Watch for changes in LastAccess and LastWrite times, and
				   the renaming of files or directories. */
				watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName /* | NotifyFilters.Size*/;
				// Only watch text files.
				watcher.Filter = filename;

				// Add event handlers.
				watcher.Changed += new FileSystemEventHandler(OnWatchedFileChanged);
				watcher.Renamed += new RenamedEventHandler(OnWatchedFileRenamed); // photoshop

				// Begin watching.
				watcher.EnableRaisingEvents = true;

				Utils.OpenFolder(folder);
			}
		}

		private void OnWatchedFileRenamed(object sender, RenamedEventArgs e) {
			watchedFileChanged = true;
			watcherCooldown = defaultWatcherCooldown;
		}

		private void OnWatchedFileChanged(object sender, FileSystemEventArgs e) {
			watchedFileChanged = true;
			watcherCooldown = defaultWatcherCooldown;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (watchedFileChanged) {
				if (watcherCooldown > 0)
					watcherCooldown--;
				if (watcherCooldown == 0) {
					Texture2D file;
					using (Stream stream = File.OpenRead(path)) {
						file = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
					}
					if (file.Width != selectedTexture2D.Width || file.Height != selectedTexture2D.Height) {
						Main.NewText("Width or height changed, aborting.");
					}
					else {
						Color[] c = new Color[file.Width * file.Height];
						file.GetData<Color>(c, 0, c.Length);
						selectedTexture2D.SetData<Color>(c);
						Main.NewText("Texture updated from watch file.");
						Main.instance.TilePaintSystem.Reset(); // should reset tile paints. TODO, could be more efficient, check if tile texture.
					}
					watchedFileChanged = false;
				}
			}

			if (watchedFileChangedSources) {
				if (watcherCooldownSources > 0)
					watcherCooldownSources--;
				if (watcherCooldownSources == 0) {
					watchedFileChangedSources = false;
					string dir = ModdersToolkit.ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
					string innername = watchedFileChangedSourcesFileName.Substring(dir.Length);
					string innernamenoext = System.IO.Path.ChangeExtension(innername, null);
					//innernamenoext = innernamenoext.Replace("\\", "/");

					var textures = selectedMod.Assets.GetLoadedAssets().Where(x => x is Asset<Texture2D>).Cast<Asset<Texture2D>>();
					//FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
					//var textures = (Dictionary<string, Texture2D>)texturesField.GetValue(selectedMod);

					if (!textures.Any(x => x.Name == innernamenoext)) {
						Main.NewText("Detected png change, but file not present at load: " + innername);
						return;
					}

					Texture2D modTexture = textures.First(x => x.Name == innernamenoext).Value;

					Texture2D file;
					using (Stream stream = File.OpenRead(watchedFileChangedSourcesFileName)) {
						file = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
					}

					if (file.Width != modTexture.Width || file.Height != modTexture.Height) {
						Main.NewText("Width or height changed, aborting.");
					}
					else {
						Color[] c = new Color[file.Width * file.Height];
						file.GetData<Color>(c, 0, c.Length);
						modTexture.SetData<Color>(c);
						Main.NewText("Texture updated from watch ModSources folder: " + selectedMod.Name + Path.DirectorySeparatorChar + innername);
						Main.instance.TilePaintSystem.Reset(); // should reset tile paints. TODO, could be more efficient, check if tile texture.
					}
				}
			}

			if (!updateneeded) { return; }
			updateneeded = false;

			if (modList.Count == 0) {
				foreach (var otherMod in ModLoader.Mods) {
					UITextPanel<string> button = new UITextPanel<string>(otherMod.DisplayName);
					button.OnLeftClick += (a, b) => {
						selectedMod = otherMod;
						updateneeded = true;
						watchModSources.Selected = false;
						watchModSources.Clickable = Directory.Exists(ModdersToolkit.ModSourcePath + Path.DirectorySeparatorChar + otherMod.Name);
						if (watchModSources.Clickable)
							watchModSources.Selected = true;
					};
					modList.Add(button);
				}
			}

			if (selectedMod != null) {
				textureList.Clear();
				var textures = selectedMod.Assets.GetLoadedAssets().Where(x => x is Asset<Texture2D>).Cast<Asset<Texture2D>>();

				//FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
				//var textures = (Dictionary<string, Texture2D>)texturesField.GetValue(selectedMod);
				foreach (var textureEntry in textures) {
					if (searchFilter.Text.Length > 0) {
						if (textureEntry.Name.ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) == -1)
							continue;
					}
					UIImage image = new UIHoverImage(textureEntry.Value, textureEntry.Name);
					//image.ImageScale = Math.Min(1f, 60f/textureEntry.Value.Height);
					image.OnLeftClick += (a, b) => {
						selectedTexture2D = textureEntry.Value;
						currentTexture.SetText("Current: " + textureEntry.Name);
						SelectedTexture2DChanged();
					};
					textureList.Add(image);
				}
			}
		}
	}
}
