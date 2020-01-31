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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Items
{
    public class ProceduralItem : ModItem
    {
        public float Dps { get; set; }
        public int EnemyDef { get; set; }
        public Texture2D texture { get; set; }

        public override bool CanPickup(Player player)
        {
            if (Main.netMode == 0) return true;
            return item.value > 100;
        }

        public override ModItem Clone(Item tItem)
        {
            var copy = (ProceduralItem) base.Clone(tItem);
            copy.texture = texture;
            return copy;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
        }

        public virtual void Initialize()
        {
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Insert(1, new TooltipLine(mod, "power", "Power level: " + (int) Math.Round(Dps / 2)));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin,
            float scale)
        {
            if (Main.netMode == 2 || texture == null) return false;
            if (Main.itemTexture[item.type] == null) Main.itemTexture[item.type] = texture;
            float s = scale * Main.itemTexture[item.type].Height / texture.Height;
            Draw(spriteBatch, position, drawColor, 0f, s);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (Main.netMode == 2 || texture == null) return false;
            Draw(spriteBatch, item.position - Main.screenPosition, lightColor, rotation, scale);
            return false;
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 48;
            item.scale = 1f;
            item.noMelee = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Item; Please Ignore");
        }
    }
}