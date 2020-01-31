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
using System.Linq;
using kRPG2.Enums;
using kRPG2.Items;
using kRPG2.Items.Glyphs;
using kRPG2.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2
{
    public class kProjectile : GlobalProjectile
    {
        public Dictionary<ELEMENT, int> ElementalDamage { get; set; }

        public override bool InstancePerEntity => true;
        private Item Item { get; set; }

        public override void AI(Projectile projectile)
        {
            if (ElementalDamage != null || Main.netMode == 1)
                return;
            ElementalDamage = new Dictionary<ELEMENT, int> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

            if (Main.npc.GetUpperBound(0) >= projectile.owner)
                if (projectile.hostile && !projectile.friendly)
                {
                    bool bossfight = false;
                    foreach (var n in Main.npc)
                        if (n.active)
                            if (n.boss)
                                bossfight = true;
                    if (bossfight) return;

                    var player = Main.netMode == 2 ? Main.player[0] : Main.player[Main.myPlayer];
                    var haselement = new Dictionary<ELEMENT, bool>
                    {
                        {
                            ELEMENT.FIRE,
                            player.ZoneUnderworldHeight || player.ZoneTowerSolar || player.ZoneMeteor || player.ZoneDesert ||
                            Main.rand.Next(10) == 0 && Main.netMode == 0
                        },
                        {
                            ELEMENT.COLD,
                            player.ZoneSnow || player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneDungeon || player.ZoneRain ||
                            Main.rand.Next(10) == 0 && Main.netMode == 0
                        },
                        {
                            ELEMENT.LIGHTNING,
                            player.ZoneSkyHeight || player.ZoneTowerVortex || player.ZoneTowerStardust || player.ZoneMeteor || player.ZoneHoly ||
                            Main.rand.Next(10) == 0 && Main.netMode == 0
                        },
                        {
                            ELEMENT.SHADOW,
                            player.ZoneCorrupt || player.ZoneCrimson || player.ZoneUnderworldHeight || player.ZoneTowerNebula ||
                            !Main.dayTime && Main.rand.Next(10) == 0 && Main.netMode == 0 && player.ZoneOverworldHeight
                        }
                    };
                    int count = Enum.GetValues(typeof(ELEMENT)).Cast<ELEMENT>().Count(element => haselement[element]);
                    int portionsize = (int) Math.Round(projectile.damage * kNPC.EleDmgModifier / 3.0 / count);
                    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                        if (haselement[element])
                            ElementalDamage[element] = Math.Max(1, portionsize);
                    return;
                }

            if (projectile.type == ModContent.ProjectileType<ProceduralSpellProj>())
            {
                var character = Main.player[projectile.owner].GetModPlayer<PlayerCharacter>();
                var spell = (ProceduralSpellProj) projectile.modProjectile;
                if (spell.Source == null)
                {
                    SelectItem(projectile);
                }
                else
                {
                    var cross = (Cross) spell.Source.Glyphs[(int) GLYPHTYPE.CROSS].modItem;
                    if (cross is Cross_Orange)
                        SelectItem(projectile, character.LastSelectedWeapon);
                    else
                        foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                            ElementalDamage[element] = (int) Math.Round(cross.EleDmg[element] * projectile.damage);
                }
            }
            else if (projectile.friendly && !projectile.hostile && Main.player[projectile.owner] != null)
            {
                var player = Main.player[projectile.owner];
                if (!player.active)
                    return;
                if (player.inventory[player.selectedItem] == null)
                    return;
                if (player.inventory[player.selectedItem].active && projectile.type != ModContent.ProjectileType<Explosion>() &&
                    projectile.type != ModContent.ProjectileType<SmokePellets>())
                    SelectItem(projectile);
            }

            //if (Main.netMode != 0)
            //{
            //    ModPacket packet = mod.GetPacket();
            //    packet.Write((byte)Message.InitProjEleDmg);
            //    packet.Write(projectile.whoAmI);
            //    foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
            //        packet.Write(elementalDamage[element]);
            //    packet.Send();
            //}
        }

        public int GetEleDamage(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            var ele = new Dictionary<ELEMENT, int>();
            ele = GetIndividualElements(projectile, player, ignoreModifiers);
            return ele[ELEMENT.FIRE] + ele[ELEMENT.COLD] + ele[ELEMENT.LIGHTNING] + ele[ELEMENT.SHADOW];
        }

        public Dictionary<ELEMENT, int> GetIndividualElements(Projectile projectile, Player player, bool ignoreModifiers = false)
        {
            var dictionary = new Dictionary<ELEMENT, int>();
            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                dictionary[element] = 0;
            if (ElementalDamage == null)
                ElementalDamage = new Dictionary<ELEMENT, int> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};
            if (player.GetModPlayer<PlayerCharacter>().Rituals[RITUAL.DEMON_PACT])
                dictionary[ELEMENT.SHADOW] = GetEleDamage(projectile, player);
            else
                foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                    dictionary[element] = (int) Math.Round(ElementalDamage[element] * (ignoreModifiers
                                                               ? 1
                                                               : player.GetModPlayer<PlayerCharacter>().DamageMultiplier(element, projectile.melee,
                                                                   projectile.ranged, projectile.magic, projectile.thrown, projectile.minion)));

            return dictionary;
        }

        public void SelectItem(Projectile projectile, Item item)
        {
            Item = item;

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                ElementalDamage[element] = Item.GetGlobalItem<kItem>().ElementalDamage[element];
        }

        public void SelectItem(Projectile projectile)
        {
            var owner = Main.player[projectile.owner];
            Item = owner.inventory[owner.selectedItem];
            projectile.minion = Item.summon || projectile.minion;

            foreach (ELEMENT element in Enum.GetValues(typeof(ELEMENT)))
                ElementalDamage[element] = Item.GetGlobalItem<kItem>().ElementalDamage[element];
        }
    }
}