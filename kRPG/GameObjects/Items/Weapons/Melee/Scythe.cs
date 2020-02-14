using kRPG.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Weapons.Melee
{
    public class Scythe : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 14;
            item.mana = 4;
            item.magic = true;
            item.noMelee = true;
            item.width = 52;
            item.height = 52;
            item.useTime = 31;
            item.useAnimation = 31;
            item.useStyle = (int)UseStyles.GeneralSwingingThrowing;
            item.autoReuse = true;
            item.shoot = 274;
            item.shootSpeed = 9f;
            item.knockBack = 6.5f;
            item.value = 12000;
            item.UseSound = SoundID.Item1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Scythe");
        }
    }
}