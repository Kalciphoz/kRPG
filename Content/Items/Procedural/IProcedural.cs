using Terraria.ModLoader.IO;

namespace kRPG.Content.Items.Procedural
{
    public interface IProcedural
    {
        void ResetStats();
        void Initialize();
        void Load(TagCompound tag);

    }
}
