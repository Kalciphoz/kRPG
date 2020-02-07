using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace kRPG.GameObjects.Items.Weapons.Ranged
{
    public class DemonBow : RangedWeapon
    {
        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.5f;
        }

        public override float DpsModifier()
        {
            return 1.3f;
        }

        public override int Iterations()
        {
            return 4;
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Bow");
            Tooltip.SetDefault("50% chance to not consume ammo.");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(12));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }

        public override int UseTime()
        {
            return 7;
        }
    }
}