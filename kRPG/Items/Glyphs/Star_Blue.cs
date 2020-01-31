using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Star_Blue : Star
    {
        public override float BaseDamageModifier()
        {
            return 1.25f;
        }

        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 target)
            {
                spell.remaining = spell.cooldown;
                spell.CastSpell(player, player.Center, target, player);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Star Glyph");
            Tooltip.SetDefault("Casts spell directly");
        }
    }
}