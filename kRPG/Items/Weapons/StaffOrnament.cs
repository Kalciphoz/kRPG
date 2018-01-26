using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class StaffOrnament : StaffPart
    {
        public static StaffOrnament none;
        public static StaffOrnament bow;
        public static StaffOrnament twig;
        public static StaffOrnament loop;
        public static StaffOrnament arcane;
        public static StaffOrnament cage;
        public static StaffOrnament demonic;
        public static StaffOrnament explosive;

        public static Dictionary<int, StaffOrnament> ornament;
        public static Dictionary<STAFFTHEME, List<StaffOrnament>> ornamentByTheme;

        public int type = 0;
        public float dpsModifier = 1f;
        public float speedModifier = 1f;
        public float knockBack = 0f;
        public int critBonus = 0;
        public string suffix = "";
        public float mana = 1f;
        public bool front = true;
        public int repetitions = 0;

        public Action<Player, NPC, Item, int, bool> onHit;

        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>()
        {
            { ELEMENT.FIRE, 0f },
            { ELEMENT.COLD, 0f },
            { ELEMENT.LIGHTNING, 0f },
            { ELEMENT.SHADOW, 0f }
        };

        public StaffOrnament(string texture, int origin_x, int origin_y, string suffix, bool front = true, float mana = 1f, float dpsModifier = 1f, float speedModifier = 1f, float knockBack = 0f, int critBonus = 0, int repetitions = 0)
        {
            this.type = ornament.Count;
            if (Main.netMode != 2)
                if (texture != null) this.texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Ornaments/" + texture);
            this.origin = new Vector2(origin_x, origin_y);
            this.dpsModifier = dpsModifier;
            this.speedModifier = speedModifier;
            this.knockBack = knockBack;
            this.critBonus = critBonus;
            this.suffix = suffix;
            this.mana = mana;
            this.front = front;
            this.repetitions = repetitions;
            if (!ornament.ContainsKey(type))
                ornament.Add(type, this);
        }

        public StaffOrnament SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            this.eleDamage = eleDamage;
            return this;
        }

        public StaffOrnament SetEffect(Action<Player, NPC, Item, int, bool> onHit)
        {
            this.onHit = onHit;
            return this;
        }

        public static void Initialize()
        {
            ornament = new Dictionary<int, StaffOrnament>();

            none = new StaffOrnament(null, 0, 0, "");
            bow = new StaffOrnament("Bow", 4, 3, " of Wizardry", false, 1.1f, 1.1f, 1.1f, 1f, 5);
            twig = new StaffOrnament("Twig", 2, 7, " of Longevity", true, 1.3f, 1f, 0.9f, 1f).SetEffect(delegate (Player player, NPC npc,  Item item, int damage, bool crit)
            {
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                float distance = Vector2.Distance(npc.Center, character.player.Center);
                int count = (int)(distance / 32);
                Trail trail = new Trail(npc.Center, 60, delegate (SpriteBatch spriteBatch, Player p, Vector2 end, Vector2[] displacement, float scale)
                {
                    for (int i = 0; i < count; i += 1)
                    {
                        Vector2 position = (npc.position - p.Center) * i / count + p.Center;
                        spriteBatch.Draw(GFX.heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                });
                trail.displacement = new Vector2[count];
                for (int i = 0; i < count; i += 1)
                    trail.displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                character.trails.Add(trail);
                int healAmount = Main.rand.Next(4) + 2;
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            });
            loop = new StaffOrnament("Loop", 0, 6, " of Reverberation", true, 1.5f, 1.2f, 1.5f, 0f, 0, 1);
            arcane = new StaffOrnament("ArcaneSpider", 7, 8, " of Articulation", true, 1.1f, 1.2f);
            cage = new StaffOrnament("ArcaneCage", 3, 10, " of Resonance", true, 2f, 1f, 3, 0f, 0, 2);
            demonic = new StaffOrnament("Demonic", 8, 6, " of Demons", true, 1.5f, 1.2f, 1.5f, 0, 2, 1);
            explosive = new StaffOrnament("Explosive", 6, 4, " of Blasting", false, 1.2f, 0.9f, 0.9f).SetEffect(delegate (Player player, NPC npc, Item item, int damage, bool crit)
            {
                Main.PlaySound(new LegacySoundStyle(2, 14, Terraria.Audio.SoundType.Sound).WithVolume(0.5f), player.Center);
                Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModLoader.GetMod("kRPG").ProjectileType<Explosion>(), damage / 2, 0f, player.whoAmI)];
            });

            ornamentByTheme = new Dictionary<STAFFTHEME, List<StaffOrnament>>()
            {
                { STAFFTHEME.WOODEN, new List<StaffOrnament>() { bow, twig, loop } },
                { STAFFTHEME.DUNGEON, new List<StaffOrnament>() { arcane, cage } },
                { STAFFTHEME.UNDERWORLD, new List<StaffOrnament>() { demonic, explosive } }
            };
        }

        public static StaffOrnament RandomOrnament(STAFFTHEME theme)
        {
            return ornamentByTheme[theme].Random();
        }

        public static void Unload()
        {
            foreach (StaffOrnament o in ornament.Values)
                o.texture = null;
        }
    }
}
