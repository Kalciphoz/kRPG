//  Fairfield Tek L.L.C.
//  Copyright (c) 2016, Fairfield Tek L.L.C.
// 
// 
// THIS SOFTWARE IS PROVIDED BY FairfieldTek LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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

        public Dictionary<ELEMENT, float> EleDamage { get; set; } = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public StaffGem Gem { get; set; }
        public StaffOrnament Ornament { get; set; }
        public Staff Staff { get; set; }

        public override ModItem Clone(Item tItem)
        {
            ProceduralStaff copy = (ProceduralStaff) base.Clone(tItem);
            copy.Staff = Staff;
            copy.Gem = Gem;
            copy.Ornament = Ornament;
            copy.Dps = Dps;
            copy.EnemyDef = EnemyDef;
            copy.EleDamage = new Dictionary<ELEMENT, float>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                copy.EleDamage[element] = EleDamage[element];
            copy.item.SetNameOverride(tItem.Name);
            return copy;
        }

        public Point CombinedTextureSize()
        {
            if (Ornament.Type == 0)
                return new Point(Gem.Texture.Width - (int) Gem.Origin.X + (int) Staff.Origin.X,
                    (int) Gem.Origin.Y + Staff.Texture.Height - (int) Staff.Origin.Y);
            return Ornament.Origin.Y > Gem.Origin.Y
                ? new Point(Ornament.Texture.Width - (int) Ornament.Origin.X + (int) Staff.Origin.X,
                    (int) Ornament.Origin.Y + Staff.Texture.Height - (int) Staff.Origin.Y)
                : new Point(Gem.Texture.Width - (int) Gem.Origin.X + (int) Staff.Origin.X, (int) Gem.Origin.Y + Staff.Texture.Height - (int) Staff.Origin.Y);
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
            staff.Staff = staffstaff;
            staff.Gem = staffgem;
            staff.Ornament = staffornament;
            staff.Dps = dps;
            staff.EnemyDef = enemyDef;
            staff.Initialize();
            if (Main.netMode != 2)
                return staff;
            ModPacket packet = mod.GetPacket();
            packet.Write((byte) Message.StaffInit);
            packet.Write(id);
            packet.Write(staffstaff.Type);
            packet.Write(staffgem.Type);
            packet.Write(staffornament.Type);
            packet.Write(dps);
            packet.Write(enemyDef);
            packet.Send();
            return staff;
        }

        public static Item GenerateStaff(Mod mod, Vector2 position, STAFFTHEME theme, float dps, int enemyDef)
        {
            ProceduralStaff staff;
            staff = DropStaff(mod, position, Staff.RandomStaff(theme), StaffGem.RandomGem(theme),
                Main.rand.Next(3) < 2 ? StaffOrnament.RandomOrnament(theme) : StaffOrnament.None, dps, enemyDef);
            return staff.item;
        }

        public override void Initialize()
        {
            ResetStats();
            List<StaffPart> parts = new List<StaffPart>();
            if (!Ornament.Front) parts.Add(Ornament);
            if (!Staff.Front && !Gem.Back) parts.Add(Staff);
            parts.Add(Gem);
            if (Staff.Front || Gem.Back) parts.Add(Staff);
            if (Ornament.Front) parts.Add(Ornament);
            if (Main.netMode != 2)
                texture = GFX.CombineTextures(new List<Texture2D> {parts[0].Texture, parts[1].Texture, parts[2].Texture},
                    new List<Point>
                    {
                        parts[0].GetDrawOrigin(new Point(Staff.Texture.Width, Staff.Texture.Height), new Point((int) Staff.Origin.X, (int) Staff.Origin.Y),
                            CombinedTextureSize()),
                        parts[1].GetDrawOrigin(new Point(Staff.Texture.Width, Staff.Texture.Height), new Point((int) Staff.Origin.X, (int) Staff.Origin.Y),
                            CombinedTextureSize()),
                        parts[2].GetDrawOrigin(new Point(Staff.Texture.Width, Staff.Texture.Height), new Point((int) Staff.Origin.X, (int) Staff.Origin.Y),
                            CombinedTextureSize())
                    }, CombinedTextureSize());
            if (Main.netMode != 2) item.width = texture.Width;
            if (Main.netMode != 2) item.height = texture.Height;
            item.shoot = Gem.Shoot;
            item.shootSpeed = Staff.ShootSpeed;
            item.GetGlobalItem<kItem>().ApplyStats(item, true);
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                Staff = Staff.Staffs[tag.GetInt("staff_id")];
                Gem = StaffGem.Gems[tag.GetInt("gem_id")];
                Ornament = StaffOrnament.Ornament[tag.GetInt("ornament_id")];
                Dps = tag.GetFloat("dps");
                EnemyDef = tag.GetInt("enemy_defence");
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
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int) Math.Round(Dps / 2.4f)));
        }

        public override void NetRecieve(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;
            Gem = StaffGem.Gems[reader.ReadInt32()];
            Staff = Staff.Staffs[reader.ReadInt32()];
            Ornament = StaffOrnament.Ornament[reader.ReadInt32()];
            Dps = reader.ReadSingle();
            EnemyDef = reader.ReadInt32();
            Initialize();
        }

        public override void NetSend(BinaryWriter writer)
        {
            bool proceed = Gem != null;
            writer.Write(proceed);
            if (!proceed) return;
            writer.Write(Gem.Type);
            writer.Write(Staff.Type);
            writer.Write(Ornament.Type);
            writer.Write(Dps);
            writer.Write(EnemyDef);
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
                item.rare = (int) Math.Min(Math.Floor(Dps / 18.0), 9);
                item.useTime = (int) (Staff.UseTime / Gem.SpeedModifier / Ornament.SpeedModifier);
                item.damage = (int) Math.Round(Dps * Gem.DpsModifier * Ornament.DpsModifier * item.useTime / 60f + EnemyDef);
                item.useTime = (int) Math.Round((item.damage - (float) EnemyDef) * 60f / (Dps * Gem.DpsModifier * Ornament.DpsModifier));
                item.useAnimation = item.useTime * Staff.Iterations * (1 + Ornament.Repetitions) - 2;
                item.knockBack = Staff.KnockBack + Gem.KnockBack + Ornament.KnockBack;
                item.SetNameOverride(Staff.Prefix + Gem.Name + Ornament.Suffix);
                item.autoReuse = true;
                item.value = (int) (Dps * 315);
                item.crit = Staff.CritBonus + Gem.CritBonus + Ornament.CritBonus;
                item.mana = (int) Math.Round(item.damage * Ornament.Mana * Staff.Mana * Gem.Mana / 5);
                EleDamage = new Dictionary<ELEMENT, float>();
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    EleDamage[element] = Staff.EleDamage[element] + Gem.eleDamage[element] + Ornament.EleDamage[element];
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
                    {"staff_id", Staff.Type},
                    {"gem_id", Gem.Type},
                    {"ornament_id", Ornament.Type},
                    {"dps", Dps},
                    {"enemy_defence", EnemyDef}
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
            return Gem.Projectile == null;
        }

        public override bool UseItem(Player player)
        {
            try
            {
                if (Gem.Projectile != null)
                {
                    Gem.Projectile(player, item);
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