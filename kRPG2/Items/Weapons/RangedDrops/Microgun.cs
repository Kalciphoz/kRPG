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

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace kRPG2.Items.Weapons.RangedDrops
{
    public class Microgun : RangedWeapon
    {
        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() >= 0.5f;
        }

        public override float DpsModifier()
        {
            return 1.1f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4f, 10f);
        }

        public override string RandomName()
        {
            switch (Main.rand.Next(5))
            {
                default:
                    return "Minigun";
                case 1:
                    return "Gatling Gun";
                case 2:
                    return "Chaingun";
                case 3:
                    return "XM214";
                case 4:
                    return "XM556";
            }
        }

        public override void SetDefaults()
        {
            item.ranged = true;
            item.width = 32;
            item.height = 24;
            item.useStyle = 5;
            item.knockBack = 1f;
            item.scale = 1f;
            item.noMelee = true;
            item.useAmmo = AmmoID.Bullet;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 5.5f;
            item.damage = 1;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item11;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Microgun");
            Tooltip.SetDefault("50% chance to not consume ammo.");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            var perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(9));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }

        public override int UseTime()
        {
            return 8;
        }
    }
}