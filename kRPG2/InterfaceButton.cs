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

namespace kRPG2
{
    public class InterfaceButton
    {
        private readonly Action<Player, SpriteBatch> hoverAction;

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction)
        {
            Position = position;
            PressAction = pressAction;
        }

        public InterfaceButton(Func<Rectangle> position, Action<Player> pressAction, Action<Player, SpriteBatch> hoverAction)
        {
            Position = position;
            PressAction = pressAction;
            this.hoverAction = hoverAction;
            HoverActionSet = true;
        }

        private bool HoverActionSet { get; }
        private Func<Rectangle> Position { get; }
        private Action<Player> PressAction { get; }

        public virtual void Update(SpriteBatch spriteBatch, Player player)
        {
            if (!Position().Contains(Main.mouseX, Main.mouseY))
                return;
            Main.LocalPlayer.mouseInterface = true;

            if (HoverActionSet)
                hoverAction(player, spriteBatch);

            if (Main.mouseLeft && Main.mouseLeftRelease)
                PressAction(player);
        }
    }
}