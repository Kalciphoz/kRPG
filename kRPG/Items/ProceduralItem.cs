using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items
{
    public class ProceduralItem : ModItem
    {
        public float dps;
        public int enemyDef;
        public Texture2D texture;

        public override bool CanPickup(Player player)
        {
            if (Main.netMode == 0) return true;
            return item.value > 100;
        }

        public override ModItem Clone(Item item)
        {
            ProceduralItem copy = (ProceduralItem) base.Clone(item);
            copy.texture = texture;
            return copy;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
        }

        public virtual void Initialize()
        {
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int) Math.Round(dps / 2)));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin,
            float scale)
        {
            if (Main.netMode == 2 || texture == null) return false;
            if (Main.itemTexture[item.type] == null) Main.itemTexture[item.type] = texture;
            float s = scale * Main.itemTexture[item.type].Height / texture.Height;
            Draw(spriteBatch, position, drawColor, 0f, s);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Main.netMode == 2 || texture == null) return false;
            Draw(spriteBatch, item.position - Main.screenPosition, lightColor, rotation, scale);
            return false;
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.scale = 1f;
            item.noMelee = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Item; Please Ignore");
        }
    }
}