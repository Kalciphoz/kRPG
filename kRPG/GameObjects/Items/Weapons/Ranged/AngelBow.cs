using Terraria;
using Terraria.ID;

namespace kRPG.GameObjects.Items.Weapons.Ranged
{
    public class AngelBow : RangedWeapon
    {
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angel Bow");
        }

        public override int UseTime()
        {
            return 20;
        }
    }
}