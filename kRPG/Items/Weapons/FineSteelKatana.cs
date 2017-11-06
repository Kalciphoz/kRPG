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
    public class FineSteelKatana : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 13;
            item.melee = true;
            item.width = 42;
            item.height = 42;
            item.useTime = 22;
            item.useAnimation = 22;
            item.mana = 3;
            item.autoReuse = true;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 2.5f;
            item.value = 2750;
            item.crit = 12;
            item.UseSound = SoundID.Item1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Steel Chokuto");
        }
    }
}
