using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class RustyKatana : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 9;
            item.melee = true;
            item.width = 36;
            item.height = 36;
            item.useTime = 29;
            item.useAnimation = 29;
            item.autoReuse = true;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 3f;
            item.value = 2150;
            item.crit = 3;
            item.UseSound = SoundID.Item1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Katana");
        }
    }
}
