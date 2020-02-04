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
using kRPG.Enums;
using kRPG.Items.Dusts;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Items.Glyphs
{
    public class Cross_Blue : Cross
    {
        public override Dictionary<ELEMENT, float> EleDmg { get; set; } =
            new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0}, {ELEMENT.COLD, 1f}, {ELEMENT.LIGHTNING, 0}, {ELEMENT.SHADOW, 0}};

        public override float BaseDamageModifier()
        {
            return 1.05f;
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (Main.rand.NextFloat(0f, 2f) <= spell.Alpha)
                {
                    int dust = Dust.NewDust(spell.projectile.position, spell.projectile.width, spell.projectile.height, ModContent.GetInstance<Ice>().Type, 0f,
                        0f, 100, Color.White, 0.5f + spell.Alpha);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(spell.projectile.Center, 0f, 0.4f, 1f);
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                spell.LocalTexture = GFX.ProjectileFrostbolt;
                spell.projectile.width = spell.LocalTexture.Width;
                spell.projectile.height = spell.LocalTexture.Height;
                spell.projectile.magic = true;
                spell.Lighted = true;
            };
        }

        public override Action<ProceduralSpellProj> GetKillAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                for (int k = 0; k < 8; k++)
                    try
                    {
                        Dust.NewDust(spell.projectile.position + spell.projectile.velocity, spell.projectile.width, spell.projectile.height,
                            ModContent.DustType<Ice>(), spell.projectile.oldVelocity.X * 0.5f, spell.projectile.oldVelocity.Y * 0.5f);
                    }
                    catch (SystemException e)
                    {
                        ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
                    }
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Cross Glyph");
            Tooltip.SetDefault("Casts magical ice cubes");
        }
    }
}