using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items
{
    public class BlacksmithCrown : ModItem
    {
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<BlacksmithCrown>()], item.position, Color.White);
            Lighting.AddLight(item.position, 0f, 0.92f, 1f);
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
            item.value = 62500;
            item.rare = 9;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown of Transcendence");
        }
    }
}