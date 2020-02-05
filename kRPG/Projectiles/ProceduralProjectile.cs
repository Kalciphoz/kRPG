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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Projectiles
{
    public class ProceduralProjectile : ModProjectile
    {
        public Texture2D LocalTexture { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
        }

        public virtual void Initialize()
        {
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.owner != Main.myPlayer && !(this is ProceduralSpellProj) && !(this is WingedEyeball)) return true;
            Draw(spriteBatch, projectile.position - Main.screenPosition, lightColor, projectile.rotation, projectile.scale);
            return false;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.scale = 1f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procedurally Generated Projectile; Please Ignore");
        }
    }

    //public class ProceduralSwordThrow : ProceduralProjectile
    //{
    //    public SwordHilt hilt;
    //    public SwordBlade blade;
    //    public SwordAccent accent;
    //    public Item sword;

    //    public override void Initialize()
    //    {
    //        this.LocalTexture = GFX.CombineTextures(new List<Texture2D>(){
    //            { blade.LocalTexture },
    //            { hilt.LocalTexture },
    //            { accent.LocalTexture }
    //        }, new List<Point>(){
    //            { new Point(CombinedTextureSize().X - blade.LocalTexture.Width, 0) },
    //            { new Point(0, CombinedTextureSize().Y - hilt.LocalTexture.Height) },
    //            { new Point((int)hilt.origin.X - (int)accent.origin.X, CombinedTextureSize().Y - hilt.LocalTexture.Height + (int)hilt.origin.Y - (int)accent.origin.Y) }
    //        }, CombinedTextureSize());
    //        projectile.width = LocalTexture.Width;
    //        projectile.height = LocalTexture.Height;
    //    }

    //    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    //    {
    //        hitbox = new Rectangle((int)projectile.playerPosition.X - LocalTexture.Width / 2, (int)projectile.playerPosition.Y - LocalTexture.Height / 2, LocalTexture.Width, LocalTexture.Height);
    //    }

    //    public override bool? CanHitNPC(NPC target)
    //    {
    //        Player owner = Main.player[projectile.owner];
    //        if ((target.playerPosition.X - owner.playerPosition.X) * owner.direction > -1f)
    //            return base.CanHitNPC(target);
    //        else return false;
    //    }

    //    public Point CombinedTextureSize()
    //    {
    //        return new Point(blade.LocalTexture.Width - (int)blade.origin.X + (int)hilt.origin.X, (int)blade.origin.Y + hilt.LocalTexture.Height - (int)hilt.origin.Y);
    //    }

    //    public override void Draw(SpriteBatch spriteBatch, Vector2 playerPosition, Color color, float rotation, float scale)
    //    {
    //        if (LocalTexture == null)
    //        {
    //            Initialize();
    //            return;
    //        }
    //        spriteBatch.Draw(LocalTexture, playerPosition + LocalTexture.Size() / 2f, null, blade.lighted ? Color.White : color, rotation, LocalTexture.Bounds.Center(), scale, projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
    //    }

    //    public override void SetDefaults()
    //    {
    //        projectile.width = 40;
    //        projectile.height = 40;
    //        projectile.scale = 1f;
    //        projectile.friendly = true;
    //        projectile.hostile = false;
    //        projectile.penetrate = -1;
    //        projectile.timeLeft = 3600;
    //        projectile.tileCollide = false;
    //        projectile.aiStyle = 3;
    //    }

    //    public override void SetStaticDefaults()
    //    {
    //        DisplayName.SetDefault("Procedurally Generated Sword Projectile; Please Ignore");
    //    }

    //    public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
    //    {
    //        Player owner = Main.player[projectile.owner];
    //        //if (accent.onHit != null) accent.onHit(owner, target, (ProceduralSword)owner.inventory[owner.selectedItem].modItem, damage, crit);
    //    }
    //}
}