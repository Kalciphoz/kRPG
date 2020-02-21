﻿using System;
using System.Linq;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.NPCs;
using kRPG.Content.Players;
using kRPG.Content.Spells;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Glyphs
{
    public class Star_Purple : Star
    {
        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 target)
            {
                Main.PlaySound(0, player.position);
                spell.Remaining = spell.Cooldown;
                int placementHeight = 0;
                bool placeable = false;
                for (int y = (int) (Main.screenPosition.Y / 16); y < (int) ((Main.screenPosition.Y + Main.screenHeight) / 16); y += 1)
                {
                    int x = (int) (target.X / 16f);
                    Tile tile = Main.tile[x, y];
                    if ((!tile.active() || !Main.tileSolidTop[tile.type]) && (tile.collisionType != 1 || Main.tile[x, y - 1].collisionType == 1))
                        continue;
                    placeable = true;
                    placementHeight = y;
                    if (target.Y / 16 - 4 <= y)
                        break;
                }

                if (!placeable) return;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                if (character.Minions.Exists(minion => minion is Obelisk))
                    foreach (ProceduralMinion obelisk in character.Minions.Where(minions => minions.projectile.type == ModContent.ProjectileType<Obelisk>()))
                    {
                        foreach (ProceduralSpellProj psp in obelisk.CirclingProtection)
                            psp.projectile.Kill();
                        obelisk.CirclingProtection.Clear();
                        obelisk.SmallProt?.projectile.Kill();
                        obelisk.projectile.Kill();
                    }

                Projectile totem = Main.projectile[
                    Projectile.NewProjectile(new Vector2((int) (target.X / 16) * 16, placementHeight * 16) + new Vector2(8f, -32f), Vector2.Zero,
                        ModContent.GetInstance<Obelisk>().projectile.type, 0, 0f, player.whoAmI)];
                totem.position = new Vector2((int) (target.X / 16) * 16, placementHeight * 16) - new Vector2(8f, 62f);
                ((Obelisk) totem.modProjectile).Source = spell;
                character.Minions.Add((Obelisk) totem.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Star Glyph");
            Tooltip.SetDefault("Summons an obelisk sentry to cast the spell");
        }
    }
}