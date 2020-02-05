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
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Moon_Blue : Moon
    {
        public override float BaseDamageModifier()
        {
            return 1.2f;
        }

        public override float BaseManaModifier()
        {
            return 0.72f + ProjCount * 0.06f;
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                float spread = GetSpread(spell.ProjCount);
                Vector2 unitVelocity = target - origin;
                unitVelocity.Normalize();
                Vector2 velocity = unitVelocity * 6f;
                for (int i = 0; i < spell.ProjCount; i += 1)
                    spell.CreateProjectile(player, velocity, spell.ProjCount * -spread / 2f + i * spread + spread / 2f, origin, caster);
            };
        }

        private static float GetSpread(int projCount)
        {
            return 0.020f - projCount * 0.001f;
        }

        public override void Randomize()
        {
            base.Randomize();
            ProjCount = Main.rand.Next(3, 8);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Moon Glyph");
            Tooltip.SetDefault("Fires an array of projectiles outwards");
        }
    }
}