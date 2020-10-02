using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ModdersToolkit.UIElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModdersToolkit.Tools.Shaders
{
	internal class ShaderUI : UIState
	{
		internal UIPanel mainPanel;
		private UserInterface userInterface;
		public ShaderUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		// TODOS:
		// Parameters panels?
		// Select specific .fx from mod to test?
		// More shader templates (screen, armor, others?)
		// Automatically apply shader via checkbox, rather than Acid Dye
		// Test New shaders without binding to anything, Select pass name?

		private static string cacheFolder = Path.Combine(Main.SavePath, "Mods", "Cache");
		private static string shaderFilename = "ModdersToolkit_Shader.fx";
		private static string shaderFilePath = Path.Combine(cacheFolder, shaderFilename);
		private static string ModSourcePath = Path.Combine(Main.SavePath, "Mod Sources");

		private static string exeFilename = "fxcompiler.exe";
		private static string exePath = Path.Combine(cacheFolder, exeFilename);

		private static string exeVersionFileName = "ModdersToolkitFXBuilderVersion.txt";
		private static string exeVersionPath = Path.Combine(cacheFolder, exeVersionFileName);

		public bool updateneeded;

		public UIList modList;
		internal NewUITextBox searchFilter;
		public UIText currentShader;
		public UIList shaderList;
		private UICheckbox watchTestShaderCheckbox;
		private UICheckbox watchModSourcesCheckbox;
		internal UICheckbox forceShaderCheckbox;

		private Dusts.ColorDataRangeProperty colorDataProperty;
		private UIFloatRangedDataValue intensityData;

		private UIRadioButton armorShaderRadioButton;
		private UIRadioButton screenShaderRadioButton;

		public Mod selectedMod;
		//private Effect selectedShader;
		internal bool lastShaderIsScreenShader;

		internal FileSystemWatcher quickShaderWatcher;
		internal FileSystemWatcher modSourcesWatcher;
		internal const int defaultWatcherCooldown = 15;
		internal int quickShaderWatcherCooldown;
		internal int modSourcesWatcherCooldown;
		internal HashSet<string> watchedFileChangedSourcesFileNames = new HashSet<string>();

		public override void OnInitialize() {
			mainPanel = new UIPanel();
			mainPanel.Left.Set(-350f, 1f);
			mainPanel.Top.Set(-740f, 1f);
			mainPanel.Width.Set(310f, 0f);
			mainPanel.Height.Set(640f, 0f);
			mainPanel.SetPadding(6);
			mainPanel.BackgroundColor = Color.PeachPuff * 0.8f;

			int top = 0;
			UIText text = new UIText("Shaders:", 0.85f);
			text.Top.Set(top, 0f);
			mainPanel.Append(text);
			top += 20;

			int topModSourcesPanel = 0;
			var modSourcesPanel = new UIPanel();
			modSourcesPanel.Top.Set(top, 0f);
			modSourcesPanel.Width.Set(0, 1f);
			modSourcesPanel.Height.Set(370f, 0f);
			modSourcesPanel.SetPadding(6);
			modSourcesPanel.BackgroundColor = Color.Blue * 0.7f;
			top += 376;

			text = new UIText("Mod Sources:", 0.85f);
			text.Top.Set(topModSourcesPanel, 0f);
			modSourcesPanel.Append(text);
			topModSourcesPanel += 20;

			var modListBackPanel = new UIPanel();
			modListBackPanel.Top.Set(topModSourcesPanel, 0);
			modListBackPanel.Width.Set(0, 1f);
			modListBackPanel.Height.Set(120f, 0f);
			modListBackPanel.SetPadding(6);
			modListBackPanel.BackgroundColor = Color.Green * 0.7f;

			modList = new UIList();
			modList.Width.Set(-25f, 1f);
			modList.Height.Set(0, 1f);
			modList.ListPadding = 0f;
			modListBackPanel.Append(modList);

			var modListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			modListScrollbar.SetView(100f, 1000f);
			modListScrollbar.Top.Pixels = 4;
			modListScrollbar.Height.Set(-8, 1f);
			modListScrollbar.HAlign = 1f;
			modListBackPanel.Append(modListScrollbar);
			modList.SetScrollbar(modListScrollbar);

			modSourcesPanel.Append(modListBackPanel);
			mainPanel.Append(modSourcesPanel);
			topModSourcesPanel += 130;

			text = new UIText("Filter:", 0.85f);
			text.Top.Set(topModSourcesPanel, 0f);
			modSourcesPanel.Append(text);

			searchFilter = new NewUITextBox("Search", 0.85f);
			searchFilter.SetPadding(0);
			searchFilter.OnTextChanged += () => { updateneeded = true; };
			searchFilter.Top.Set(topModSourcesPanel, 0f);
			searchFilter.Left.Set(text.GetInnerDimensions().Width + 6, 0f);
			searchFilter.Width.Set(-(text.GetInnerDimensions().Width + 6), 1f);
			searchFilter.Height.Set(20, 0f);
			modSourcesPanel.Append(searchFilter);
			topModSourcesPanel += 26;

			currentShader = new UIText("Current: None selected", 0.85f);
			currentShader.Top.Set(topModSourcesPanel, 0f);
			currentShader.Left.Set(0, 0f);
			currentShader.Width.Set(0, 1);
			currentShader.HAlign = 0;
			modSourcesPanel.Append(currentShader);
			topModSourcesPanel += 20;

			var shaderListBackPanel = new UIPanel();
			shaderListBackPanel.Top.Set(topModSourcesPanel, 0);
			shaderListBackPanel.Width.Set(0, 1f);
			shaderListBackPanel.Height.Set(140f, 0f);
			shaderListBackPanel.SetPadding(6);
			shaderListBackPanel.BackgroundColor = Color.Green * 0.7f;

			shaderList = new UIList();
			shaderList.Width.Set(-25f, 1f);
			shaderList.Height.Set(0, 1f);
			shaderList.ListPadding = 0f;
			shaderListBackPanel.Append(shaderList);

			var shaderListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
			shaderListScrollbar.SetView(100f, 1000f);
			shaderListScrollbar.Top.Pixels = 4;
			shaderListScrollbar.Height.Set(-8, 1f);
			shaderListScrollbar.HAlign = 1f;
			shaderListBackPanel.Append(shaderListScrollbar);
			shaderList.SetScrollbar(shaderListScrollbar);
			modSourcesPanel.Append(shaderListBackPanel);

			topModSourcesPanel += 146;
			watchModSourcesCheckbox = new UICheckbox("Watch Mod Sources", "Automatically Compile Mod Sources Shaders when Changed on Disk", false);
			watchModSourcesCheckbox.Top.Set(topModSourcesPanel, 0);
			watchModSourcesCheckbox.OnSelectedChanged += WatchModSources_OnSelectedChanged;
			modSourcesPanel.Append(watchModSourcesCheckbox);
			topModSourcesPanel += 20;


			int topTestShaderPanel = 0;
			var testShaderPanel = new UIPanel();
			testShaderPanel.Top.Set(top, 0f);
			testShaderPanel.Width.Set(0, 1f);
			testShaderPanel.Height.Set(230f, 0f);
			testShaderPanel.SetPadding(6);
			testShaderPanel.BackgroundColor = Color.LightBlue * 0.7f;
			top += 230;
			mainPanel.Append(testShaderPanel);

			text = new UIText("Test Shader:", 0.85f);
			text.Top.Set(topTestShaderPanel, 0f);
			testShaderPanel.Append(text);
			topTestShaderPanel += 20;

			// Current Working Shader
			// Status Text: "Compiled .fx"
			// Filter?
			// Checkbox: Force

			UIRadioButtonGroup g = new UIRadioButtonGroup();
			armorShaderRadioButton = new UIRadioButton("Armor", "Generate an Armor Shader");
			screenShaderRadioButton = new UIRadioButton("Screen", "Generate a Screen Shader");
			g.Add(armorShaderRadioButton);
			g.Add(screenShaderRadioButton);
			g.Top.Pixels = topTestShaderPanel;
			g.Width.Set(0, .75f);
			testShaderPanel.Append(g);
			armorShaderRadioButton.Selected = true;
			topTestShaderPanel += (int)g.Height.Pixels;

			var createTestShaderButton = new UITextPanel<string>("Create/Recreate Test Shader");
			createTestShaderButton.OnClick += CreateTestShaderFileButton_OnClick;
			createTestShaderButton.Top.Set(topTestShaderPanel, 0f);
			createTestShaderButton.Left.Set(0, 0f);
			createTestShaderButton.SetPadding(4);
			testShaderPanel.Append(createTestShaderButton);
			topTestShaderPanel += 26;

			var openTestShaderButton = new UITextPanel<string>("Edit Test Shader");
			openTestShaderButton.OnClick += OpenTestShaderFileButton_OnClick;
			openTestShaderButton.Top.Set(topTestShaderPanel, 0f);
			openTestShaderButton.Left.Set(0, 0f);
			openTestShaderButton.SetPadding(4);
			testShaderPanel.Append(openTestShaderButton);
			topTestShaderPanel += 26;

			var compileShaderButton = new UITextPanel<string>("Compile Test Shader");
			compileShaderButton.OnClick += CompileTestShaderButton_OnClick;
			compileShaderButton.Top.Set(topTestShaderPanel, 0f);
			compileShaderButton.Left.Set(0, 0f);
			compileShaderButton.SetPadding(4);
			testShaderPanel.Append(compileShaderButton);
			topTestShaderPanel += 26;

			topTestShaderPanel += 4;
			watchTestShaderCheckbox = new UICheckbox("Watch Test Shader File", "Automatically Compile Test Shader when Changed on Disk", true);
			watchTestShaderCheckbox.Top.Set(topTestShaderPanel, 0);
			watchTestShaderCheckbox.OnSelectedChanged += WatchTestShader_OnSelectedChanged;
			testShaderPanel.Append(watchTestShaderCheckbox);
			topTestShaderPanel += 20;

			forceShaderCheckbox = new UICheckbox("Force Shader Active", "Automatically enable the last compiled screen shader.", true);
			forceShaderCheckbox.Top.Set(topTestShaderPanel, 0);
			//forceShaderCheckbox.OnSelectedChanged += ForceShaderCheckbox_OnSelectedChanged;
			testShaderPanel.Append(forceShaderCheckbox);
			topTestShaderPanel += 20;

			var uiRanges = new List<UIElement>();

			colorDataProperty = new Dusts.ColorDataRangeProperty("uColor:");
			colorDataProperty.range.Top.Set(top, 0f);
			colorDataProperty.OnValueChanged += () => Filters.Scene["ModdersToolkit:TestScreenShader"].GetShader().UseColor(colorDataProperty.Data);
			uiRanges.Add(colorDataProperty.range);

			intensityData = new UIFloatRangedDataValue("uIntensity:", 1f, 0f, 1f);
			intensityData.DataGetter = () => Filters.Scene["ModdersToolkit:TestScreenShader"].GetShader().Intensity;
			intensityData.DataSetter = (value) => Filters.Scene["ModdersToolkit:TestScreenShader"].GetShader().UseIntensity(value);
			uiRanges.Add(new UIRange<float>(intensityData));

			//var uDirectionXProperty = new UIFloatRangedDataValue("uDirectionX:", 0, -1, 1);
			//uDirectionXProperty.DataGetter = () => Filters.Scene["ModdersToolkit:TestScreenShader"].GetShader().UseDirection.Intensity;
			//uDirectionXProperty.DataSetter = (value) => Filters.Scene["ModdersToolkit:TestScreenShader"].GetShader().UseDirection(value);
			//var uDirectionYProperty = new UIFloatRangedDataValue("uDirectionY:", 0, -1, 1);
			//var ui2DRange = new UI2DRange<float>(uDirectionXProperty, uDirectionYProperty);
			//uiRanges.Add(ui2DRange);
			//ui2DRange.Top.Set(top, 0f);
			//ui2DRange.Left.Set(200, 0f);
			//mainPanel.Append(ui2DRange);
			//top += 30;

			foreach (var uiRange in uiRanges) {
				uiRange.Top.Set(topTestShaderPanel, 0f);
				uiRange.Width.Set(0, 1f);
				testShaderPanel.Append(uiRange);
				topTestShaderPanel += 22;
			}

			updateneeded = true;
			Append(mainPanel);
		}

		private const string ArmorShaderTemplate = @"sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    return color * sampleColor;
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}";

		private const string ScreenShaderTemplate = @"sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    return color;
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}";

		private void CreateTestShaderFileButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			Directory.CreateDirectory(cacheFolder);
			File.WriteAllText(shaderFilePath, armorShaderRadioButton.Selected ? ArmorShaderTemplate : ScreenShaderTemplate);
		}

		private void OpenTestShaderFileButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			if (!File.Exists(shaderFilePath)) {
				Main.NewText($"Could not find {shaderFilename}, use the create/re-create test shader button first.");
				return;
			}
			AttemptOpenFxFile(shaderFilePath);
		}

		private static void AttemptOpenFxFile(string path) {
			try {
				Process.Start(path);
			}
			catch (Exception) {
				Main.NewText($"Could not open {path}, check that you have a text editor associated with .fx files.");
			}
		}

		private void CompileTestShaderButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
			// Ensure shader file exists
			if (!File.Exists(shaderFilePath)) {
				Main.NewText($"Could not find {shaderFilename}, check that you have first created and opened the file.");
				return;
			}

			RunShaderCompileCommand(shaderFilePath);
		}

		private const int fxcompilerVersion = 1;
		private bool fxCompilerUpToDate = false;
		private void RunShaderCompileCommand(string file) {
			// Ensure shader compiler exists
			if (File.Exists(exeVersionPath)) {
				if (int.TryParse(File.ReadAllText(exeVersionPath), out int installedVersion)) {
					fxCompilerUpToDate = installedVersion == fxcompilerVersion;
				}
			}

			if (!fxCompilerUpToDate) {
				const string
					LibReflectedBase = "libReflected",
					WCFXCompilerDLL = "wcfxcompiler.dll",
					EffectImporterDLL = "Microsoft.Xna.Framework.Content.Pipeline.EffectImporter.dll",
					PipelineDLL = "Microsoft.Xna.Framework.Content.Pipeline.dll";

				Directory.CreateDirectory(cacheFolder);
				ModdersToolkit.Instance.Logger.Info("Unpacking fxbuilder.exe and Pipeline dlls");
				File.WriteAllBytes(exePath, ModdersToolkit.Instance.GetFileBytes($"{LibReflectedBase}/{exeFilename}"));
				File.WriteAllBytes(Path.Combine(cacheFolder, WCFXCompilerDLL), ModdersToolkit.Instance.GetFileBytes($"{LibReflectedBase}/{WCFXCompilerDLL}"));
				File.WriteAllBytes(Path.Combine(cacheFolder, EffectImporterDLL), ModdersToolkit.Instance.GetFileBytes($"{LibReflectedBase}/{EffectImporterDLL}"));
				File.WriteAllBytes(Path.Combine(cacheFolder, PipelineDLL), ModdersToolkit.Instance.GetFileBytes($"{LibReflectedBase}/{PipelineDLL}"));
				File.WriteAllText(exeVersionPath, fxcompilerVersion.ToString());
				ModdersToolkit.Instance.Logger.Info("Unpacking complete");
				fxCompilerUpToDate = true;
			}

			// make async?
			Process process = new Process {
				StartInfo =
				{
					FileName = exePath,
					Arguments = $"\"{file}\"",
					WorkingDirectory = cacheFolder,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			};
			//	process.StartInfo.Arguments = "/c DIR"; // Note the /c command (*)
			process.Start();
			//* Read the output (or the error)
			string output = process.StandardOutput.ReadToEnd();
			Console.WriteLine(output);
			string err = process.StandardError.ReadToEnd();
			Console.WriteLine(err);

			Main.NewTextMultiline(output);

			process.WaitForExit();

			int exitCode = process.ExitCode;

			if (exitCode == 0) {
				Main.NewText("Success");
				lastShaderIsScreenShader = false;

				var myContentManager = new ContentManager(Main.instance.Content.ServiceProvider, Path.GetDirectoryName(file));
				string assetName = Path.GetFileNameWithoutExtension(file);
				var newEffect = myContentManager.Load<Effect>(assetName);
				Ref<Effect> newShaderRef = new Ref<Effect>(newEffect);
				bool shaderIsScreenShader = newEffect.Parameters.Any(x => x.Name == "uZoom");
				if (shaderIsScreenShader) {
					Main.NewText("Check Force Shader Active checkbox to test.");
					Filters.Scene["ModdersToolkit:TestScreenShader"].Active = false;
					Filters.Scene["ModdersToolkit:TestScreenShader"] = new Filter(new ScreenShaderData(newShaderRef, "ModdersToolkitShaderPass").UseImage("Images/Misc/Perlin").UseImage("Images/Misc/noise", 1).UseColor(colorDataProperty.Data).UseIntensity(intensityData.Data), EffectPriority.VeryHigh);
					lastShaderIsScreenShader = true;
				}

				FieldInfo shaderRefField = typeof(ShaderData).GetField("_shader", BindingFlags.Instance | BindingFlags.NonPublic);

				// If existing Effect, Update all Effect references
				if (file.StartsWith(ModSourcePath)) {
					FieldInfo modEffectsField = typeof(Mod).GetField("effects", BindingFlags.Instance | BindingFlags.NonPublic);
					var modEffects = (Dictionary<string, Effect>)modEffectsField.GetValue(selectedMod);
					var key = file.Substring(Path.Combine(ModSourcePath, selectedMod.Name).Length + 1).Replace("\\", "/");
					key = key.Substring(0, key.LastIndexOf(".fx"));
					Effect originalEffect = null;
					if (modEffects.ContainsKey(key)) {
						originalEffect = modEffects[key];
					}
					else {
						updateneeded = true;
					}
					modEffects[key] = newEffect;

					FieldInfo shaderDataField = typeof(ArmorShaderDataSet).GetField("_shaderData", BindingFlags.Instance | BindingFlags.NonPublic);
					var armorShaderDataList = (List<ArmorShaderData>)typeof(ArmorShaderDataSet).GetField("_shaderData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameShaders.Armor);
					var hairShaderDataList = (List<HairShaderData>)typeof(HairShaderDataSet).GetField("_shaderData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameShaders.Hair);
					var miscShaderDataList = GameShaders.Misc.Values.ToList();
					var filtersShaderDataList = ((Dictionary<string, Filter>)typeof(EffectManager<Filter>).GetField("_effects", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Filters.Scene)).Values.ToList().Select(x => (ScreenShaderData)typeof(Filter).GetField("_shader", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(x));

					int entriesFound = 0;
					var shaderData = armorShaderDataList.Cast<ShaderData>().Union(hairShaderDataList.Cast<ShaderData>()).Union(miscShaderDataList.Cast<ShaderData>()).Union(filtersShaderDataList.Cast<ShaderData>());
					foreach (var shaderDataShader in shaderData) {
						if (originalEffect != null && shaderDataShader.Shader == originalEffect) {
							entriesFound++;
							shaderRefField.SetValue(shaderDataShader, newShaderRef);
						}
					}
					Main.NewText(entriesFound > 0 ? $"{entriesFound} existing bound shader entries updated" : "No existing bound shader entries found");
				}
				else {
					// If we are compiling the Test/Quick Shader, instruct the modder on how to test
					if (!shaderIsScreenShader) {
						Main.NewText("Wear Acid Dye to test."); // TODO: Checkbox to force Armor/Hair/other dye

						// Need to replace Acid Dye shader and passname
						var shader = GameShaders.Armor.GetShaderFromItemId(ItemID.AcidDye);
						shaderRefField.SetValue(shader, newShaderRef);
						FieldInfo passNameField = typeof(ArmorShaderData).GetField("_passName", BindingFlags.Instance | BindingFlags.NonPublic);
						passNameField.SetValue(shader, "ModdersToolkitShaderPass");
					}
				}

			}
			else if (exitCode == 1) {
				Main.NewText("Failure");
			}
			else if (exitCode == 2) {
				Main.NewText("No File");
			}
			else if (exitCode == 3) {
				Main.NewText("File Not Found");
			}
		}

		private void WatchModSources_OnSelectedChanged() {
			if (modSourcesWatcher != null) {
				modSourcesWatcher.Dispose();
			}

			if (watchModSourcesCheckbox.Selected) {
				if (!Directory.Exists(ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name)) {
					Main.NewText("Error somehow");
					return;
				}

				modSourcesWatcher = new FileSystemWatcher();
				modSourcesWatcher.Path = ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
				/* Watch for changes in LastAccess and LastWrite times, and
				   the renaming of files or directories. */
				modSourcesWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
				// Only watch text files.
				modSourcesWatcher.Filter = "*.fx";
				modSourcesWatcher.IncludeSubdirectories = true;

				// Add event handlers.
				modSourcesWatcher.Changed += (a, b) => {
					//watchedFileChangedSources = true;
					modSourcesWatcherCooldown = defaultWatcherCooldown;
					//watchedFileChangedSourcesFileName = b.FullPath;
					watchedFileChangedSourcesFileNames.Add(b.FullPath);
					updateneeded = true;
				};
				modSourcesWatcher.Renamed += (a, b) => {
					//watchedFileChangedSources = true;
					modSourcesWatcherCooldown = defaultWatcherCooldown;
					//watchedFileChangedSourcesFileName = b.FullPath;
					watchedFileChangedSourcesFileNames.Add(b.FullPath);
					updateneeded = true;
				};

				// Begin watching.
				modSourcesWatcher.EnableRaisingEvents = true;
			}
		}

		// Watches the Quick/Test shader
		private void WatchTestShader_OnSelectedChanged() {
			quickShaderWatcher?.Dispose();

			if (!watchTestShaderCheckbox.Selected)
				return;

			Directory.CreateDirectory(cacheFolder);
			if (!Directory.Exists(cacheFolder)) {
				Main.NewText("Error somehow");
				return;
			}

			quickShaderWatcher = new FileSystemWatcher {
				Path = cacheFolder,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
				Filter = shaderFilename,
				IncludeSubdirectories = false
			};
			// ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
			/* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
			// Only watch text files.

			// Add event handlers.
			quickShaderWatcher.Changed += (a, b) => {
				if (b.FullPath.EndsWith("TMP"))
					return; // VisualStudio generates those upon edit/save.

				quickShaderWatcherCooldown = defaultWatcherCooldown;
				watchedFileChangedSourcesFileNames.Add(b.FullPath);
			};
			quickShaderWatcher.Renamed += (a, b) => {
				if (b.FullPath.EndsWith("TMP"))
					return; // VisualStudio generates those upon edit/save.

				quickShaderWatcherCooldown = defaultWatcherCooldown;
				watchedFileChangedSourcesFileNames.Add(b.FullPath);
			};

			// Begin watching.
			quickShaderWatcher.EnableRaisingEvents = true;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (watchedFileChangedSourcesFileNames.Count > 0) {
				if (quickShaderWatcherCooldown > 0)
					quickShaderWatcherCooldown--;

				if (quickShaderWatcherCooldown == 0) {
					foreach (var filename in watchedFileChangedSourcesFileNames) {
						Main.NewText($"Shader source changed for file {filename}, attempting to compile.");
						RunShaderCompileCommand(filename);
					}
					watchedFileChangedSourcesFileNames.Clear();
				}
			}

			if (!updateneeded)
				return;
			updateneeded = false;

			if (modList.Count == 0) {
				foreach (var otherMod in ModLoader.Mods) {
					if (otherMod.Name == "ModLoader")
						continue;

					UITextPanel<string> button = new UITextPanel<string>(otherMod.DisplayName);

					button.OnClick += (a, b) => {
						selectedMod = otherMod;
						updateneeded = true;
						watchModSourcesCheckbox.Selected = false;
						watchModSourcesCheckbox.Clickable = Directory.Exists(ModSourcePath + Path.DirectorySeparatorChar + otherMod.Name);

						if (watchModSourcesCheckbox.Clickable)
							watchModSourcesCheckbox.Selected = true;
					};

					modList.Add(button);
				}
			}

			shaderList.Clear();
			if (selectedMod != null && Directory.Exists(Path.Combine(ModSourcePath, selectedMod.Name))) {

				FieldInfo effectsField = typeof(Mod).GetField("effects", BindingFlags.Instance | BindingFlags.NonPublic);
				var loadedEffects = (Dictionary<string, Effect>)effectsField.GetValue(selectedMod);

				var fxFiles = Directory.EnumerateFiles(Path.Combine(ModSourcePath, selectedMod.Name), "*.fx", SearchOption.AllDirectories).Where(x => string.Equals(Path.GetExtension(x), ".fx", StringComparison.InvariantCultureIgnoreCase));
				fxFiles = fxFiles.Select(x => Path.ChangeExtension(x.Substring(Path.Combine(ModSourcePath, selectedMod.Name).Length + 1), null).Replace("\\", "/"));

				var allEntries = loadedEffects.Keys.Union(fxFiles);

				foreach (var textureEntry in allEntries) {
					bool loaded = loadedEffects.ContainsKey(textureEntry);

					if (searchFilter.Text.Length > 0) {
						if (textureEntry.ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) == -1)
							continue;
					}

					UIElement panel = new UIElement();
					panel.Width.Set(0, 1f);
					panel.Height.Set(36, 0f);

					UITextPanel<string> button = new UITextPanel<string>(loaded ? textureEntry : textureEntry + " (unloaded)", 0.7f);

					// loaded vs unloaded/new .fx file
					button.OnClick += (a, b) => {
						//selectedShader = textureEntry.Value;
						currentShader.SetText("Current: " + textureEntry);

						// Open .fx file for convinience.
						string path = Path.Combine(ModSourcePath, selectedMod.Name, textureEntry + ".fx");
						if (File.Exists(path)) {
							AttemptOpenFxFile(path);
						}
						else {
							Main.NewText($"{path} not found.");
						}

					};
					panel.Append(button);

					button = new UITextPanel<string>("Compile", 0.7f);
					button.OnClick += (a, b) => {
						string path = Path.Combine(ModSourcePath, selectedMod.Name, textureEntry + ".fx");
						if (File.Exists(path)) {
							Main.NewText($"Manually compiling {path}.");
							RunShaderCompileCommand(path);
						}
						else {
							Main.NewText($"{path} not found.");
						}
					};
					button.PaddingLeft = 6;
					button.PaddingRight = 6;
					button.HAlign = 1f;
					panel.Append(button);

					shaderList.Add(panel);
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			if (mainPanel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		protected override void DrawChildren(SpriteBatch spriteBatch) {
			base.DrawChildren(spriteBatch);

			if (Filters.Scene["ModdersToolkit:TestScreenShader"].IsInUse()) {
				var destination = forceShaderCheckbox.GetDimensions();
				Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, $"uOpacity: {Filters.Scene["ModdersToolkit:TestScreenShader"].GetShader().CombinedOpacity:n2}", destination.X + 210, destination.Y + 2, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
			}
		}
	}
}
