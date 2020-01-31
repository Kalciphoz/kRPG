using System;
using System.Collections.Generic;
using System.IO;
using kRPG.Enums;
using kRPG.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace kRPG.Items
{
    public class ProceduralStaff : ProceduralItem
    {
        /*public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "dps", "base dps " + dps.ToString()));
            tooltips.Add(new TooltipLine(mod, "enemyDef", "average enemy defense " + dps.ToString()));
        }*/

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public StaffGem gem;
        public StaffOrnament ornament;
        public Staff staff;

        public override ModItem Clone(Item item)
        {
            ProceduralStaff copy = (ProceduralStaff) base.Clone(item);
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

        public Point CombinedTextureSize()
        {
            if (ornament.type == 0)
                return new Point(gem.texture.Width - (int) gem.origin.X + (int) staff.origin.X,
                    (int) gem.origin.Y + staff.texture.Height - (int) staff.origin.Y);
            return ornament.origin.Y > gem.origin.Y
                ? new Point(ornament.texture.Width - (int) ornament.origin.X + (int) staff.origin.X,
                    (int) ornament.origin.Y + staff.texture.Height - (int) staff.origin.Y)
                : new Point(gem.texture.Width - (int) gem.origin.X + (int) staff.origin.X, (int) gem.origin.Y + staff.texture.Height - (int) staff.origin.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (texture == null)
                item.SetDefaults(0, true);
            spriteBatch.Draw(texture, position, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawHeld(PlayerDrawInfo drawinfo, Color color, float rotation, float scale, Vector2 playerCenter)
        {
            try
            {
                Player player = Main.player[Main.myPlayer];
                Vector2 position = new Vector2(4f * player.direction, -4f).RotatedBy(rotation) + playerCenter;
                if (texture == null)
                    item.SetDefaults(0, true);
                SpriteEffects effects = player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                DrawData draw = new DrawData(texture, position, null, color, rotation, new Vector2(player.direction > 0 ? 0 : texture.Width, texture.Height), scale,
                    effects, 0);
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

        public static ProceduralStaff DropStaff(Mod mod, Vector2 position, Staff staffstaff, StaffGem staffgem, StaffOrnament staffornament, float dps,
            int enemyDef)
        {
            int id = Item.NewItem(position, mod.GetItem("ProceduralStaff").item.type);
            ProceduralStaff staff = (ProceduralStaff) Main.item[id].modItem;
            staff.staff = staffstaff;
            staff.gem = staffgem;
            staff.ornament = staffornament;
            staff.dps = dps;
            staff.enemyDef = enemyDef;
            staff.Initialize();
            if (Main.netMode != 2)
                return staff;
            ModPacket packet = mod.GetPacket();
            packet.Write((byte) Message.StaffInit);
            packet.Write(id);
            packet.Write(staffstaff.type);
            packet.Write(staffgem.type);
            packet.Write(staffornament.type);
            packet.Write(dps);
            packet.Write(enemyDef);
            packet.Send();
            return staff;
        }

        public static Item GenerateStaff(Mod mod, Vector2 position, STAFFTHEME theme, float dps, int enemyDef)
        {
            ProceduralStaff staff;
            staff = DropStaff(mod, position, Staff.RandomStaff(theme), StaffGem.RandomGem(theme),
                Main.rand.Next(3) < 2 ? StaffOrnament.RandomOrnament(theme) : StaffOrnament.none, dps, enemyDef);
            return staff.item;
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
            if (Main.netMode != 2)
                texture = GFX.CombineTextures(new List<Texture2D> {parts[0].texture, parts[1].texture, parts[2].texture},
                    new List<Point>
                    {
                        parts[0].GetDrawOrigin(new Point(staff.texture.Width, staff.texture.Height), new Point((int) staff.origin.X, (int) staff.origin.Y),
                            CombinedTextureSize()),
                        parts[1].GetDrawOrigin(new Point(staff.texture.Width, staff.texture.Height), new Point((int) staff.origin.X, (int) staff.origin.Y),
                            CombinedTextureSize()),
                        parts[2].GetDrawOrigin(new Point(staff.texture.Width, staff.texture.Height), new Point((int) staff.origin.X, (int) staff.origin.Y),
                            CombinedTextureSize())
                    }, CombinedTextureSize());
            if (Main.netMode != 2) item.width = texture.Width;
            if (Main.netMode != 2) item.height = texture.Height;
            item.shoot = gem.shoot;
            item.shootSpeed = staff.shootSpeed;
            item.GetGlobalItem<kItem>().ApplyStats(item, true);
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int) Math.Round(dps / 2.4f)));
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

        public Texture2D OverhaulGetTexture()
        {
            return texture;
        }

        public bool? OverhaulHasTag(string tag)
        {
            return tag == "magicWeapon" ? (bool?) true : null;
        }

        public void ResetStats()
        {
            try
            {
                item.rare = (int) Math.Min(Math.Floor(dps / 18.0), 9);
                item.useTime = (int) (staff.useTime / gem.speedModifier / ornament.speedModifier);
                item.damage = (int) Math.Round(dps * gem.dpsModifier * ornament.dpsModifier * item.useTime / 60f + enemyDef);
                item.useTime = (int) Math.Round((item.damage - (float) enemyDef) * 60f / (dps * gem.dpsModifier * ornament.dpsModifier));
                item.useAnimation = item.useTime * staff.iterations * (1 + ornament.repetitions) - 2;
                item.knockBack = staff.knockBack + gem.knockBack + ornament.knockBack;
                item.SetNameOverride(staff.prefix + gem.name + ornament.suffix);
                item.autoReuse = true;
                item.value = (int) (dps * 315);
                item.crit = staff.critBonus + gem.critBonus + ornament.critBonus;
                item.mana = (int) Math.Round(item.damage * ornament.mana * staff.mana * gem.mana / 5);
                eleDamage = new Dictionary<ELEMENT, float>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    eleDamage[element] = staff.eleDamage[element] + gem.eleDamage[element] + ornament.eleDamage[element];
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
                    {"staff_id", staff.type},
                    {"gem_id", gem.type},
                    {"ornament_id", ornament.type},
                    {"dps", dps},
                    {"enemy_defence", enemyDef}
                };
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat("@NewTagCompound :: " + e);
            }

            return new TagCompound();
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
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }

            return false;
        }
    }
}