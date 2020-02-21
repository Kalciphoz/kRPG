using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.GameObjects.Items.Projectiles;
using kRPG.GameObjects.NPCs;
using kRPG.GameObjects.Players;
using kRPG.GameObjects.SFX;
using kRPG.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.GameObjects.Items.Weapons.Melee
{
    public class StaffOrnament : StaffPart
    {
        public StaffOrnament(string texture, int originX, int originY, string suffix, bool front = true, float mana = 1f, float dpsModifier = 1f,
            float speedModifier = 1f, float knockBack = 0f, int critBonus = 0, int repetitions = 0)
        {
            Type = Ornament.Count;
            if (Main.netMode != NetmodeID.Server)
                if (texture != null)
                    Texture = ModLoader.GetMod(Constants.ModName).GetTexture("GameObjects/GFX/Items/Ornaments/" + texture);
            Origin = new Vector2(originX, originY);
            DpsModifier = dpsModifier;
            SpeedModifier = speedModifier;
            KnockBack = knockBack;
            CritBonus = critBonus;
            Suffix = suffix;
            Mana = mana;
            Front = front;
            Repetitions = repetitions;
            if (!Ornament.ContainsKey(Type))
                Ornament.Add(Type, this);
        }

        public static StaffOrnament Arcane { get; set; }
        public static StaffOrnament Bow { get; set; }
        public static StaffOrnament Cage { get; set; }
        public int CritBonus { get; set; }
        public static StaffOrnament Demonic { get; set; }
        public float DpsModifier { get; set; }

        public Dictionary<Element, float> EleDamage { get; set; } = new Dictionary<Element, float>
        {
            {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}
        };

        public static StaffOrnament Explosive { get; set; }

        public bool Front { get; set; }
        public float KnockBack { get; set; }
        public static StaffOrnament Loop { get; set; }
        public float Mana { get; set; }
        public static StaffOrnament None { get; set; }

        public Action<Player, NPC, Item, int, bool> OnHit { get; set; }

        public static Dictionary<int, StaffOrnament> Ornament { get; set; }
        public static Dictionary<StaffTheme, List<StaffOrnament>> OrnamentByTheme { get; set; }
        public int Repetitions { get; set; }
        public float SpeedModifier { get; set; }
        public string Suffix { get; set; }
        public static StaffOrnament Twig { get; set; }

        public int Type { get; set; }

        public static void Initialize()
        {
            Ornament = new Dictionary<int, StaffOrnament>();

            None = new StaffOrnament(null, 0, 0, "");
            Bow = new StaffOrnament("Bow", 4, 3, " of Wizardry", false, 1.1f, 1.1f, 1.1f, 1f, 5);
            Twig = new StaffOrnament("Twig", 2, 7, " of Longevity", true, 1.3f, 1f, 0.9f, 1f).SetEffect(
                delegate(Player player, NPC npc, Item item, int damage, bool crit)
                {
                    PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                    float distance = Vector2.Distance(npc.Center, character.player.Center);
                    int count = (int) (distance / 32);
                    Trail trail = new Trail(npc.Center, 60, delegate(SpriteBatch spriteBatch, Player p, Vector2 end, Vector2[] displacement, float scale)
                    {
                        for (int i = 0; i < count; i += 1)
                        {
                            Vector2 position = (npc.position - p.Center) * i / count + p.Center;
                            spriteBatch.Draw(GFX.GFX.Heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale,
                                SpriteEffects.None, 0f);
                        }
                    });
                    trail.Displacement = new Vector2[count];
                    for (int i = 0; i < count; i += 1)
                        trail.Displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    character.Trails.Add(trail);
                    int healAmount = Main.rand.Next(4) + 2;
                    player.statLife += healAmount;
                    player.HealEffect(healAmount);
                });
            Loop = new StaffOrnament("Loop", 0, 6, " of Reverberation", true, 1.5f, 1.2f, 1.5f, 0f, 0, 1);
            Arcane = new StaffOrnament("ArcaneSpider", 7, 8, " of Articulation", true, 1.1f, 1.2f);
            Cage = new StaffOrnament("ArcaneCage", 3, 10, " of Resonance", true, 2f, 1f, 3, 0f, 0, 2);
            Demonic = new StaffOrnament("Demonic", 8, 6, " of Demons", true, 1.5f, 1.2f, 1.5f, 0, 2, 1);
            Explosive = new StaffOrnament("Explosive", 6, 4, " of Blasting", false, 1.2f, 0.9f, 0.9f).SetEffect(
                delegate(Player player, NPC npc, Item item, int damage, bool crit)
                {
                    SoundManager.PlaySound(Sounds.LegacySoundStyle_Item14, player.Center, .5f);
                    //Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), player.Center);
                    Projectile proj = Main.projectile[
                        Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<Explosion>(), damage / 2, 0f,
                            player.whoAmI)];
                });

            OrnamentByTheme = new Dictionary<StaffTheme, List<StaffOrnament>>
            {
                {StaffTheme.Wooden, new List<StaffOrnament> {Bow, Twig, Loop}},
                {StaffTheme.Dungeon, new List<StaffOrnament> {Arcane, Cage}},
                {StaffTheme.Underworld, new List<StaffOrnament> {Demonic, Explosive}}
            };
        }

        public static StaffOrnament RandomOrnament(StaffTheme theme)
        {
            return OrnamentByTheme[theme].Random();
        }

        public StaffOrnament SetEffect(Action<Player, NPC, Item, int, bool> onHit)
        {
            OnHit = onHit;
            return this;
        }

        public StaffOrnament SetEleDamage(Dictionary<Element, float> eleDamage)
        {
            EleDamage = eleDamage;
            return this;
        }

        public static void Unload()
        {
            foreach (StaffOrnament o in Ornament.Values)
                o.Texture = null;
        }
    }
}