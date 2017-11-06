using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace kRPG.Items
{
    public class GloryPoint : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 24;
            item.value = 62500;
            item.maxStack = 100;
            item.rare = 8;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glory Point");
        }
    }
}
