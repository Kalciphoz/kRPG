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
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items
{
    public class ScintillatingBloodLacrima : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "Description", "'A large tear shed by a gigantic blood-drinking unicorn'"));
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.value = 100000;
            item.rare = 10;
            item.consumable = true;
            item.useStyle = 2;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scintillating Blood Lacrima");
            Tooltip.SetDefault("Consume to reset stat points");
        }

        public override bool UseItem(Player player)
        {
            PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
            foreach (STAT stat in Enum.GetValues(typeof(STAT)))
                character.BaseStats[stat] = 0;
            return true;
        }
    }
}