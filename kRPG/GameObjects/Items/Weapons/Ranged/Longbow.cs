using Terraria;
using Terraria.ID;

namespace kRPG.GameObjects.Items.Weapons.Ranged
{
    public class Longbow : RangedWeapon
    {
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Longbow");
        }

        public override int UseTime()
        {
            return 28;
        }
    }
}