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
using System.Linq;
using kRPG.Enums;
using kRPG.Items.Glyphs;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.Projectiles
{
    public class ProceduralMinion : ProceduralProjectile
    {
        public bool Attack { get; set; }
        public List<ProceduralSpellProj> CirclingProtection { get; set; } = new List<ProceduralSpellProj>();

        public int Cooldown { get; set; }
        protected float Distance { get; set; }
        public List<Action<ProceduralMinion>> GlyphModifiers { get; set; } = new List<Action<ProceduralMinion>>();

        // ReSharper disable once IdentifierTypo
        public ProceduralSpellProj SmallProt { get; set; } = null;
        public ProceduralSpell Source { get; set; }
        protected NPC Target { get; set; }

        public override void AI()
        {
            if (Main.netMode == 2) return;
            bool self = Source.Glyphs[(byte) GLYPHTYPE.MOON].modItem is Moon_Green;
            if ((!self || CirclingProtection.Count(spell => spell.projectile.active) <= Source.ProjCount - 3) && Cooldown <= 0)
            {
                if (!self)
                {
                    GetTarget();
                    if (Distance <= 480f && Attack)
                        if (this is ProceduralMinion)
                            Source.CastSpell(Main.player[projectile.owner], projectile.Center, Target.Center, projectile);
                }
                else if (this is ProceduralMinion)
                {
                    Source.CastSpell(Main.player[projectile.owner], projectile.Center, projectile.Center, projectile);
                }

                Cooldown = Source.Cooldown * 2;
            }
            else
            {
                Cooldown -= 1;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public NPC GetTarget()
        {
            Attack = false;
            Target = Main.npc.First();
            Player player = Main.player[projectile.owner];
            Distance = Vector2.Distance(projectile.Center, Target.Center);
            foreach (NPC npc in Main.npc)
            {
                float f = Vector2.Distance(projectile.Center, npc.Center);
                if (!(f < Distance) || !npc.active || npc.life <= 0 || npc.friendly || npc.damage <= 0)
                    continue;
                Target = npc;
                Distance = f;
                Attack = true;
            }

            if (!player.HasMinionAttackTargetNPC)
                return Target;
            Target = Main.npc[player.MinionAttackTargetNPC];
            Attack = true;

            return Target;
        }

        public override void Kill(int timeLeft)
        {
            foreach (ProceduralSpellProj spell in CirclingProtection)
                spell.projectile.Kill();
            CirclingProtection.Clear();
            SmallProt?.projectile.Kill();
        }

        public override void PostAI()
        {
            foreach (Action<ProceduralMinion> modifier in GlyphModifiers)
                modifier(this);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Minion; Please Ignore");
        }
    }
}