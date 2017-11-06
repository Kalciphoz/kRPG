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
    public class BarbarianSword : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 12;
            item.melee = true;
            item.width = 46;
            item.height = 46;
            item.useTime = 33;
            item.useAnimation = 33;
            item.useStyle = 1;
            item.knockBack = 6.2f;
            item.value = 3275;
            item.UseSound = SoundID.Item1;
            item.scale = 1.1f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barbarian Longsword");
        }
    }
}
