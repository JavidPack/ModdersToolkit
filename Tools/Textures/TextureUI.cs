using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using ModdersToolkit.UIElements;
using System.Collections.Generic;
using Terraria.DataStructures;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System;

namespace ModdersToolkit.Tools.Textures
{
	class TextureUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public TextureUI(UserInterface userInterface)
		{
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

		static string filename = "ModdersToolkit_Texture.png";
		static string folder = Path.Combine(Main.SavePath, "Mods", "Cache");
		static string path = Path.Combine(folder, filename);

		static string ModSourcePath = Path.Combine(Main.SavePath, "Mod Sources");

		public override void OnInitialize()
		{
			mainPanel = new UIPanel();
			mainPanel.Left.Set(-350f, 1f);
			mainPanel.Top.Set(-620f, 1f);
			mainPanel.Width.Set(310f, 0f);
			mainPanel.Height.Set(520f, 0f);
			mainPanel.SetPadding(12);
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

			watchModSources = new UICheckbox("Watch Mod Sources", "Automatically Watch Mod Sources for Changes", false);
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

			UIHoverImageButton editImageButton = new UIHoverImageButton(ModdersToolkit.Instance.GetTexture("UIElements/eyedropper"), "Open Exported Image in Default Editor");
			editImageButton.OnClick += EditImageButton_OnClick;
			editImageButton.Top.Set(top + 5, 0f);
			editImageButton.Left.Set(0, 0f);
			mainPanel.Append(editImageButton);

			updateneeded = true;
			Append(mainPanel);
		}

		internal FileSystemWatcher modSourcesWatcher;
		private void WatchModSources_OnSelectedChanged()
		{
			if (modSourcesWatcher != null)
			{
				modSourcesWatcher.Dispose();
			}

			if (watchModSources.Selected)
			{
				if (!Directory.Exists(ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name))
				{
					Main.NewText("Error somehow");
					return;
				}

				modSourcesWatcher = new FileSystemWatcher();
				modSourcesWatcher.Path = ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
				/* Watch for changes in LastAccess and LastWrite times, and
				   the renaming of files or directories. */
				modSourcesWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
				// Only watch text files.
				modSourcesWatcher.Filter = "*.png";
				modSourcesWatcher.IncludeSubdirectories = true;

				// Add event handlers.
				modSourcesWatcher.Changed += (a, b) =>
				{
					watchedFileChangedSources = true;
					watcherCooldownSources = defaultWatcherCooldown;
					watchedFileChangedSourcesFileName = b.FullPath;
				};
				modSourcesWatcher.Renamed += (a, b) =>
				{
					watchedFileChangedSources = true;
					watcherCooldownSources = defaultWatcherCooldown;
					watchedFileChangedSourcesFileName = b.FullPath;
				};

				// Begin watching.
				modSourcesWatcher.EnableRaisingEvents = true;
			}
		}

		//private void ExportImageButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		private void EditImageButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (selectedTexture2D == null)
			{
				Main.NewText("No texture selected");
			}
			else
			{
				Process.Start(path);
			}
		}

		private void SelectedTexture2DChanged()
		{
			if (selectedTexture2D == null)
			{
				Main.NewText("No texture selected");
			}
			else
			{
				using (Stream stream = File.OpenWrite(path))
				{
					selectedTexture2D.SaveAsPng(stream, selectedTexture2D.Width, selectedTexture2D.Height);
				}
				//if (!File.Exists(path))
				//{
				//	File.WriteAllText(path, "// Write code statements here");
				//}
				if (watcher != null)
				{
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

				Process.Start(folder);
			}
		}

		private void OnWatchedFileRenamed(object sender, RenamedEventArgs e)
		{
			watchedFileChanged = true;
			watcherCooldown = defaultWatcherCooldown;
		}

		private void OnWatchedFileChanged(object sender, FileSystemEventArgs e)
		{
			watchedFileChanged = true;
			watcherCooldown = defaultWatcherCooldown;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (watchedFileChanged)
			{
				if (watcherCooldown > 0)
					watcherCooldown--;
				if (watcherCooldown == 0)
				{
					Texture2D file;
					using (Stream stream = File.OpenRead(path))
					{
						file = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
					}
					if (file.Width != selectedTexture2D.Width || file.Height != selectedTexture2D.Height)
					{
						Main.NewText("Width or height changed, aborting.");
					}
					else
					{
						Color[] c = new Color[file.Width * file.Height];
						file.GetData<Color>(c, 0, c.Length);
						selectedTexture2D.SetData<Color>(c);
						Main.NewText("Texture updated from watch file.");
					}
					watchedFileChanged = false;
				}
			}

			if (watchedFileChangedSources)
			{
				if (watcherCooldownSources > 0)
					watcherCooldownSources--;
				if (watcherCooldownSources == 0)
				{
					watchedFileChangedSources = false;
					string dir = ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
					string innername = watchedFileChangedSourcesFileName.Substring(dir.Length);
					string innernamenoext = System.IO.Path.ChangeExtension(innername, null);
					innernamenoext = innernamenoext.Replace("\\", "/");

					FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
					var textures = (Dictionary<string, Texture2D>)texturesField.GetValue(selectedMod);

					if (!textures.ContainsKey(innernamenoext))
					{
						Main.NewText("Detected png change, but file not present at load: " + innername);
						return;
					}

					Texture2D modTexture = textures[innernamenoext];

					Texture2D file;
					using (Stream stream = File.OpenRead(watchedFileChangedSourcesFileName))
					{
						file = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
					}

					if (file.Width != modTexture.Width || file.Height != modTexture.Height)
					{
						Main.NewText("Width or height changed, aborting.");
					}
					else
					{
						Color[] c = new Color[file.Width * file.Height];
						file.GetData<Color>(c, 0, c.Length);
						modTexture.SetData<Color>(c);
						Main.NewText("Texture updated from watch mod sources folder.");
					}
				}
			}

			if (!updateneeded) { return; }
			updateneeded = false;

			if (modList.Count == 0)
			{
				foreach (var otherMod in ModLoader.Mods)
				{
					UITextPanel<string> button = new UITextPanel<string>(otherMod.DisplayName);
					button.OnClick += (a, b) =>
					{
						selectedMod = otherMod;
						updateneeded = true;
						watchModSources.Selected = false;
						watchModSources.Clickable = Directory.Exists(ModSourcePath + Path.DirectorySeparatorChar + otherMod.Name);
						if(watchModSources.Clickable)
							watchModSources.Selected = true;
					};
					modList.Add(button);
				}
			}

			if (selectedMod != null)
			{
				textureList.Clear();
				FieldInfo texturesField = typeof(Mod).GetField("textures", BindingFlags.Instance | BindingFlags.NonPublic);
				var textures = (Dictionary<string, Texture2D>)texturesField.GetValue(selectedMod);
				foreach (var textureEntry in textures)
				{
					if (searchFilter.Text.Length > 0)
					{
						if (textureEntry.Key.ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) == -1)
							continue;
					}
					UIImage image = new UIHoverImage(textureEntry.Value, textureEntry.Key);
					//image.ImageScale = Math.Min(1f, 60f/textureEntry.Value.Height);
					image.OnClick += (a, b) =>
					{
						selectedTexture2D = textureEntry.Value;
						currentTexture.SetText("Current: " + textureEntry.Key);
						SelectedTexture2DChanged();
					};
					textureList.Add(image);
				}
			}
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
