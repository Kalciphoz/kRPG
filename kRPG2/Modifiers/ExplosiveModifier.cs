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

using System.IO;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace kRPG2.Modifiers
{
    public class ExplosiveModifier : NpcModifier
    {
        public ExplosiveModifier(kNPC kNpc, NPC npc, float lifeModifier = 0.5f) : base(kNpc, npc)
        {
            this.npc = npc;
            npc.GivenName = "Explosive " + npc.GivenName;
            Apply();
        }

        private float LifeModifier { get; set; } = 0.5f;

        public override void Apply()
        {
            npc.lifeMax = (int) (npc.lifeMax * LifeModifier);
            npc.life = (int) (npc.life * LifeModifier);
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new ExplosiveModifier(kNpc, npc);
        }

        public override void NPCLoot(NPC npc)
        {
            Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), npc.Center);
            var proj = Main.projectile[
                Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<NpcExplosion>(), npc.damage * 5 / 4, 0f)];
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new ExplosiveModifier(kNpc, npc, Main.rand.NextFloat(0.5f, 0.9f));
        }

        public override void Read(BinaryReader reader)
        {
            LifeModifier = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(LifeModifier);
        }
    }
}