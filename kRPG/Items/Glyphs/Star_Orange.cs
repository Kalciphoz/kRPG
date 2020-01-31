using System;
using System.Linq;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class Star_Orange : Star
    {
        public override float BaseDamageModifier()
        {
            return 0.6f;
        }

        public override float BaseManaModifier()
        {
            return 1.35f;
        }

        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 target)
            {
                Main.PlaySound(SoundID.Item6, player.position);
                spell.remaining = spell.cooldown;
                var character = player.GetModPlayer<PlayerCharacter>();
                if (character.minions.Exists(minion => minion is WingedEyeball))
                    foreach (var eyeball in character.minions.Where(minion => minion.projectile.type == ModContent.ProjectileType<WingedEyeball>()))
                    {
                        foreach (var psp in eyeball.circlingProtection)
                            psp.projectile.Kill();
                        eyeball.circlingProtection.Clear();
                        eyeball.smallProt?.projectile.Kill();
                        eyeball.projectile.Kill();
                    }

                var eye = Main.projectile[
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<WingedEyeball>(), 0, 0f, player.whoAmI)];
                eye.Center = target;
                var we = (WingedEyeball) eye.modProjectile;
                we.source = spell;
                foreach (var modifier in spell.modifiers.Where(modifier => modifier.minionAI != null))
                    we.glyphModifiers.Add(modifier.minionAI);
                character.minions.Add((WingedEyeball) eye.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Star Glyph");
            Tooltip.SetDefault("Summons a winged eyeball to cast the spell");
        }
    }
}