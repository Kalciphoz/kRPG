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
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG2.Items.Glyphs
{
    public class Moon_Violet : Moon
    {
        public const float area = 192f;

        public override float BaseDamageModifier()
        {
            return 1.1f - ProjCount * 0.02f;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                new SpellEffect(spell, target, ProjCount * 8, delegate(ProceduralSpell ability, int timeLeft)
                {
                    if (timeLeft % 8 != 0)
                        return;
                    var proj = spell.CreateProjectile(player, new Vector2(0f, 8f), 0f,
                        new Vector2(target.X - area / 2f + Main.rand.NextFloat(area), target.Y - 240f), caster);
                    if (proj.Alpha < 1f) proj.Alpha = 0.5f;
                    proj.projectile.timeLeft = 60;
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
            DisplayName.SetDefault("Violet Moon Glyph");
            Tooltip.SetDefault("Rains down projectiles at the target location");
        }
    }
}