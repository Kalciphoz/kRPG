using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace kRPG.Items.Weapons
{
    public class Arbalest : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 18;
            item.noMelee = true;
            item.ranged = true;
            item.width = 30;
            item.height = 22;
            item.useTime = 44;
            item.useAnimation = 44;
            item.useStyle = 5;
            item.shootSpeed = 11f;
            item.knockBack = 5.5f;
            item.shoot = 1;
            item.crit = 12;
            item.useAmmo = AmmoID.Arrow;
            item.value = 6000;
            item.UseSound = SoundID.Item5;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arbalest");
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, -2f);
        }
    }
}
