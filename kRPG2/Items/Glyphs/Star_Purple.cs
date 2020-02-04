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
using System.Linq;
using kRPG2.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace kRPG2.Items.Glyphs
{
    public class Star_Purple : Star
    {
        public override Action<ProceduralSpell, Player, Vector2> GetUseAbility()
        {
            return delegate(ProceduralSpell spell, Player player, Vector2 target)
            {
                Main.PlaySound(0, player.position);
                spell.Remaining = spell.Cooldown;
                int placementHeight = 0;
                bool placeable = false;
                for (int y = (int) (Main.screenPosition.Y / 16); y < (int) ((Main.screenPosition.Y + Main.screenHeight) / 16); y += 1)
                {
                    int x = (int) (target.X / 16f);
                    var tile = Main.tile[x, y];
                    if ((!tile.active() || !Main.tileSolidTop[tile.type]) && (tile.collisionType != 1 || Main.tile[x, y - 1].collisionType == 1))
                        continue;
                    placeable = true;
                    placementHeight = y;
                    if (target.Y / 16 - 4 <= y)
                        break;
                }

                if (!placeable) return;
                var character = player.GetModPlayer<PlayerCharacter>();
                if (character.Minions.Exists(minion => minion is Obelisk))
                    foreach (var obelisk in character.Minions.Where(minions => minions.projectile.type == ModContent.ProjectileType<Obelisk>()))
                    {
                        foreach (var psp in obelisk.CirclingProtection)
                            psp.projectile.Kill();
                        obelisk.CirclingProtection.Clear();
                        obelisk.SmallProt?.projectile.Kill();
                        obelisk.projectile.Kill();
                    }

                var totem = Main.projectile[
                    Projectile.NewProjectile(new Vector2((int) (target.X / 16) * 16, placementHeight * 16) + new Vector2(8f, -32f), Vector2.Zero,
                        ModContent.GetInstance<Obelisk>().projectile.type, 0, 0f, player.whoAmI)];
                totem.position = new Vector2((int) (target.X / 16) * 16, placementHeight * 16) - new Vector2(8f, 62f);
                ((Obelisk) totem.modProjectile).Source = spell;
                character.Minions.Add((Obelisk) totem.modProjectile);
            };
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Star Glyph");
            Tooltip.SetDefault("Summons an obelisk sentry to cast the spell");
        }
    }
}