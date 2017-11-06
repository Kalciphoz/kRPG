using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    public class ManaCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mana Sickness");
            Description.SetDefault("Cannot use mana potions");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().canHealMana = false;
            player.ClearBuff(BuffID.ManaSickness);
        }
    }
}
