﻿using System;
using System.Collections.Generic;
using System.IO;
using kRPG.Content.Items.Procedural;
using kRPG.Enums;
using kRPG.Packets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.Content.Items.Weapons.Ranged
{
    public class RangedWeapon : ModItem, IProcedural
    {
        public float dps;
        public int enemyDef;
        public string name = "";

        public override bool CanPickup(Player player)
        {
            return item.value > 100;
        }

        public override ModItem Clone(Item tItem)
        {
            RangedWeapon copy = (RangedWeapon) base.Clone(tItem);
            copy.dps = dps;
            copy.name = name;
            copy.enemyDef = enemyDef;
            copy.item.SetNameOverride(tItem.Name);
            return copy;
        }

        public virtual float DpsModifier()
        {
            return 1f;
        }

        public void Initialize()
        {
            name = RandomName();
            ResetStats();
        }

        public virtual int Iterations()
        {
            return 1;
        }

        public override void Load(TagCompound tag)
        {
            dps = tag.GetFloat("dps");
            enemyDef = tag.GetInt("enemyDef");
            name = tag.GetString("name");
            ResetStats();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int) Math.Round(dps / 2f)));
        }

        public override void NetRecieve(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;
            dps = reader.ReadSingle();
            enemyDef = reader.ReadInt32();
            name = reader.ReadString();
            ResetStats();
        }

        public override void NetSend(BinaryWriter writer)
        {
            bool proceed = item.value > 100;
            writer.Write(proceed);
            if (!proceed) return;
            writer.Write(dps);
            writer.Write(enemyDef);
            writer.Write(name);
        }

        public static int NewRangedWeapon(Mod mod, Vector2 position, int npcLevel, int playerLevel, float dps, int enemyDef)
        {
            int combined = npcLevel + playerLevel;
            int ammo ;
            string weapon ;
            if (combined >= 35)
                switch (Main.rand.Next(5))
                {
                    default:
                        weapon = "AngelBow";
                        ammo = ItemID.WoodenArrow;
                        break;
                    case 1:
                        weapon = "DemonBow";
                        ammo = ItemID.WoodenArrow;
                        break;
                    case 2:
                        weapon = "Kalashnikov";
                        ammo = ItemID.MusketBall;
                        break;
                    case 3:
                        weapon = "M16";
                        ammo = ItemID.MusketBall;
                        break;
                    case 4:
                        weapon = "Microgun";
                        ammo = ItemID.MusketBall;
                        break;
                }
            else
                switch (Main.rand.Next(4))
                {
                    default:
                        weapon = "Longbow";
                        ammo = ItemID.WoodenArrow;
                        break;
                    case 1:
                        weapon = "GoldenPistol";
                        ammo = ItemID.MusketBall;
                        break;
                    case 2:
                        weapon = "WoodenBow";
                        ammo = ItemID.WoodenArrow;
                        break;
                    case 3:
                        weapon = "NambuPistol";
                        ammo = ItemID.MusketBall;
                        break;
                }

            RangedWeapon item = (RangedWeapon) Main.item[Item.NewItem(position, mod.ItemType(weapon))].modItem;
            item.dps = dps;
            item.enemyDef = enemyDef;
            item.Initialize();

            BowInitPacket.Write(item.item.whoAmI,item.dps,item.enemyDef);

            return ammo;
        }

        public virtual string RandomName()
        {
            return "Generic Name; Please Ignore";
        }

        public override TagCompound Save()
        {
            return new TagCompound {{"dps", dps}, {"enemyDef", enemyDef}, {"name", name}};
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 48;
            item.height = 48;
            item.useStyle = (int)UseStyles.HoldingOut;
            item.knockBack = 1f;
            item.scale = 1f;
            item.noMelee = true;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Ranged Weapon; Please Ignore");
        }

        

        public virtual void ResetStats()
        {
            item.SetNameOverride(name);
            item.rare = (int) Math.Min(Math.Floor(dps / 15.0), 9);
            item.useTime = UseTime();
            item.damage = (int) Math.Round(dps * DpsModifier() * item.useTime / 60f + enemyDef - 2);
            if (item.damage < 1) item.damage = 1;
            item.useTime = (int) Math.Round(((float) item.damage - enemyDef + 2) * 60f / (dps * DpsModifier()));
            item.useAnimation = item.useTime * Iterations() - 1;
            item.value = (int) (dps * 315);
        }

        public virtual int UseTime()
        {
            return 30;
        }
    }
}