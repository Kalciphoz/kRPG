using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace kRPG.GameObjects.Items.Weapons.Ranged
{
    public class GoldenPistol : RangedWeapon
    {
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1f, -6f);
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(3))
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Pistol");
        }

        public override int UseTime()
        {
            return 13;
        }
    }
}