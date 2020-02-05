// Kalciphoz's RPG Mod
//  Copyright (c) 2016, Kalciphoz's RPG Mod
// 
// 
// THIS SOFTWARE IS PROVIDED BY Kalciphoz's ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
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
using System.Linq;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class Star_Orange : Star
    {
        public override float BaseDamageModifier()
        {
            return 0.6f;
        }

        public override float BaseManaModifier()
        {
            return 1.35f;
        }

        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 target)
            {
                Main.PlaySound(SoundID.Item6, player.position);
                spell.Remaining = spell.Cooldown;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                if (character.Minions.Exists(minion => minion is WingedEyeball))
                    foreach (ProceduralMinion eyeball in character.Minions.Where(minion => minion.projectile.type == ModContent.ProjectileType<WingedEyeball>())
                    )
                    {
                        foreach (ProceduralSpellProj psp in eyeball.CirclingProtection)
                            psp.projectile.Kill();
                        eyeball.CirclingProtection.Clear();
                        eyeball.SmallProt?.projectile.Kill();
                        eyeball.projectile.Kill();
                    }

                Projectile eye = Main.projectile[
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<WingedEyeball>(), 0, 0f, player.whoAmI)];
                eye.Center = target;
                WingedEyeball we = (WingedEyeball) eye.modProjectile;
                we.Source = spell;
                foreach (GlyphModifier modifier in spell.Modifiers.Where(modifier => modifier.MinionAi != null))
                    we.GlyphModifiers.Add(modifier.MinionAi);
                character.Minions.Add((WingedEyeball) eye.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Star Glyph");
            Tooltip.SetDefault("Summons a winged eyeball to cast the spell");
        }
    }
}