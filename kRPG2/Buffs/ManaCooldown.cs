using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG2.Buffs
{
    /// <summary>
    ///     Mana Cooldown
    /// </summary>
    public class ManaCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            //Name shown when in effect
            DisplayName.SetDefault("Mana Sickness");
            //Description of effect
            Description.SetDefault("Cannot use mana potions");
            //This buff is a debuff
            Main.debuff[Type] = true;
            //Don't save the effect on the player when they leave the game.
            Main.buffNoSave[Type] = true;
        }

        /// <summary>
        ///     Player Update
        /// </summary>
        /// <param name="player"></param>
        /// <param name="buffIndex"></param>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().CanHealMana = false;
            player.ClearBuff(BuffID.ManaSickness);
        }
    }
}