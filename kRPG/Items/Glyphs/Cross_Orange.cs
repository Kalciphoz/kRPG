using System;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
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
            Player owner = Main.player[Main.myPlayer];
            PlayerCharacter character = owner.GetModPlayer<PlayerCharacter>();
            Item item = character.lastSelectedWeapon;
            return owner.inventory.Contains(item);
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.basePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                Vector2 v = spell.basePosition != Vector2.Zero ? spell.basePosition : spell.origin;
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
                        PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();

                        spell.texture = character.lastSelectedWeapon.modItem is ProceduralSword
                            ? ((ProceduralSword) character.lastSelectedWeapon.modItem).texture
                            : Main.itemTexture[character.lastSelectedWeapon.type];
                    }
                    else
                    {
                        spell.texture = GFX.projectileBoulder;
                    }

                    spell.projectile.width = spell.texture.Width;
                    spell.projectile.height = spell.texture.Height;
                }
                else
                {
                    spell.projectile.width = 48;
                    spell.projectile.height = 48;
                }

                spell.projectile.melee = true;
                spell.drawTrail = true;
                spell.alpha = 1f;
                spell.lighted = true;
                spell.projectile.scale = spell.minion ? 0.6f : 1f;
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Cross Glyph");
            Tooltip.SetDefault("Creates copies of your selected melee weapon");
        }
    }
}