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
using Terraria.ID;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Content;

namespace ModdersToolkit.Tools.Shaders
{
    class ShaderUI : UIState
    {
        internal UIPanel mainPanel;
        private UserInterface userInterface;
        public ShaderUI(UserInterface userInterface)
        {
            this.userInterface = userInterface;
        }

        static string cacheFolder = Path.Combine(Main.SavePath, "Mods", "Cache", "ModdersToolkit");
        static string shaderFilename = "ModdersToolkit_Shader.fx";
        static string shaderFilePath = Path.Combine(cacheFolder, shaderFilename);
        static string ModSourcePath = Path.Combine(Main.SavePath, "Mod Sources");

        static string exeFilename = "fxcompiler.exe";
        static string exePath = Path.Combine(cacheFolder, exeFilename);

        public bool updateneeded;

        public UIList modList;
        internal NewUITextBox searchFilter;
        public UIText currentShader;
        public UIList textureList;
        private UICheckbox watchQuickShaderCheckbox;
        private UICheckbox watchModSourcesCheckbox;

        public Mod selectedMod;
        private Effect selectedShader;

        internal FileSystemWatcher quickShaderWatcher;
        internal FileSystemWatcher modSourcesWatcher;
        internal const int defaultWatcherCooldown = 15;
        internal int quickShaderWatcherCooldown;
        internal int modSourcesWatcherCooldown;
        internal HashSet<string> watchedFileChangedSourcesFileNames = new HashSet<string>();

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
            UIText text = new UIText("Shaders:", 0.85f);
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

            currentShader = new UIText("Current: None selected", 0.85f);
            currentShader.Top.Set(top, 0f);
            currentShader.Left.Set(0, 0f);
            currentShader.Width.Set(0, 1);
            currentShader.HAlign = 0;
            mainPanel.Append(currentShader);
            top += 20;

            textureList = new UIList();
            textureList.Top.Pixels = top;
            textureList.Width.Set(-25f, 1f);
            textureList.Height.Set(150, 0f);
            textureList.ListPadding = 0f;
            mainPanel.Append(textureList);

            var textureListScrollbar = new UIElements.FixedUIScrollbar(userInterface);
            textureListScrollbar.SetView(100f, 1000f);
            textureListScrollbar.Top.Pixels = top + 10;// + spacing;
            textureListScrollbar.Height.Set(130, 0f);
            textureListScrollbar.HAlign = 1f;
            mainPanel.Append(textureListScrollbar);
            textureList.SetScrollbar(textureListScrollbar);

            top += 150;

            // Current Working Shader
            // Status Text: "Compiled .fx"
            // Filter?

            // Checkbox: Force

            UIHoverImageButton editFxButton = new UIHoverImageButton(ModdersToolkit.instance.GetTexture("UIElements/eyedropper"), "Open Fx file in Default Editor");
            editFxButton.OnClick += EditFxFileButton_OnClick;
            editFxButton.Top.Set(top + 5, 0f);
            editFxButton.Left.Set(0, 0f);
            mainPanel.Append(editFxButton);
            top += 20;

            UIHoverImageButton compileFxButton = new UIHoverImageButton(ModdersToolkit.instance.GetTexture("UIElements/eyedropper"), "Compile Shader");
            compileFxButton.OnClick += CompileShaderButton_OnClick;
            compileFxButton.Top.Set(top + 5, 0f);
            compileFxButton.Left.Set(0, 0f);
            mainPanel.Append(compileFxButton);
            top += 40;

            watchModSourcesCheckbox = new UICheckbox("Watch Mod Sources", "Automatically Watch Mod Sources Shaders for Changes", false);
            watchModSourcesCheckbox.Top.Set(top, 0);
            watchModSourcesCheckbox.OnSelectedChanged += WatchModSources_OnSelectedChanged;
            mainPanel.Append(watchModSourcesCheckbox);
            top += 20;

            watchQuickShaderCheckbox = new UICheckbox("Watch Quick Shader File", "Automatically Watch Shader for Changes", true);
            watchQuickShaderCheckbox.Top.Set(top, 0);
            watchQuickShaderCheckbox.OnSelectedChanged += WatchShader_OnSelectedChanged;
            mainPanel.Append(watchQuickShaderCheckbox);
            top += 20;

            // TODOS:
            // Watch/Auto compile checkbox
            // Select specific .fx from mod/replace it. (With a UIList, as TextureUI does)
            // Different shader templates (screen, armor, others?)
            // Automatically apply shader via checkbox, rather than Acid Dye

            // Test New shaders without binding to anything, Select pass name?
            // Workload: Testing shader

            updateneeded = true;
            Append(mainPanel);
        }

        private void EditFxFileButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {

            if (!File.Exists(shaderFilePath))
            {
                Directory.CreateDirectory(cacheFolder);
                File.WriteAllText(shaderFilePath, $@"
sampler uImage0 : register(s0);
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

// This is a shader.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{{
	float4 color = tex2D(uImage0, coords);
	
    if (!any(color))
        return color;
    
    int choice = uTime % 5;

    if (choice == 0)
    {{
		color.r = 1;
    }}
    else if (choice == 1)
    {{
		color.g = 1;
    }}
    else if (choice == 2)
    {{
        color.b = 1;
    }}
    else if (choice == 3)
    {{
		color.rgb += 0.3;
    }}
    else if (choice == 4)
    {{
		// Take a break!
	}}
    
	return color;
}}

technique Technique1
{{
	pass ModdersToolkitShaderPass
    {{
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }}
}}
");
            }
            try
            {
                Process.Start(shaderFilePath);
            }
            catch (Exception)
            {
                Main.NewText($"Could not open {shaderFilename}, check that you have a text editor associated with .fx files.");
            }
        }

        private void CompileShaderButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Ensure shader compiler exists
            if (!File.Exists(exePath))
            {
                const string
                    LibReflectedBase = "libReflected",
                    WCFXCompilerDLL = "wcfxcompiler.dll",
                    EffectImporterDLL = "Microsoft.Xna.Framework.Content.Pipeline.EffectImporter.dll",
                    PipelineDLL = "Microsoft.Xna.Framework.Content.Pipeline.dll";

                // TODO Add size checking to files.
                Directory.CreateDirectory(cacheFolder);
                ModdersToolkit.instance.Logger.Info("Unpacking fxbuilder.exe and Pipeline dlls");
                File.WriteAllBytes(exePath, ModdersToolkit.instance.GetFileBytes($"{LibReflectedBase}/fxcompiler.exe"));
                File.WriteAllBytes(Path.Combine(cacheFolder, WCFXCompilerDLL), ModdersToolkit.instance.GetFileBytes($"{LibReflectedBase}/{WCFXCompilerDLL}"));
                File.WriteAllBytes(Path.Combine(cacheFolder, EffectImporterDLL), ModdersToolkit.instance.GetFileBytes($"{LibReflectedBase}/{EffectImporterDLL}"));
                File.WriteAllBytes(Path.Combine(cacheFolder, PipelineDLL), ModdersToolkit.instance.GetFileBytes($"{LibReflectedBase}/{PipelineDLL}"));
                ModdersToolkit.instance.Logger.Info("Unpacking complete");
            }

            // Ensure shader file exists
            if (!File.Exists(shaderFilePath))
            {
                Main.NewText($"Could not find {shaderFilename}, check that you have first created and opened the file.");
                return;
            }

            RunCommand(shaderFilePath);
        }

        private void RunCommand(string file)
        {
            // make async?
            Process process = new Process
            {
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

            if (exitCode == 0)
            {
                Main.NewText("Success");

                var myContentManager = new ContentManager(Main.instance.Content.ServiceProvider, Path.GetDirectoryName(file));
                string assetName = Path.GetFileNameWithoutExtension(file);
                var newEffect = myContentManager.Load<Effect>(assetName);
                Ref<Effect> newShaderRef = new Ref<Effect>(newEffect);

                FieldInfo shaderRefField = typeof(ShaderData).GetField("_shader", BindingFlags.Instance | BindingFlags.NonPublic);

                // If existing Effect, Update all Effect references
                if (file.StartsWith(ModSourcePath))
                {
                    FieldInfo modEffectsField = typeof(Mod).GetField("effects", BindingFlags.Instance | BindingFlags.NonPublic);
                    var modEffects = (Dictionary<string, Effect>)modEffectsField.GetValue(selectedMod);
                    var key = file.Substring(Path.Combine(ModSourcePath, selectedMod.Name).Length + 1).Replace("\\", "/");
                    key = key.Substring(0, key.LastIndexOf(".fx"));
                    var originalEffect = modEffects[key];
                    modEffects[key] = newEffect;

                    FieldInfo shaderDataField = typeof(ArmorShaderDataSet).GetField("_shaderData", BindingFlags.Instance | BindingFlags.NonPublic);
                    var armorShaderDataList = (List<ArmorShaderData>)typeof(ArmorShaderDataSet).GetField("_shaderData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameShaders.Armor);
                    var hairShaderDataList = (List<HairShaderData>)typeof(HairShaderDataSet).GetField("_shaderData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameShaders.Hair);
                    var miscShaderDataList = GameShaders.Misc.Values.ToList();

                    int entriesFound = 0;
                    var shaderData = armorShaderDataList.Cast<ShaderData>().Union(hairShaderDataList.Cast<ShaderData>()).Union(miscShaderDataList.Cast<ShaderData>());
                    foreach (var shaderDataShader in shaderData)
                    {
                        if (shaderDataShader.Shader == originalEffect)
                        {
                            entriesFound++;
                            shaderRefField.SetValue(shaderDataShader, newShaderRef);
                        }
                    }
                    Main.NewText(entriesFound > 0 ? $"{entriesFound} existing bound shader entries updated" : "No existing bound shader entries found");
                }
                else
                {
                    // If we are compiling the Test/Quick Shader, instruct the modder on how to test
                    Main.NewText("Wear Acid Dye to test."); // TODO: Checkbox to force Armor/Hair/other dye

                    // Need to replace Acid Dye shader and passname
                    var shader = GameShaders.Armor.GetShaderFromItemId(ItemID.AcidDye);
                    shaderRefField.SetValue(shader, newShaderRef);
                    FieldInfo passNameField = typeof(ArmorShaderData).GetField("_passName", BindingFlags.Instance | BindingFlags.NonPublic);
                    passNameField.SetValue(shader, "ModdersToolkitShaderPass");
                }

            }
            else if (exitCode == 1)
            {
                Main.NewText("Failure");
            }
            else if (exitCode == 2)
            {
                Main.NewText("No File");
            }
            else if (exitCode == 3)
            {
                Main.NewText("File Not Found");
            }
        }

        private void WatchModSources_OnSelectedChanged()
        {
            if (modSourcesWatcher != null)
            {
                modSourcesWatcher.Dispose();
            }

            if (watchModSourcesCheckbox.Selected)
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
                modSourcesWatcher.Filter = "*.fx";
                modSourcesWatcher.IncludeSubdirectories = true;

                // Add event handlers.
                modSourcesWatcher.Changed += (a, b) =>
                {
                    //watchedFileChangedSources = true;
                    modSourcesWatcherCooldown = defaultWatcherCooldown;
                    //watchedFileChangedSourcesFileName = b.FullPath;
                    watchedFileChangedSourcesFileNames.Add(b.FullPath);
                };
                modSourcesWatcher.Renamed += (a, b) =>
                {
                    //watchedFileChangedSources = true;
                    modSourcesWatcherCooldown = defaultWatcherCooldown;
                    //watchedFileChangedSourcesFileName = b.FullPath;
                    watchedFileChangedSourcesFileNames.Add(b.FullPath);
                };

                // Begin watching.
                modSourcesWatcher.EnableRaisingEvents = true;
            }
        }

        // Watches the Quick/Test shader
        private void WatchShader_OnSelectedChanged()
        {
            quickShaderWatcher?.Dispose();

            if (!watchQuickShaderCheckbox.Selected)
                return;

            if (!Directory.Exists(cacheFolder))
            {
                Main.NewText("Error somehow");
                return;
            }

            quickShaderWatcher = new FileSystemWatcher
            {
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
            quickShaderWatcher.Changed += (a, b) =>
            {
                if (b.FullPath.EndsWith("TMP"))
                    return; // VisualStudio generates those upon edit/save.

                quickShaderWatcherCooldown = defaultWatcherCooldown;
                watchedFileChangedSourcesFileNames.Add(b.FullPath);
            };
            quickShaderWatcher.Renamed += (a, b) =>
            {
                if (b.FullPath.EndsWith("TMP"))
                    return; // VisualStudio generates those upon edit/save.

                quickShaderWatcherCooldown = defaultWatcherCooldown;
                watchedFileChangedSourcesFileNames.Add(b.FullPath);
            };

            // Begin watching.
            quickShaderWatcher.EnableRaisingEvents = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (watchedFileChangedSourcesFileNames.Count > 0)
            {
                if (quickShaderWatcherCooldown > 0)
                    quickShaderWatcherCooldown--;

                if (quickShaderWatcherCooldown == 0)
                {
                    foreach (var filename in watchedFileChangedSourcesFileNames)
                    {
                        Main.NewText($"Shader source changed for file {filename}, attempting to compile.");
                        RunCommand(filename);
                    }
                    watchedFileChangedSourcesFileNames.Clear();
                }
            }

            if (!updateneeded) return;
            updateneeded = false;

            if (modList.Count == 0)
            {
                foreach (var otherMod in ModLoader.Mods)
                {
                    if (otherMod.Name == "ModLoader")
                        continue;

                    UITextPanel<string> button = new UITextPanel<string>(otherMod.DisplayName);

                    button.OnClick += (a, b) =>
                    {
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

            if (selectedMod != null)
            {
                textureList.Clear();

                FieldInfo effectsField = typeof(Mod).GetField("effects", BindingFlags.Instance | BindingFlags.NonPublic);
                var effects = (Dictionary<string, Effect>)effectsField.GetValue(selectedMod);

                foreach (var textureEntry in effects)
                {
                    if (searchFilter.Text.Length > 0)
                    {
                        if (textureEntry.Key.ToLower().IndexOf(searchFilter.Text, StringComparison.OrdinalIgnoreCase) == -1)
                            continue;
                    }

                    UITextPanel<string> button = new UITextPanel<string>(textureEntry.Key);

                    button.OnClick += (a, b) =>
                    {
                        selectedShader = textureEntry.Value;
                        currentShader.SetText("Current: " + textureEntry.Key);

                        // Open .fx file for convinience.
                        string path = Path.Combine(ModSourcePath, selectedMod.Name, textureEntry.Key + ".fx");
                        if (File.Exists(path))
                        {
                            Process.Start(path);
                        }
                    };
                    textureList.Add(button);
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
