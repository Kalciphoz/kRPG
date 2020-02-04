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
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG2.Items.Glyphs
{
    public class Moon_Purple : Moon
    {
        public override float BaseDamageModifier()
        {
            return 2.6f - ProjCount * 0.06f;
        }

        public override float BaseManaModifier()
        {
            return 1.2f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell) { spell.projectile.velocity.Y += 0.3f; };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                new SpellEffect(spell, target, ProjCount * 10, delegate(ProceduralSpell ability, int timeLeft)
                {
                    if (timeLeft % 10 != 0)
                        return;
                    var proj = spell.CreateProjectile(player, new Vector2(0, -9f), Main.rand.NextFloat(-0.07f, 0.07f), caster.Center + new Vector2(0, -16f),
                        caster);
                    if (proj.Alpha < 1f) proj.Alpha = 0.5f;
                    proj.projectile.tileCollide = true;
                });
            };
        }

        public override void Randomize()
        {
            base.Randomize();
            ProjCount = Main.rand.Next(10, 21);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Moon Glyph");
            Tooltip.SetDefault("Shoots up a fountain of projectiles to rain down around you");
        }
    }
}