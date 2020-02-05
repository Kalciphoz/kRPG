using Terraria;
using Terraria.ID;

namespace kRPG.Items.Weapons.RangedDrops
{
    public class WoodenBow : RangedWeapon
    {
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("WoodenBow");
        }

        public override int UseTime()
        {
            return 41;
        }
    }
}