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
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Items.Glyphs
{
    public class Cross_Orange : Cross
    {
        public override float BaseDamageModifier()
        {
            return 0.9f;
        }

        public override float BaseManaModifier()
        {
            return 0.9f;
        }

        public override bool CanUse()
        {
            Player owner = Main.player[Main.myPlayer];
            PlayerCharacter character = owner.GetModPlayer<PlayerCharacter>();
            Item item = character.LastSelectedWeapon;
            return owner.inventory.Contains(item);
        }

        public override Action<ProceduralSpellProj> GetAiAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (spell.projectile.velocity.X < 0 && spell.BasePosition == Vector2.Zero) spell.projectile.spriteDirection = -1;
                Vector2 v = spell.BasePosition != Vector2.Zero ? spell.BasePosition : spell.Origin;
                if (spell.projectile.spriteDirection == -1)
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() - (float) API.Tau * 5f / 8f;
                else
                    spell.projectile.rotation = (spell.projectile.Center - v).ToRotation() + (float) API.Tau / 8f;
            };
        }

        public override Action<ProceduralSpellProj> GetInitAction()
        {
            return delegate(ProceduralSpellProj spell)
            {
                if (Main.netMode != 2)
                {
                    if (Main.netMode == 0 || spell.projectile.owner == Main.myPlayer)
                    {
                        PlayerCharacter character = Main.player[spell.projectile.owner].GetModPlayer<PlayerCharacter>();

                        spell.LocalTexture = character.LastSelectedWeapon.modItem is ProceduralSword
                            ? ((ProceduralSword) character.LastSelectedWeapon.modItem).texture
                            : Main.itemTexture[character.LastSelectedWeapon.type];
                    }
                    else
                    {
                        spell.LocalTexture = GFX.ProjectileBoulder;
                    }

                    spell.projectile.width = spell.LocalTexture.Width;
                    spell.projectile.height = spell.LocalTexture.Height;
                }
                else
                {
                    spell.projectile.width = 48;
                    spell.projectile.height = 48;
                }

                spell.projectile.melee = true;
                spell.DrawTrail = true;
                spell.Alpha = 1f;
                spell.Lighted = true;
                spell.projectile.scale = spell.Minion ? 0.6f : 1f;
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orange Cross Glyph");
            Tooltip.SetDefault("Creates copies of your selected melee weapon");
        }
    }
}