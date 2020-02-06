using System;
using System.Linq;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.Spells;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Glyphs
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
                spell.Remaining = spell.Cooldown;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                if (character.Minions.Exists(minion => minion is WingedEyeball))
                    foreach (ProceduralMinion eyeball in character.Minions.Where(minion => minion.projectile.type == ModContent.ProjectileType<WingedEyeball>())
                    )
                    {
                        foreach (ProceduralSpellProj psp in eyeball.CirclingProtection)
                            psp.projectile.Kill();
                        eyeball.CirclingProtection.Clear();
                        eyeball.SmallProt?.projectile.Kill();
                        eyeball.projectile.Kill();
                    }

                Projectile eye = Main.projectile[
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<WingedEyeball>(), 0, 0f, player.whoAmI)];
                eye.Center = target;
                WingedEyeball we = (WingedEyeball) eye.modProjectile;
                we.Source = spell;
                foreach (GlyphModifier modifier in spell.Modifiers.Where(modifier => modifier.MinionAi != null))
                    we.GlyphModifiers.Add(modifier.MinionAi);
                character.Minions.Add((WingedEyeball) eye.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Star Glyph");
            Tooltip.SetDefault("Summons a winged eyeball to cast the spell");
        }
    }
}