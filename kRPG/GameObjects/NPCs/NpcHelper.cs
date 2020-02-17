using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace kRPG.GameObjects.NPCs
{
 public static   class NpcHelper
    {
        public static void NinjaDodge(this NPC npc, Entity dustPos, int time, bool factorLongImmune = true)
        {
            npc.GetGlobalNPC<kNPC>().ImmuneTime = time;
            for (int j = 0; j < 100; j++)
            {
                int num = Dust.NewDust(new Vector2(dustPos.position.X, dustPos.position.Y), dustPos.width, dustPos.height, 31, 0f, 0f, 152, default, 2f);
                Dust dust = Main.dust[num];
                dust.position.X = dust.position.X + Main.rand.Next(-20, 21);
                Dust dust2 = Main.dust[num];
                dust2.position.Y = dust2.position.Y + Main.rand.Next(-20, 21);
                Main.dust[num].velocity *= 0.4f;
                Main.dust[num].scale *= 0.7f + Main.rand.Next(30) * 0.01f;
                if (Main.rand.Next(2) != 0)
                    continue;
                Main.dust[num].scale *= 1f + Main.rand.Next(40) * 0.01f;
                Main.dust[num].noGravity = true;
            }

            int num2 = Gore.NewGore(new Vector2(dustPos.position.X + dustPos.width / 2f - 24f, dustPos.position.Y + dustPos.height / 2f - 24f), default, Main.rand.Next(61, 64));
            Main.gore[num2].scale = 0.8f;
            Main.gore[num2].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity *= 0.4f;
            num2 = Gore.NewGore(new Vector2(dustPos.position.X + dustPos.width / 2f - 24f, dustPos.position.Y + dustPos.height / 2f - 24f), default, Main.rand.Next(61, 64));
            Main.gore[num2].scale = 0.8f;
            Main.gore[num2].velocity.X = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity *= 0.4f;
            num2 = Gore.NewGore(new Vector2(dustPos.position.X + dustPos.width / 2f - 24f, dustPos.position.Y + dustPos.height / 2f - 24f), default, Main.rand.Next(61, 64));
            Main.gore[num2].scale = 0.8f;
            Main.gore[num2].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity *= 0.4f;
            num2 = Gore.NewGore(new Vector2(dustPos.position.X + dustPos.width / 2f - 24f, dustPos.position.Y + dustPos.height / 2f - 24f), default, Main.rand.Next(61, 64));
            Main.gore[num2].scale = 0.8f;
            Main.gore[num2].velocity.X = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity.Y = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity *= 0.4f;
            num2 = Gore.NewGore(new Vector2(dustPos.position.X + dustPos.width / 2f - 24f, dustPos.position.Y + dustPos.height / 2f - 24f), default, Main.rand.Next(61, 64));
            Main.gore[num2].scale = 0.8f;
            Main.gore[num2].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity.Y = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num2].velocity *= 0.4f;
        }
    }
}
