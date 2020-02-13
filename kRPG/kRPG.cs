using System;
using System.Collections.Generic;
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
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
// ReSharper disable StringLiteralTypo

namespace kRPG
{
    public class kRPG : Mod
    {


        //public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>
        //{
        //    {Message.AddXp, new List<DataTag> {DataTag.Amount, DataTag.NpcId}},
        //    {
        //        Message.CreateProjectile, new List<DataTag>
        //        {
        //            DataTag.PlayerId,
        //            DataTag.ProjId,
        //            DataTag.GlyphStar,
        //            DataTag.GlyphCross,
        //            DataTag.GlyphMoon,
        //            DataTag.Damage,
        //            DataTag.Flag,
        //            DataTag.EntityId,
        //            DataTag.ModifierCount
        //        }
        //    },
        //    {
        //        Message.SwordInit, new List<DataTag>
        //        {
        //            DataTag.ItemId,
        //            DataTag.PartPrimary,
        //            DataTag.PartSecondary,
        //            DataTag.PartTertiary,
        //            DataTag.ItemDps,
        //            DataTag.ItemDef
        //        }
        //    },
        //    {
        //        Message.StaffInit, new List<DataTag>
        //        {
        //            DataTag.ItemId,
        //            DataTag.PartPrimary,
        //            DataTag.PartSecondary,
        //            DataTag.PartTertiary,
        //            DataTag.ItemDps,
        //            DataTag.ItemDef
        //        }
        //    },
        //    {Message.BowInit, new List<DataTag> {DataTag.ItemId, DataTag.ItemDps, DataTag.ItemDef}},
        //    {Message.SyncHit, new List<DataTag> {DataTag.PlayerId, DataTag.AmountSingle}},
        //    {Message.SyncCritHit, new List<DataTag> {DataTag.PlayerId, DataTag.AmountSingle}},
        //    {Message.SyncLevel, new List<DataTag> {DataTag.PlayerId, DataTag.Amount}},
        //    {
        //        Message.InitProjEleDmg, new List<DataTag>
        //        {
        //            DataTag.ProjId,
        //            DataTag.Fire,
        //            DataTag.Cold,
        //            DataTag.Lightning,
        //            DataTag.Shadow
        //        }
        //    },
        //    {
        //        Message.SyncStats, new List<DataTag>
        //        {
        //            DataTag.PlayerId,
        //            DataTag.Amount,
        //            DataTag.Resilience,
        //            DataTag.Quickness,
        //            DataTag.Potency,
        //            DataTag.Wits
        //        }
        //    },
        //    {Message.SyncSpear, new List<DataTag> {DataTag.ProjId, DataTag.PartPrimary, DataTag.PartSecondary, DataTag.PartTertiary}},
        //    {Message.PrefixNpc, new List<DataTag> {DataTag.NpcId, DataTag.Amount}},
        //    {
        //        Message.NpcEleDmg, new List<DataTag>
        //        {
        //            DataTag.NpcId,
        //            DataTag.Flag,
        //            DataTag.Flag2,
        //            DataTag.Flag3,
        //            DataTag.Flag4
        //        }
        //    }
        //};

        public static Dictionary<string, Ritual> ritualByName = new Dictionary<string, Ritual>
        {
            {"demon_pact", Ritual.DemonPact},
            {"warrior_oath", Ritual.WarriorOath},
            {"elan_vital", Ritual.ElanVital},
            {"stone_aspect", Ritual.StoneAspect},
            {"eldritch_fury", Ritual.EldritchFury},
            {"mind_fortress", Ritual.MindFortress},
            {"blood_drinking", Ritual.BloodDrinking}
        };

        public kRPG()
        {
            Properties = new ModProperties { Autoload = true, AutoloadGores = true, AutoloadSounds = true };
            Mod = this;

            //for (int i = 0; i < Lang.inter.Length;i++)
            //{
            //    var t = Lang.inter[i];
            //    Debug.WriteLine($"{i} -- {t.Key} -- {t.Value}");
            //}
        }
        public static void LogMessage(string msg)
        {
            ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(msg);
        }

        public Texture2D[] InvSlot { get; set; } = new Texture2D[16];

        public static kRPG Mod { get; set; }

        //  public static Mod Overhaul { get; set; }

        public bool DrawInterface()
        {
            if (Main.netMode == 2 || Main.gameMenu) return true;
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

            return true;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message)reader.ReadByte();
            LogMessage($"Handling {msg}");
            //Dictionary<DataTag, object> tags = new Dictionary<DataTag, object>();

            //foreach (DataTag tag in dataTags[msg])
            //    tags.Add(tag, tag.Read(reader));

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
                    break;
                case Message.CreateProjectile:
                    try
                    {
                        int playerWhoAmI=reader.ReadInt32();
                        int projectileWhoAmI=reader.ReadInt32();
                        int starType =reader.ReadInt32();
                        int crossType=reader.ReadInt32();
                        int moonType=reader.ReadInt32();
                        float damage=reader.ReadSingle();
                        bool minion = reader.ReadBoolean();
                        int casterWhoAmI = reader.ReadInt32();
                        int modCount = reader.ReadInt32();



                        if (Main.netMode == 1)
                            if (playerWhoAmI == Main.myPlayer)
                                return;


                        int modifierCount = modCount;
                        List<GlyphModifier> modifiers = new List<GlyphModifier>();
                        for (int i = 0; i < modifierCount; i += 1)
                            modifiers.Add(GlyphModifier.Modifiers[reader.ReadInt32()]);



                        Projectile projectile = Main.projectile[projectileWhoAmI];
                        if (projectile == null)
                            return;

                        projectile.owner = playerWhoAmI;

                        if (!(projectile.modProjectile is ProceduralSpellProj))
                            return;
                        ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
                        ps.Source = new ProceduralSpell(kRPG.Mod) { Glyphs = new Item[3] };
                        for (int i = 0; i < ps.Source.Glyphs.Length; i += 1)
                        {
                            ps.Source.Glyphs[i] = new Item();
                            ps.Source.Glyphs[i].SetDefaults(0, true);
                        }

                        ps.Source.Glyphs[(byte)GlyphType.Star].SetDefaults(starType, true);
                        ps.Source.Glyphs[(byte)GlyphType.Cross].SetDefaults(crossType, true);
                        ps.Source.Glyphs[(byte)GlyphType.Moon].SetDefaults(moonType, true);

                        projectile.damage =(int) damage;
                        projectile.minion = minion;
                        try
                        {
                            if (projectile.minion)
                                ps.Caster = Main.projectile[casterWhoAmI];
                            else if (projectile.hostile)
                                ps.Caster = Main.npc[casterWhoAmI];
                            else
                                ps.Caster = Main.player[casterWhoAmI];
                        }
                        catch (SystemException e)
                        {
                            kRPG.LogMessage("Source-assignment failed, aborting..." + e);
                            return;
                        }

                        ps.Source.ModifierOverride = modifiers;
                        foreach (Item item in ps.Source.Glyphs)
                        {
                            if (item == null)
                                continue;
                            Glyph glyph = (Glyph)item.modItem;
                            if (glyph.GetAiAction() != null)
                                ps.Ai.Add(glyph.GetAiAction());
                            if (glyph.GetInitAction() != null)
                                ps.Inits.Add(glyph.GetInitAction());
                            if (glyph.GetImpactAction() != null)
                                ps.Impacts.Add(glyph.GetImpactAction());
                            if (glyph.GetKillAction() != null)
                                ps.Kills.Add(glyph.GetKillAction());
                        }

                        foreach (GlyphModifier modifier in modifiers)
                        {
                            if (modifier.Impact != null)
                                ps.Impacts.Add(modifier.Impact);
                            if (modifier.Draw != null)
                                ps.SpellDraw.Add(modifier.Draw);
                            if (modifier.Init != null)
                                ps.Inits.Add(modifier.Init);
                        }

                        ps.Initialize();

                        CreateProjectilePacket.Write(projectile.owner,
                            projectile.whoAmI,
                            ps.Source.Glyphs[(byte)GlyphType.Star].type,
                            ps.Source.Glyphs[(byte)GlyphType.Cross].type,
                            ps.Source.Glyphs[(byte)GlyphType.Moon].type,
                            projectile.damage,
                            projectile.minion,
                            ps.Caster.whoAmI,
                            modifiers
                            );
                    }
                    catch (SystemException e)
                    {
                        kRPG.LogMessage("Error handling packet: CreateProjectilePacket on " + (Main.netMode == 2 ? "serverside" : "clientSide") +
                                   ", full error trace: " + e);

                    }
                    break;
                case Message.AddXp:
                    AddXPPacket.Read(reader);
                    break;
                case Message.SyncSpear:
                    SyncSpearPacket.Read(reader);
                    break;
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
            if (Main.netMode != 2)
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
            GlyphModifier.Initialize(this);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
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