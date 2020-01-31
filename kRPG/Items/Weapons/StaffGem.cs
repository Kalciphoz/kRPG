using System;
using System.Collections.Generic;
using kRPG.Enums;
using kRPG.Items.Glyphs;
using kRPG.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Items.Weapons
{
    public class StaffGem : StaffPart
    {
        public Dictionary<ELEMENT, float> eleDamage = new Dictionary<ELEMENT, float>
        {
            {ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
        };

        public StaffGem(string texture, int originX, int originY, string name, int shoot, bool back = false, float mana = 1f, float dpsModifier = 1f,
            float speedModifier = 1f, float knockBack = 0f, int critBonus = 0)
        {
            Type = Gems.Count + 1;
            if (Main.netMode != 2)
                this.Texture = ModLoader.GetMod("kRPG").GetTexture("GFX/Items/Gemstones/" + texture);
            Origin = new Vector2(originX, originY);
            DpsModifier = dpsModifier;
            SpeedModifier = speedModifier;
            KnockBack = knockBack;
            CritBonus = critBonus;
            Name = name;
            Mana = mana;
            Shoot = shoot;
            Back = back;
            if (!Gems.ContainsKey(Type))
                Gems.Add(Type, this);
        }

        public static StaffGem Amber { get; set; }
        public static StaffGem AmberLarge { get; set; }
        public static StaffGem Amethyst { get; set; }
        public static StaffGem AmethystDark { get; set; }

        public bool Back { get; set; }
        public int CritBonus { get; set; }
        public static StaffGem CrystalDark { get; set; }
        public static StaffGem CrystalGreen { get; set; }
        public static StaffGem Diamond { get; set; }
        public float DpsModifier { get; set; }
        public static StaffGem Emerald { get; set; }
        public static StaffGem Fire { get; set; }
        public static StaffGem FireOrb { get; set; }

        public static Dictionary<int, StaffGem> Gems { get; set; }
        public static Dictionary<STAFFTHEME, List<StaffGem>> GemsByTheme { get; set; }

        public float KnockBack { get; set; }
        public float Mana { get; set; }
        public string Name { get; set; }

        public Action<Player, Item> Projectile { get; set; }
        public static StaffGem Ruby { get; set; }
        public static StaffGem Sapphire { get; set; }
        public static StaffGem Shattered { get; set; }
        public int Shoot { get; set; }
        public float SpeedModifier { get; set; }

        private static Action<Player, Item, int, Cross, float> SpellEffect { get; } =
            delegate(Player player, Item item, int projCount, Cross cross, float scale)
            {
                
                float spread = 0.030f - projCount * 0.002f;
                var unitVelocity = Main.MouseWorld - player.Center;
                unitVelocity.Normalize();
                var velocity = unitVelocity * item.shootSpeed;
                for (int i = 0; i < projCount; i += 1)
                {
                    var projectile =
                        Main.projectile[
                            Terraria.Projectile.NewProjectile(player.Center, velocity.RotatedBy(API.Tau * (-spread / 2f + i * spread + spread / 2f)),
                                ModContent.ProjectileType<ProceduralSpellProj>(), item.damage, item.knockBack, player.whoAmI)];
                    var ps = (ProceduralSpellProj) projectile.modProjectile;
                    ps.Origin = projectile.position;
                    if (cross.GetAiAction() != null)
                        ps.ai.Add(cross.GetAiAction());
                    if (cross.GetInitAction() != null)
                        ps.Inits.Add(cross.GetInitAction());
                    if (cross.GetImpactAction() != null)
                        ps.Impacts.Add(cross.GetImpactAction());
                    if (cross.GetKillAction() != null)
                        ps.Kills.Add(cross.GetKillAction());
                    ps.Caster = player;
                    ps.projectile.minion = false;
                    ps.Initialize();
                    if (projCount == 1) projectile.tileCollide = true;
                    projectile.scale *= scale;
                }
            };

        public int Type { get; set; }

        public static void Initialize()
        {
            Gems = new Dictionary<int, StaffGem>();

            Ruby = new StaffGem("Ruby", 0, 6, "Ruby Staff", ProjectileID.RubyBolt).SetEleDamage(new Dictionary<ELEMENT, float>
            {
                {ELEMENT.FIRE, 0.3f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}
            });
            Diamond = new StaffGem("Diamond", 1, 5, "Diamond Staff", ProjectileID.DiamondBolt, false, 0.9f);
            Emerald = new StaffGem("Emerald", 1, 4, "Emerald Staff", ProjectileID.EmeraldBolt, true);
            Amber = new StaffGem("Amber", 1, 5, "Amber Staff", ProjectileID.AmberBolt);
            Sapphire = new StaffGem("Sapphire", 2, 5, "Sapphire Staff", ProjectileID.SapphireBolt, false, 0.9f, 1.1f, 1.1f, 1f);
            Amethyst = new StaffGem("BrightAmethyst", 2, 7, "Mana Scepter", ProjectileID.DiamondBolt);
            AmethystDark = new StaffGem("DarkAmethyst", 2, 7, "Dungeon Scepter", ProjectileID.AmethystBolt, false, 1.1f, 1.2f, 1.1f).SetEleDamage(
                new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0.2f}});
            AmberLarge = new StaffGem("LargeAmber", 2, 7, "Runic Scepter", ProjectileID.AmberBolt);
            CrystalDark = new StaffGem("DarkCrystal", 2, 7, "Shadow Scepter", 0, false, 1.1f, 1.1f, 1.2f)
                .SetEleDamage(new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0.2f}})
                .SetProjectile(delegate(Player player, Item item) { SpellEffect(player, item, 1, new Cross_Violet(), 1f); });
            CrystalGreen = new StaffGem("GreenCrystal", 2, 7, "Crystal Scepter", ProjectileID.EmeraldBolt, false, 1f, 1f, 1.4f, 2f, 5);
            Fire = new StaffGem("FireCrystal", 2, 7, "Flame Scepter", ProjectileID.BallofFire, false, 1f, 1.2f, 0.9f, 1f, 5).SetEleDamage(
                new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.5f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}});
            FireOrb = new StaffGem("FireOrb", 2, 7, "Immolation Scepter", ProjectileID.InfernoFriendlyBolt, false, 1.2f, 0.6f, 1.2f, 2f).SetEleDamage(
                new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.5f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}});
            Shattered = new StaffGem("Shatter", 1, 8, "Lava Staff", 0, false, 1f, 0.65f)
                .SetEleDamage(new Dictionary<ELEMENT, float> {{ELEMENT.FIRE, 0.4f}, {ELEMENT.COLD, 0f}, {ELEMENT.LIGHTNING, 0f}, {ELEMENT.SHADOW, 0f}})
                .SetProjectile(delegate(Player player, Item item) { SpellEffect(player, item, 3, new Cross_Red(), 0.8f); });

            GemsByTheme = new Dictionary<STAFFTHEME, List<StaffGem>>
            {
                {
                    STAFFTHEME.WOODEN, new List<StaffGem>
                    {
                        Ruby,
                        Diamond,
                        Emerald,
                        Amber,
                        Sapphire
                    }
                },
                {
                    STAFFTHEME.DUNGEON, new List<StaffGem>
                    {
                        Amethyst,
                        AmethystDark,
                        AmberLarge,
                        CrystalDark,
                        CrystalGreen
                    }
                },
                {STAFFTHEME.UNDERWORLD, new List<StaffGem> {Fire, FireOrb, Shattered, CrystalDark}}
            };
        }

        public static StaffGem RandomGem(STAFFTHEME theme)
        {
            return GemsByTheme[theme].Random();
        }

        // ReSharper disable once ParameterHidesMember
        public StaffGem SetEleDamage(Dictionary<ELEMENT, float> eleDamage)
        {
            this.eleDamage = eleDamage;
            return this;
        }

        public StaffGem SetProjectile(Action<Player, Item> projectile)
        {
            Projectile = projectile;
            return this;
        }

        public static void Unload()
        {
            foreach (var gem in Gems.Values)
                gem.Texture = null;
        }
    }
}