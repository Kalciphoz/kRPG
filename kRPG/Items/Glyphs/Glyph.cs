using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using kRPG.Projectiles;
using Terraria.ModLoader.IO;
using kRPG.Buffs;
using Microsoft.Xna.Framework;
using kRPG.Dusts;
using Terraria.ID;
using System.IO;

namespace kRPG.Items.Glyphs
{
    public class Glyph : ModItem
    {
        public virtual Action<ProceduralSpell, Player, Vector2> GetUseAbility() { return null; }
        public virtual Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction() { return null; }
        public virtual Action<ProceduralSpellProj> GetInitAction() { return null; }
        public virtual Action<ProceduralSpellProj, NPC, int> GetImpactAction() { return null; }
        public virtual Action<ProceduralSpellProj> GetAIAction() { return null; }
        public virtual Action<ProceduralSpellProj> GetKillAction() { return null; }

        public List<GlyphModifier> modifiers = new List<GlyphModifier>();
        public bool initialized = false;

        public bool minion
        {
            get
            {
                return this is Star && !(this is Star_Blue);
            }
        }

        public override void SetDefaults()
        {
            if (item == null) return;
            item.width = 48;
            item.height = 48;
            item.value = 2500;
            item.rare = 2;
        }

        public virtual void Randomize()
        {
            initialized = true;
            foreach (GlyphModifier modifier in GlyphModifier.modifiers)
                if (modifier.match(this) && modifier.odds())
                {
                    if (modifier.group == null)
                        modifiers.Add(modifier);
                    else
                        modifiers.Add(modifier.group());
                }
        }

        public override ModItem Clone(Item item)
        {
            Glyph copy = (Glyph)base.Clone(item);
            copy.modifiers = new List<GlyphModifier>();
            if (modifiers != null)
                foreach (GlyphModifier modifier in modifiers)
                    copy.modifiers.Add(modifier);
            return copy;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Glyph; Please Ignore");
        }

        public virtual bool CanUse()
        {
            return true;
        }

        public float ModifierDamageModifier()
        {
            float modifier = 1f;
            foreach (GlyphModifier mod in modifiers)
                modifier *= mod.dmgModifier;
            return modifier;
        }

        public virtual float BaseDamageModifier()
        {
            return 1f;
        }

        public float DamageModifier()
        {
            return ModifierDamageModifier() * BaseDamageModifier();
        }

        public float ModifierManaModifier()
        {
            float modifier = 1f;
            foreach (GlyphModifier mod in modifiers)
                modifier *= mod.manaModifier;
            return modifier;
        }

        public virtual float BaseManaModifier()
        {
            return 1f;
        }

        public float ManaModifier()
        {
            return ModifierManaModifier() * BaseManaModifier();
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode == 0) return;
            if (!initialized) Randomize();
        }

        public static string GetRandom()
        {
            switch (Main.rand.Next(26))
            {
                default:
                    return "Star_Blue";
                case 1:
                    return "Star_Orange";
                case 2:
                    return "Star_Purple";
                case 3:
                    return "Cross_Red";
                case 4:
                    return "Cross_Orange";
                case 5:
                    return "Cross_Yellow";
                case 6:
                    return "Cross_Green";
                case 7:
                    return "Cross_Blue";
                case 8:
                    return "Cross_Violet";
                case 9:
                    return "Cross_Purple";
                case 10:
                    return "Moon_Yellow";
                case 11:
                    return "Moon_Green";
                case 12:
                    return "Moon_Blue";
                case 13:
                    return "Moon_Violet";
                case 14:
                    return "Cross_Orange";
                case 15:
                    return "Cross_Green";
                case 16:
                    return "Moon_Yellow";
                case 17:
                    return "Moon_Green";
                case 18:
                    return "Moon_Blue";
                case 19:
                    return "Moon_Violet";
                case 20:
                    return "Star_Orange";
                case 21:
                    return "Star_Purple";
                case 22:
                    return "Moon_Purple";
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < modifiers.Count; i += 1)
                tooltips.Add(new TooltipLine(mod, "modifier" + i, modifiers[i].tooltip));
            tooltips.Add(new TooltipLine(mod, "damage", ((int)Math.Round(DamageModifier()*100)).ToString()+ "% damage"));
            tooltips.Add(new TooltipLine(mod, "mana", ((int)Math.Round(ManaModifier() * 100)).ToString() + "% mana cost"));
        }

        public override TagCompound Save()
        {
            TagCompound compound = new TagCompound()
            {
                { "ModifierCount", modifiers.Count }
            };
            for (int i = 0; i < modifiers.Count; i += 1)
                compound.Add("Modifier_" + i, modifiers[i].id);
            return compound;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(modifiers.Count);
            for (int i = 0; i < modifiers.Count; i += 1)
                writer.Write(modifiers[i].id);
        }

        public override void Load(TagCompound tag)
        {
            modifiers.Clear();
            int count = tag.GetInt("ModifierCount");
            for (int i = 0; i < count; i += 1)
                modifiers.Add(GlyphModifier.modifiers[tag.GetInt("Modifier_" + i)]);
            initialized = true;
        }

        public override void NetRecieve(BinaryReader reader)
        {
            modifiers.Clear();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i += 1)
                modifiers.Add(GlyphModifier.modifiers[reader.ReadInt32()]);
            initialized = true;
        }
    }

    public class Star : Glyph
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Star Glyph; Please Ignore");
        }
    }
    public class Cross : Glyph
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Cross Glyph; Please Ignore");
        }

        public virtual Dictionary<ELEMENT, float> eleDmg
        {
            get
            {
                return new Dictionary<ELEMENT, float>()
                {
                    {ELEMENT.FIRE, 0},
                    {ELEMENT.COLD, 0},
                    {ELEMENT.LIGHTNING, 0},
                    {ELEMENT.SHADOW, 0}
                };
            }
        }
    }
    public class Moon : Glyph
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Generic Moon Glyph; Please Ignore");
        }

        public int projCount = 5;

        public override TagCompound Save()
        {
            TagCompound compound = base.Save();
            compound.Add("projCount", projCount);
            return compound;
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(projCount);
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            projCount = tag.GetInt("projCount");
        }

        public override void NetRecieve(BinaryReader reader)
        {
            base.NetRecieve(reader);
            projCount = reader.ReadInt32();
        }

        public override ModItem Clone(Item item)
        {
            Moon copy = (Moon)base.Clone(item);
            copy.projCount = projCount;
            return copy;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Add(new TooltipLine(mod, "projCount", projCount.ToString() + " Projectiles"));
        }
    }

    public class Star_Orange : Star
    {
        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 target)
            {
                Main.PlaySound(Terraria.ID.SoundID.Item6, player.position);
                spell.remaining = spell.cooldown;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                if (character.minions.Exists(minion => minion is WingedEyeball))
                {
                    foreach (ProceduralMinion eyeball in character.minions.Where(minion => minion.projectile.type == mod.ProjectileType<WingedEyeball>()))
                    {
                        foreach (ProceduralSpellProj psp in eyeball.circlingProtection)
                            psp.projectile.Kill();
                        eyeball.circlingProtection.Clear();
                        if (eyeball.smallProt != null) eyeball.smallProt.projectile.Kill();
                        eyeball.projectile.Kill();
                    }
                }
                Projectile eye = Main.projectile[Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType<WingedEyeball>(), 0, 0f, player.whoAmI)];
                eye.Center = target;
                WingedEyeball we = (WingedEyeball)eye.modProjectile;
                we.source = spell;
                foreach (GlyphModifier modifier in spell.modifiers)
                    if (modifier.minionAI != null)
                        we.glyphModifiers.Add(modifier.minionAI);
                character.minions.Add((WingedEyeball)eye.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Star Glyph");
            Tooltip.SetDefault("Summons a winged eyeball to cast the spell");
        }

        public override float BaseDamageModifier()
        {
            return 0.6f;
        }

        public override float BaseManaModifier()
        {
            return 1.35f;
        }
    }

    public class Star_Blue : Star
    {
        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 target)
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

        public override float BaseDamageModifier()
        {
            return 1.25f;
        }
    }

    public class Star_Purple : Star
    {
        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 target)
            {
                Main.PlaySound(0, player.position);
                spell.remaining = spell.cooldown;
                int placementHeight = 0;
                bool placeable = false;
                for (int y = (int)(Main.screenPosition.Y / 16); y < (int)((Main.screenPosition.Y + Main.screenHeight) / 16); y += 1)
                {
                    int x = (int)(target.X / 16f);
                    Tile tile = Main.tile[x, y];
                    if (tile.active() && Main.tileSolidTop[tile.type] || tile.collisionType == 1 && Main.tile[x, y - 1].collisionType != 1)
                    {
                        placeable = true;
                        placementHeight = y;
                        if (target.Y / 16 - 4 <= y)
                            break;
                    }
                }
                if (!placeable) return;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                if (character.minions.Exists(minion => minion is Obelisk))
                {
                    foreach (ProceduralMinion obelisk in character.minions.Where(minion => minion.projectile.type == mod.ProjectileType<Obelisk>()))
                    {
                        foreach (ProceduralSpellProj psp in obelisk.circlingProtection)
                            psp.projectile.Kill();
                        obelisk.circlingProtection.Clear();
                        if (obelisk.smallProt != null) obelisk.smallProt.projectile.Kill();
                        obelisk.projectile.Kill();
                    }
                }
                Projectile totem = Main.projectile[Projectile.NewProjectile(new Vector2((int)(target.X / 16) * 16, placementHeight * 16) + new Vector2(8f, -32f), Vector2.Zero, mod.GetProjectile<Obelisk>().projectile.type, 0, 0f, player.whoAmI)];
                totem.position = new Vector2((int)(target.X / 16) * 16, placementHeight * 16) - new Vector2(8f, 62f);
                ((Obelisk)totem.modProjectile).source = spell;
                character.minions.Add((Obelisk)totem.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Star Glyph");
            Tooltip.SetDefault("Summons an obelisk sentry to cast the spell");
        }
    }

    public class Cross_Red : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                try
                { 
                    spell.texture = GFX.projectile_fireball;
                    spell.projectile.width = spell.texture.Width;
                    spell.projectile.height = spell.texture.Height;
                    spell.projectile.magic = true;
                    spell.draw_trail = true;
                    spell.lighted = true;
                    spell.projectile.scale = spell.minion ? 0.7f : 1f;
                }
                catch (SystemException e)
                {
                    ErrorLogger.Log(e.ToString());
                }
            };
        }
        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                try
                {
                    ProceduralSpellProj.AI_RotateToVelocity(spell);
                    if (Main.rand.NextFloat(0f, 1.5f) <= spell.alpha)
                    {
                        int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Fire, spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 1f + spell.alpha * 2f);
                        Main.dust[dust].noGravity = true;
                    }
                }
                catch (SystemException e)
                {
                    ErrorLogger.Log(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                try
                { 
                    for (int k = 0; k < 20; k++)
                    {
                        Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Fire, spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default(Color), 1.5f);
                    }
                }
                catch (SystemException e)
                {
                    ErrorLogger.Log(e.ToString());
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Cross Glyph");
            Tooltip.SetDefault("Casts magical fireballs");
        }

        public override Dictionary<ELEMENT, float> eleDmg
        {
            get
            {
                return new Dictionary<ELEMENT, float>()
                {
                    {ELEMENT.FIRE, 1f},
                    {ELEMENT.COLD, 0},
                    {ELEMENT.LIGHTNING, 0},
                    {ELEMENT.SHADOW, 0}
                };
            }
        }
    }

    public class Cross_Orange : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                if (Main.netMode != 2)
                {
                    if (Main.netMode == 0 || spell.projectile.owner == Main.myPlayer)
                    {
                        PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();

                        if (character.lastSelectedWeapon.modItem is ProceduralSword)
                            spell.texture = ((ProceduralSword)character.lastSelectedWeapon.modItem).texture;
                        else
                            spell.texture = Main.itemTexture[character.lastSelectedWeapon.type];
                    }
                    else
                        spell.texture = GFX.projectile_boulder;

                    spell.projectile.width = spell.texture.Width;
                    spell.projectile.height = spell.texture.Height;
                }
                else
                {
                    spell.projectile.width = 48;
                    spell.projectile.height = 48;
                }
                spell.projectile.melee = true;
                spell.draw_trail = true;
                spell.alpha = 1f;
                spell.lighted = true;
                spell.projectile.scale = spell.minion ? 0.6f : 1f;
            };
        }

        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.basePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                Vector2 v = spell.basePosition != Vector2.Zero ? spell.basePosition : spell.origin;
                if (spell.projectile.spriteDirection == -1)
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() - (float)API.Tau * 5f / 8f;
                else
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() + (float)API.Tau / 8f;
            };
        }

        public override bool CanUse()
        {
            Player owner = Main.player[Main.myPlayer];
            PlayerCharacter character = owner.GetModPlayer<PlayerCharacter>();
            Item item = character.lastSelectedWeapon;
            return owner.inventory.Contains<Item>(item);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Cross Glyph");
            Tooltip.SetDefault("Creates copies of your selected melee weapon");
        }

        public override float BaseManaModifier()
        {
            return 0.9f;
        }

        public override float BaseDamageModifier()
        {
            return 0.9f;
        }
    }

    public class Cross_Yellow : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                spell.texture = GFX.projectile_boulder;
                spell.projectile.width = spell.texture.Width;
                spell.projectile.height = spell.texture.Height;
                spell.projectile.magic = true;
                spell.alpha = 1f;
                spell.draw_trail = true;
                spell.projectile.knockBack = 11f;
                spell.projectile.scale = spell.minion ? 0.8f : 1f;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Stone, spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default(Color), 2f);
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Cross Glyph");
            Tooltip.SetDefault("Conjures large magical boulders");
        }

        public override float BaseDamageModifier()
        {
            return 1.1f;
        }
    }

    public class Cross_Green : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                spell.texture = Main.itemTexture[Terraria.ID.ItemID.WoodenArrow];
                spell.projectile.width = spell.texture.Width;
                spell.projectile.height = spell.texture.Height;
                spell.projectile.ranged = true;
                spell.draw_trail = true;
                spell.alpha = 1f;
                spell.lighted = true;
                spell.projectile.scale = spell.minion ? 1f : 1.5f;
            };
        }

        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.basePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                Vector2 v = spell.basePosition != Vector2.Zero ? spell.basePosition : spell.origin;
                spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() - (float)API.Tau / 4f;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Stone, spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f);
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Cross Glyph");
            Tooltip.SetDefault("Conjures giant arrows that deal ranged damage");
        }

        public override float BaseManaModifier()
        {
            return 0.9f;
        }
    }

    public class Cross_Blue : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                spell.texture = GFX.projectile_frostbolt;
                spell.projectile.width = spell.texture.Width;
                spell.projectile.height = spell.texture.Height;
                spell.projectile.magic = true;
                spell.lighted = true;
            };
        }

        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                if (Main.rand.NextFloat(0f, 2f) <= spell.alpha)
                {
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, spell.mod.GetDust<Ice>().Type, 0f, 0f, 100, Color.White, 0.5f + spell.alpha);
                    Main.dust[dust].noGravity = true;
                }
                Lighting.AddLight(spell.projectile.Center, 0f, 0.4f, 1f);
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                for (int k = 0; k < 8; k++)
                {
                    try
                    {
                        Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, mod.DustType<Ice>(), spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f);
                    }
                    catch (SystemException e)
                    {
                        ErrorLogger.Log(e.ToString());
                    }
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Cross Glyph");
            Tooltip.SetDefault("Casts magical ice cubes");
        }

        public override float BaseDamageModifier()
        {
            return 1.05f;
        }

        public override Dictionary<ELEMENT, float> eleDmg
        {
            get
            {
                return new Dictionary<ELEMENT, float>()
                {
                    {ELEMENT.FIRE, 0},
                    {ELEMENT.COLD, 1f},
                    {ELEMENT.LIGHTNING, 0},
                    {ELEMENT.SHADOW, 0}
                };
            }
        }
    }

    public class Cross_Violet : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                spell.texture = GFX.projectile_shadowbolt;
                spell.projectile.width = spell.texture.Width;
                spell.projectile.height = spell.texture.Height;
                spell.projectile.magic = true;
                spell.draw_trail = true;
                spell.lighted = true;
                spell.projectile.scale = spell.minion ? 0.7f : 1f;
            };
        }
        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                ProceduralSpellProj.AI_RotateToVelocity(spell);
                if (Main.rand.NextFloat(0f, 1.5f) <= spell.alpha)
                {
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Shadowflame, spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 0.4f + spell.alpha * 1.2f);
                    Main.dust[dust].noGravity = true;
                }
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Shadowflame, spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default(Color), 1.5f);
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Violet Cross Glyph");
            Tooltip.SetDefault("Casts magical shadowbolts");
        }

        public override Dictionary<ELEMENT, float> eleDmg
        {
            get
            {
                return new Dictionary<ELEMENT, float>()
                {
                    {ELEMENT.FIRE, 0},
                    {ELEMENT.COLD, 0},
                    {ELEMENT.LIGHTNING, 0},
                    {ELEMENT.SHADOW, 1f}
                };
            }
        }
    }

    public class Cross_Purple : Cross
    {
        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                spell.texture = GFX.projectile_thunderbolt;
                spell.projectile.width = spell.texture.Width;
                spell.projectile.height = spell.texture.Height;
                spell.projectile.magic = true;
                spell.lighted = true;
            };
        }
        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                ProceduralSpellProj.AI_RotateToVelocity(spell);
                if (Main.rand.NextFloat(0f, 2f) <= spell.alpha)
                {
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Electric, spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 0.2f + spell.alpha);
                    Main.dust[dust].noGravity = true;
                }
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                for (int k = 0; k < 8; k++)
                {
                    Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Electric, spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default(Color), 0.5f);
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Cross Glyph");
            Tooltip.SetDefault("Casts magical lightning orbs");
        }

        public override Dictionary<ELEMENT, float> eleDmg
        {
            get
            {
                return new Dictionary<ELEMENT, float>()
                {
                    {ELEMENT.FIRE, 0},
                    {ELEMENT.COLD, 0},
                    {ELEMENT.LIGHTNING, 1f},
                    {ELEMENT.SHADOW, 0}
                };
            }
        }
    }

    public class Moon_Yellow : Moon
    {
        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(2, 5);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Moon Glyph");
            Tooltip.SetDefault("Throws whirling projectiles that pierce the enemies");
        }

        private static float GetSpread(int projCount)
        {
            return 1f / projCount;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                int rotDistance = spell.minion ? 32 : 48;
                float spread =  GetSpread(spell.projCount);
                for (int i = 0; i < spell.projCount; i += 1)
                {
                    float angle = (float)i * spread * (float)API.Tau;
                    ProceduralSpellProj proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin + new Vector2(0f, -rotDistance).RotatedBy(angle), caster);
                    proj.basePosition = origin;
                    Vector2 unitVelocity = (target - origin);
                    unitVelocity.Normalize();
                    proj.baseVelocity = unitVelocity * 8f;
                    proj.displacementAngle = angle;
                    proj.projectile.penetrate = 3;
                }
            };
        }

        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                int rotDistance = spell.minion ? 32 : 48;
                spell.basePosition += spell.baseVelocity;
                Vector2 unitRelativePos = spell.RelativePos(spell.basePosition);
                unitRelativePos.Normalize();
                spell.projectile.Center = spell.basePosition + unitRelativePos * rotDistance;
                spell.displacementVelocity = new Vector2(12f / spell.source.projCount, 0f).RotatedBy((spell.RelativePos(spell.basePosition)).ToRotation() + (float)API.Tau / 4f);

                float angle = spell.displacementAngle + 0.24f * (- spell.projectile.timeLeft - rotDistance) / projCount;
                spell.projectile.Center = spell.basePosition + new Vector2(0f, -rotDistance).RotatedBy(angle);
                
                spell.projectile.velocity = spell.displacementVelocity + spell.baseVelocity;
            };
        }

        public override float BaseDamageModifier()
        {
            return 1f - projCount * 0.05f;
        }
    }

    public class Moon_Green : Moon
    {
        public const int RotTimeLeft = 3600;

        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(3, 11);
        }

        private static float GetSpread(int projCount)
        {
            return 1f / projCount;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                if (caster is Player)
                {
                    PlayerCharacter character = ((Player)caster).GetModPlayer<PlayerCharacter>();
                    foreach (ProceduralSpellProj proj in character.circlingProtection)
                        if (proj.projectile.modProjectile is ProceduralSpellProj)
                            proj.projectile.Kill();
                    character.circlingProtection.Clear();
                }
                else if (caster is Projectile)
                {
                    ProceduralMinion minion = (ProceduralMinion)((Projectile)caster).modProjectile;
                    foreach (ProceduralSpellProj proj in minion.circlingProtection)
                        if (proj.projectile.modProjectile is ProceduralSpellProj)
                            proj.projectile.Kill();
                    minion.circlingProtection.Clear();
                }
                float spread = GetSpread(spell.projCount);
                Vector2 velocity = new Vector2(0f, -1.5f);
                for (int i = 0; i < spell.projCount; i += 1)
                {
                    ProceduralSpellProj proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin, caster);
                    proj.projectile.timeLeft = RotTimeLeft;
                    proj.displacementVelocity = velocity.RotatedBy(i * spread * API.Tau);
                    proj.displacementAngle = i * spread * (float)API.Tau;
                    if (caster is Player)
                    {
                        player.GetModPlayer<PlayerCharacter>().circlingProtection.Add(proj);
                    }
                    else if (caster is Projectile)
                    {
                        ((ProceduralMinion)((Projectile)caster).modProjectile).circlingProtection.Add(proj);
                    }
                }
            };
        }

        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                try
                {
                    int rotDistance = spell.minion ? 72 : 96;
                    if (RotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.caster.Center + unitRelativePos * rotDistance;
                        spell.displacementVelocity = new Vector2(1.5f, 0f).RotatedBy((spell.RelativePos(spell.caster.Center)).ToRotation() + (float)API.Tau / 4f);

                        float angle = spell.displacementAngle + 0.04f * (float)(RotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.caster.Center + new Vector2(0f, -1.5f).RotatedBy(spell.displacementAngle) * (RotTimeLeft - spell.projectile.timeLeft);
                    }
                    spell.projectile.velocity = spell.displacementVelocity + spell.caster.velocity;
                    spell.basePosition = spell.caster.position;
                }
                catch (SystemException e)
                {
                    ErrorLogger.Log(e.ToString());
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Moon Glyph");
            Tooltip.SetDefault("Casts projectiles to orbit around you");
        }

        public override float BaseDamageModifier()
        {
            return 1.24f - projCount * 0.08f;
        }
    }

    public class Moon_Blue : Moon
    {
        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(3, 8);
        }

        private static float GetSpread(int projCount)
        {
            return 0.020f - projCount * 0.001f;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                float spread = GetSpread(spell.projCount);
                Vector2 unitVelocity = (target - origin);
                unitVelocity.Normalize();
                Vector2 velocity = unitVelocity * 6f;
                for (int i = 0; i < spell.projCount; i += 1)
                {
                    spell.CreateProjectile(player, velocity, spell.projCount * -spread / 2f + i * spread + spread / 2f, origin, caster);
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Moon Glyph");
            Tooltip.SetDefault("Fires an array of projectiles outwards");
        }

        public override float BaseDamageModifier()
        {
            return 1.2f;
        }

        public override float BaseManaModifier()
        {
            return 0.72f + projCount * 0.06f;
        }
    }

    public class Moon_Violet : Moon
    {
        public const float area = 192f;

        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(10, 21);
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                new SpellEffect(spell, target, projCount * 8, delegate (ProceduralSpell ability, int timeLeft)
                {
                    if (timeLeft % 8 == 0)
                    {
                        ProceduralSpellProj proj = spell.CreateProjectile(player, new Vector2(0f, 8f), 0f, new Vector2(target.X - area / 2f + Main.rand.NextFloat(area), target.Y - 240f), caster);
                        if (proj.alpha < 1f) proj.alpha = 0.5f;
                        proj.projectile.timeLeft = 60;
                    }
                });
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Violet Moon Glyph");
            Tooltip.SetDefault("Rains down projectiles at the target location");
        }

        public override float BaseDamageModifier()
        {
            return 1.1f - projCount * 0.02f;
        }
    }

    public class Moon_Purple : Moon
    {
        public override void Randomize()
        {
            base.Randomize();
            projCount = Main.rand.Next(10, 21);
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate (ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                new SpellEffect(spell, target, projCount * 10, delegate (ProceduralSpell ability, int timeLeft)
                {
                    if (timeLeft % 10 == 0)
                    {
                        ProceduralSpellProj proj = spell.CreateProjectile(player, new Vector2(0, -9f), Main.rand.NextFloat(-0.07f, 0.07f), caster.Center + new Vector2(0, -16f), caster);
                        if (proj.alpha < 1f) proj.alpha = 0.5f;
                        proj.projectile.tileCollide = true;
                    }
                });
            };
        }

        public override Action<ProceduralSpellProj> GetAIAction()
        {
            return delegate (ProceduralSpellProj spell)
            {
                spell.projectile.velocity.Y += 0.3f;
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Moon Glyph");
            Tooltip.SetDefault("Shoots up a fountain of projectiles to rain down around you");
        }

        public override float BaseDamageModifier()
        {
            return 2.6f - projCount * 0.06f;
        }

        public override float BaseManaModifier()
        {
            return 1.2f;
        }
    }
}
