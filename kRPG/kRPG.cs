using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using kRPG.Classes;
using kRPG.Enums;
using kRPG.GUI;
using kRPG.Items;
using kRPG.Items.Glyphs;
using kRPG.Items.Weapons;
using kRPG.Items.Weapons.RangedDrops;
using kRPG.Modifiers;
using kRPG.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace kRPG
{
    public class kRPG : Mod
    {
        public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>
        {
            {Message.AddXP, new List<DataTag> {DataTag.Amount, DataTag.NpcId}},
            {
                Message.CreateProjectile, new List<DataTag>
                {
                    DataTag.PlayerId,
                    DataTag.ProjId,
                    DataTag.GlyphStar,
                    DataTag.GlyphCross,
                    DataTag.GlyphMoon,
                    DataTag.Damage,
                    DataTag.Flag,
                    DataTag.EntityId,
                    DataTag.ModifierCount
                }
            },
            {
                Message.SwordInit, new List<DataTag>
                {
                    DataTag.ItemId,
                    DataTag.PartPrimary,
                    DataTag.PartSecondary,
                    DataTag.PartTertiary,
                    DataTag.ItemDps,
                    DataTag.ItemDef
                }
            },
            {
                Message.StaffInit, new List<DataTag>
                {
                    DataTag.ItemId,
                    DataTag.PartPrimary,
                    DataTag.PartSecondary,
                    DataTag.PartTertiary,
                    DataTag.ItemDps,
                    DataTag.ItemDef
                }
            },
            {Message.BowInit, new List<DataTag> {DataTag.ItemId, DataTag.ItemDps, DataTag.ItemDef}},
            {Message.SyncHit, new List<DataTag> {DataTag.PlayerId, DataTag.AmountSingle}},
            {Message.SyncCritHit, new List<DataTag> {DataTag.PlayerId, DataTag.AmountSingle}},
            {Message.SyncLevel, new List<DataTag> {DataTag.PlayerId, DataTag.Amount}},
            {
                Message.InitProjEleDmg, new List<DataTag>
                {
                    DataTag.ProjId,
                    DataTag.Fire,
                    DataTag.Cold,
                    DataTag.Lightning,
                    DataTag.Shadow
                }
            },
            {
                Message.SyncStats, new List<DataTag>
                {
                    DataTag.PlayerId,
                    DataTag.Amount,
                    DataTag.Resilience,
                    DataTag.Quickness,
                    DataTag.Potency,
                    DataTag.Wits
                }
            },
            {Message.SyncSpear, new List<DataTag> {DataTag.ProjId, DataTag.PartPrimary, DataTag.PartSecondary, DataTag.PartTertiary}},
            {Message.PrefixNPC, new List<DataTag> {DataTag.NpcId, DataTag.Amount}},
            {
                Message.NPCEleDmg, new List<DataTag>
                {
                    DataTag.NpcId,
                    DataTag.Flag,
                    DataTag.Flag2,
                    DataTag.Flag3,
                    DataTag.Flag4
                }
            }
        };

        public static kRPG mod;

        public static Mod overhaul;

        public static Dictionary<string, RITUAL> ritualByName = new Dictionary<string, RITUAL>
        {
            {"demon_pact", RITUAL.DEMON_PACT},
            {"warrior_oath", RITUAL.WARRIOR_OATH},
            {"elan_vital", RITUAL.ELAN_VITAL},
            {"stone_aspect", RITUAL.STONE_ASPECT},
            {"eldritch_fury", RITUAL.ELDRITCH_FURY},
            {"mind_fortress", RITUAL.MIND_FORTRESS},
            {"blood_drinking", RITUAL.BLOOD_DRINKING}
        };

        public Texture2D[] invslot = new Texture2D[16];

        public kRPG()
        {
            Properties = new ModProperties {Autoload = true, AutoloadGores = true, AutoloadSounds = true};
            mod = this;

            //for (int i = 0; i < Lang.inter.Length;i++)
            //{
            //    var t = Lang.inter[i];
            //    Debug.WriteLine($"{i} -- {t.Key} -- {t.Value}");
            //}
        }

        public bool DrawInterface()
        {
            if (Main.netMode == 2 || Main.gameMenu) return true;
            try
            {
                for (int i = 0; i < BaseGui.guiElements.Count; i += 1)
                {
                    BaseGui gui = BaseGui.guiElements[i];
                    if (gui.PreDraw())
                        gui.Draw(Main.spriteBatch, Main.LocalPlayer);
                }
            }
            catch (SystemException e)
            {
                Logger.InfoFormat(e.ToString());
            }

            return true;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message) reader.ReadByte();
            Dictionary<DataTag, object> tags = dataTags[msg].ToDictionary(tag => tag, tag => tag.Read(reader));
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
                case Message.NPCEleDmg:
                    if (Main.netMode == 1)
                    {
                        NPC npc = Main.npc[(int) tags[DataTag.NpcId]];
                        kNPC kn = npc.GetGlobalNPC<kNPC>();
                        Dictionary<ELEMENT, bool> haselement = new Dictionary<ELEMENT, bool>
                        {
                            {ELEMENT.FIRE, (bool) tags[DataTag.Flag]},
                            {ELEMENT.COLD, (bool) tags[DataTag.Flag2]},
                            {ELEMENT.LIGHTNING, (bool) tags[DataTag.Flag3]},
                            {ELEMENT.SHADOW, (bool) tags[DataTag.Flag4]}
                        };
                        int count = Enum.GetValues(typeof(ELEMENT)).Cast<ELEMENT>().Count(element => haselement[element]);
                        int portionsize = (int) Math.Round(npc.damage * kNPC.ELE_DMG_MODIFIER / 2.0 / count);
                        foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                            if (haselement[element])
                                kn.elementalDamage[element] = Math.Max(1, portionsize);
                        kn.dealseledmg = count > 0;
                    }

                    break;
                case Message.PrefixNPC:
                    if (Main.netMode == 1)
                    {
                        NPC npc = Main.npc[(int) tags[DataTag.NpcId]];
                        kNPC kn = npc.GetGlobalNPC<kNPC>();
                        for (int i = 0; i < (int) tags[DataTag.Amount]; i += 1)
                        {
                            NPCModifier modifier = kn.modifierFuncs[reader.ReadInt32()].Invoke(kn, npc);
                            modifier.Read(reader);
                            modifier.Apply();
                            kn.modifiers.Add(modifier);
                        }

                        kn.MakeNotable(npc);
                    }

                    break;
                case Message.SyncStats:
                    if (Main.netMode == 2)
                    {
                        PlayerCharacter character = Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>();
                        character.level = (int) tags[DataTag.Amount];
                        character.baseStats[STAT.RESILIENCE] = (int) tags[DataTag.Resilience];
                        character.baseStats[STAT.QUICKNESS] = (int) tags[DataTag.Quickness];
                        character.baseStats[STAT.POTENCY] = (int) tags[DataTag.Potency];
                        character.baseStats[STAT.WITS] = (int) tags[DataTag.Wits];
                    }

                    break;
                case Message.SyncLevel:
                    if (Main.netMode == 2)
                        Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>().level = (int) tags[DataTag.Amount];
                    break;
                case Message.CreateProjectile:
                    try
                    {
                        if (Main.netMode == 1)
                            if ((int) tags[DataTag.PlayerId] == Main.myPlayer)
                                break;

                        int modifierCount = (int) tags[DataTag.ModifierCount];
                        List<GlyphModifier> modifiers = new List<GlyphModifier>();
                        for (int i = 0; i < modifierCount; i += 1)
                            modifiers.Add(GlyphModifier.modifiers[reader.ReadInt32()]);

                        Projectile projectile = Main.projectile[(int) tags[DataTag.ProjId]];
                        if (projectile == null) break;
                        projectile.owner = (int) tags[DataTag.PlayerId];
                        if (!(projectile.modProjectile is ProceduralSpellProj)) break;
                        ProceduralSpellProj ps = (ProceduralSpellProj) projectile.modProjectile;
                        ps.source = new ProceduralSpell(mod) {glyphs = new Item[3]};
                        for (int i = 0; i < ps.source.glyphs.Length; i += 1)
                        {
                            ps.source.glyphs[i] = new Item();
                            ps.source.glyphs[i].SetDefaults(0, true);
                        }

                        ps.source.glyphs[(byte) GLYPHTYPE.STAR].SetDefaults((int) tags[DataTag.GlyphStar], true);
                        ps.source.glyphs[(byte) GLYPHTYPE.CROSS].SetDefaults((int) tags[DataTag.GlyphCross], true);
                        ps.source.glyphs[(byte) GLYPHTYPE.MOON].SetDefaults((int) tags[DataTag.GlyphMoon], true);
                        projectile.damage = (int) tags[DataTag.Damage];
                        projectile.minion = (bool) tags[DataTag.Flag];
                        try
                        {
                            if (projectile.minion)
                                ps.caster = Main.projectile[(int) tags[DataTag.EntityId]];
                            else if (projectile.hostile)
                                ps.caster = Main.npc[(int) tags[DataTag.EntityId]];
                            else
                                ps.caster = Main.player[(int) tags[DataTag.EntityId]];
                        }
                        catch (SystemException e)
                        {
                            Logger.InfoFormat("Source-assignment failed, aborting..." + e);
                            break;
                        }

                        ps.source.modifierOverride = modifiers;
                        foreach (Item item in ps.source.glyphs)
                        {
                            if (item == null)
                                continue;
                            Glyph glyph = (Glyph) item.modItem;
                            if (glyph.GetAiAction() != null)
                                ps.ai.Add(glyph.GetAiAction());
                            if (glyph.GetInitAction() != null)
                                ps.init.Add(glyph.GetInitAction());
                            if (glyph.GetImpactAction() != null)
                                ps.impact.Add(glyph.GetImpactAction());
                            if (glyph.GetKillAction() != null)
                                ps.kill.Add(glyph.GetKillAction());
                        }

                        foreach (GlyphModifier modifier in modifiers)
                        {
                            if (modifier.impact != null)
                                ps.impact.Add(modifier.impact);
                            if (modifier.draw != null)
                                ps.draw.Add(modifier.draw);
                            if (modifier.init != null)
                                ps.init.Add(modifier.init);
                        }

                        ps.Initialize();

                        if (Main.netMode == 2)
                        {
                            ModPacket packet = mod.GetPacket();
                            packet.Write((byte) Message.CreateProjectile);
                            packet.Write(projectile.owner);
                            packet.Write(projectile.whoAmI);
                            packet.Write(ps.source.glyphs[(byte) GLYPHTYPE.STAR].type);
                            packet.Write(ps.source.glyphs[(byte) GLYPHTYPE.CROSS].type);
                            packet.Write(ps.source.glyphs[(byte) GLYPHTYPE.MOON].type);
                            packet.Write(projectile.damage);
                            packet.Write(projectile.minion);
                            packet.Write(ps.caster.whoAmI);
                            List<GlyphModifier> mods = modifiers;
                            packet.Write(mods.Count);
                            for (int j = 0; j < mods.Count; j += 1)
                                packet.Write(mods[j].id);
                            packet.Send();
                        }
                    }
                    catch (SystemException e)
                    {
                        Logger.InfoFormat("Error handling packet: " + msg + " on " + (Main.netMode == 2 ? "serverside" : "clientSide") +
                                          ", full error trace: " + e);
                    }

                    break;
                case Message.AddXP:
                    if (Main.netMode == 1)
                    {
                        //Player player = Main.player[Main.myPlayer];
                        //if (Vector2.Distance(player.Center, Main.npc[(int)tags[DataTag.npcId]].Center) > 1024)
                        //    break;
                        PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();
                        character.AddXp((int) tags[DataTag.Amount]);
                    }

                    break;
                case Message.SyncSpear:
                    ProceduralSpear spear = (ProceduralSpear) Main.projectile[(int) tags[DataTag.ProjId]].modProjectile;
                    spear.blade = SwordBlade.blades[(int) tags[DataTag.PartPrimary]];
                    spear.hilt = SwordHilt.hilts[(int) tags[DataTag.PartSecondary]];
                    spear.accent = SwordAccent.accents[(int) tags[DataTag.PartTertiary]];
                    if (Main.netMode == 1) spear.Initialize();
                    break;
                case Message.SwordInit:
                    if (Main.netMode == 1)
                    {
                        ProceduralSword sword = (ProceduralSword) Main.item[(int) tags[DataTag.ItemId]].modItem;
                        sword.blade = SwordBlade.blades[(int) tags[DataTag.PartPrimary]];
                        sword.hilt = SwordHilt.hilts[(int) tags[DataTag.PartSecondary]];
                        sword.accent = SwordAccent.accents[(int) tags[DataTag.PartTertiary]];
                        sword.dps = (float) tags[DataTag.ItemDps];
                        Main.NewText(sword.dps.ToString(CultureInfo.InvariantCulture));
                        sword.enemyDef = (int) tags[DataTag.ItemDef];
                        sword.Initialize();
                    }

                    break;
                case Message.StaffInit:
                    if (Main.netMode == 1)
                    {
                        ProceduralStaff staff = (ProceduralStaff) Main.item[(int) tags[DataTag.ItemId]].modItem;
                        staff.staff = Staff.staves[(int) tags[DataTag.PartPrimary]];
                        staff.gem = StaffGem.gems[(int) tags[DataTag.PartSecondary]];
                        staff.ornament = StaffOrnament.ornament[(int) tags[DataTag.PartTertiary]];
                        staff.dps = (float) tags[DataTag.ItemDps];
                        staff.enemyDef = (int) tags[DataTag.ItemDef];
                        staff.Initialize();
                    }

                    break;
                case Message.BowInit:
                    if (Main.netMode == 1)
                    {
                        RangedWeapon bow = (RangedWeapon) Main.item[(int) tags[DataTag.ItemId]].modItem;
                        bow.dps = (float) tags[DataTag.ItemDps];
                        bow.enemyDef = (int) tags[DataTag.ItemDef];
                        bow.Initialize();
                    }

                    break;
                case Message.SyncHit:
                    if (Main.netMode == 1)
                    {
                        PlayerCharacter character = Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>();
                        character.accuracyCounter = (float) tags[DataTag.AmountSingle];
                    }

                    break;
                case Message.SyncCritHit:
                    if (Main.netMode == 1)
                    {
                        PlayerCharacter character = Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>();
                        character.critAccuracyCounter = (float) tags[DataTag.AmountSingle];
                    }

                    break;
            }
        }

        public override void Load()
        {
            overhaul = ModLoader.GetMod("TerrariaOverhaul");

            kConfig.Initialize();
            if (Main.netMode != 2)
            {
                GFX.LoadGfx();
                invslot[0] = Main.inventoryBackTexture;
                invslot[1] = Main.inventoryBack2Texture;
                invslot[2] = Main.inventoryBack3Texture;
                invslot[3] = Main.inventoryBack4Texture;
                invslot[4] = Main.inventoryBack5Texture;
                invslot[5] = Main.inventoryBack6Texture;
                invslot[6] = Main.inventoryBack7Texture;
                invslot[7] = Main.inventoryBack8Texture;
                invslot[8] = Main.inventoryBack9Texture;
                invslot[9] = Main.inventoryBack10Texture;
                invslot[10] = Main.inventoryBack11Texture;
                invslot[11] = Main.inventoryBack12Texture;
                invslot[12] = Main.inventoryBack13Texture;
                invslot[13] = Main.inventoryBack14Texture;
                invslot[14] = Main.inventoryBack15Texture;
                invslot[15] = Main.inventoryBack16Texture;
                Main.inventoryBackTexture = GFX.itemSlot;
                Main.inventoryBack2Texture = GFX.itemSlot;
                Main.inventoryBack3Texture = GFX.itemSlot;
                Main.inventoryBack4Texture = GFX.itemSlot;
                Main.inventoryBack5Texture = GFX.itemSlot;
                Main.inventoryBack6Texture = GFX.itemSlot;
                Main.inventoryBack7Texture = GFX.itemSlot;
                Main.inventoryBack8Texture = GFX.itemSlot;
                Main.inventoryBack9Texture = GFX.itemSlot;
                Main.inventoryBack10Texture = GFX.favouritedSlot;
                Main.inventoryBack11Texture = GFX.itemSlot;
                Main.inventoryBack12Texture = GFX.itemSlot;
                Main.inventoryBack13Texture = GFX.itemSlot;
                Main.inventoryBack14Texture = GFX.selectedSlot;
                Main.inventoryBack15Texture = GFX.itemSlot;
                Main.inventoryBack16Texture = GFX.itemSlot;
            }

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
            layers[layers.FindIndex(layer => layer.Name == "Vanilla: Inventory")] = new LegacyGameInterfaceLayer("kRPG", DrawInterface, InterfaceScaleType.UI);
            layers.Find(layer => layer.Name == "Vanilla: Hotbar").Active = false;
        }

        public override void Unload()
        {
            Main.inventoryBackTexture = invslot[0];
            Main.inventoryBack2Texture = invslot[1];
            Main.inventoryBack3Texture = invslot[2];
            Main.inventoryBack4Texture = invslot[3];
            Main.inventoryBack5Texture = invslot[4];
            Main.inventoryBack6Texture = invslot[5];
            Main.inventoryBack7Texture = invslot[6];
            Main.inventoryBack8Texture = invslot[7];
            Main.inventoryBack9Texture = invslot[8];
            Main.inventoryBack10Texture = invslot[9];
            Main.inventoryBack11Texture = invslot[10];
            Main.inventoryBack12Texture = invslot[11];
            Main.inventoryBack13Texture = invslot[12];
            Main.inventoryBack14Texture = invslot[13];
            Main.inventoryBack15Texture = invslot[14];
            Main.inventoryBack16Texture = invslot[15];
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
                Version currentVersion = mod.Version;
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
                        else if (latestVersion == currentVersion && new Version(kConfig.stats.lastStartVersion) < currentVersion)
                        {
                            //After installing a public update
                            Main.NewText("[c/cccccc:KRPG is now up to date!]");
                            Main.NewTextMultiline("[c/cccccc:Summary changes:] " + versionInfo.summary, WidthLimit: 725);
                        }

                        kConfig.stats.lastStartVersion = currentVersion.ToString();
                        kConfig.SaveStats();
                    }
                    catch
                    {
                    }
                };
                client.DownloadStringAsync(new Uri(url), url);
            }
            catch
            {
            }
#pragma warning restore 162
        }

        #endregion
    }
}