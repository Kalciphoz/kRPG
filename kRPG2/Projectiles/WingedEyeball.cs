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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Projectiles
{
    public class WingedEyeball : ProceduralMinion
    {
        public override void AI()
        {
            base.AI();
            var player = Main.player[projectile.owner];

            float acceleration = 0.4f;
            projectile.tileCollide = false;
            var v = player.Center - projectile.Center;
            v.X += Main.rand.Next(-10, 21);
            v.X += Main.rand.Next(-10, 21);
            v.X += 60f * -player.direction;
            v.Y -= 60f;
            float someDist = (float) Math.Sqrt(v.X * v.X + v.Y * v.Y);
            float num22 = 14f;

            if (someDist < 100 && Math.Abs(player.velocity.Y) < .01 && projectile.Bottom.Y <= player.Bottom.Y &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                    projectile.velocity.Y = -6f;
            }

            if (someDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                    projectile.velocity *= 0.99f;
                acceleration = 0.01f;
            }
            else
            {
                if (someDist < 100f)
                    acceleration = 0.1f;
                if (someDist > 300f)
                    acceleration = 0.6f;
                someDist = num22 / someDist;
                v.X *= someDist;
                v.Y *= someDist;
            }

            if (projectile.velocity.X < v.X)
            {
                projectile.velocity.X = projectile.velocity.X + acceleration;
                if (acceleration > 0.05f && projectile.velocity.X < 0f)
                    projectile.velocity.X = projectile.velocity.X + acceleration;
            }

            if (projectile.velocity.X > v.X)
            {
                projectile.velocity.X = projectile.velocity.X - acceleration;
                if (acceleration > 0.05f && projectile.velocity.X > 0f)
                    projectile.velocity.X = projectile.velocity.X - acceleration;
            }

            if (projectile.velocity.Y < v.Y)
            {
                projectile.velocity.Y = projectile.velocity.Y + acceleration;
                if (acceleration > 0.05f && projectile.velocity.Y < 0f)
                    projectile.velocity.Y = projectile.velocity.Y + acceleration * 2f;
            }

            if (projectile.velocity.Y > v.Y)
            {
                projectile.velocity.Y = projectile.velocity.Y - acceleration;
                if (acceleration > 0.05f && projectile.velocity.Y > 0f)
                    projectile.velocity.Y = projectile.velocity.Y - acceleration * 2f;
            }

            if (projectile.velocity.X > 0.25)
                projectile.direction = -1;
            else if (projectile.velocity.X < -0.25)
                projectile.direction = 1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.05f;
            int num9 = projectile.frameCounter;
            projectile.frameCounter = num9 + 1;
            if (projectile.frameCounter > 2)
            {
                num9 = projectile.frame;
                projectile.frame = num9 + 1;
                projectile.frameCounter = 0;
            }

            if (projectile.frame <= 3)
                return;
            projectile.frame = 0;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            var t = Main.projectileTexture[ModContent.ProjectileType<WingedEyeball>()];
            spriteBatch.Draw(t, position + t.Bounds.Center(), new Rectangle(0, projectile.frame * 40, 90, 40), color, rotation, t.Bounds.Center(), scale,
                projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
        //public Vector2 acceleration = new Vector2(1f, 0.5f);
        //public Vector2 maxSpeed = new Vector2(6f, 2f);

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 7200;
            projectile.tileCollide = false;
            projectile.knockBack = 0f;
        }
    }
}