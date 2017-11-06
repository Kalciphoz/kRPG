using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace kRPG.Items.Weapons
{
    public class ChainHook : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 11;
            item.melee = true;
            item.width = 36;
            item.height = 32;
            item.useTime = 44;
            item.useAnimation = 44;
            item.useStyle = 5;
            item.shootSpeed = 16;
            item.shoot = mod.ProjectileType("ChainHook");
            item.knockBack = 6.5f;
            item.value = 2475;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chain-Hook");
        }
    }
}
