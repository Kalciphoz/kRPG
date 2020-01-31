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
using kRPG2.Enums;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace kRPG2.Items.Weapons
{
    public class StaffOrnament : StaffPart
    {
        public StaffOrnament(string texture, int originX, int originY, string suffix, bool front = true, float mana = 1f, float dpsModifier = 1f,
            float speedModifier = 1f, float knockBack = 0f, int critBonus = 0, int repetitions = 0)
        {
            Type = Ornament.Count;
            if (Main.netMode != 2)
                if (texture != null)
                    Texture = ModLoader.GetMod("kRPG2").GetTexture("GFX/Items/Ornaments/" + texture);
            Origin = new Vector2(originX, originY);
            DpsModifier = dpsModifier;
            SpeedModifier = speedModifier;
            KnockBack = knockBack;
            CritBonus = critBonus;
            Suffix = suffix;
            Mana = mana;
            Front = front;
            Repetitions = repetitions;
            if (!Ornament.ContainsKey(Type))
                Ornament.Add(Type, this);
        }

        public static StaffOrnament Arcane { get; set; }
        public static StaffOrnament Bow { get; set; }
        public static StaffOrnament Cage { get; set; }
        public int CritBonus { get; set; }
        public static StaffOrnament Demonic { get; set; }
        public float DpsModifier { get; set; }

        public Dictionary<ELEMENT, float> EleDamage { get; set; } = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public static StaffOrnament Explosive { get; set; }

        public bool Front { get; set; }
        public float KnockBack { get; set; }
        public static StaffOrnament Loop { get; set; }
        public float Mana { get; set; }
        public static StaffOrnament None { get; set; }

        public Action<Player, NPC, Item, int, bool> OnHit { get; set; }

        public static Dictionary<int, StaffOrnament> Ornament { get; set; }
        public static Dictionary<STAFFTHEME, List<StaffOrnament>> OrnamentByTheme { get; set; }
        public int Repetitions { get; set; }
        public float SpeedModifier { get; set; }
        public string Suffix { get; set; }
        public static StaffOrnament Twig { get; set; }

        public int Type { get; set; }

        public static void Initialize()
        {
            Ornament = new Dictionary<int, StaffOrnament>();

            None = new StaffOrnament(null, 0, 0, "");
            Bow = new StaffOrnament("Bow", 4, 3, " of Wizardry", false, 1.1f, 1.1f, 1.1f, 1f, 5);
            Twig = new StaffOrnament("Twig", 2, 7, " of Longevity", true, 1.3f, 1f, 0.9f, 1f).SetEffect(
                delegate(Player player, NPC npc, Item item, int damage, bool crit)
                {
                    var character = player.GetModPlayer<PlayerCharacter>();
                    float distance = Vector2.Distance(npc.Center, character.player.Center);
                    int count = (int) (distance / 32);
                    var trail = new Trail(npc.Center, 60, delegate(SpriteBatch spriteBatch, Player p, Vector2 end, Vector2[] displacement, float scale)
                    {
                        for (int i = 0; i < count; i += 1)
                        {
                            var position = (npc.position - p.Center) * i / count + p.Center;
                            spriteBatch.Draw(GFX.Heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale,
                                SpriteEffects.None, 0f);
                        }
                    });
                    trail.Displacement = new Vector2[count];
                    for (int i = 0; i < count; i += 1)
                        trail.Displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    character.Trails.Add(trail);
                    int healAmount = Main.rand.Next(4) + 2;
                    player.statLife += healAmount;
                    player.HealEffect(healAmount);
                });
            Loop = new StaffOrnament("Loop", 0, 6, " of Reverberation", true, 1.5f, 1.2f, 1.5f, 0f, 0, 1);
            Arcane = new StaffOrnament("ArcaneSpider", 7, 8, " of Articulation", true, 1.1f, 1.2f);
            Cage = new StaffOrnament("ArcaneCage", 3, 10, " of Resonance", true, 2f, 1f, 3, 0f, 0, 2);
            Demonic = new StaffOrnament("Demonic", 8, 6, " of Demons", true, 1.5f, 1.2f, 1.5f, 0, 2, 1);
            Explosive = new StaffOrnament("Explosive", 6, 4, " of Blasting", false, 1.2f, 0.9f, 0.9f).SetEffect(
                delegate(Player player, NPC npc, Item item, int damage, bool crit)
                {
                    Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), player.Center);
                    var proj = Main.projectile[
                        Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<Explosion>(), damage / 2, 0f,
                            player.whoAmI)];
                });

            OrnamentByTheme = new Dictionary<STAFFTHEME, List<StaffOrnament>>
            {
                {STAFFTHEME.WOODEN, new List<StaffOrnament> {Bow, Twig, Loop}},
                {STAFFTHEME.DUNGEON, new List<StaffOrnament> {Arcane, Cage}},
                {STAFFTHEME.UNDERWORLD, new List<StaffOrnament> {Demonic, Explosive}}
            };
        }

        public static StaffOrnament RandomOrnament(STAFFTHEME theme)
        {
            return OrnamentByTheme[theme].Random();
        }

        public StaffOrnament SetEffect(Action<Player, NPC, Item, int, bool> onHit)
        {
            OnHit = onHit;
            return this;
        }

        public StaffOrnament SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            EleDamage = eleDamage;
            return this;
        }

        public static void Unload()
        {
            foreach (var o in Ornament.Values)
                o.Texture = null;
        }
    }
}