using System;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG2.Items.Glyphs
{
    public class Cross_Orange : Cross
    {
        public override float BaseDamageModifier()
        {
            return 0.9f;
        }

        public override float BaseManaModifier()
        {
            return 0.9f;
        }

        public override bool CanUse()
        {
            var owner = Main.player[Main.myPlayer];
            var character = owner.GetModPlayer<PlayerCharacter>();
            var item = character.LastSelectedWeapon;
            return owner.inventory.Contains(item);
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.BasePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                var v = spell.BasePosition != Vector2.Zero ? spell.BasePosition : spell.Origin;
                if (spell.projectile.spriteDirection == -1)
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() - (float) API.Tau * 5f / 8f;
                else
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() + (float) API.Tau / 8f;
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (Main.netMode != 2)
                {
                    if (Main.netMode == 0 || spell.projectile.owner == Main.myPlayer)
                    {
                        var character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();

                        spell.LocalTexture = character.LastSelectedWeapon.modItem is ProceduralSword
                            ? ((ProceduralSword) character.LastSelectedWeapon.modItem).texture
                            : Main.itemTexture[character.LastSelectedWeapon.type];
                    }
                    else
                    {
                        spell.LocalTexture = GFX.ProjectileBoulder;
                    }

                    spell.projectile.width = spell.LocalTexture.Width;
                    spell.projectile.height = spell.LocalTexture.Height;
                }
                else
                {
                    spell.projectile.width = 48;
                    spell.projectile.height = 48;
                }

                spell.projectile.melee = true;
                spell.DrawTrail = true;
                spell.Alpha = 1f;
                spell.Lighted = true;
                spell.projectile.scale = spell.Minion ? 0.6f : 1f;
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Cross Glyph");
            Tooltip.SetDefault("Creates copies of your selected melee weapon");
        }
    }
}