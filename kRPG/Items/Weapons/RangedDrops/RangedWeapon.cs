using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.Items.Weapons.RangedDrops
{
    public class RangedWeapon : ModItem
    {
        public float dps = 0f;
        public int enemyDef = 0;
        public string name = "";

        public override bool CanPickup(Player player)
        {
            return item.value > 100;
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 48;
            item.height = 48;
            item.useStyle = 5;
            item.knockBack = 1f;
            item.scale = 1f;
            item.noMelee = true;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
        }

        public void Initialize()
        {
            name = RandomName();
            SetStats();
        }

        public virtual string RandomName()
        {
            return "Generic Name; Please Ignore";
        }

        public virtual void SetStats()
        {
            item.SetNameOverride(name);
            item.rare = (int)Math.Min(Math.Floor(dps / 15.0), 9);
            item.useTime = UseTime();
            item.damage = (int)Math.Round(dps * DPSModifier() * (float)item.useTime / 60f + enemyDef - 2);
            if (item.damage < 1) item.damage = 1;
            item.useTime = (int)Math.Round(((float)item.damage - enemyDef + 2) * 60f / (dps * DPSModifier()));
            item.useAnimation = item.useTime * Iterations() - 1;
            item.value = (int)(dps * 315);
        }

        public override ModItem Clone(Item item)
        {
            RangedWeapon copy = (RangedWeapon)base.Clone(item);
            copy.dps = dps;
            copy.name = name;
            copy.enemyDef = enemyDef;
            copy.item.SetNameOverride(item.Name);
            return copy;
        }

        public virtual int UseTime()
        {
            return 30;
        }

        public virtual float DPSModifier()
        {
            return 1f;
        }

        public virtual int Iterations()
        {
            return 1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Ranged Weapon; Please Ignore");
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                {"dps", dps },
                {"enemyDef", enemyDef },
                {"name", name }
            };
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

        public override void Load(TagCompound tag)
        {
            dps = tag.GetFloat("dps");
            enemyDef = tag.GetInt("enemyDef");
            name = tag.GetString("name");
            SetStats();
        }

        public override void NetRecieve(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;
            dps = reader.ReadSingle();
            enemyDef = reader.ReadInt32();
            name = reader.ReadString();
            SetStats();
        }

        public static int NewRangedWeapon(Mod mod, Vector2 position, int npclevel, int playerlevel, float dps, int enemyDef)
        {
            int combined = npclevel + playerlevel;
            int ammo = 0;
            string weapon = "";
            if (combined >= 35)
            {
                switch(Main.rand.Next(5))
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
            }
            else
            {
                switch(Main.rand.Next(4))
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
            }

            RangedWeapon item = (RangedWeapon)Main.item[Item.NewItem(position, mod.ItemType(weapon))].modItem;
            item.dps = dps;
            item.enemyDef = enemyDef;
            item.Initialize();
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.BowInit);
                packet.Write(item.item.whoAmI);
                packet.Write(item.dps);
                packet.Write(item.enemyDef);
                packet.Send();
            }
            return ammo;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int)Math.Round(dps / 2f)));
        }
    }

    public class GoldenPistol : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Pistol");
        }

        public override string RandomName()
        {
            switch(Main.rand.Next(3))
            {
                default:
                    return "Golden Pistol";
                case 1:
                    return "Golden Revolver";
                case 2:
                    return "Golden Handgun";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 42;
            item.height = 30;
            item.useStyle = 5;
            item.knockBack = 0f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Bullet;
            item.shoot = 10;
            item.shootSpeed = 7f;
            item.damage = 1;
            item.useTime = 30;
            item.scale = 0.8f;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1f, -6f);
        }

        public override int UseTime()
        {
            return 13;
        }
    }

    public class NambuPistol : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nambu Pistol");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(3))
            {
                default:
                    return "Nambu Pistol";
                case 1:
                    return "Exotic Pistol";
                case 2:
                    return "Retrograded Revolver";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 32;
            item.height = 24;
            item.useStyle = 5;
            item.knockBack = 1f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Bullet;
            item.shoot = 10;
            item.shootSpeed = 5.5f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1f, -4f);
        }

        public override int UseTime()
        {
            return 24;
        }

        public override float DPSModifier()
        {
            return 1.1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(6));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }

    public class Longbow : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Longbow");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(4))
            {
                default:
                    return "Longbow";
                case 1:
                    return "Wooden Bow";
                case 2:
                    return "Flatbow";
                case 3:
                    return "Battle Bow";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 14;
            item.height = 44;
            item.useStyle = 5;
            item.knockBack = 0f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Arrow;
            item.shoot = 1;
            item.shootSpeed = 5.5f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item5;
        }

        public override int UseTime()
        {
            return 28;
        }
    }

    public class WoodenBow : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("WoodenBow");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(4))
            {
                default:
                    return "Wooden Bow";
                case 1:
                    return "Recurve Bow";
                case 2:
                    return "Sharpshooter";
                case 3:
                    return "Marksman's Mastery";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 14;
            item.height = 36;
            item.useStyle = 5;
            item.knockBack = 3f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Arrow;
            item.shoot = 1;
            item.shootSpeed = 7.5f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item5;
        }

        public override int UseTime()
        {
            return 41;
        }
    }

    public class AngelBow : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angel Bow");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(6))
            {
                default:
                    return "Feathered Arc";
                case 1:
                    return "Celestial Bow";
                case 2:
                    return "Heavenly Gale";
                case 3:
                    return "Arc of the Heavens";
                case 4:
                    return "Uriel's Glare";
                case 5:
                    return "Angelic War Bow";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 84;
            item.height = 20;
            item.useStyle = 5;
            item.knockBack = 2f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Arrow;
            item.shoot = 1;
            item.shootSpeed = 13f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item5;
        }

        public override int UseTime()
        {
            return 20;
        }
    }

    public class DemonBow : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Bow");
            Tooltip.SetDefault("50% chance to not consume ammo.");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(4))
            {
                default:
                    return "Mockingjay's Song";
                case 1:
                    return "Thamiel's Reign";
                case 2:
                    return "Tempest Eye";
                case 3:
                    return "Nightingale Bow";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 72;
            item.height = 22;
            item.useStyle = 5;
            item.knockBack = 5f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Arrow;
            item.autoReuse = true;
            item.shoot = 1;
            item.shootSpeed = 3.5f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item5;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.5f;
        }

        public override int UseTime()
        {
            return 7;
        }

        public override int Iterations()
        {
            return 4;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(12));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }

        public override float DPSModifier()
        {
            return 1.3f;
        }
    }

    public class Kalashnikov : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kalashnikov");
            Tooltip.SetDefault("33% chance to not consume ammo.");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(4))
            {
                default:
                    return "Kalashnikov";
                case 1:
                    return "Assault Rifle";
                case 2:
                    return "AK-47";
                case 3:
                    return "AK-74";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 76;
            item.height = 24;
            item.useStyle = 5;
            item.knockBack = 1f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Bullet;
            item.shoot = 10;
            item.shootSpeed = 5.5f;
            item.autoReuse = true;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-18f, 4f);
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.33f;
        }

        public override int UseTime()
        {
            return 13;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(6));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }

    public class M16 : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("M16");
            Tooltip.SetDefault("33% chance to not consume ammo.");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(4))
            {
                default:
                    return "M16";
                case 1:
                    return "Assault Rifle";
                case 2:
                    return "Automatic Firearm";
                case 3:
                    return "Autogun";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 32;
            item.height = 24;
            item.useStyle = 5;
            item.knockBack = 1.5f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Bullet;
            item.shoot = 10;
            item.shootSpeed = 5.5f;
            item.autoReuse = true;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14f, 2f);
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.33f;
        }

        public override int UseTime()
        {
            return 10;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(2));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }

    public class Microgun : RangedWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Microgun");
            Tooltip.SetDefault("50% chance to not consume ammo.");
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(5))
            {
                default:
                    return "Minigun";
                case 1:
                    return "Gatling Gun";
                case 2:
                    return "Chaingun";
                case 3:
                    return "XM214";
                case 4:
                    return "XM556";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 32;
            item.height = 24;
            item.useStyle = 5;
            item.knockBack = 1f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Bullet;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 5.5f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4f, 10f);
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.5f;
        }

        public override int UseTime()
        {
            return 8;
        }

        public override float DPSModifier()
        {
            return 1.1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(9));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }
}
