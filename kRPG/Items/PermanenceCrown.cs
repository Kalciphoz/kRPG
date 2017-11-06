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
using Microsoft.Xna.Framework.Graphics;

namespace kRPG.Items
{
    public class PermanenceCrown : ModItem
    {
        public int charges = 3;

        public override bool CloneNewInstances
        {
            get { return true;  }
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
            item.value = 62500;
            item.rare = 5;
            charges = 3;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown of Permanence");
            Tooltip.SetDefault("When an upgrade fails, items remain but are downgraded");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "tooltip2", "Has " + charges.ToString() + " charges"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "GloryPoint");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(Main.itemTexture[mod.ItemType<PermanenceCrown>()], item.position, Color.White);
            Lighting.AddLight(item.position, 1f, 0.4f, 1f);
        }
    }
}
