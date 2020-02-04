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
    public class Moon_Yellow : Moon
    {
        public override float BaseDamageModifier()
        {
            return 1f - ProjCount * 0.05f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                int rotDistance = spell.Minion ? 32 : 48;
                spell.BasePosition += spell.BaseVelocity;
                var unitRelativePos = spell.RelativePos(spell.BasePosition);
                unitRelativePos.Normalize();
                spell.projectile.Center = spell.BasePosition + unitRelativePos * rotDistance;
                spell.DisplacementVelocity =
                    new Vector2(12f / spell.Source.ProjCount, 0f).RotatedBy(spell.RelativePos(spell.BasePosition).ToRotation() + (float) API.Tau / 4f);

                float angle = spell.DisplacementAngle + 0.24f * (-spell.projectile.timeLeft - rotDistance) / ProjCount;
                spell.projectile.Center = spell.BasePosition + new Vector2(0f, -rotDistance).RotatedBy(angle);

                spell.projectile.velocity = spell.DisplacementVelocity + spell.BaseVelocity;
            };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                int rotDistance = spell.Minion ? 32 : 48;
                float spread = GetSpread(spell.ProjCount);
                for (int i = 0; i < spell.ProjCount; i += 1)
                {
                    float angle = i * spread * (float) API.Tau;
                    var proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin + new Vector2(0f, -rotDistance).RotatedBy(angle), caster);
                    proj.BasePosition = origin;
                    var unitVelocity = target - origin;
                    unitVelocity.Normalize();
                    proj.BaseVelocity = unitVelocity * 8f;
                    proj.DisplacementAngle = angle;
                    proj.projectile.penetrate = 3;
                }
            };
        }

        private static float GetSpread(int projCount)
        {
            return 1f / projCount;
        }

        public override void Randomize()
        {
            base.Randomize();
            ProjCount = Main.rand.Next(2, 5);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Moon Glyph");
            Tooltip.SetDefault("Throws whirling projectiles that pierce the enemies");
        }
    }
}