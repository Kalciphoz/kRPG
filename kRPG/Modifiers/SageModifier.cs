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
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class SageModifier : NpcModifier
    {
        public SageModifier(kNPC kNpc, NPC npc) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Sagely " + npc.GivenName;
        }

        public ProceduralSpellProj RotMissile { get; set; }
        public ProceduralSpellProj RotSecondary { get; set; }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SageModifier(kNpc, npc);
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new SageModifier(kNpc, npc);
        }

        public override void Update(NPC kNpc)
        {
            try
            {
                const int rotDistance = 64;
                const int rotTimeLeft = 36000;

                if (RotMissile != null)
                    if (RotMissile.projectile.active && kNpc.active)
                        goto Secondary;
                    else
                        RotMissile.projectile.Kill();

                Projectile proj1 = Main.projectile[
                    Projectile.NewProjectile(kNpc.Center, new Vector2(0f, -1.5f), ModContent.ProjectileType<ProceduralSpellProj>(), kNpc.damage, 3f)];
                proj1.hostile = true;
                proj1.friendly = false;
                ProceduralSpellProj ps1 = (ProceduralSpellProj) proj1.modProjectile;
                ps1.Origin = proj1.position;
                Cross cross1 = Main.rand.Next(2) == 0 ? (Cross) new Cross_Red() : new Cross_Violet();
                ps1.ai.Add(delegate(ProceduralSpellProj spell)
                {
                    cross1.GetAiAction()(spell);

                    float displacementAngle = (float) API.Tau / 4f;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.Caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity = new Vector2(-2f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float) API.Tau / 4f);

                        float angle = displacementAngle - 0.06f * (rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.Caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.Caster.Center +
                                                  new Vector2(0f, -1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = displacementVelocity + spell.Caster.velocity;
                    spell.BasePosition = spell.Caster.position;
                });
                ps1.Inits.Add(cross1.GetInitAction());
                ps1.Caster = kNpc;
                ps1.Initialize();
                ps1.projectile.penetrate = -1;
                ps1.projectile.timeLeft = rotTimeLeft;
                RotMissile = ps1;

                Secondary:

                if (RotSecondary != null)
                    if (RotSecondary.projectile.active && kNpc.active)
                        return;
                    else
                        RotSecondary.projectile.Kill();

                Projectile proj2 = Main.projectile[
                    Projectile.NewProjectile(kNpc.Center, new Vector2(0f, 1.5f), ModContent.ProjectileType<ProceduralSpellProj>(), kNpc.damage, 3f)];
                proj2.hostile = true;
                proj2.friendly = false;
                ProceduralSpellProj ps2 = (ProceduralSpellProj) proj2.modProjectile;
                ps2.Origin = proj2.position;
                Cross cross2 = Main.rand.Next(2) == 0 ? (Cross) new Cross_Blue() : new Cross_Purple();
                ps2.ai.Add(delegate(ProceduralSpellProj spell)
                {
                    cross2.GetAiAction()(spell);

                    float displacementAngle = (float) API.Tau / 4f + (float) Math.PI;
                    Vector2 displacementVelocity = Vector2.Zero;
                    if (rotTimeLeft - spell.projectile.timeLeft >= rotDistance * 2 / 3)
                    {
                        Vector2 unitRelativePos = spell.RelativePos(spell.Caster.Center);
                        unitRelativePos.Normalize();
                        spell.projectile.Center = spell.Caster.Center + unitRelativePos * rotDistance;
                        displacementVelocity = new Vector2(-2f, 0f).RotatedBy(spell.RelativePos(spell.Caster.Center).ToRotation() + (float) API.Tau / 4f);

                        float angle = displacementAngle - 0.06f * (rotTimeLeft - spell.projectile.timeLeft - rotDistance * 2 / 3);
                        spell.projectile.Center = spell.Caster.Center + new Vector2(0f, -rotDistance).RotatedBy(angle);
                    }
                    else
                    {
                        spell.projectile.Center = spell.Caster.Center +
                                                  new Vector2(0f, 1.5f).RotatedBy(displacementAngle) * (rotTimeLeft - spell.projectile.timeLeft);
                    }

                    spell.projectile.velocity = displacementVelocity + spell.Caster.velocity;
                    spell.BasePosition = spell.Caster.position;
                });
                ps2.Inits.Add(cross2.GetInitAction());
                ps2.Caster = kNpc;
                ps2.Initialize();
                ps2.projectile.penetrate = -1;
                ps2.projectile.timeLeft = rotTimeLeft;
                RotSecondary = ps2;
            }
            catch (SystemException e)
            {
                Main.NewText(e.ToString());
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }
    }
}