using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

using kRPG.Projectiles;
using kRPG.Items.Weapons;
using System.IO;

namespace kRPG.Items
{
    public enum SWORDTHEME : byte { GENERIC, MONSTROUS, RUNIC, HELLISH, HARDMODE };
    public enum STAFFTHEME : byte { WOODEN, DUNGEON, UNDERWORLD };

    public class ProceduralItem : ModItem
    {
        public Texture2D texture;
        public float dps;
        public int enemyDef;

        public override bool CanPickup(Player player)
        {
            if (Main.netMode == 0) return true;
            else return item.value > 100;
        }

        public virtual void Initialize() { }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale) { }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.scale = 1f;
            item.noMelee = true;
        }

        public override ModItem Clone(Item item)
        {
            ProceduralItem copy = (ProceduralItem)base.Clone(item);
            copy.texture = texture;
            return copy;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Item; Please Ignore");
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Main.netMode == 2 || texture == null) return false;
            Draw(spriteBatch, item.position - Main.screenPosition, lightColor, rotation, scale);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.netMode == 2 || texture == null) return false;
            float s = scale * Main.itemTexture[item.type].Height / texture.Height;
            Draw(spriteBatch, position, drawColor, 0f, s);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int)Math.Round(dps / 2)));
        }
    }

    public class ProceduralStaff : ProceduralItem
    {
        public Staff staff;
        public StaffGem gem;
        public StaffOrnament ornament;

        /*public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "dps", "base dps " + dps.ToString()));
            tooltips.Add(new TooltipLine(mod, "enemyDef", "average enemy defense " + dps.ToString()));
        }*/

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>()
        {
            { ELEMENT.FIRE, 0f },
            { ELEMENT.COLD, 0f },
            { ELEMENT.LIGHTNING, 0f },
            { ELEMENT.SHADOW, 0f }
        };

        public override ModItem Clone(Item item)
        {
            ProceduralStaff copy = (ProceduralStaff)base.Clone(item);
            copy.staff = staff;
            copy.gem = gem;
            copy.ornament = ornament;
            copy.dps = dps;
            copy.enemyDef = enemyDef;
            copy.eleDamage = new Dictionary<ELEMENT, float>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                copy.eleDamage[element] = eleDamage[element];
            copy.item.SetNameOverride(item.Name);
            return copy;
        }

        public override void SetDefaults()
        {
            item.damage = 1;
            item.magic = true;
            item.width = 34;
            item.height = 34;
            item.useTime = 1;
            item.useAnimation = 1;
            item.useStyle = 5;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item43;
            item.scale = 1f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useTurn = false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Staff; Please Ignore");
        }

        public override void NetSend(BinaryWriter writer)
        {
            bool proceed = gem != null;
            writer.Write(proceed);
            if (!proceed) return;
            writer.Write(gem.type);
            writer.Write(staff.type);
            writer.Write(ornament.type);
            writer.Write(dps);
            writer.Write(enemyDef);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;
            gem = StaffGem.gems[reader.ReadInt32()];
            staff = Staff.staves[reader.ReadInt32()];
            ornament = StaffOrnament.ornament[reader.ReadInt32()];
            dps = reader.ReadSingle();
            enemyDef = reader.ReadInt32();
            Initialize();
        }

        public override void Initialize()
        {
            ResetStats();
            List<StaffPart> parts = new List<StaffPart>();
            if (!ornament.front) parts.Add(ornament);
            if (!staff.front && !gem.back) parts.Add(staff);
            parts.Add(gem);
            if (staff.front || gem.back) parts.Add(staff);
            if (ornament.front) parts.Add(ornament);
            if (Main.netMode != 2) texture = GFX.CombineTextures(new List<Texture2D>() {
                { parts[0].texture },
                { parts[1].texture },
                { parts[2].texture }
            }, new List<Point>() {
                { parts[0].GetDrawOrigin(new Point(staff.texture.Width, staff.texture.Height), new Point((int)staff.origin.X, (int)staff.origin.Y), CombinedTextureSize()) },
                { parts[1].GetDrawOrigin(new Point(staff.texture.Width, staff.texture.Height), new Point((int)staff.origin.X, (int)staff.origin.Y), CombinedTextureSize()) },
                { parts[2].GetDrawOrigin(new Point(staff.texture.Width, staff.texture.Height), new Point((int)staff.origin.X, (int)staff.origin.Y), CombinedTextureSize()) }
            }, CombinedTextureSize());
            if (Main.netMode != 2) item.width = texture.Width;
            if (Main.netMode != 2) item.height = texture.Height;
            item.shoot = gem.shoot;
            item.shootSpeed = staff.shootSpeed;
            item.GetGlobalItem<kItem>().ApplyStats(item, true);
        }

        public void ResetStats()
        {
            try
            {
                item.rare = (int)Math.Min(Math.Floor(dps / 18.0), 9);
                item.useTime = (int)(staff.useTime / gem.speedModifier / ornament.speedModifier);
                item.damage = (int)Math.Round(dps * gem.dpsModifier * ornament.dpsModifier * (float)item.useTime / 60f + (float)enemyDef);
                item.useTime = (int)Math.Round(((float)item.damage - (float)enemyDef) * 60f / (dps * gem.dpsModifier * ornament.dpsModifier));
                item.useAnimation = item.useTime * staff.iterations * (1 + ornament.repetitions) - 2;
                item.knockBack = staff.knockBack + gem.knockBack + ornament.knockBack;
                item.SetNameOverride(staff.prefix + gem.name + ornament.suffix);
                item.autoReuse = true;
                item.value = (int)(dps * 315);
                item.crit = staff.critBonus + gem.critBonus + ornament.critBonus;
                item.mana = (int)Math.Round((item.damage * ornament.mana * staff.mana * gem.mana) / 5);
                eleDamage = new Dictionary<ELEMENT, float>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                {
                    eleDamage[element] = staff.eleDamage[element] + gem.eleDamage[element] + ornament.eleDamage[element];
                }
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            return gem.projectile == null;
        }

        public override bool UseItem(Player player)
        {
            try
            {
                if (gem.projectile != null)
                {
                    gem.projectile(player, item);
                    Main.PlaySound(item.UseSound, player.Center);
                    return true;
                }
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
            return false;
        }

        public Point CombinedTextureSize()
        {
            if (ornament.type != 0)
            {
                if (ornament.origin.Y > gem.origin.Y)
                    return new Point(ornament.texture.Width - (int)ornament.origin.X + (int)staff.origin.X, (int)ornament.origin.Y + staff.texture.Height - (int)staff.origin.Y);
            }
            return new Point(gem.texture.Width - (int)gem.origin.X + (int)staff.origin.X, (int)gem.origin.Y + staff.texture.Height - (int)staff.origin.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (texture == null)
            {
                item.SetDefaults(0,true);
            }
            spriteBatch.Draw(texture, position, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawHeld(PlayerDrawInfo drawinfo, Color color, float rotation, float scale, Vector2 playerCenter)
        {
            try
            {
                Player player = Main.player[Main.myPlayer];
                Vector2 position = new Vector2(4f * player.direction, -4f).RotatedBy(rotation) + playerCenter;
                if (texture == null)
                {
                    item.SetDefaults(0, true);
                }
                SpriteEffects effects = player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                DrawData draw = new DrawData(texture, position, null, color, rotation, new Vector2(player.direction > 0 ? 0 : texture.Width, texture.Height), scale, effects, 0);
                for (int i = 0; i < Main.playerDrawData.Count; i += 1)
                {
                    if (Main.playerDrawData[i].texture == Main.itemTexture[player.inventory[player.selectedItem].type])
                    {
                        Main.playerDrawData.Insert(i, draw);
                        return;
                    }
                }
                Main.playerDrawData.Add(draw);
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public static Item GenerateStaff(Mod mod, Vector2 position, STAFFTHEME theme, float dps, int enemyDef)
        {
            ProceduralStaff staff;
            staff = DropStaff(mod, position, Staff.RandomStaff(theme), StaffGem.RandomGem(theme), Main.rand.Next(3) < 2 ? StaffOrnament.RandomOrnament(theme) : StaffOrnament.none, dps, enemyDef);
            return staff.item;
        }

        public static ProceduralStaff DropStaff(Mod mod, Vector2 position, Staff staffstaff, StaffGem staffgem, StaffOrnament staffornament, float dps, int enemyDef)
        {
            int id = Item.NewItem(position, mod.GetItem("ProceduralStaff").item.type);
            ProceduralStaff staff = (ProceduralStaff)Main.item[id].modItem;
            staff.staff = staffstaff;
            staff.gem = staffgem;
            staff.ornament = staffornament;
            staff.dps = dps;
            staff.enemyDef = enemyDef;
            staff.Initialize();
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.StaffInit);
                packet.Write(id);
                packet.Write(staffstaff.type);
                packet.Write(staffgem.type);
                packet.Write(staffornament.type);
                packet.Write(dps);
                packet.Write(enemyDef);
                packet.Send();
            }
            return staff;
        }

        public override TagCompound Save()
        {
            try
            {
                return new TagCompound
                {
                    { "staff_id", staff.type },
                    { "gem_id", gem.type },
                    { "ornament_id", ornament.type },
                    { "dps", dps },
                    { "enemy_defence", enemyDef }
                };
            }
            catch (SystemException e)
            {
                ErrorLogger.Log("@NewTagCompound :: " + e.ToString());
            }
            return new TagCompound();
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                staff = Staff.staves[tag.GetInt("staff_id")];
                gem = StaffGem.gems[tag.GetInt("gem_id")];
                ornament = StaffOrnament.ornament[tag.GetInt("ornament_id")];
                dps = tag.GetFloat("dps");
                enemyDef = tag.GetInt("enemy_defence");
            }
            catch (SystemException e)
            {
                ErrorLogger.Log("@Loading :: " + e.ToString());
            }
            try
            {
                Initialize();
            }
            catch (SystemException e)
            {
                ErrorLogger.Log("@Initialize :: " + e.ToString());
            }
        }

        public bool? OverhaulHasTag(string tag)
        {
            return tag == "magicWeapon" ? (bool?)true : null;
        }

        public Texture2D OverhaulGetTexture()
        {
            return texture;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int)Math.Round(dps / 2.4f)));
        }
    }

    public class ProceduralSword : ProceduralItem
    {
        public SwordHilt hilt;
        public SwordBlade blade;
        public SwordAccent accent;
        public bool spear = false;
        public bool lighted = false;

        //public override void ModifyTooltips(List<TooltipLine> tooltips)
        //{
        //    tooltips.Add(new TooltipLine(mod, "dps", "base dps " + dps.ToString()));
        //    tooltips.Add(new TooltipLine(mod, "enemyDef", "average enemy defense " + dps.ToString()));
        //}

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>()
        {
            { ELEMENT.FIRE, 0f },
            { ELEMENT.COLD, 0f },
            { ELEMENT.LIGHTNING, 0f },
            { ELEMENT.SHADOW, 0f }
        };

        public override ModItem Clone(Item item)
        {
            ProceduralSword copy = (ProceduralSword)base.Clone(item);
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

        public override void Initialize()
        {
            try
            {
                ResetStats();
                if (Main.netMode != 2) texture = GFX.CombineTextures(new List<Texture2D>() {
                    { blade.texture },
                    { hilt.texture },
                    { accent.texture }
                }, new List<Point>() {
                    { new Point(CombinedTextureSize().X - blade.texture.Width, 0) },
                    { new Point(0, CombinedTextureSize().Y - hilt.texture.Height) },
                    { new Point((int)hilt.origin.X + hilt.accentOffset.X - (int)accent.origin.X, hilt.accentOffset.Y + CombinedTextureSize().Y - hilt.texture.Height + (int)hilt.origin.Y - (int)accent.origin.Y) }
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
                ErrorLogger.Log(e.ToString());
                ErrorLogger.Log("Blade|Hilt|Accent" + (blade == null) + (hilt == null) + (accent == null));
            }
        }

        public void ResetStats()
        {
            try
            {
                item.rare = (int)Math.Min(Math.Floor(dps / 15.0), 9);
                item.useAnimation = (int)(blade.useTime / hilt.speedModifier);
                item.damage = (int)Math.Round(dps * hilt.dpsModifier * accent.dpsModifier * (float)item.useAnimation / 60f + (float)enemyDef);
                item.useAnimation = (int)Math.Round(((float)item.damage - (float)enemyDef) * 60f / (dps * hilt.dpsModifier * accent.dpsModifier));
                item.useTime = item.useAnimation;
                item.knockBack = blade.knockBack + hilt.knockBack;
                item.SetNameOverride(hilt.prefix + blade.name + accent.suffix);
                item.autoReuse = hilt.autoswing || blade.autoswing;
                item.useTurn = item.autoReuse;
                item.value = (int)(dps * 315);
                item.crit = blade.critBonus + hilt.critBonus + accent.critBonus;
                item.scale = 1f + blade.scale + hilt.scale;
                item.mana = hilt.mana + accent.mana;
                eleDamage = new Dictionary<ELEMENT, float>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                {
                    eleDamage[element] = blade.eleDamage[element] + accent.eleDamage[element];
                }
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        /*public override bool AltFunctionUse(Player player)
        {
            Vector2 pos = player.position; //  + new Vector2(2f * player.direction, 4f)
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
                if (spear/* && player.altFunctionUse != 2*/)
                {
                    Vector2 pos = player.position;
                    Vector2 unitVelocity = (new Vector2(Main.mouseX - 12f, Main.mouseY - 24f) + Main.screenPosition - pos);
                    unitVelocity.Normalize();
                    Vector2 velocity = unitVelocity * 60f / item.useAnimation;
                    Projectile projectile = Main.projectile[Projectile.NewProjectile(pos, velocity, mod.GetProjectile<ProceduralSpear>().projectile.type, item.damage, item.knockBack, player.whoAmI)];
                    projectile.GetGlobalProjectile<kProjectile>().elementalDamage = item.GetGlobalItem<kItem>().elementalDamage;
                    projectile.scale = item.scale;
                    ProceduralSpear ps = (ProceduralSpear)projectile.modProjectile;
                    ps.hilt = hilt;
                    ps.blade = blade;
                    ps.accent = accent;
                    if (Main.netMode != 2) ps.Initialize();
                    if (Main.netMode == 1)
                    {
                        ModPacket packet = mod.GetPacket();
                        packet.Write((byte)Message.SyncSpear);
                        packet.Write(blade.type);
                        packet.Write(hilt.type);
                        packet.Write(accent.type);
                        packet.Send();
                    }
                    return true;
                }
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (accent.onHit != null) accent.onHit(player, target, this, damage, crit);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (blade.effect != null)
            {
                blade.effect(hitbox, player);
            }

            if (accent.effect != null)
            {
                accent.effect(hitbox, player);
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (spear/* || player.altFunctionUse == 2*/)
            {
                noHitbox = true;
                return;
            }
            noHitbox = false;
        }

        public Point CombinedTextureSize()
        {
            return new Point(Math.Max(blade.texture.Width, blade.texture.Width - (int)blade.origin.X + (int)hilt.origin.X), Math.Max(blade.texture.Height, (int)blade.origin.Y + hilt.texture.Height - (int)hilt.origin.Y));
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (texture == null)
            {
                item.SetDefaults(0,true);
                return;
            }
            spriteBatch.Draw(texture, position, null, lighted ? Color.White : color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawHeld(PlayerDrawInfo drawinfo, Color color, float rotation, float scale, Vector2 playerCenter)
        {
            try
            {
                Player player = Main.player[Main.myPlayer];
                Vector2 position = new Vector2(4f * player.direction, -4f).RotatedBy(rotation) + playerCenter;
                if (texture == null)
                {
                    item.SetDefaults(0);
                    return;
                }
                SpriteEffects effects = player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                DrawData draw = new DrawData(texture, position, null, lighted ? Color.White : color, rotation, new Vector2(player.direction > 0 ? 0 : texture.Width, texture.Height), scale, effects, 0);
                for (int i = 0; i < Main.playerDrawData.Count; i += 1)
                {
                    if (Main.playerDrawData[i].texture == Main.itemTexture[player.inventory[player.selectedItem].type])
                    {
                        Main.playerDrawData.Insert(i, draw);
                        return;
                    }
                }
                Main.playerDrawData.Add(draw);
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public static Item GenerateSword(Mod mod, Vector2 position, SWORDTHEME theme, float dps, int enemyDef)
        {
            ProceduralSword sword;
            sword = NewSword(mod, position, SwordHilt.RandomHilt(theme), SwordBlade.RandomBlade(theme), Main.rand.Next(5) < 3 ? SwordAccent.RandomAccent() : SwordAccent.none, dps, enemyDef);
            return sword.item;
        }

        public static ProceduralSword NewSword(Mod mod, Vector2 position, SwordHilt hilt, SwordBlade blade, SwordAccent accent, float dps, int enemyDef)
        {
            int id = Item.NewItem(position, mod.GetItem("ProceduralSword").item.type);
            ProceduralSword sword = (ProceduralSword)Main.item[id].modItem;
            sword.hilt = hilt;
            sword.blade = blade;
            sword.accent = accent;
            sword.dps = dps;
            sword.enemyDef = enemyDef;
            sword.Initialize();
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.SwordInit);
                packet.Write(id);
                packet.Write(blade.type);
                packet.Write(hilt.type);
                packet.Write(accent.type);
                packet.Write(dps);
                packet.Write(enemyDef);
                packet.Send();
            }
            return sword;
        }

        public override TagCompound Save()
        {
            try
            {
                return new TagCompound
                {
                    { "hilt_id", hilt.type },
                    { "blade_id", blade.type },
                    { "accent_id", accent.type },
                    { "dps", dps },
                    { "enemy_defence", enemyDef }
                };
            }
            catch (SystemException e)
            {
                ErrorLogger.Log("@NewTagCompound :: " + e.ToString());
                ErrorLogger.Log("Blade|Hilt|Accent" + (blade == null) + (hilt == null) + (accent == null));
            }

            return new TagCompound();
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
                ErrorLogger.Log("@Loading :: " + e.ToString());
            }
            try
            { 
                Initialize();
            }
            catch (SystemException e)
            {
                ErrorLogger.Log("@Initialize :: " + e.ToString());
            }
        }

        public bool? OverhaulHasTag(string tag)
        {
            return (spear ? tag == "spear" : tag == "broadsword") ? (bool?)true : null;
        }

        public Texture2D OverhaulGetTexture()
        {
            return texture;
        }
    }
}
