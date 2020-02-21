using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

using kRPG.Enums;
using kRPG.GameObjects.GFX;
using kRPG.GameObjects.GUI.Base;
using kRPG.GameObjects.Items.Glyphs;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.Items.Weapons.Melee;
using kRPG.GameObjects.Items.Weapons.Ranged;
using kRPG.GameObjects.Modifiers;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.Spells;
using kRPG.Packets;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using On.Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using ItemID = Terraria.ID.ItemID;

// ReSharper disable StringLiteralTypo

namespace kRPG
{
    public class kRPG : Mod
    {
       

        public kRPG()
        {
            Properties = new ModProperties { Autoload = true, AutoloadGores = true, AutoloadSounds = true };
            Mod = this;
        }
        public static void LogMessage(string msg)
        {
            Debug.WriteLine("MESSAGE: " + msg);
            ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(msg);
        }

        public Texture2D[] InvSlot { get; set; } = new Texture2D[16];

        public static kRPG Mod { get; set; }

        public static bool PlayerEnteredWorld { get; set; } = false;

        //  public static Mod Overhaul { get; set; }

        

        public bool DrawInterface()
        {
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            if (Main.netMode == NetmodeID.Server || Main.gameMenu) return true;
            try
            {
                for (int i = 0; i < BaseGui.GuiElements.Count; i += 1)
                {
                    BaseGui gui = BaseGui.GuiElements[i];
                    if (gui.PreDraw())
                        gui.Draw(Main.spriteBatch, Main.LocalPlayer);
                }
            }
            catch (SystemException e)
            {
                LogMessage(e.ToString());

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            return true;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message)reader.ReadByte();
#if DEBUG
            LogMessage($"Handling {msg}");
#endif
            switch (msg)
            {
                //case Message.InitProjEleDmg:
                //    if (!Main.projectile.IndexInRange((int)tags[DataTag.projId])) break;
                //    Projectile p = Main.projectile[(int)tags[DataTag.projId]];
                //    try
                //    {
                //        kProjectile proj = p.GetGlobalProjectile<kProjectile>();
                //        proj.elementalDamage = new Dictionary<ELEMENT, int>()
                //        {
                //            { ELEMENT.FIRE, (int)tags[DataTag.fire] },
                //            { ELEMENT.COLD, (int)tags[DataTag.cold] },
                //            { ELEMENT.LIGHTNING, (int)tags[DataTag.lightning] },
                //            { ELEMENT.SHADOW, (int)tags[DataTag.shadow] }
                //        };
                //    }
                //    catch (SystemException e)
                //    {
                //        Main.NewText(e.ToString());
                //    }
                //    break;
                case Message.NpcEleDmg:
                    NPCEleDmgPacket.Read(reader);
                    break;
                case Message.PrefixNpc:
                    PrefixNPCPacket.Read(reader);
                    break;
                case Message.SyncStats:
                    SyncStatsPacket.Read(reader);
                    break;
                case Message.SyncLevel:
                    SyncLevelPacket.Read(reader);
                    kRPG.PlayerEnteredWorld = true;
                    break;
                case Message.CreateProjectile:
                    CreateProjectilePacket.Read(reader);
                    break;
                case Message.AddXp:
                    AddXPPacket.Read(reader);
                    break;
                //case Message.SyncSpear:
                //    SyncSpearPacket.Read(reader);
                //    break;
                case Message.SwordInit:
                    SwordInitPacket.Read(reader);
                    break;
                case Message.StaffInit:
                    StaffInitPacket.Read(reader);
                    break;
                case Message.BowInit:
                    BowInitPacket.Read(reader);
                    break;
                case Message.SyncHit:
                    SyncHitPacket.Read(reader);
                    break;
                case Message.SyncCritHit:
                    SyncCritHitPacket.Read(reader);
                    break;
            }
        }

        public override void Load()
        {
            //Overhaul = ModLoader.GetMod("TerrariaOverhaul");

            kConfig.Initialize();
            if (Main.netMode != NetmodeID.Server)
            {
                GFX.LoadGfx();
                InvSlot[0] = Main.inventoryBackTexture;
                InvSlot[1] = Main.inventoryBack2Texture;
                InvSlot[2] = Main.inventoryBack3Texture;
                InvSlot[3] = Main.inventoryBack4Texture;
                InvSlot[4] = Main.inventoryBack5Texture;
                InvSlot[5] = Main.inventoryBack6Texture;
                InvSlot[6] = Main.inventoryBack7Texture;
                InvSlot[7] = Main.inventoryBack8Texture;
                InvSlot[8] = Main.inventoryBack9Texture;
                InvSlot[9] = Main.inventoryBack10Texture;
                InvSlot[10] = Main.inventoryBack11Texture;
                InvSlot[11] = Main.inventoryBack12Texture;
                InvSlot[12] = Main.inventoryBack13Texture;
                InvSlot[13] = Main.inventoryBack14Texture;
                InvSlot[14] = Main.inventoryBack15Texture;
                InvSlot[15] = Main.inventoryBack16Texture;
                Main.inventoryBackTexture = GFX.ItemSlot;
                Main.inventoryBack2Texture = GFX.ItemSlot;
                Main.inventoryBack3Texture = GFX.ItemSlot;
                Main.inventoryBack4Texture = GFX.ItemSlot;
                Main.inventoryBack5Texture = GFX.ItemSlot;
                Main.inventoryBack6Texture = GFX.ItemSlot;
                Main.inventoryBack7Texture = GFX.ItemSlot;
                Main.inventoryBack8Texture = GFX.ItemSlot;
                Main.inventoryBack9Texture = GFX.ItemSlot;
                Main.inventoryBack10Texture = GFX.FavouritedSlot;
                Main.inventoryBack11Texture = GFX.ItemSlot;
                Main.inventoryBack12Texture = GFX.ItemSlot;
                Main.inventoryBack13Texture = GFX.ItemSlot;
                Main.inventoryBack14Texture = GFX.SelectedSlot;
                Main.inventoryBack15Texture = GFX.ItemSlot;
                Main.inventoryBack16Texture = GFX.ItemSlot;
            }

            Main.player[Main.myPlayer].hbLocked = false;

            SwordHilt.Initialize();
            SwordBlade.Initialize();
            SwordAccent.Initialize();
            Staff.Initialize();
            StaffGem.Initialize();
            StaffOrnament.Initialize();
            GlyphModifier.Initialize();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            Mod MagicStorage = ModLoader.GetMod("MagicStorage");
            MagicStorage?.ModifyInterfaceLayers(layers);


            layers.Find(layer => layer.Name == "Vanilla: Resource Bars").Active = false;
            layers[layers.FindIndex(layer => layer.Name == "Vanilla: Inventory")] = new LegacyGameInterfaceLayer(Constants.ModName, DrawInterface, InterfaceScaleType.UI);
            layers.Find(layer => layer.Name == "Vanilla: Hotbar").Active = false;
        }

        public override void Unload()
        {
            Main.inventoryBackTexture = InvSlot[0];
            Main.inventoryBack2Texture = InvSlot[1];
            Main.inventoryBack3Texture = InvSlot[2];
            Main.inventoryBack4Texture = InvSlot[3];
            Main.inventoryBack5Texture = InvSlot[4];
            Main.inventoryBack6Texture = InvSlot[5];
            Main.inventoryBack7Texture = InvSlot[6];
            Main.inventoryBack8Texture = InvSlot[7];
            Main.inventoryBack9Texture = InvSlot[8];
            Main.inventoryBack10Texture = InvSlot[9];
            Main.inventoryBack11Texture = InvSlot[10];
            Main.inventoryBack12Texture = InvSlot[11];
            Main.inventoryBack13Texture = InvSlot[12];
            Main.inventoryBack14Texture = InvSlot[13];
            Main.inventoryBack15Texture = InvSlot[14];
            Main.inventoryBack16Texture = InvSlot[15];
            GFX.UnloadGfx();
            SwordBlade.Unload();
            SwordHilt.Unload();
            SwordAccent.Unload();
            StaffGem.Unload();
            Staff.Unload();
            StaffOrnament.Unload();
            Main.instance.invBottom = 210;
        }

        #region UpdateCheck

        public class VersionInfo
        {
            public string summary;
            public string version;
        }

        //Mirsario's code in Mirsario's code style
        public static void CheckForUpdates()
        {
#pragma warning disable 162
            try
            {
                string url = @"http://raw.githubusercontent.com/FairfieldTekLLC/kRPG/master/kRPG_VersionInfo.json";
                WebClient client = new WebClient();
                Version currentVersion = Mod.Version;
                client.DownloadStringCompleted += (sender, e) =>
                {
                    try
                    {
                        string text = e.Result;
                        VersionInfo versionInfo = JsonConvert.DeserializeObject<VersionInfo>(text);
                        Version latestVersion = new Version(versionInfo.version);
                        if (latestVersion > currentVersion)
                        {
                            //Public update available
                            Main.NewText("[c/cccccc:New version of] [c/ffdb00:KRPG] [c/cccccc:available]");
                            Main.NewTextMultiline("[c/cccccc:Summary:] " + versionInfo.summary, WidthLimit: 725);
                            Main.NewText("[c/cccccc:Get the update from Mod Browser]");
                        }
                        else if (latestVersion == currentVersion && new Version(kConfig.Stats.LastStartVersion) < currentVersion)
                        {
                            //After installing a public update
                            Main.NewText("[c/cccccc:KRPG is now up to date!]");
                            Main.NewTextMultiline("[c/cccccc:Summary changes:] " + versionInfo.summary, WidthLimit: 725);
                        }

                        kConfig.Stats.LastStartVersion = currentVersion.ToString();
                        kConfig.SaveStats();
                    }
                    catch
                    {
                        //
                    }
                };
                client.DownloadStringAsync(new Uri(url), url);
            }
            catch
            {
                //
            }
#pragma warning restore 162
        }

        #endregion
    }
}