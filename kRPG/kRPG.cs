using kRPG.GUI;
using kRPG.Items;
using kRPG.Items.Glyphs;
using kRPG.Items.Weapons;
using kRPG.Items.Weapons.RangedDrops;
using kRPG.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace kRPG
{
	public enum STAT : byte { RESILIENCE, QUICKNESS, POTENCY, WITS };
    public enum ELEMENT : byte { FIRE, COLD, LIGHTNING, SHADOW };
    public enum RITUAL : byte { DEMON_PACT, WARRIOR_OATH, ELAN_VITAL, STONE_ASPECT, ELDRITCH_FURY, MIND_FORTRESS, BLOOD_DRINKING };

    public enum Message : byte { AddXP, CreateProjectile, SwordInit, StaffInit, BowInit, SyncHit, SyncCritHit, SyncLevel, InitProjEleDmg, SyncStats, SyncSpear, PrefixNPC, NPCEleDmg };

    public class DataTag
    {
        public static DataTag amount = new DataTag(reader => reader.ReadInt32());
        public static DataTag amount_single = new DataTag(reader => reader.ReadSingle());
        public static DataTag playerId = new DataTag(reader => reader.ReadInt32());
        public static DataTag npcId = new DataTag(reader => reader.ReadInt32());
        public static DataTag projId = new DataTag(reader => reader.ReadInt32());
        public static DataTag entityId = new DataTag(reader => reader.ReadInt32());
        public static DataTag targetX = new DataTag(reader => reader.ReadSingle());
        public static DataTag targetY = new DataTag(reader => reader.ReadSingle());
        public static DataTag glyph_star = new DataTag(reader => reader.ReadInt32());
        public static DataTag glyph_cross = new DataTag(reader => reader.ReadInt32());
        public static DataTag glyph_moon = new DataTag(reader => reader.ReadInt32());
        public static DataTag damage = new DataTag(reader => reader.ReadInt32());
        public static DataTag projCount = new DataTag(reader => reader.ReadInt32());
        public static DataTag modifierCount = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemId = new DataTag(reader => reader.ReadInt32());
        public static DataTag partPrimary = new DataTag(reader => reader.ReadInt32());
        public static DataTag partSecondary = new DataTag(reader => reader.ReadInt32());
        public static DataTag partTertiary = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemDps = new DataTag(reader => reader.ReadSingle());
        public static DataTag itemDef = new DataTag(reader => reader.ReadInt32());
        public static DataTag flag = new DataTag(reader => reader.ReadBoolean());
        public static DataTag flag2 = new DataTag(reader => reader.ReadBoolean());
        public static DataTag flag3 = new DataTag(reader => reader.ReadBoolean());
        public static DataTag flag4 = new DataTag(reader => reader.ReadBoolean());
        public static DataTag fire = new DataTag(reader => reader.ReadInt32());
        public static DataTag cold = new DataTag(reader => reader.ReadInt32());
        public static DataTag lightning = new DataTag(reader => reader.ReadInt32());
        public static DataTag shadow = new DataTag(reader => reader.ReadInt32());
        public static DataTag resilience = new DataTag(reader => reader.ReadInt32());
        public static DataTag quickness = new DataTag(reader => reader.ReadInt32());
        public static DataTag potency = new DataTag(reader => reader.ReadInt32());
        public static DataTag wits = new DataTag(reader => reader.ReadInt32());
        public Func<BinaryReader, object> read;
        public DataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }

    public class kRPG : Mod
    {
		public static kRPG mod;

		public static Mod overhaul;

        public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>()
        {
            { Message.AddXP, new List<DataTag>(){ DataTag.amount, DataTag.npcId } },
            { Message.CreateProjectile, new List<DataTag>(){ DataTag.playerId, DataTag.projId, DataTag.glyph_star, DataTag.glyph_cross, DataTag.glyph_moon, DataTag.damage, DataTag.flag, DataTag.entityId, DataTag.modifierCount } },
            { Message.SwordInit, new List<DataTag>(){ DataTag.itemId, DataTag.partPrimary, DataTag.partSecondary, DataTag.partTertiary, DataTag.itemDps, DataTag.itemDef } },
            { Message.StaffInit, new List<DataTag>(){ DataTag.itemId, DataTag.partPrimary, DataTag.partSecondary, DataTag.partTertiary, DataTag.itemDps, DataTag.itemDef } },
            { Message.BowInit, new List<DataTag>(){ DataTag.itemId, DataTag.itemDps, DataTag.itemDef } },
            { Message.SyncHit, new List<DataTag>(){ DataTag.playerId, DataTag.amount_single } },
            { Message.SyncCritHit, new List<DataTag>(){ DataTag.playerId, DataTag.amount_single } },
            { Message.SyncLevel, new List<DataTag>(){ DataTag.playerId, DataTag.amount } },
            { Message.InitProjEleDmg, new List<DataTag>(){ DataTag.projId, DataTag.fire, DataTag.cold, DataTag.lightning, DataTag.shadow } },
            { Message.SyncStats, new List<DataTag>(){ DataTag.playerId, DataTag.amount, DataTag.resilience, DataTag.quickness, DataTag.potency, DataTag.wits } },
            { Message.SyncSpear, new List<DataTag>(){ DataTag.projId, DataTag.partPrimary, DataTag.partSecondary, DataTag.partTertiary } },
            { Message.PrefixNPC, new List<DataTag>(){ DataTag.npcId, DataTag.amount } },
            { Message.NPCEleDmg, new List<DataTag>(){ DataTag.npcId, DataTag.flag, DataTag.flag2, DataTag.flag3, DataTag.flag4 } }
        };
        public Texture2D[] invslot = new Texture2D[16];


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message)reader.ReadByte();
            Dictionary<DataTag, object> tags = new Dictionary<DataTag, object>();
            foreach (DataTag tag in dataTags[msg])
                tags.Add(tag, tag.read(reader));
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
                        NPC npc = Main.npc[(int)tags[DataTag.npcId]];
                        kNPC kn = npc.GetGlobalNPC<kNPC>();
                        Dictionary<ELEMENT, bool> haselement = new Dictionary<ELEMENT, bool>()
                        {
                            { ELEMENT.FIRE, (bool)tags[DataTag.flag] },
                            { ELEMENT.COLD, (bool)tags[DataTag.flag2] },
                            { ELEMENT.LIGHTNING, (bool)tags[DataTag.flag3] },
                            { ELEMENT.SHADOW, (bool)tags[DataTag.flag4] }
                        };
                        int count = 0;
                        foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                            if (haselement[element]) count += 1;
                        int portionsize = (int)Math.Round((double)npc.damage * kNPC.ELE_DMG_MODIFIER / 2.0 / count);
                        foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                            if (haselement[element]) kn.elementalDamage[element] = Math.Max(1, portionsize);
                        kn.dealseledmg = count > 0;
                    }
                    break;
                //case Message.PrefixNPC:
                //    if (Main.netMode == 1)
                //    {
                //        NPC npc = Main.npc[(int)tags[DataTag.npcId]];
                //        kNPC kn = npc.GetGlobalNPC<kNPC>();
                //        kn.Prefix(npc, (int)tags[DataTag.amount]);
                //    }
                //    break;
                case Message.SyncStats:
                    if (Main.netMode == 2)
                    {
                        PlayerCharacter character = Main.player[(int)tags[DataTag.playerId]].GetModPlayer<PlayerCharacter>();
                        character.level = (int)tags[DataTag.amount];
                        character.baseStats[STAT.RESILIENCE] = (int)tags[DataTag.resilience];
                        character.baseStats[STAT.QUICKNESS] = (int)tags[DataTag.quickness];
                        character.baseStats[STAT.POTENCY] = (int)tags[DataTag.potency];
                        character.baseStats[STAT.WITS] = (int)tags[DataTag.wits];
                    }
                    break;
                case Message.SyncLevel:
                    if (Main.netMode == 2)
                        Main.player[(int)tags[DataTag.playerId]].GetModPlayer<PlayerCharacter>().level = (int)tags[DataTag.amount];
                    break;
                case Message.CreateProjectile:
                    try
                    {
                        if (Main.netMode == 1)
                            if ((int)tags[DataTag.playerId] == Main.myPlayer)
                                break;

                        int modifierCount = (int)tags[DataTag.modifierCount];
                        List<GlyphModifier> modifiers = new List<GlyphModifier>();
                        for (int i = 0; i < modifierCount; i += 1)
                            modifiers.Add(GlyphModifier.modifiers[reader.ReadInt32()]);

                        Projectile projectile = Main.projectile[(int)tags[DataTag.projId]];
                        if (projectile == null) break;
                        projectile.owner = (int)tags[DataTag.playerId];
                        if (!(projectile.modProjectile is ProceduralSpellProj)) break;
                        ProceduralSpellProj ps = (ProceduralSpellProj)projectile.modProjectile;
                        ps.source = new ProceduralSpell(mod);
                        ps.source.glyphs = new Item[3];
                        for (int i = 0; i < ps.source.glyphs.Length; i += 1)
                        {
                            ps.source.glyphs[i] = new Item();
                            ps.source.glyphs[i].SetDefaults(0, true);
                        }
                        ps.source.glyphs[(byte)GLYPHTYPE.STAR].SetDefaults((int)tags[DataTag.glyph_star], true);
                        ps.source.glyphs[(byte)GLYPHTYPE.CROSS].SetDefaults((int)tags[DataTag.glyph_cross], true);
                        ps.source.glyphs[(byte)GLYPHTYPE.MOON].SetDefaults((int)tags[DataTag.glyph_moon], true);
                        projectile.damage = (int)tags[DataTag.damage];
                        projectile.minion = (bool)tags[DataTag.flag];
                        try
                        {
                            if (projectile.minion)
                                ps.caster = Main.projectile[(int)tags[DataTag.entityId]];
                            else if (projectile.hostile)
                                ps.caster = Main.npc[(int)tags[DataTag.entityId]];
                            else
                                ps.caster = Main.player[(int)tags[DataTag.entityId]];
                        }
                        catch (SystemException e)
                        {
                            ErrorLogger.Log("Source-assignment failed, aborting...");
                            break;
                        }
                        ps.source.modifierOverride = modifiers;
                        foreach (Item item in ps.source.glyphs)
                        {
                            if (item != null)
                            {
                                Glyph glyph = (Glyph)item.modItem;
                                if (glyph.GetAIAction() != null)
                                    ps.ai.Add(glyph.GetAIAction());
                                if (glyph.GetInitAction() != null)
                                    ps.init.Add(glyph.GetInitAction());
                                if (glyph.GetImpactAction() != null)
                                    ps.impact.Add(glyph.GetImpactAction());
                                if (glyph.GetKillAction() != null)
                                    ps.kill.Add(glyph.GetKillAction());
                            }
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
                            packet.Write((byte)Message.CreateProjectile);
                            packet.Write(projectile.owner);
                            packet.Write(projectile.whoAmI);
                            packet.Write(ps.source.glyphs[(byte)GLYPHTYPE.STAR].type);
                            packet.Write(ps.source.glyphs[(byte)GLYPHTYPE.CROSS].type);
                            packet.Write(ps.source.glyphs[(byte)GLYPHTYPE.MOON].type);
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
                        ErrorLogger.Log("Error handling packet: " + msg.ToString() + " on " + (Main.netMode == 2 ? "serverside" : "clientside") + ", full error trace: " + e.ToString());
                    }
                    break;
                case Message.AddXP:
                    if (Main.netMode == 1)
                    {
                        //Player player = Main.player[Main.myPlayer];
                        //if (Vector2.Distance(player.Center, Main.npc[(int)tags[DataTag.npcId]].Center) > 1024)
                        //    break;
                        PlayerCharacter character = Main.LocalPlayer.GetModPlayer<PlayerCharacter>();
                        character.AddXP((int)tags[DataTag.amount]);
                    }
                    break;
                case Message.SyncSpear:
                    ProceduralSpear spear = (ProceduralSpear)Main.projectile[(int)tags[DataTag.projId]].modProjectile;
                    spear.blade = SwordBlade.blades[(int)tags[DataTag.partPrimary]];
                    spear.hilt = SwordHilt.hilts[(int)tags[DataTag.partSecondary]];
                    spear.accent = SwordAccent.accents[(int)tags[DataTag.partTertiary]];
                    if (Main.netMode == 1) spear.Initialize();
                    break;
                case Message.SwordInit:
                    if (Main.netMode == 1)
                    {
                        ProceduralSword sword = (ProceduralSword)Main.item[(int)tags[DataTag.itemId]].modItem;
                        sword.blade = SwordBlade.blades[(int)tags[DataTag.partPrimary]];
                        sword.hilt = SwordHilt.hilts[(int)tags[DataTag.partSecondary]];
                        sword.accent = SwordAccent.accents[(int)tags[DataTag.partTertiary]];
                        sword.dps = (float)tags[DataTag.itemDps];
                        Main.NewText(sword.dps.ToString());
                        sword.enemyDef = (int)tags[DataTag.itemDef];
                        sword.Initialize();
                    }
                    break;
                case Message.StaffInit:
                    if (Main.netMode == 1)
                    {
                        ProceduralStaff staff = (ProceduralStaff)Main.item[(int)tags[DataTag.itemId]].modItem;
                        staff.staff = Staff.staves[(int)tags[DataTag.partPrimary]];
                        staff.gem = StaffGem.gems[(int)tags[DataTag.partSecondary]];
                        staff.ornament = StaffOrnament.ornament[(int)tags[DataTag.partTertiary]];
                        staff.dps = (float)tags[DataTag.itemDps];
                        staff.enemyDef = (int)tags[DataTag.itemDef];
                        staff.Initialize();
                    }
                    break;
                case Message.BowInit:
                    if (Main.netMode == 1)
                    {
                        RangedWeapon bow = (RangedWeapon)Main.item[(int)tags[DataTag.itemId]].modItem;
                        bow.dps = (float)tags[DataTag.itemDps];
                        bow.enemyDef = (int)tags[DataTag.itemDef];
                        bow.Initialize();
                    }
                    break;
                case Message.SyncHit:
                    if (Main.netMode == 1)
                    {
                        PlayerCharacter character = Main.player[(int)tags[DataTag.playerId]].GetModPlayer<PlayerCharacter>();
                        character.accuracyCounter = (float)tags[DataTag.amount_single];
                    }
                    break;
                case Message.SyncCritHit:
                    if (Main.netMode == 1)
                    {
                        PlayerCharacter character = Main.player[(int)tags[DataTag.playerId]].GetModPlayer<PlayerCharacter>();
                        character.critAccuracyCounter = (float)tags[DataTag.amount_single];
                    }
                    break;
            }
        }

        public static Dictionary<string, RITUAL> ritualByName = new Dictionary<string, RITUAL>()
        {
            {"demon_pact", RITUAL.DEMON_PACT},
            {"warrior_oath", RITUAL.WARRIOR_OATH},
            {"elan_vital", RITUAL.ELAN_VITAL},
            {"stone_aspect", RITUAL.STONE_ASPECT},
            {"eldritch_fury", RITUAL.ELDRITCH_FURY},
            {"mind_fortress", RITUAL.MIND_FORTRESS},
            {"blood_drinking", RITUAL.BLOOD_DRINKING}
        };

        public kRPG() : base()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
			mod = this;
        }

        public override void ModifyInterfaceLayers(List<Terraria.UI.GameInterfaceLayer> layers)
        {
            layers.Find(layer => layer.Name == "Vanilla: Resource Bars").Active = false;
            layers[layers.FindIndex(layer => layer.Name == "Vanilla: Inventory")] = new LegacyGameInterfaceLayer("kRPG", new GameInterfaceDrawMethod(DrawInterface), InterfaceScaleType.UI);
            layers.Find(layer => layer.Name == "Vanilla: Hotbar").Active = false;
        }

        public override void Load()
        {
			overhaul = ModLoader.GetMod("TerrariaOverhaul");

            kConfig.Initialize();
            if (Main.netMode != 2)
            {
                GFX.LoadGFX(ModLoader.GetMod("kRPG"));
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
            GFX.UnloadGFX();
            SwordBlade.Unload();
            SwordHilt.Unload();
            SwordAccent.Unload();
            StaffGem.Unload();
            Staff.Unload();
            StaffOrnament.Unload();
            Main.instance.invBottom = 210;
        }

        public bool DrawInterface()
        {
            if (Main.netMode == 2 || Main.gameMenu) return true;
            try
            {
                for (int i = 0; i < BaseGUI.gui_elements.Count; i += 1)
                {
                    BaseGUI gui = BaseGUI.gui_elements[i];
                    if (gui.PreDraw())
                    {
                        gui.Draw(Main.spriteBatch, Main.LocalPlayer);
                    }
                }
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
            return true;
        }

		#region UpdateCheck
		public class VersionInfo
		{
			public string version;
			public string summary;
		}
		//Mirsario's code in Mirsario's code style
		public static void CheckForUpdates()
		{
			#pragma warning disable 162
			try {
				string url=				@"http://raw.githubusercontent.com/Kalciphoz/kRPG/master/kRPG_VersionInfo.json";
				WebClient client=		new WebClient();
				Version currentVersion=	mod.Version;
				client.DownloadStringCompleted+= (sender,e) => { 
					try {
						string text=				e.Result;
						VersionInfo versionInfo=	JsonConvert.DeserializeObject<VersionInfo>(text);
						Version latestVersion=		new Version(versionInfo.version);
						if(latestVersion>currentVersion) {
							//Public update available
							Main.NewText("[c/cccccc:New version of] [c/ffdb00:KRPG] [c/cccccc:available]");
							Main.NewTextMultiline("[c/cccccc:Summary:] "+versionInfo.summary,WidthLimit:725);
							Main.NewText("[c/cccccc:Get the update from Mod Browser]");
						}else if(latestVersion==currentVersion && new Version(kConfig.stats.lastStartVersion)<currentVersion) {
							//After installing a public update
							Main.NewText("[c/cccccc:KRPG is now up to date!]");
							Main.NewTextMultiline("[c/cccccc:Summary changes:] "+versionInfo.summary,WidthLimit:725);
						}
						kConfig.stats.lastStartVersion=	currentVersion.ToString();
						kConfig.SaveStats();
					}
					catch {}
				};
				client.DownloadStringAsync(new Uri(url),url);
			}
			catch {}
			#pragma warning restore 162
		}
		#endregion
	}
}
