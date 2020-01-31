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
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Modifiers
{
    public abstract class NpcModifier
    {
        public NPC npc;

        public NpcModifier(kNPC kNpc, NPC npc)
        {
        }

        public virtual void Apply()
        {
        }

        public virtual void DrawEffects(NPC npc, ref Color drawColor)
        {
        }

        public virtual void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
        }

        public static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return null;
        }

        public virtual void NPCLoot(NPC npc)
        {
        }

        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
        }

        public virtual void PostAI(NPC npc)
        {
        }

        public static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return null;
        }

        public virtual void Read(BinaryReader reader)
        {
        }

        public virtual float StrikeNPC(NPC npc, double damage, int defense, float knockback, int hitDirection, bool crit)
        {
            return 1f;
        }

        public virtual void Update(NPC kNpc)
        {
        }

        public virtual void Write(ModPacket packet)
        {
        }
    }
}