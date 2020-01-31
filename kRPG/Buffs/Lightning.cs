using kRPG.Enums;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    /// <summary>
    ///     Lightning debuff
    /// </summary>
    public class Lightning : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shocked");
            Description.SetDefault("Take increased damage");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().hasAilment[ELEMENT.LIGHTNING] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().hasAilment[ELEMENT.LIGHTNING] = true;
        }
    }
}