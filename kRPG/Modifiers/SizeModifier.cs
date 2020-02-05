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
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Modifiers
{
    public class SizeModifier : NpcModifier
    {
        public SizeModifier(kNPC kNpc, NPC npc, float scaleModifier = 1.1f, float lifeModifier = 1.4f) : base(kNpc, npc)
        {
            this.npc = npc;
            ScaleModifier = scaleModifier;
            LifeModifier = lifeModifier;
            Apply();
        }

        private float LifeModifier { get; set; }
        private float ScaleModifier { get; set; }

        public override void Apply()
        {
            npc.scale *= ScaleModifier;
            npc.lifeMax = (int) (npc.lifeMax * LifeModifier);
            npc.life = (int) (npc.life * LifeModifier);
            if (ScaleModifier < 1)
                npc.GivenName = "Small " + npc.GivenName;
            else
                npc.GivenName = "Massive " + npc.GivenName;
            npc.GetGlobalNPC<kNPC>().SpeedModifier *= (float) Math.Pow(ScaleModifier, 0.9);
        }

        public new static NpcModifier New(kNPC kNpc, NPC npc)
        {
            return new SizeModifier(kNpc, npc);
        }

        public new static NpcModifier Random(kNPC kNpc, NPC npc)
        {
            return new SizeModifier(kNpc, npc, .5f + Main.rand.NextFloat(2), .5f + Main.rand.NextFloat(1));
        }

        public override void Read(BinaryReader reader)
        {
            ScaleModifier = reader.ReadSingle();
            LifeModifier = reader.ReadSingle();
        }

        public override void Write(ModPacket packet)
        {
            packet.Write(ScaleModifier);
            packet.Write(LifeModifier);
        }
    }
}