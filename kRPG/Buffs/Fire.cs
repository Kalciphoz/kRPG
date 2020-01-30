using Terraria;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    public class Fire : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Burning");
            Description.SetDefault("Taking fire damage over time");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().hasAilment[ELEMENT.FIRE] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().hasAilment[ELEMENT.FIRE] = true;
        }
    }
}