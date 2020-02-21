﻿using System;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.Items.Weapons.Melee;
using kRPG.Content.Players;
using kRPG.Util;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Glyphs
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
            Item item = character.LastSelectedWeapon;
            return owner.inventory.Contains(item);
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.BasePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                Vector2 v = spell.BasePosition != Vector2.Zero ? spell.BasePosition : spell.Origin;
                if (spell.projectile.spriteDirection == -1)
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() - (float) Constants.Tau * 5f / 8f;
                else
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() + (float) Constants.Tau / 8f;
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.netMode ==NetmodeID.SinglePlayer || spell.projectile.owner == Main.myPlayer)
                    {
                        PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();

                        spell.LocalTexture = character.LastSelectedWeapon.modItem is ProceduralSword
                            ? ((ProceduralSword) character.LastSelectedWeapon.modItem).LocalTexture
                            : Main.itemTexture[character.LastSelectedWeapon.type];
                    }
                    else
                    {
                        spell.LocalTexture = GFX.GFX.ProjectileBoulder;
                    }

                    if (spell.LocalTexture == null)
                    {
                        ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("GetInitAction, spell.localtexture = null.");
                        spell.projectile.width = 48;
                        spell.projectile.height = 48;
                    }
                    else
                    {
                        spell.projectile.width = spell.LocalTexture.Width;
                        spell.projectile.height = spell.LocalTexture.Height;
                    }

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