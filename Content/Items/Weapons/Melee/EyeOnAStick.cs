using kRPG.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Weapons.Melee
{
    public class EyeOnAStick : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 10;
            item.melee = true;
            item.width = 30;
            item.height = 30;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = (int)UseStyles.GeneralSwingingThrowing;
            item.knockBack = 4f;
            item.value = 8000;
            item.UseSound = SoundID.Item1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye on a Stick");
        }
    }
}