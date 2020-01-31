using System;
using System.Collections.Generic;
using System.IO;
using kRPG.Enums;
using kRPG.Items.Weapons;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace kRPG.Items
{
    public class ProceduralSword : ProceduralItem
    {
        public SwordAccent accent;
        public SwordBlade blade;

        //public override void ModifyTooltips(List<TooltipLine> tooltips)
        //{
        //    tooltips.Add(new TooltipLine(mod, "dps", "base dps " + dps.ToString()));
        //    tooltips.Add(new TooltipLine(mod, "enemyDef", "average enemy defense " + dps.ToString()));
        //}

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public SwordHilt hilt;
        public bool lighted;
        public bool spear;

        public override ModItem Clone(Item item)
        {
            var copy = (ProceduralSword) base.Clone(item);
            copy.hilt = hilt;
            copy.blade = blade;
            copy.accent = accent;
            copy.dps = dps;
            copy.enemyDef = enemyDef;
            copy.spear = spear;
            copy.eleDamage = new Dictionary<ELEMENT, float>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                copy.eleDamage[element] = eleDamage[element];
            copy.item.SetNameOverride(item.Name);
            return copy;
        }

        public Point CombinedTextureSize()
        {
            return new Point(Math.Max(blade.texture.Width, blade.texture.Width - (int) blade.origin.X + (int) hilt.origin.X),
                Math.Max(blade.texture.Height, (int) blade.origin.Y + hilt.texture.Height - (int) hilt.origin.Y));
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (texture == null)
            {
                item.SetDefaults(0, true);
                return;
            }

            spriteBatch.Draw(texture, position, null, lighted ? Color.White : color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawHeld(PlayerDrawInfo drawinfo, Color color, float rotation, float scale, Vector2 playerCenter)
        {
            try
            {
                var player = Main.player[Main.myPlayer];
                var position = new Vector2(4f * player.direction, -4f).RotatedBy(rotation) + playerCenter;
                if (texture == null)
                {
                    item.SetDefaults();
                    return;
                }

                var effects = player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                var draw = new DrawData(texture, position, null, lighted ? Color.White : color, rotation,
                    new Vector2(player.direction > 0 ? 0 : texture.Width, texture.Height), scale, effects, 0);
                for (int i = 0; i < Main.playerDrawData.Count; i += 1)
                {
                    if (Main.playerDrawData[i].texture != Main.itemTexture[player.inventory[player.selectedItem].type])
                        continue;
                    Main.playerDrawData.Insert(i, draw);
                    return;
                }

                Main.playerDrawData.Add(draw);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        public static Item GenerateSword(Mod mod, Vector2 position, SWORDTHEME theme, float dps, int enemyDef)
        {
            ProceduralSword sword;
            sword = NewSword(mod, position, SwordHilt.RandomHilt(theme), SwordBlade.RandomBlade(theme),
                Main.rand.Next(5) < 3 ? SwordAccent.RandomAccent() : SwordAccent.none, dps, enemyDef);
            return sword.item;
        }

        public override void Initialize()
        {
            try
            {
                ResetStats();
                if (Main.netMode != 2)
                    texture = GFX.CombineTextures(new List<Texture2D> {blade.texture, hilt.texture, accent.texture},
                        new List<Point>
                        {
                            new Point(CombinedTextureSize().X - blade.texture.Width, 0),
                            new Point(0, CombinedTextureSize().Y - hilt.texture.Height),
                            new Point((int) hilt.origin.X + hilt.accentOffset.X - (int) accent.origin.X,
                                hilt.accentOffset.Y + CombinedTextureSize().Y - hilt.texture.Height + (int) hilt.origin.Y - (int) accent.origin.Y)
                        }, CombinedTextureSize());
                if (Main.netMode != 2) item.width = texture.Width;
                if (Main.netMode != 2) item.height = texture.Height;
                if (accent.type == SwordAccent.gemPurple.type)
                {
                    item.melee = false;
                    item.magic = true;
                }

                lighted = blade.lighted;
                spear = hilt.spear && blade.spearable;
                if (spear)
                {
                    item.noMelee = true;
                    item.noUseGraphic = true;
                    item.useStyle = 5;
                }

                item.GetGlobalItem<kItem>().ApplyStats(item, true);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                ModLoader.GetMod("kRPG").Logger.InfoFormat("Blade|Hilt|Accent" + (blade == null) + (hilt == null) + (accent == null));
            }
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                hilt = SwordHilt.hilts[tag.GetInt("hilt_id")];
                blade = SwordBlade.blades[tag.GetInt("blade_id")];
                accent = SwordAccent.accents[tag.GetInt("accent_id")];
                dps = tag.GetFloat("dps");
                enemyDef = tag.GetInt("enemy_defence");
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat("@Loading :: " + e);
            }

            try
            {
                Initialize();
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat("@Initialize :: " + e);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            blade.effect?.Invoke(hitbox, player);

            accent.effect?.Invoke(hitbox, player);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;
            blade = SwordBlade.blades[reader.ReadInt32()];
            hilt = SwordHilt.hilts[reader.ReadInt32()];
            accent = SwordAccent.accents[reader.ReadInt32()];
            dps = reader.ReadSingle();
            enemyDef = reader.ReadInt32();
            Initialize();
        }

        public override void NetSend(BinaryWriter writer)
        {
            bool proceed = blade != null;
            writer.Write(proceed);
            if (!proceed) return;
            writer.Write(blade.type);
            writer.Write(hilt.type);
            writer.Write(accent.type);
            writer.Write(dps);
            writer.Write(enemyDef);
        }

        public static ProceduralSword NewSword(Mod mod, Vector2 position, SwordHilt hilt, SwordBlade blade, SwordAccent accent, float dps, int enemyDef)
        {
            int id = Item.NewItem(position, mod.GetItem("ProceduralSword").item.type);
            var sword = (ProceduralSword) Main.item[id].modItem;
            sword.hilt = hilt;
            sword.blade = blade;
            sword.accent = accent;
            sword.dps = dps;
            sword.enemyDef = enemyDef;
            sword.Initialize();
            if (Main.netMode != 2)
                return sword;
            var packet = mod.GetPacket();
            packet.Write((byte) Message.SwordInit);
            packet.Write(id);
            packet.Write(blade.type);
            packet.Write(hilt.type);
            packet.Write(accent.type);
            packet.Write(dps);
            packet.Write(enemyDef);
            packet.Send();
            return sword;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            accent.onHit?.Invoke(player, target, this, damage, crit);
        }

        public Texture2D OverhaulGetTexture()
        {
            return texture;
        }

        public bool? OverhaulHasTag(string tag)
        {
            return (spear ? tag == "spear" : tag == "broadsword") ? (bool?) true : null;
        }

        public void ResetStats()
        {
            try
            {
                item.rare = (int) Math.Min(Math.Floor(dps / 15.0), 9);
                item.useAnimation = (int) (blade.useTime / hilt.speedModifier);
                item.damage = (int) Math.Round(dps * hilt.dpsModifier * accent.dpsModifier * item.useAnimation / 60f + enemyDef);
                item.useAnimation = (int) Math.Round((item.damage - (float) enemyDef) * 60f / (dps * hilt.dpsModifier * accent.dpsModifier));
                item.useTime = item.useAnimation;
                item.knockBack = blade.knockBack + hilt.knockBack;
                item.SetNameOverride(hilt.prefix + blade.name + accent.suffix);
                item.autoReuse = hilt.autoswing || blade.autoswing;
                item.useTurn = item.autoReuse;
                item.value = (int) (dps * 315);
                item.crit = blade.critBonus + hilt.critBonus + accent.critBonus;
                item.scale = 1f + blade.scale + hilt.scale;
                item.mana = hilt.mana + accent.mana;
                eleDamage = new Dictionary<ELEMENT, float>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    eleDamage[element] = blade.eleDamage[element] + accent.eleDamage[element];
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        public override TagCompound Save()
        {
            try
            {
                return new TagCompound
                {
                    {"hilt_id", hilt.type},
                    {"blade_id", blade.type},
                    {"accent_id", accent.type},
                    {"dps", dps},
                    {"enemy_defence", enemyDef}
                };
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat("@NewTagCompound :: " + e);
                ModLoader.GetMod("kRPG").Logger.InfoFormat("Blade|Hilt|Accent" + (blade == null) + (hilt == null) + (accent == null));
            }

            return new TagCompound();
        }

        public override void SetDefaults()
        {
            item.damage = 1;
            item.melee = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 1;
            item.useAnimation = 1;
            item.useStyle = 1;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.scale = 1f;
            item.noMelee = false;
            item.noUseGraphic = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Sword; Please Ignore");
        }

        /*public override bool AltFunctionUse(Player player)
        {
            Vector2 pos = player.playerPosition; //  + new Vector2(2f * player.direction, 4f)
            Vector2 unitVelocity = (Main.MouseWorld - pos);
            unitVelocity.Normalize();
            Vector2 velocity = unitVelocity * 8f;
            Projectile projectile = Main.projectile[Projectile.NewProjectile(pos, velocity, mod.ProjectileType<ProceduralSwordThrow>(), item.damage, item.knockBack, player.whoAmI)];
            projectile.GetGlobalProjectile<kProjectile>().elementalDamage = item.GetGlobalItem<kItem>().elementalDamage;
            projectile.scale = item.scale;
            ProceduralSwordThrow ps = (ProceduralSwordThrow)projectile.modProjectile;
            ps.hilt = hilt;
            ps.blade = blade;
            ps.accent = accent;
            ps.sword = item;
            ps.Initialize();
            if (player.meleeDamage >= player.rangedDamage && player.meleeDamage >= player.thrownDamage)
                projectile.melee = true;
            else if (player.thrownDamage >= player.rangedDamage)
                projectile.thrown = true;
            else
                projectile.ranged = true;
            return true;
        }*/

        public override bool UseItem(Player player)
        {
            try
            {
                if (spear /* && player.altFunctionUse != 2*/)
                {
                    var pos = player.position;
                    var unitVelocity = new Vector2(Main.mouseX - 12f, Main.mouseY - 24f) + Main.screenPosition - pos;
                    unitVelocity.Normalize();
                    var velocity = unitVelocity * 60f / item.useAnimation;
                    var projectile =
                        Main.projectile[
                            Projectile.NewProjectile(pos, velocity, GetInstance<ProceduralSpear>().projectile.type, item.damage, item.knockBack,
                                player.whoAmI)];
                    projectile.GetGlobalProjectile<kProjectile>().elementalDamage = item.GetGlobalItem<kItem>().elementalDamage;
                    projectile.scale = item.scale;
                    var ps = (ProceduralSpear) projectile.modProjectile;
                    ps.hilt = hilt;
                    ps.blade = blade;
                    ps.accent = accent;
                    if (Main.netMode != 2) ps.Initialize();
                    if (Main.netMode != 1)
                        return true;
                    var packet = mod.GetPacket();
                    packet.Write((byte) Message.SyncSpear);
                    packet.Write(blade.type);
                    packet.Write(hilt.type);
                    packet.Write(accent.type);
                    packet.Send();
                    return true;
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }

            return false;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (spear /* || player.altFunctionUse == 2*/)
            {
                noHitbox = true;
                return;
            }

            noHitbox = false;
        }
    }
}