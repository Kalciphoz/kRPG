using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace kRPG.Items
{
    public class GuardianCrown : ModItem
    {
        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
            item.value = 62500;
            item.rare = 2;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown of Experience");
            Tooltip.SetDefault("Increases success chance of upgrades by 10%\nat the cost of a much higher price in currency");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "GloryPoint");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
