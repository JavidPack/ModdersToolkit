using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.ID;
using System.Reflection;
using ReLogic.Reflection;
using System.Diagnostics;

namespace ModdersToolkit.Tools.Miscellaneous
{
    internal class MiscellaneousTool : Tool
    {
        internal static MiscellaneousUI miscellaneousUI;
        internal static bool showNPCInfo;
        internal static bool showTileGrid;
        internal static bool showCollisionCircle;
        internal static bool logSounds;

        internal static IDictionary<SoundType, IDictionary<int, ModSound>> modSounds; // reference to private field in SoundLoader
        internal static IDictionary<SoundType, IDictionary<string, int>> sounds; // reference to private field in SoundLoader
                                                                                 //internal static readonly IdDictionary Search = IdDictionary.Create<SoundID, int>();

        public override void Initialize()
        {
            ToggleTooltip = "Click to toggle Miscellaneous Tool";
        }

        public override void ClientInitialize()
        {
            Interface = new UserInterface();

            miscellaneousUI = new MiscellaneousUI(Interface);
            miscellaneousUI.Activate();

            Interface.SetState(miscellaneousUI);

            On.Terraria.Main.PlaySound_int_int_int_int_float_float += Main_PlaySound_int_int_int_int_float_float;

            FieldInfo modSoundsField = typeof(SoundLoader).GetField("modSounds", BindingFlags.Static | BindingFlags.NonPublic);
            modSounds = (Dictionary<SoundType, IDictionary<int, ModSound>>)modSoundsField.GetValue(null);

            FieldInfo soundsField = typeof(SoundLoader).GetField("sounds", BindingFlags.Static | BindingFlags.NonPublic);
            sounds = (Dictionary<SoundType, IDictionary<string, int>>)soundsField.GetValue(null);

            // TODO: IdDictionary-type soundID
        }

        public override void ClientTerminate()
        {
            On.Terraria.Main.PlaySound_int_int_int_int_float_float -= Main_PlaySound_int_int_int_int_float_float;

            modSounds = default;
            sounds = default;

            Interface = default;

            miscellaneousUI?.Deactivate();
            miscellaneousUI = default;
        }


        private Microsoft.Xna.Framework.Audio.SoundEffectInstance Main_PlaySound_int_int_int_int_float_float(On.Terraria.Main.orig_PlaySound_int_int_int_int_float_float orig, int type, int x, int y, int Style, float volumeScale, float pitchOffset)
        {
            var result = orig(type, x, y, Style, volumeScale, pitchOffset); // null results are off screen

            if (logSounds && Main.soundVolume != 0f && result != null)
            {
                if ((type >= 30 && type <= 35) || type == 39)
                {
                    // Maybe a toggle to ignore?
                }
                else
                {
                    float soundVolume = Main.soundVolume;
                    Main.soundVolume = 0f;
                    Main.NewText("");
                    Main.NewText($"Type: {type}, Style: {Style}{(x != -1 ? $", x: {x}" : "")}{(y != -1 ? $", y: {y}" : "")}{(volumeScale != 1 ? $", volumeScale: {volumeScale}" : "")}{(pitchOffset != 0 ? $", pitchOffset: {pitchOffset}" : "")}");

                    if (sounds.ContainsKey((SoundType)type))
                    {
                        var kvp = sounds[(SoundType)type].FirstOrDefault(s => s.Value == Style);
                        if (kvp.Key != null)
                        { // struct
                            Main.NewText($"Key: {kvp.Key}");
                        }
                        //var modSound = modSounds[SoundType.Custom][Style];
                        //Main.NewText($"modSound: {modSound}");
                    }

                    var frames = new StackTrace(true).GetFrames();
                    Logging.PrettifyStackTraceSources(frames);
                    int index = 2;
                    while (frames[index].GetMethod().Name.Contains("PlaySound"))
                        index++;
                    var frame = frames[index];
                    Main.NewText(frame.GetMethod().DeclaringType.FullName + "." + frame.GetMethod().Name + ":" + frame.GetFileLineNumber());
                    Main.soundVolume = soundVolume;
                }
            }
            return result;
        }

        public override void UIDraw()
        {
            if (Visible)
            {
                miscellaneousUI.Draw(Main.spriteBatch);
            }
        }
    }

    internal class NPCInfoGlobalNPC : GlobalNPC
    {
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (MiscellaneousTool.showNPCInfo)
            {
                if (npc.realLife == -1 || npc.realLife == npc.whoAmI)
                {
                    var font = Main.fontMouseText;
                    //Main.NewText("Drawing: " + npc.TypeName);
                    Rectangle infoRectangle = new Rectangle((int)(npc.BottomRight.X - Main.screenPosition.X), (int)(npc.BottomRight.Y - Main.screenPosition.Y), 300, 160);
                    spriteBatch.Draw(Main.magicPixel, infoRectangle, Color.NavajoWhite);

                    /*
					var d = npc.Hitbox;
					Main.spriteBatch.Draw(Main.magicPixel, d, Color.Red);

					spriteBatch.DrawString(font, $"X {npc.position.X,5:##0.0}", infoRectangle.TopLeft() + new Vector2(5, 5), Color.Black);
					spriteBatch.DrawString(font, $"Y {npc.position.Y,5:##0.0}", infoRectangle.TopLeft() + new Vector2(5, 25), Color.Black);
					return;
					*/

                    int y = 5;
                    spriteBatch.DrawString(font, $"ai:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black); y += 20;
                    spriteBatch.DrawString(font, $"localAI:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black); y += 20;
                    for (int i = 0; i < 4; i++)
                    {
                        spriteBatch.DrawString(font, $"{npc.ai[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 5), Color.Black);
                        spriteBatch.DrawString(font, $"{npc.localAI[i],5:##0.0}", infoRectangle.TopLeft() + new Vector2(5 + 65 + i * 50, 25), Color.Black);
                    }

                    spriteBatch.DrawString(font, $"npc.immune[]: {npc.immune[Main.myPlayer]}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black); y += 20;
                    spriteBatch.DrawString(font, $"spriteDirection: {npc.spriteDirection}  direction: {npc.direction}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black); y += 20;
                    spriteBatch.DrawString(font, $"type: {npc.type} aiStyle: {npc.aiStyle} whoAmI: {npc.whoAmI}", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black); y += 20;
                    spriteBatch.DrawString(font, $"Buffs:", infoRectangle.TopLeft() + new Vector2(5, y), Color.Black);
                    bool anyBuffs = false;
                    for (int i = 0; i < 5; i++)
                    {
                        if (npc.buffType[i] > 0)
                        {
                            spriteBatch.DrawString(font, $"{Lang.GetBuffName(npc.buffType[i])}({npc.buffType[i]}) {npc.buffTime[i]} ", infoRectangle.TopLeft() + new Vector2(5 + 65, y), Color.Black); y += 20;
                            anyBuffs = true;
                        }
                    }
                    if (!anyBuffs)
                        y += 20;
                    //if(npc.realLife != -1)
                    //{
                    //	Utils.DrawLine(spriteBatch, npc.Center.ToPoint(), Main.npc[npc.realLife].Center.ToPoint(), Color.White);
                    //}
                }
            }
        }
    }

    internal class TileGridModWorld : ModWorld
    {
        public override void PostDrawTiles()
        {
            if (MiscellaneousTool.showTileGrid)
            {
                //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 center = Main.LocalPlayer.Center.ToTileCoordinates().ToVector2() * 16;

                for (int i = -13; i < 14; i++)
                {
                    Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(center.X + i * 16 - Main.screenPosition.X), (int)(center.Y - 13 * 16 - Main.screenPosition.Y), 2, 416), new Rectangle(0, 0, 1, 1), Color.White);
                    Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(center.X - 13 * 16 - Main.screenPosition.X), (int)(center.Y - i * 16 - Main.screenPosition.Y), 416, 2), new Rectangle(0, 0, 1, 1), Color.White);
                    //Utils.DrawLine(Main.spriteBatch, new Vector2(center.X + i * 16, center.Y - 13 * 16), new Vector2(center.X + i * 16, center.Y + 13 * 16), Color.White, Color.White, 1);
                    //Utils.DrawLine(Main.spriteBatch, new Vector2(center.X - 13 * 16, center.Y + i * 16), new Vector2(center.X + 13 * 16, center.Y + i * 16), Color.White, Color.White, 1);
                }
                /* Shows some coordinates for making guides on the wiki
				var font = Main.fontMouseText;
				var b = Main.LocalPlayer.Center.ToTileCoordinates();
				Main.spriteBatch.DrawString(font, $"{b.X - 3}, {b.Y - 3}", center - Main.screenPosition + new Vector2(-3*16)+ new Vector2(0,-20), Color.Orange);
				Main.spriteBatch.DrawString(font, $"{b.X + 2}, {b.Y + 2}", center - Main.screenPosition + new Vector2(3*16, 2*16), Color.Orange);

				var c = center - Main.screenPosition + new Vector2(-3 * 16);
				Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)c.X, (int)c.Y, 4, 4), Color.Red);

				var d = center - Main.screenPosition + new Vector2(2 * 16);
				Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)d.X, (int)d.Y, 4, 4), Color.Red);
				*/
                Main.spriteBatch.End();

                // Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            }
            if (MiscellaneousTool.showCollisionCircle)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                float max = 360f; // TODO: use a real circle drawing algorithm and adjustable radius
                bool showTileCorners = Main.GameUpdateCount / 60 % 2 == 0;
                for (int i = 0; i < max; i++)
                {
                    float radians = MathHelper.TwoPi * (i / max);
                    Vector2 end = new Vector2(100, 0).RotatedBy(radians) + Main.LocalPlayer.Center;
                    bool noCollision = Collision.CanHit(Main.LocalPlayer.Center, 0, 0, end, 0, 0);
                    //Dust.QuickDust(end, noCollision ? Color.Green : Color.Red);

                    if (!showTileCorners)
                        Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(end.X - Main.screenPosition.X), (int)(end.Y - Main.screenPosition.Y), 1, 1), noCollision ? Color.Green : Color.Red);

                    var Position1 = Main.LocalPlayer.Center;
                    var Position2 = end;
                    var Width1 = 0;
                    var Width2 = 0;
                    var Height1 = 0;
                    var Height2 = 0;
                    int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
                    int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
                    int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
                    int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);

                    Vector2 actualCheck = new Vector2(num3, num4) * 16;
                    if (showTileCorners)
                    {
                        Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 2, (int)(actualCheck.Y - Main.screenPosition.Y) + 2, 1, 1), noCollision ? Color.Green : Color.Red);
                        Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 2, (int)(actualCheck.Y - Main.screenPosition.Y) + 14, 1, 1), noCollision ? Color.Green : Color.Red);
                        Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 14, (int)(actualCheck.Y - Main.screenPosition.Y) + 2, 1, 1), noCollision ? Color.Green : Color.Red);
                        Main.spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(actualCheck.X - Main.screenPosition.X) + 14, (int)(actualCheck.Y - Main.screenPosition.Y) + 14, 1, 1), noCollision ? Color.Green : Color.Red);
                    }
                }
                Main.spriteBatch.End();
            }
        }
    }
}
