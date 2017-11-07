using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Microsoft.Xna.Framework;
using kRPG.Items.Weapons;
using kRPG.Items.Glyphs;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using kRPG.Items;
using System;
using kRPG.Items.Weapons.RangedDrops;

namespace kRPG
{
    public enum STAT : byte { RESILIENCE, QUICKNESS, POTENCY, WITS };
    public enum ELEMENT : byte { SHADOW, LIGHTNING, COLD, FIRE };
    public enum RITUAL : byte { DEMON_PACT, WARRIOR_OATH, ELAN_VITAL, STONE_ASPECT, ELDRITCH_FURY, MIND_FORTRESS, BLOOD_DRINKING };

    public enum Message : byte { AddXP, CastSpell, SwordInit, StaffInit, BowInit, SyncHit, SyncCritHit };

    public class DataTag
    {
        public static DataTag amount = new DataTag(reader => reader.ReadInt32());
        public static DataTag amount_single = new DataTag(reader => reader.ReadSingle());
        public static DataTag playerId = new DataTag(reader => reader.ReadInt32());
        public static DataTag npcId = new DataTag(reader => reader.ReadInt32());
        public static DataTag spellId = new DataTag(reader => reader.ReadInt32());
        public static DataTag targetX = new DataTag(reader => reader.ReadSingle());
        public static DataTag targetY = new DataTag(reader => reader.ReadSingle());
        public static DataTag itemId = new DataTag(reader => reader.ReadInt32());
        public static DataTag partPrimary = new DataTag(reader => reader.ReadInt32());
        public static DataTag partSecondary = new DataTag(reader => reader.ReadInt32());
        public static DataTag partTertiary = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemDps = new DataTag(reader => reader.ReadSingle());
        public static DataTag itemDef = new DataTag(reader => reader.ReadInt32());
        public Func<BinaryReader, object> read;
        public DataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }

    public class kRPG : Mod
    {
        public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>()
        {
            { Message.AddXP, new List<DataTag>(){ DataTag.amount, DataTag.npcId } },
            { Message.CastSpell, new List<DataTag>(){ DataTag.playerId, DataTag.spellId, DataTag.targetX, DataTag.targetY } },
            { Message.SwordInit, new List<DataTag>(){ DataTag.itemId, DataTag.partPrimary, DataTag.partSecondary, DataTag.partTertiary, DataTag.itemDps, DataTag.itemDef } },
            { Message.StaffInit, new List<DataTag>(){ DataTag.itemId, DataTag.partPrimary, DataTag.partSecondary, DataTag.partTertiary, DataTag.itemDps, DataTag.itemDef } },
            { Message.BowInit, new List<DataTag>(){ DataTag.itemId, DataTag.itemDps, DataTag.itemDef } },
            { Message.SyncHit, new List<DataTag>(){ DataTag.playerId, DataTag.amount_single } },
            { Message.SyncCritHit, new List<DataTag>(){ DataTag.playerId, DataTag.amount_single } }
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
                case Message.CastSpell:
                    if (Main.netMode == 2)
                    {
                        Player player = Main.player[(int)tags[DataTag.playerId]];
                        PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                        character.abilities[(int)tags[DataTag.spellId]].UseAbility(player, new Vector2((float)tags[DataTag.targetX], (float)tags[DataTag.targetY]));
                    }
                    break;
                case Message.AddXP:
                    if (Main.netMode == 1)
                    {
                        Player player = Main.player[Main.myPlayer];
                        //if (Vector2.Distance(player.Center, Main.npc[(int)tags[DataTag.npcId]].Center) > 1024)
                        //    break;
                        PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                        character.AddXP((int)tags[DataTag.amount]);
                    }
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
        }

        public override void ModifyInterfaceLayers(List<Terraria.UI.GameInterfaceLayer> layers)
        {
            layers.Remove(layers.Find(layer => layer.Name == "Vanilla: Resource Bars"));
            layers.Remove(layers.Find(layer => layer.Name == "Vanilla: Inventory"));
            layers.Remove(layers.Find(layer => layer.Name == "Vanilla: Hotbar"));
        }

        public override void Load()
        {
            kConfig.Initialize();
            if (Main.netMode != 2)
            {
                GFX.LoadGFX(this);
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
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.netMode == 2) return;
            foreach (BaseGUI gui in BaseGUI.gui_elements)
            {
                if (gui.PreDraw())
                {
                    gui.Draw(spriteBatch, Main.LocalPlayer);
                }
            }
        }
    }
}
