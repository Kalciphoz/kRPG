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

using kRPG.Enums;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Buffs
{
    /// <summary>
    ///     The cold buff
    /// </summary>
    public class Cold : ModBuff
    {
        /// <summary>
        ///     Default Values
        /// </summary>
        public override void SetDefaults()
        {
            //Name shown when in effect
            DisplayName.SetDefault("Chilled");
            //Description of effect
            Description.SetDefault("Slowed movement and increased chance to receive critical hits");
            //This buff is a debuff
            Main.debuff[Type] = true;
            //Don't save the effect on the player when they leave the game.
            Main.buffNoSave[Type] = true;
        }

        /// <summary>
        ///     Allows you to make this buff give certain effects to the given NPC. If you remove the buff from the NPC, make sure
        ///     to decrement the buffIndex parameter by 1.
        /// </summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<kNPC>().HasAilment[ELEMENT.COLD] = true;
            if (npc.velocity.Length() > 0.2f && !npc.boss)
            {
                npc.velocity.Normalize();
                npc.velocity *= 0.2f;
            }
            else if (npc.velocity.Length() > 6f)
            {
                npc.velocity.Normalize();
                npc.velocity *= 6f;
            }
        }

        /// <summary>
        ///     Allows you to make this buff give certain effects to the given player. If you remove the buff from the player, make
        ///     sure the decrement the buffIndex parameter by 1.
        /// </summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PlayerCharacter>().HasAilment[ELEMENT.COLD] = true;
            if (player.velocity.X > player.maxRunSpeed * 6 / 10)
                player.velocity.X = player.maxRunSpeed * 6 / 10;
        }
    }
}