using System;
using System.Collections.Generic;
using System.IO;
using kRPG2.Enums;
using kRPG2.Items.Weapons;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace kRPG2.Items
{
    public class ProceduralSword : ProceduralItem
    {
        public SwordAccent Accent { get; set; }
        public SwordBlade Blade { get; set; }

        //public override void ModifyTooltips(List<TooltipLine> tooltips)
        //{
        //    tooltips.Add(new TooltipLine(mod, "dps", "base dps " + dps.ToString()));
        //    tooltips.Add(new TooltipLine(mod, "enemyDef", "average enemy defense " + dps.ToString()));
        //}

        public Dictionary<ELEMENT, float> EleDamage { get; set; } = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public SwordHilt Hilt { get; set; }
        public bool Lighted { get; set; }
        public bool Spear { get; set; }

        public override ModItem Clone(Item tItem)
        {
            var copy = (ProceduralSword) base.Clone(tItem);
            copy.Hilt = Hilt;
            copy.Blade = Blade;
            copy.Accent = Accent;
            copy.Dps = Dps;
            copy.EnemyDef = EnemyDef;
            copy.Spear = Spear;
            copy.EleDamage = new Dictionary<ELEMENT, float>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                copy.EleDamage[element] = EleDamage[element];
            copy.item.SetNameOverride(tItem.Name);
            return copy;
        }

        public Point CombinedTextureSize()
        {
            return new Point(Math.Max(Blade.Texture.Width, Blade.Texture.Width - (int) Blade.Origin.X + (int) Hilt.Origin.X),
                Math.Max(Blade.Texture.Height, (int) Blade.Origin.Y + Hilt.Texture.Height - (int) Hilt.Origin.Y));
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (texture == null)
            {
                item.SetDefaults(0, true);
                return;
            }

            spriteBatch.Draw(texture, position, null, Lighted ? Color.White : color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
                var draw = new DrawData(texture, position, null, Lighted ? Color.White : color, rotation,
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
                ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
            }
        }

        public static Item GenerateSword(Mod mod, Vector2 position, SWORDTHEME theme, float dps, int enemyDef)
        {
            ProceduralSword sword;
            sword = NewSword(mod, position, SwordHilt.RandomHilt(theme), SwordBlade.RandomBlade(theme),
                Main.rand.Next(5) < 3 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, enemyDef);
            return sword.item;
        }

        public override void Initialize()
        {
            try
            {
                ResetStats();
                if (Main.netMode != 2)
                    texture = GFX.CombineTextures(new List<Texture2D> {Blade.Texture, Hilt.Texture, Accent.Texture},
                        new List<Point>
                        {
                            new Point(CombinedTextureSize().X - Blade.Texture.Width, 0),
                            new Point(0, CombinedTextureSize().Y - Hilt.Texture.Height),
                            new Point((int) Hilt.Origin.X + Hilt.AccentOffset.X - (int) Accent.Origin.X,
                                Hilt.AccentOffset.Y + CombinedTextureSize().Y - Hilt.Texture.Height + (int) Hilt.Origin.Y - (int) Accent.Origin.Y)
                        }, CombinedTextureSize());
                if (Main.netMode != 2) item.width = texture.Width;
                if (Main.netMode != 2) item.height = texture.Height;
                if (Accent.Type == SwordAccent.GemPurple.Type)
                {
                    item.melee = false;
                    item.magic = true;
                }

                Lighted = Blade.Lighted;
                Spear = Hilt.Spear && Blade.Spearable;
                if (Spear)
                {
                    item.noMelee = true;
                    item.noUseGraphic = true;
                    item.useStyle = 5;
                }

                item.GetGlobalItem<kItem>().ApplyStats(item, true);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("Blade|Hilt|Accent" + (Blade == null) + (Hilt == null) + (Accent == null));
            }
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                Hilt = SwordHilt.Hilts[tag.GetInt("hilt_id")];
                Blade = SwordBlade.Blades[tag.GetInt("blade_id")];
                Accent = SwordAccent.Accents[tag.GetInt("accent_id")];
                Dps = tag.GetFloat("dps");
                EnemyDef = tag.GetInt("enemy_defence");
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Loading :: " + e);
            }

            try
            {
                Initialize();
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@Initialize :: " + e);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Blade.Effect?.Invoke(hitbox, player);

            Accent.Effect?.Invoke(hitbox, player);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;
            Blade = SwordBlade.Blades[reader.ReadInt32()];
            Hilt = SwordHilt.Hilts[reader.ReadInt32()];
            Accent = SwordAccent.Accents[reader.ReadInt32()];
            Dps = reader.ReadSingle();
            EnemyDef = reader.ReadInt32();
            Initialize();
        }

        public override void NetSend(BinaryWriter writer)
        {
            bool proceed = Blade != null;
            writer.Write(proceed);
            if (!proceed) return;
            writer.Write(Blade.Type);
            writer.Write(Hilt.Type);
            writer.Write(Accent.Type);
            writer.Write(Dps);
            writer.Write(EnemyDef);
        }

        public static ProceduralSword NewSword(Mod mod, Vector2 position, SwordHilt hilt, SwordBlade blade, SwordAccent accent, float dps, int enemyDef)
        {
            int id = Item.NewItem(position, mod.GetItem("ProceduralSword").item.type);
            var sword = (ProceduralSword) Main.item[id].modItem;
            sword.Hilt = hilt;
            sword.Blade = blade;
            sword.Accent = accent;
            sword.Dps = dps;
            sword.EnemyDef = enemyDef;
            sword.Initialize();
            if (Main.netMode != 2)
                return sword;
            var packet = mod.GetPacket();
            packet.Write((byte) Message.SwordInit);
            packet.Write(id);
            packet.Write(blade.Type);
            packet.Write(hilt.Type);
            packet.Write(accent.Type);
            packet.Write(dps);
            packet.Write(enemyDef);
            packet.Send();
            return sword;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Accent.OnHit?.Invoke(player, target, this, damage, crit);
        }

        public Texture2D OverhaulGetTexture()
        {
            return texture;
        }

        public bool? OverhaulHasTag(string tag)
        {
            return (Spear ? tag == "spear" : tag == "broadsword") ? (bool?) true : null;
        }

        public void ResetStats()
        {
            try
            {
                item.rare = (int) Math.Min(Math.Floor(Dps / 15.0), 9);
                item.useAnimation = (int) (Blade.UseTime / Hilt.SpeedModifier);
                item.damage = (int) Math.Round(Dps * Hilt.DpsModifier * Accent.DpsModifier * item.useAnimation / 60f + EnemyDef);
                item.useAnimation = (int) Math.Round((item.damage - (float) EnemyDef) * 60f / (Dps * Hilt.DpsModifier * Accent.DpsModifier));
                item.useTime = item.useAnimation;
                item.knockBack = Blade.KnockBack + Hilt.KnockBack;
                item.SetNameOverride(Hilt.Prefix + Blade.Name + Accent.Suffix);
                item.autoReuse = Hilt.AutoSwing || Blade.AutoSwing;
                item.useTurn = item.autoReuse;
                item.value = (int) (Dps * 315);
                item.crit = Blade.CritBonus + Hilt.CritBonus + Accent.CritBonus;
                item.scale = 1f + Blade.Scale + Hilt.Scale;
                item.mana = Hilt.Mana + Accent.Mana;
                EleDamage = new Dictionary<ELEMENT, float>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    EleDamage[element] = Blade.EleDamage[element] + Accent.EleDamage[element];
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
            }
        }

        public override TagCompound Save()
        {
            try
            {
                return new TagCompound
                {
                    {"hilt_id", Hilt.Type},
                    {"blade_id", Blade.Type},
                    {"accent_id", Accent.Type},
                    {"dps", Dps},
                    {"enemy_defence", EnemyDef}
                };
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("@NewTagCompound :: " + e);
                ModLoader.GetMod("kRPG2").Logger.InfoFormat("Blade|Hilt|Accent" + (Blade == null) + (Hilt == null) + (Accent == null));
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
            Projectile projectile = Main.projectile[Projectile.NewProjectile(pos, velocity, mod.ProjectileType<ProceduralSwordThrow>(), tItem.damage, tItem.knockBack, player.whoAmI)];
            projectile.GetGlobalProjectile<kProjectile>().elementalDamage = tItem.GetGlobalItem<kItem>().elementalDamage;
            projectile.scale = tItem.scale;
            ProceduralSwordThrow ps = (ProceduralSwordThrow)projectile.modProjectile;
            ps.hilt = hilt;
            ps.blade = blade;
            ps.accent = accent;
            ps.sword = tItem;
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
                if (Spear /* && player.altFunctionUse != 2*/)
                {
                    var pos = player.position;
                    var unitVelocity = new Vector2(Main.mouseX - 12f, Main.mouseY - 24f) + Main.screenPosition - pos;
                    unitVelocity.Normalize();
                    var velocity = unitVelocity * 60f / item.useAnimation;
                    var projectile =
                        Main.projectile[
                            Projectile.NewProjectile(pos, velocity, GetInstance<ProceduralSpear>().projectile.type, item.damage, item.knockBack,
                                player.whoAmI)];
                    projectile.GetGlobalProjectile<kProjectile>().ElementalDamage = item.GetGlobalItem<kItem>().ElementalDamage;
                    projectile.scale = item.scale;
                    var ps = (ProceduralSpear) projectile.modProjectile;
                    ps.Hilt = Hilt;
                    ps.Blade = Blade;
                    ps.Accent = Accent;
                    if (Main.netMode != 2) ps.Initialize();
                    if (Main.netMode != 1)
                        return true;
                    var packet = mod.GetPacket();
                    packet.Write((byte) Message.SyncSpear);
                    packet.Write(Blade.Type);
                    packet.Write(Hilt.Type);
                    packet.Write(Accent.Type);
                    packet.Send();
                    return true;
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
            }

            return false;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Spear /* || player.altFunctionUse == 2*/)
            {
                noHitbox = true;
                return;
            }

            noHitbox = false;
        }
    }
}