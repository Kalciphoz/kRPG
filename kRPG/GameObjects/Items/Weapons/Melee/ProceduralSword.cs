using System;
using System.Collections.Generic;
using System.IO;
using kRPG.Enums;
using kRPG.GameObjects.Items.Procedural;
using kRPG.GameObjects.Items.Projectiles.Base;
using kRPG.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace kRPG.GameObjects.Items.Weapons.Melee
{
    public class ProceduralSword : ProceduralItem, IProcedural
    {
        public SwordAccent Accent { get; set; }
        public SwordBlade Blade { get; set; }

        //public override void ModifyTooltips(List<TooltipLine> tooltips)
        //{
        //    tooltips.Add(new TooltipLine(mod, "dps", "base dps " + dps.ToString()));
        //    tooltips.Add(new TooltipLine(mod, "enemyDef", "average enemy defense " + dps.ToString()));
        //}

        public Dictionary<Element, float> EleDamage { get; set; } = new Dictionary<Element, float> {{Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}};

        public SwordHilt Hilt { get; set; }
        public bool Lighted { get; set; }
        public bool Spear { get; set; }

        public override void Initialize()
        {
            try
            {
                ResetStats();
                if (Main.netMode != Constants.NetModes.Server)
                    LocalTexture = GFX.GFX.CombineTextures(new List<Texture2D> {Blade.Texture, Hilt.Texture, Accent.Texture},
                        new List<Point>
                        {
                            new Point(CombinedTextureSize().X - Blade.Texture.Width, 0),
                            new Point(0, CombinedTextureSize().Y - Hilt.Texture.Height),
                            new Point((int) Hilt.Origin.X + Hilt.AccentOffset.X - (int) Accent.Origin.X, Hilt.AccentOffset.Y + CombinedTextureSize().Y - Hilt.Texture.Height + (int) Hilt.Origin.Y - (int) Accent.Origin.Y)
                        }, CombinedTextureSize());
                if (Main.netMode != Constants.NetModes.Server)
                    item.width = LocalTexture.Width;

                if (Main.netMode != Constants.NetModes.Server)
                    item.height = LocalTexture.Height;

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
                    item.useStyle = (int)UseStyles.HoldingOut;
                }

                item.GetGlobalItem<kItem>().ApplyStats(item, true);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("Blade|Hilt|Accent" + (Blade == null) + (Hilt == null) + (Accent == null));
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
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Loading :: " + e);
            }

            try
            {
                Initialize();
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@Initialize :: " + e);
            }
        }

        //public Texture2D OverhaulGetTexture()
        //{
        //    return LocalTexture;
        //}

        //public bool? OverhaulHasTag(string tag)
        //{
        //    return (Spear ? tag == "spear" : tag == "broadsword") ? (bool?) true : null;
        //}

        public override void ResetStats()
        {
            if (Blade == null || Hilt == null)
            {
                kRPG.LogMessage("Um, Blade or the Hilt is null!  This shouldn't happen.");
                return;
            }

            try
            {
                //blade and hilt are null!.

                item.rare = (int) Math.Min(Math.Floor(Dps / 15.0), 9);
                item.useAnimation = (int) (Blade.UseTime / Hilt.SpeedModifier);
                item.damage = (int) Math.Round(Dps * Hilt.DpsModifier * (Accent?.DpsModifier ?? 1) * item.useAnimation / 60f + EnemyDef);
                item.useAnimation = (int) Math.Round((item.damage - (float) EnemyDef) * 60f / (Dps * Hilt.DpsModifier * (Accent?.DpsModifier ?? 1)));
                item.useTime = item.useAnimation;
                item.knockBack = Blade.KnockBack + Hilt.KnockBack;
                item.SetNameOverride(Hilt.Prefix + Blade.Name + (Accent?.Suffix ?? ""));
                item.autoReuse = Hilt.AutoSwing || Blade.AutoSwing;
                item.useTurn = item.autoReuse;
                item.value = (int) (Dps * 315);
                item.crit = Blade.CritBonus + Hilt.CritBonus + (Accent?.CritBonus ?? 0);
                item.scale = 1f + Blade.Scale + Hilt.Scale;
                item.mana = Hilt.Mana + (Accent?.Mana ?? 0);
                EleDamage = new Dictionary<Element, float>();
                foreach (Element element in Enum.GetValues(typeof(Element)))
                    EleDamage[element] = Blade.EleDamage[element] + (Accent?.EleDamage[element] ?? 0);
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }
        }

        public override ModItem Clone(Item tItem)
        {
            ProceduralSword copy = (ProceduralSword) base.Clone(tItem);
            copy.Hilt = Hilt;
            copy.Blade = Blade;
            copy.Accent = Accent;
            copy.Dps = Dps;
            copy.EnemyDef = EnemyDef;
            copy.Spear = Spear;
            copy.EleDamage = new Dictionary<Element, float>();
            foreach (Element element in Enum.GetValues(typeof(Element)))
                copy.EleDamage[element] = EleDamage[element];
            copy.item.SetNameOverride(tItem.Name);
            return copy;
        }

        public Point CombinedTextureSize()
        {
            return new Point(Math.Max(Blade.Texture.Width, Blade.Texture.Width - (int) Blade.Origin.X + (int) Hilt.Origin.X), Math.Max(Blade.Texture.Height, (int) Blade.Origin.Y + Hilt.Texture.Height - (int) Hilt.Origin.Y));
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (LocalTexture == null)
            {
                item.SetDefaults(0, true);
                return;
            }

            spriteBatch.Draw(LocalTexture, position, null, Lighted ? Color.White : color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawHeld(PlayerDrawInfo drawInfo, Color color, float rotation, float scale, Vector2 playerCenter)
        {
            try
            {
                Player player = Main.player[Main.myPlayer];
                Vector2 position = new Vector2(4f * player.direction, -4f).RotatedBy(rotation) + playerCenter;
                if (LocalTexture == null)
                    //Removing references to SetDefaults
                    //item.SetDefaults();
                    return;

                SpriteEffects effects = player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                DrawData draw = new DrawData(LocalTexture, position, null, Lighted ? Color.White : color, rotation, new Vector2(player.direction > 0 ? 0 : LocalTexture.Width, LocalTexture.Height), scale, effects, 0);
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
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }
        }

        public static Item GenerateSword(Mod mod, Vector2 position, SwordTheme theme, float dps, int enemyDef)
        {
            ProceduralSword sword = NewSword(mod, position, SwordHilt.RandomHilt(theme), SwordBlade.RandomBlade(theme), Main.rand.Next(5) < 3 ? SwordAccent.RandomAccent() : SwordAccent.None, dps, enemyDef);
            return sword.item;
        }

        public override void MeleeEffects(Player player, Rectangle hitBox)
        {
            

            Blade.Effect?.Invoke(hitBox, player);

            Accent.Effect?.Invoke(hitBox, player);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            bool proceed = reader.ReadBoolean();

            if (!proceed)
                return;

            int blade = reader.ReadInt32();
            int hilt = reader.ReadInt32();
            int accent = reader.ReadInt32();
            float dps = reader.ReadSingle();
            int enemyDef = reader.ReadInt32();

            Blade = SwordBlade.Blades[blade];
            Hilt = SwordHilt.Hilts[hilt];
            Accent = SwordAccent.Accents[accent];
            Dps = dps;
            EnemyDef = enemyDef;
            Initialize();
        }

        public override void NetSend(BinaryWriter writer)
        {
            bool proceed = Blade != null && Hilt != null;

            writer.Write(proceed);

            if (!proceed)
                return;

            writer.Write(Blade.Type);
            writer.Write(Hilt.Type);
            writer.Write(Accent.Type);
            writer.Write(Dps);
            writer.Write(EnemyDef);
        }

        public static ProceduralSword NewSword(Mod mod, Vector2 position, SwordHilt hilt, SwordBlade blade, SwordAccent accent, float dps, int enemyDef)
        {
            if (hilt == null)
                kRPG.LogMessage("Ummm....  Why is the hilt null?");
            if (blade == null)
                kRPG.LogMessage("Ummm....  Why is the blade null?");

            int id = Item.NewItem(position, mod.GetItem("ProceduralSword").item.type);
            ProceduralSword sword = (ProceduralSword) Main.item[id].modItem;
            sword.Hilt = hilt;
            sword.Blade = blade;
            sword.Accent = accent;
            sword.Dps = dps;
            sword.EnemyDef = enemyDef;
            sword.Initialize();

            SwordInitPacket.Write(id, blade.Type, hilt.Type, accent.Type, dps, enemyDef);

            return sword;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Accent.OnHit?.Invoke(player, target, this, damage, crit);
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
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("@NewTagCompound :: " + e);
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat("Blade|Hilt|Accent" + (Blade == null) + (Hilt == null) + (Accent == null));
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
            item.useStyle = (int)UseStyles.GeneralSwingingThrowing;
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
                    Vector2 pos = player.position;
                    Vector2 unitVelocity = new Vector2(Main.mouseX - 12f, Main.mouseY - 24f) + Main.screenPosition - pos;
                    unitVelocity.Normalize();
                    Vector2 velocity = unitVelocity * 60f / item.useAnimation;
                    Projectile projectile = Main.projectile[Projectile.NewProjectile(pos, velocity, GetInstance<ProceduralSpear>().projectile.type, item.damage, item.knockBack, player.whoAmI)];
                    projectile.GetGlobalProjectile<kProjectile>().ElementalDamage = item.GetGlobalItem<kItem>().ElementalDamage;
                    projectile.scale = item.scale;
                    ProceduralSpear ps = (ProceduralSpear) projectile.modProjectile;
                    ps.Hilt = Hilt;
                    ps.Blade = Blade;
                    ps.Accent = Accent;

                    if (Main.netMode != Constants.NetModes.Server)
                        ps.Initialize();

                    if (Main.netMode != Constants.NetModes.Client)
                        return true;

                    SyncSpearPacket.Write(Blade.Type, Hilt.Type, Accent.Type);

                    return true;
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod(Constants.ModName).Logger.InfoFormat(e.ToString());
            }

            return false;
        }

        // ReSharper disable once RedundantAssignment
        public override void UseItemHitbox(Player player, ref Rectangle hitBox, ref bool noHitBox)
        {
            if (Spear /* || player.altFunctionUse == 2*/)
            {
                noHitBox = true;
                return;
            }

            noHitBox = false;
        }
    }
}