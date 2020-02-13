using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace kRPG.GameObjects.Items.Procedural
{
    public interface IProcedural
    {
        void ResetStats();
        void Initialize();
        void Load(TagCompound tag);

    }
}
