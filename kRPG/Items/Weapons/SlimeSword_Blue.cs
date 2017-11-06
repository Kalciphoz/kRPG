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
    public class SlimeSword_Blue : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 8;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = 1;
            item.knockBack = 7.4f;
            item.crit = -4;
            item.value = 1375;
            item.scale = 1.1f;
            item.UseSound = SoundID.Item1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Sword");
        }
    }
}
