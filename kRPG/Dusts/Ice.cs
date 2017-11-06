using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace kRPG.Dusts
{
    public class Ice : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0f;
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.015f;
            float light = 0.4f * dust.scale;
            Lighting.AddLight(dust.position, light*0.6f, light*0.9f, light);
            if (dust.scale < 0.6f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}
