using System;
using System.Collections.Generic;
using kRPG.Content.Items.Dusts;
using kRPG.Content.Items.Projectiles;
using kRPG.Content.NPCs;
using kRPG.Content.Players;
using kRPG.Content.SFX;
using kRPG.Enums;
using kRPG.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace kRPG.Content.Items.Weapons.Melee
{
    public class SwordAccent
    {
        public SwordAccent(string texture, string suffix, int originX, int originY, int mana = 0, Action<Player, NPC, ProceduralSword, int, bool> onHit = null,
            float dpsModifier = 1f, int critBonus = 0, Action<Rectangle, Player> effect = null)
        {
            Type = Accents.Count;
            if (Main.netMode != NetmodeID.Server)
                if (texture != null)
                    Texture = ModLoader.GetMod(Constants.ModName).GetTexture("Content/GFX/Items/Accents/" + texture);
            Suffix = suffix;
            Origin = new Vector2(originX, originY);
            OnHit = onHit;
            DpsModifier = dpsModifier;
            CritBonus = critBonus;
            Effect = effect;
            Mana = mana;

            if (!Accents.ContainsKey(Type))
                Accents.Add(Type, this);
        }

        public static Dictionary<int, SwordAccent> Accents { get; set; } = new Dictionary<int, SwordAccent>();
        public int CritBonus { get; set; }
        public float DpsModifier { get; set; }
        public Action<Rectangle, Player> Effect { get; set; }

        public Dictionary<Element, float> EleDamage { get; set; } = new Dictionary<Element, float>
        {
            {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}
        };

        public static SwordAccent Flame { get; set; }
        public static SwordAccent GemBlue { get; set; }
        public static SwordAccent GemGreen { get; set; }
        public static SwordAccent GemPurple { get; set; }
        public static SwordAccent GemRed { get; set; }

        public int Mana { get; set; }
        public static SwordAccent None { get; set; }

        public Action<Player, NPC, ProceduralSword, int, bool> OnHit { get; set; }
        public Vector2 Origin { get; set; }
        public string Suffix { get; set; }
        public Texture2D Texture { get; set; }

        public int Type { get; set; }

        public static void Initialize()
        {
            Accents = new Dictionary<int, SwordAccent>();

            None = new SwordAccent(null, "", 0, 0);
            GemRed = new SwordAccent("GemRed", " of Rejuvenation", 2, 2, 2, delegate(Player player, NPC npc, ProceduralSword sword, int damage, bool crit)
            {
                if (Main.rand.Next(4) != 0)
                    return;
                PlayerCharacter character = player.GetModPlayer<PlayerCharacter>();
                float distance = Vector2.Distance(npc.Center, character.player.Center);
                int count = (int) (distance / 20);
                Trail trail = new Trail(npc.Center, 60, delegate(SpriteBatch spriteBatch, Player p, Vector2 end, Vector2[] displacement, float scale)
                {
                    for (int i = 0; i < count; i += 1)
                    {
                        Vector2 position = (npc.position - p.Center) * i / count + p.Center;
                        spriteBatch.Draw(GFX.GFX.Heart, position - Main.screenPosition + displacement[i], null, Color.White, 0f, Vector2.Zero, scale,
                            SpriteEffects.None, 0f);
                    }
                }) {Displacement = new Vector2[count]};
                for (int i = 0; i < count; i += 1)
                    trail.Displacement[i] = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                character.Trails.Add(trail);
                int healAmount = Main.rand.Next(5) + 3;
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            });
            Flame = new SwordAccent("Flame", " of Ignition", 2, 2, 3, delegate(Player player, NPC npc, ProceduralSword sword, int damage, bool crit)
                {
                    //Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), player.Center);
                    SoundManager.PlaySound(Sounds.LegacySoundStyle_Item14, player.Center, .5f);
                    Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(16, 32), Vector2.Zero, ModContent.ProjectileType<Explosion>(), Math.Max(1, (int) Math.Round(sword.EleDamage[Element.Fire] * damage * 2)), 0f, player.whoAmI)];

                }, 1.05f).SetEleDamage(new Dictionary<Element, float> {{Element.Fire, 0.2f}, {Element.Cold, 0f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}})
                .SetEffect(delegate(Rectangle rect, Player player)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Fire, player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 63, new Color(), 1.5f);
                    Main.dust[dust].noGravity = true;
                });
            GemGreen = new SwordAccent("GemGreen", " of Thunder", 2, 2, 2, delegate(Player player, NPC npc, ProceduralSword sword, int damage, bool crit)
                {
                    //Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(0.5f), player.Center);
                    SoundManager.PlaySound(Sounds.LegacySoundStyle_Item14, player.Center, .5f);
                    Projectile proj = Main.projectile[Projectile.NewProjectile(npc.Center - new Vector2(24, 48), Vector2.Zero, ModContent.ProjectileType<SmokePellets>(), Math.Max(1, damage / 6), 0f, player.whoAmI)];
                }, 1.1f, 4).SetEleDamage(new Dictionary<Element, float>
                    {
                        {Element.Fire, 0f}, {Element.Cold, 0f}, {Element.Lightning, 0.2f}, {Element.Shadow, 0f}
                    })
                .SetEffect(delegate(Rectangle rect, Player player)
                {
                    if (Main.rand.Next(2) != 0)
                        return;
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Electric,
                        player.velocity.X * 0.2f + player.direction * 3f,
                        player.velocity.Y * 0.2f, 63, new Color(), 0.5f);
                    Main.dust[dust].noGravity = true;
                });
            GemBlue = new SwordAccent("GemBlue", " of Everest", 2, 2, 0, null, 1.15f, 0, delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(3) == 0)
                {
                    int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.GetInstance<Ice>().Type,
                        player.velocity.X * 0.2f + player.direction * 3f, player.velocity.Y * 0.2f, 100, new Color(), 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                Lighting.AddLight(rect.Center(), 0f, 0.4f, 1f);
            }).SetEleDamage(new Dictionary<Element, float> {{Element.Fire, 0f}, {Element.Cold, 0.3f}, {Element.Lightning, 0f}, {Element.Shadow, 0f}});
            GemPurple = new SwordAccent("GemPurple", " of Starlight", 2, 1, 3, null, 1.1f).SetEffect(delegate(Rectangle rect, Player player)
            {
                if (Main.rand.Next(2) != 0)
                    return;
                int dust = Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, 27, player.velocity.X * 0.2f + player.direction * 3f,
                    player.velocity.Y * 0.2f, 63, new Color(), 1.5f);
                Main.dust[dust].noGravity = true;
            });
        }

        public static SwordAccent RandomAccent()
        {
            return Accents.Random();
        }

        public SwordAccent SetEffect(Action<Rectangle, Player> effect)
        {
            Effect = effect;
            return this;
        }

        public SwordAccent SetEleDamage(Dictionary<Element, float> eleDamage)
        {
            EleDamage = eleDamage;
            return this;
        }

        public static void Unload()
        {
            foreach (SwordAccent accent in Accents.Values)
                accent.Texture = null;
        }
    }
}