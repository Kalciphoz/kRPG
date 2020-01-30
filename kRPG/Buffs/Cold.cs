using Terraria;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    public class Cold : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Chilled");
            Description.SetDefault("Slowed movement and increased chance to receive critical hits");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().hasAilment[ELEMENT.COLD] = true;
            if (npc.velocity.Length() > 0.2f && !npc.boss)
            {
                npc.velocity.Normalize();
                npc.velocity *= 0.2f;
            }
            else if (npc.velocity.Length() > 6f)
            {
                npc.velocity.Normalize();
                npc.velocity *= 6f;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().hasAilment[ELEMENT.COLD] = true;
            if (player.velocity.X > player.maxRunSpeed * 6 / 10)
                player.velocity.X = player.maxRunSpeed * 6 / 10;
        }
    }
}