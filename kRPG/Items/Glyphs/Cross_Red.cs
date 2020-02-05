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
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class Cross_Red : Cross
    {
        public override Dictionary<ELEMENT, float> EleDmg { get; set; } =
            new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 1f}, {ELEMENT.COLD, 0}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    ProceduralSpellProj.aiRotateToVelocity(spell);
                    if (!(Main.rand.NextFloat(0f, 1.5f) <= spell.Alpha))
                        return;
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, DustID.Fire,
                        spell.projectile.velocity.X * 0.2f, spell.projectile.velocity.Y * 0.2f, 63, Color.White, 1f + spell.Alpha * 2f);
                    Main.dust[dust].noGravity = true;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    spell.LocalTexture = GFX.ProjectileFireball;
                    spell.projectile.width = spell.LocalTexture.Width;
                    spell.projectile.height = spell.LocalTexture.Height;
                    spell.projectile.magic = true;
                    spell.DrawTrail = true;
                    spell.Lighted = true;
                    spell.projectile.scale = spell.Minion ? 0.7f : 1f;
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                try
                {
                    for (int k = 0; k < 20; k++)
                        Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height, DustID.Fire,
                            spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f, 0, default, 1.5f);
                }
                catch (SystemException e)
                {
                    ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Cross Glyph");
            Tooltip.SetDefault("Casts magical fireballs");
        }
    }
}