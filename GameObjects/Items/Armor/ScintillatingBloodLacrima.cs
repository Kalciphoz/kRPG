using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.GameObjects.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Armor
{
    public class ScintillatingBloodLacrima : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "Description", "'A large tear shed by a gigantic blood-drinking unicorn'"));
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.value = 100000;
            item.rare = 10;
            item.consumable = true;
            item.useStyle =(int) UseStyles.EatingUsing;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scintillating Blood Lacrima");
            Tooltip.SetDefault("Consume to reset stat points");
        }

        public override bool UseItem(Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
                character.BaseStats[stat] = 0;
            return true;
        }
    }
}