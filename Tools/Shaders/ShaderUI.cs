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
		public ShaderUI(UserInterface userInterface) {
			this.userInterface = userInterface;
		}

		static string cacheFolder = Path.Combine(Main.SavePath, "Mods", "Cache", "ModdersToolkit");
		static string shaderFilename = "ModdersToolkit_Shader.fx";
		static string shaderFilePath = Path.Combine(cacheFolder, shaderFilename);

		static string exeFilename = "fxcompiler.exe";
		static string exePath = Path.Combine(cacheFolder, exeFilename);

		public bool updateneeded;

		private UICheckbox watchShader;
		internal FileSystemWatcher modSourcesWatcher;
		internal FileSystemWatcher watcher;
		internal const int defaultWatcherCooldown = 15;
        internal int watcherCooldownSources;
        internal List<string> watchedFileChangedSourcesFileNames = new List<string>();

		public override void OnInitialize() {
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

			watchShader = new UICheckbox("Watch Shader File", "Automatically Watch Shader for Changes", true);
			watchShader.Top.Set(top, 0);
			watchShader.OnSelectedChanged += WatchShader_OnSelectedChanged;
			mainPanel.Append(watchShader);
			top += 20;

			// TODOS:
			// Watch/Auto compile checkbox
			// Select specific .fx from mod/replace it. (With a UIList, as TextureUI does)
			// Different shader templates (screen, armor, others?)
			// Automatically apply shader via checkbox, rather than Acid Dye

			updateneeded = true;
			Append(mainPanel);
		}

		private void EditFxFileButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {

			if (!File.Exists(shaderFilePath)) {
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
	float4 color1= tex2D( uImage1 , coords.xy);

	float readRed = uOpacity * 1.1;

	if(color1.r > readRed){{
		color.rgba = 0;
	}}else if(color1.r > uOpacity ){{
		color =  float4(1, 105.0/255, 180.0/255, 1);
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
			try {
				Process.Start(shaderFilePath);
			}
			catch (Exception) {
				Main.NewText($"Could not open {shaderFilename}, check that you have a text editor associated with .fx files.");
			}
		}

		private void CompileShaderButton_OnClick(UIMouseEvent evt, UIElement listeningElement) {
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
			if (!File.Exists(shaderFilePath)) {
				Main.NewText($"Could not find {shaderFilename}, check that you have first created and opened the file.");
				return;
			}

			RunCommand(shaderFilePath);
        }

		private static void RunCommand(string file) {
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

			if (exitCode == 0) {
				Main.NewText("Success");

				Main.NewText("Wear Acid Dye to test.");

				var myContentManager = new ContentManager(Main.instance.Content.ServiceProvider, cacheFolder);

				string assetName = Path.GetFileNameWithoutExtension(shaderFilename);
				var newShaderEffect = myContentManager.Load<Effect>(assetName);


				Ref<Effect> newShader = new Ref<Effect>(newShaderEffect);
				FieldInfo shaderField = typeof(ArmorShaderData).GetField("_shader", BindingFlags.Instance | BindingFlags.NonPublic);
				var shader = GameShaders.Armor.GetShaderFromItemId(ItemID.AcidDye);
				shaderField.SetValue(shader, newShader);

				FieldInfo passNameField = typeof(ArmorShaderData).GetField("_passName", BindingFlags.Instance | BindingFlags.NonPublic);
				passNameField.SetValue(shader, "ModdersToolkitShaderPass");

				//	Main.ShaderContentManager.Load<Effect>("tmod:" + Name + "/" + path);
			}
			else if(exitCode == 1) {
				Main.NewText("Failure");
			}
			else if(exitCode == 2) {
				Main.NewText("No File");
			}
			else if (exitCode == 3) {
				Main.NewText("File Not Found");
            }
        }

		private void WatchShader_OnSelectedChanged() {
			if (modSourcesWatcher != null) {
				modSourcesWatcher.Dispose();
			}

			if (watchShader.Selected) {
				if (!Directory.Exists(cacheFolder)) {
					Main.NewText("Error somehow");
					return;
				}

				modSourcesWatcher = new FileSystemWatcher();
				modSourcesWatcher.Path = cacheFolder; // ModSourcePath + Path.DirectorySeparatorChar + selectedMod.Name + Path.DirectorySeparatorChar;
				/* Watch for changes in LastAccess and LastWrite times, and
				   the renaming of files or directories. */
				modSourcesWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
				// Only watch text files.
				modSourcesWatcher.Filter = shaderFilename;
				modSourcesWatcher.IncludeSubdirectories = false;

				// Add event handlers.
				modSourcesWatcher.Changed += (a, b) => {
					watcherCooldownSources = defaultWatcherCooldown;
					watchedFileChangedSourcesFileNames.Add(b.FullPath);
				};
				modSourcesWatcher.Renamed += (a, b) => {
                    watcherCooldownSources = defaultWatcherCooldown;
					watchedFileChangedSourcesFileNames.Add(b.FullPath);
				};

				// Begin watching.
				modSourcesWatcher.EnableRaisingEvents = true;
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (watchedFileChangedSourcesFileNames.Count > 0) {
				if (watcherCooldownSources > 0)
					watcherCooldownSources--;

				if (watcherCooldownSources == 0) {
                    for (int i = watchedFileChangedSourcesFileNames.Count - 1; i >= 0; i--)
                    {
                        Main.NewText($"Shader source changed for file {watchedFileChangedSourcesFileNames[i]}, attempting to compile.");
						RunCommand(watchedFileChangedSourcesFileNames[i]);
                    }
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
