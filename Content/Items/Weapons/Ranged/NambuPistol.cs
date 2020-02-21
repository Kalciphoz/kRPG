using kRPG.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace kRPG.Content.Items.Weapons.Ranged
{
    public class NambuPistol : RangedWeapon
    {
        public override float DpsModifier()
        {
            return 1.1f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1f, -4f);
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
            item.useStyle = (int)UseStyles.HoldingOut;
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nambu Pistol");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(6));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }

        public override int UseTime()
        {
            return 24;
        }
    }
}