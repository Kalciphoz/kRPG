﻿//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
// 
// 
// THIS SOFTWARE IS PROVIDED BY FairfieldTek LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using kRPG2.Classes;
using kRPG2.Enums;
using kRPG2.Items;
using kRPG2.Items.Glyphs;
using kRPG2.Items.Weapons;
using kRPG2.Items.Weapons.RangedDrops;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace kRPG2
{
    public class kRPG2 : Mod
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

        public kRPG2()
        {
            Properties = new ModProperties {Autoload = true, AutoloadGores = true, AutoloadSounds = true};
            Mod = this;

            //for (int i = 0; i < Lang.inter.Length;i++)
            //{
            //    var t = Lang.inter[i];
            //    Debug.WriteLine($"{i} -- {t.Key} -- {t.Value}");
            //}
        }

        public Texture2D[] InvSlot { get; set; } = new Texture2D[16];

        public static kRPG2 Mod { get; set; }

      //  public static Mod Overhaul { get; set; }

        public bool DrawInterface()
        {
            if (Main.netMode == 2 || Main.gameMenu) return true;
            try
            {
                for (int i = 0; i < BaseGui.GuiElements.Count; i += 1)
                {
                    var gui = BaseGui.GuiElements[i];
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
            var msg = (Message) reader.ReadByte();
            var tags = dataTags[msg].ToDictionary(tag => tag, tag => tag.Read(reader));
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
                        var npc = Main.npc[(int) tags[DataTag.NpcId]];
                        var kn = npc.GetGlobalNPC<kNPC>();
                        var haselement = new Dictionary<ELEMENT, bool>
                        {
                            {ELEMENT.FIRE, (bool) tags[DataTag.Flag]},
                            {ELEMENT.COLD, (bool) tags[DataTag.Flag2]},
                            {ELEMENT.LIGHTNING, (bool) tags[DataTag.Flag3]},
                            {ELEMENT.SHADOW, (bool) tags[DataTag.Flag4]}
                        };
                        int count = Enum.GetValues(typeof(ELEMENT)).Cast<ELEMENT>().Count(element => haselement[element]);
                        int portionsize = (int) Math.Round(npc.damage * kNPC.EleDmgModifier / 2.0 / count);
                        foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                            if (haselement[element])
                                kn.ElementalDamage[element] = Math.Max(1, portionsize);
                        kn.DealsEleDmg = count > 0;
                    }

                    break;
                case Message.PrefixNPC:
                    if (Main.netMode == 1)
                    {
                        var npc = Main.npc[(int) tags[DataTag.NpcId]];
                        var kn = npc.GetGlobalNPC<kNPC>();
                        for (int i = 0; i < (int) tags[DataTag.Amount]; i += 1)
                        {
                            var modifier = kn.ModifierFuncs[reader.ReadInt32()].Invoke(kn, npc);
                            modifier.Read(reader);
                            modifier.Apply();
                            kn.Modifiers.Add(modifier);
                        }

                        kn.MakeNotable(npc);
                    }

                    break;
                case Message.SyncStats:
                    if (Main.netMode == 2)
                    {
                        var character = Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>();
                        character.Level = (int) tags[DataTag.Amount];
                        character.BaseStats[STAT.RESILIENCE] = (int) tags[DataTag.Resilience];
                        character.BaseStats[STAT.QUICKNESS] = (int) tags[DataTag.Quickness];
                        character.BaseStats[STAT.POTENCY] = (int) tags[DataTag.Potency];
                        character.BaseStats[STAT.WITS] = (int) tags[DataTag.Wits];
                    }

                    break;
                case Message.SyncLevel:
                    if (Main.netMode == 2)
                        Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>().Level = (int) tags[DataTag.Amount];
                    break;
                case Message.CreateProjectile:
                    try
                    {
                        if (Main.netMode == 1)
                            if ((int) tags[DataTag.PlayerId] == Main.myPlayer)
                                break;

                        int modifierCount = (int) tags[DataTag.ModifierCount];
                        var modifiers = new List<GlyphModifier>();
                        for (int i = 0; i < modifierCount; i += 1)
                            modifiers.Add(GlyphModifier.Modifiers[reader.ReadInt32()]);

                        var projectile = Main.projectile[(int) tags[DataTag.ProjId]];
                        if (projectile == null) break;
                        projectile.owner = (int) tags[DataTag.PlayerId];
                        if (!(projectile.modProjectile is ProceduralSpellProj)) break;
                        var ps = (ProceduralSpellProj) projectile.modProjectile;
                        ps.Source = new ProceduralSpell(Mod) {Glyphs = new Item[3]};
                        for (int i = 0; i < ps.Source.Glyphs.Length; i += 1)
                        {
                            ps.Source.Glyphs[i] = new Item();
                            ps.Source.Glyphs[i].SetDefaults(0, true);
                        }

                        ps.Source.Glyphs[(byte) GLYPHTYPE.STAR].SetDefaults((int) tags[DataTag.GlyphStar], true);
                        ps.Source.Glyphs[(byte) GLYPHTYPE.CROSS].SetDefaults((int) tags[DataTag.GlyphCross], true);
                        ps.Source.Glyphs[(byte) GLYPHTYPE.MOON].SetDefaults((int) tags[DataTag.GlyphMoon], true);
                        projectile.damage = (int) tags[DataTag.Damage];
                        projectile.minion = (bool) tags[DataTag.Flag];
                        try
                        {
                            if (projectile.minion)
                                ps.Caster = Main.projectile[(int) tags[DataTag.EntityId]];
                            else if (projectile.hostile)
                                ps.Caster = Main.npc[(int) tags[DataTag.EntityId]];
                            else
                                ps.Caster = Main.player[(int) tags[DataTag.EntityId]];
                        }
                        catch (SystemException e)
                        {
                            Logger.InfoFormat("Source-assignment failed, aborting..." + e);
                            break;
                        }

                        ps.Source.ModifierOverride = modifiers;
                        foreach (var item in ps.Source.Glyphs)
                        {
                            if (item == null)
                                continue;
                            var glyph = (Glyph) item.modItem;
                            if (glyph.GetAiAction() != null)
                                ps.ai.Add(glyph.GetAiAction());
                            if (glyph.GetInitAction() != null)
                                ps.Inits.Add(glyph.GetInitAction());
                            if (glyph.GetImpactAction() != null)
                                ps.Impacts.Add(glyph.GetImpactAction());
                            if (glyph.GetKillAction() != null)
                                ps.Kills.Add(glyph.GetKillAction());
                        }

                        foreach (var modifier in modifiers)
                        {
                            if (modifier.Impact != null)
                                ps.Impacts.Add(modifier.Impact);
                            if (modifier.Draw != null)
                                ps.draw.Add(modifier.Draw);
                            if (modifier.Init != null)
                                ps.Inits.Add(modifier.Init);
                        }

                        ps.Initialize();

                        if (Main.netMode == 2)
                        {
                            var packet = Mod.GetPacket();
                            packet.Write((byte) Message.CreateProjectile);
                            packet.Write(projectile.owner);
                            packet.Write(projectile.whoAmI);
                            packet.Write(ps.Source.Glyphs[(byte) GLYPHTYPE.STAR].type);
                            packet.Write(ps.Source.Glyphs[(byte) GLYPHTYPE.CROSS].type);
                            packet.Write(ps.Source.Glyphs[(byte) GLYPHTYPE.MOON].type);
                            packet.Write(projectile.damage);
                            packet.Write(projectile.minion);
                            packet.Write(ps.Caster.whoAmI);
                            var mods = modifiers;
                            packet.Write(mods.Count);
                            for (int j = 0; j < mods.Count; j += 1)
                                packet.Write(mods[j].Id);
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
                        var character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();
                        character.AddXp((int) tags[DataTag.Amount]);
                    }

                    break;
                case Message.SyncSpear:
                    var spear = (ProceduralSpear) Main.projectile[(int) tags[DataTag.ProjId]].modProjectile;
                    spear.Blade = SwordBlade.Blades[(int) tags[DataTag.PartPrimary]];
                    spear.Hilt = SwordHilt.Hilts[(int) tags[DataTag.PartSecondary]];
                    spear.Accent = SwordAccent.Accents[(int) tags[DataTag.PartTertiary]];
                    if (Main.netMode == 1) spear.Initialize();
                    break;
                case Message.SwordInit:
                    if (Main.netMode == 1)
                    {
                        var sword = (ProceduralSword) Main.item[(int) tags[DataTag.ItemId]].modItem;
                        sword.Blade = SwordBlade.Blades[(int) tags[DataTag.PartPrimary]];
                        sword.Hilt = SwordHilt.Hilts[(int) tags[DataTag.PartSecondary]];
                        sword.Accent = SwordAccent.Accents[(int) tags[DataTag.PartTertiary]];
                        sword.Dps = (float) tags[DataTag.ItemDps];
                        Main.NewText(sword.Dps.ToString(CultureInfo.InvariantCulture));
                        sword.EnemyDef = (int) tags[DataTag.ItemDef];
                        sword.Initialize();
                    }

                    break;
                case Message.StaffInit:
                    if (Main.netMode == 1)
                    {
                        var staff = (ProceduralStaff) Main.item[(int) tags[DataTag.ItemId]].modItem;
                        staff.Staff = Staff.Staffs[(int) tags[DataTag.PartPrimary]];
                        staff.Gem = StaffGem.Gems[(int) tags[DataTag.PartSecondary]];
                        staff.Ornament = StaffOrnament.Ornament[(int) tags[DataTag.PartTertiary]];
                        staff.Dps = (float) tags[DataTag.ItemDps];
                        staff.EnemyDef = (int) tags[DataTag.ItemDef];
                        staff.Initialize();
                    }

                    break;
                case Message.BowInit:
                    if (Main.netMode == 1)
                    {
                        var bow = (RangedWeapon) Main.item[(int) tags[DataTag.ItemId]].modItem;
                        bow.dps = (float) tags[DataTag.ItemDps];
                        bow.enemyDef = (int) tags[DataTag.ItemDef];
                        bow.Initialize();
                    }

                    break;
                case Message.SyncHit:
                    if (Main.netMode == 1)
                    {
                        var character = Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>();
                        character.AccuracyCounter = (float) tags[DataTag.AmountSingle];
                    }

                    break;
                case Message.SyncCritHit:
                    if (Main.netMode == 1)
                    {
                        var character = Main.player[(int) tags[DataTag.PlayerId]].GetModPlayer<PlayerCharacter>();
                        character.CritAccuracyCounter = (float) tags[DataTag.AmountSingle];
                    }

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
            layers[layers.FindIndex(layer => layer.Name == "Vanilla: Inventory")] = new LegacyGameInterfaceLayer("kRPG2", DrawInterface, InterfaceScaleType.UI);
            layers.Find(layer => layer.Name == "Vanilla: Hotbar").Active =false;
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
                var client = new WebClient();
                var currentVersion = Mod.Version;
                client.DownloadStringCompleted += (sender, e) =>
                {
                    try
                    {
                        string text = e.Result;
                        var versionInfo = JsonConvert.DeserializeObject<VersionInfo>(text);
                        var latestVersion = new Version(versionInfo.version);
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