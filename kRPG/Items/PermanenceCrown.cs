using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items
{
    public class PermanenceCrown : ModItem
    {
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
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown of Permanence");
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(Main.itemTexture[ModContent.ItemType<PermanenceCrown>()], item.position, Color.White);
            Lighting.AddLight(item.position, 1f, 0.4f, 1f);
        }
    }
}
