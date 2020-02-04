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
using System.Linq;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Items.Glyphs
{
    public class Moon_Green : Moon
    {
        public const int RotTimeLeft = 3600;

        public override float BaseDamageModifier()
        {
            return 1.24f - ProjCount * 0.08f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    int rotDistance = spell.Minion ? 72 : 96;
                    if (RotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        var unitRelativePos = spell.RelativePos(spell.Caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                        spell.DisplacementVelocity =
                            new Vector2(1.5f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float) API.Tau / 4f);

                        float angle = spell.DisplacementAngle + 0.04f * (RotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.Caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.Caster.Center +
                                                  new Vector2(0f, -1.5f).RotatedBy(spell.DisplacementAngle) * (RotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = spell.DisplacementVelocity + spell.Caster.velocity;
                    spell.BasePosition = spell.Caster.position;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG2").Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpell, Player, Vector2, Vector2, Entity> GetCastAction()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 origin, Vector2 target, Entity caster)
            {
                switch (caster)
                {
                    case Player p:
                    {
                        var character = p.GetModPlayer<PlayerCharacter>();
                        foreach (var proj in character.CirclingProtection.Where(proj => proj.projectile.modProjectile is ProceduralSpellProj))
                            proj.projectile.Kill();
                        character.CirclingProtection.Clear();
                        break;
                    }
                    case Projectile pj:
                    {
                        var minion = (ProceduralMinion) pj.modProjectile;
                        foreach (var proj in minion.CirclingProtection.Where(proj => proj.projectile.modProjectile is ProceduralSpellProj))
                            proj.projectile.Kill();
                        minion.CirclingProtection.Clear();
                        break;
                    }
                }

                float spread = GetSpread(spell.ProjCount);
                var velocity = new Vector2(0f, -1.5f);
                for (int i = 0; i < spell.ProjCount; i += 1)
                {
                    var proj = spell.CreateProjectile(player, Vector2.Zero, spread * i, origin, caster);
                    proj.projectile.timeLeft = RotTimeLeft;
                    proj.DisplacementVelocity = velocity.RotatedBy(i * spread * API.Tau);
                    proj.DisplacementAngle = i * spread * (float) API.Tau;
                    switch (caster)
                    {
                        case Player _:
                            player.GetModPlayer<PlayerCharacter>().CirclingProtection.Add(proj);
                            break;
                        case Projectile pj:
                            ((ProceduralMinion) pj.modProjectile).CirclingProtection.Add(proj);
                            break;
                    }
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
            ProjCount = Main.rand.Next(3, 11);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Moon Glyph");
            Tooltip.SetDefault("Casts projectiles to orbit around you");
        }
    }
}