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
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Projectiles
{
    public class ChainHook : ModProjectile
    {
        public override void AI()
        {
            projectile.rotation = (float) Math.Atan2(projectile.position.Y - Main.player[projectile.owner].position.Y,
                                      projectile.position.X - Main.player[projectile.owner].position.X) + (float) Math.PI / 2f;
            projectile.spriteDirection = Main.player[projectile.owner].direction;
            base.AI();
        }

        public override void SetDefaults()
        {
            projectile.Name = "Chain Hook";
            projectile.width = 22;
            projectile.height = 22;
            projectile.aiStyle = 15;
            projectile.timeLeft = 1800;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.hide = false;
            projectile.ownerHitCheck = true;
        }
    }
}