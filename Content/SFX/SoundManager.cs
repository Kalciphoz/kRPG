using System;
using kRPG.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace kRPG.Content.SFX
{
    public class SoundManager
    {
        public static void PlaySound(Sounds soundId, Vector2 position, float volume = 1f, float pitchVariance = 0.0f)
        {
            PlaySound(soundId, (int)position.X, (int)position.Y, volume, pitchVariance);
        }


        public static void PlaySound(
            Sounds soundId,
            int x = -1,
            int y = -1,
            float volume = 1f,
            float pitchVariance = 0.0f)
        {
            switch (soundId)
            {
                case Sounds.Dig:
                case Sounds.PlayerHit:
                case Sounds.Item:
                case Sounds.NPCHit:
                case Sounds.NPCKilled:
                case Sounds.PlayerKilled:
                case Sounds.Grass:
                case Sounds.Grab:
                case Sounds.DoorOpen:
                case Sounds.DoorClosed:
                case Sounds.MenuOpen:
                case Sounds.MenuClose:
                case Sounds.MenuTick:
                case Sounds.Shatter:
                case Sounds.ZombieMoan:
                case Sounds.Roar:
                case Sounds.DoubleJump:
                case Sounds.Run:
                case Sounds.Coins:
                case Sounds.Splash:
                case Sounds.FemaleHit:
                case Sounds.Tink:
                case Sounds.Unlock:
                case Sounds.Drown:
                case Sounds.Chat:
                case Sounds.MaxMana:
                case Sounds.Mummy:
                case Sounds.Pixie:
                case Sounds.Mech:
                case Sounds.Zombie:
                case Sounds.Duck:
                case Sounds.Frog:
                case Sounds.Bird:
                case Sounds.Critter:
                case Sounds.Waterfall:
                case Sounds.Lavafall:
                case Sounds.ForceRoar:
                case Sounds.Meowmere:
                case Sounds.CoinPickup:
                case Sounds.Drip:
                case Sounds.Camera:
                case Sounds.MoonLord:
                case Sounds.Trackable:
                    Main.PlaySound((int)soundId, x, y, 1, volume, pitchVariance);
                    break;
                case Sounds.LegacySoundStyle_NPCHit1:
                    Main.PlaySound(new LegacySoundStyle(3, 1).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit2:
                    Main.PlaySound(new LegacySoundStyle(3, 2).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit3:
                    Main.PlaySound(new LegacySoundStyle(3, 3).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit4:
                    Main.PlaySound(new LegacySoundStyle(3, 4).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit5:
                    Main.PlaySound(new LegacySoundStyle(3, 5).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit6:
                    Main.PlaySound(new LegacySoundStyle(3, 6).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit7:
                    Main.PlaySound(new LegacySoundStyle(3, 7).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit8:
                    Main.PlaySound(new LegacySoundStyle(3, 8).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit9:
                    Main.PlaySound(new LegacySoundStyle(3, 9).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit10:
                    Main.PlaySound(new LegacySoundStyle(3, 10).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit11:
                    Main.PlaySound(new LegacySoundStyle(3, 11).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit12:
                    Main.PlaySound(new LegacySoundStyle(3, 12).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit13:
                    Main.PlaySound(new LegacySoundStyle(3, 13).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit14:
                    Main.PlaySound(new LegacySoundStyle(3, 14).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit15:
                    Main.PlaySound(new LegacySoundStyle(3, 15).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit16:
                    Main.PlaySound(new LegacySoundStyle(3, 16).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit17:
                    Main.PlaySound(new LegacySoundStyle(3, 17).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit18:
                    Main.PlaySound(new LegacySoundStyle(3, 18).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit19:
                    Main.PlaySound(new LegacySoundStyle(3, 19).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit20:
                    Main.PlaySound(new LegacySoundStyle(3, 20).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit21:
                    Main.PlaySound(new LegacySoundStyle(3, 21).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit22:
                    Main.PlaySound(new LegacySoundStyle(3, 22).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit23:
                    Main.PlaySound(new LegacySoundStyle(3, 23).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit24:
                    Main.PlaySound(new LegacySoundStyle(3, 24).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit25:
                    Main.PlaySound(new LegacySoundStyle(3, 25).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit26:
                    Main.PlaySound(new LegacySoundStyle(3, 26).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit27:
                    Main.PlaySound(new LegacySoundStyle(3, 27).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit28:
                    Main.PlaySound(new LegacySoundStyle(3, 28).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit29:
                    Main.PlaySound(new LegacySoundStyle(3, 29).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit30:
                    Main.PlaySound(new LegacySoundStyle(3, 30).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit31:
                    Main.PlaySound(new LegacySoundStyle(3, 31).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit32:
                    Main.PlaySound(new LegacySoundStyle(3, 32).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit33:
                    Main.PlaySound(new LegacySoundStyle(3, 33).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit34:
                    Main.PlaySound(new LegacySoundStyle(3, 34).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit35:
                    Main.PlaySound(new LegacySoundStyle(3, 35).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit36:
                    Main.PlaySound(new LegacySoundStyle(3, 36).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit37:
                    Main.PlaySound(new LegacySoundStyle(3, 37).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit38:
                    Main.PlaySound(new LegacySoundStyle(3, 38).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit39:
                    Main.PlaySound(new LegacySoundStyle(3, 39).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit40:
                    Main.PlaySound(new LegacySoundStyle(3, 40).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit41:
                    Main.PlaySound(new LegacySoundStyle(3, 41).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit42:
                    Main.PlaySound(new LegacySoundStyle(3, 42).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit43:
                    Main.PlaySound(new LegacySoundStyle(3, 43).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit44:
                    Main.PlaySound(new LegacySoundStyle(3, 44).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit45:
                    Main.PlaySound(new LegacySoundStyle(3, 45).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit46:
                    Main.PlaySound(new LegacySoundStyle(3, 46).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit47:
                    Main.PlaySound(new LegacySoundStyle(3, 47).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit48:
                    Main.PlaySound(new LegacySoundStyle(3, 48).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit49:
                    Main.PlaySound(new LegacySoundStyle(3, 49).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit50:
                    Main.PlaySound(new LegacySoundStyle(3, 50).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit51:
                    Main.PlaySound(new LegacySoundStyle(3, 51).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit52:
                    Main.PlaySound(new LegacySoundStyle(3, 52).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit53:
                    Main.PlaySound(new LegacySoundStyle(3, 53).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit54:
                    Main.PlaySound(new LegacySoundStyle(3, 54).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit55:
                    Main.PlaySound(new LegacySoundStyle(3, 55).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit56:
                    Main.PlaySound(new LegacySoundStyle(3, 56).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCHit57:
                    Main.PlaySound(new LegacySoundStyle(3, 57).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath1:
                    Main.PlaySound(new LegacySoundStyle(4, 1).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath2:
                    Main.PlaySound(new LegacySoundStyle(4, 2).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath3:
                    Main.PlaySound(new LegacySoundStyle(4, 3).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath4:
                    Main.PlaySound(new LegacySoundStyle(4, 4).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath5:
                    Main.PlaySound(new LegacySoundStyle(4, 5).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath6:
                    Main.PlaySound(new LegacySoundStyle(4, 6).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath7:
                    Main.PlaySound(new LegacySoundStyle(4, 7).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath8:
                    Main.PlaySound(new LegacySoundStyle(4, 8).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath9:
                    Main.PlaySound(new LegacySoundStyle(4, 9).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath10:
                    Main.PlaySound(new LegacySoundStyle(4, 10).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath11:
                    Main.PlaySound(new LegacySoundStyle(4, 11).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath12:
                    Main.PlaySound(new LegacySoundStyle(4, 12).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath13:
                    Main.PlaySound(new LegacySoundStyle(4, 13).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath14:
                    Main.PlaySound(new LegacySoundStyle(4, 14).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath15:
                    Main.PlaySound(new LegacySoundStyle(4, 15).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath16:
                    Main.PlaySound(new LegacySoundStyle(4, 16).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath17:
                    Main.PlaySound(new LegacySoundStyle(4, 17).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath18:
                    Main.PlaySound(new LegacySoundStyle(4, 18).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath19:
                    Main.PlaySound(new LegacySoundStyle(4, 19).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath20:
                    Main.PlaySound(new LegacySoundStyle(4, 20).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath21:
                    Main.PlaySound(new LegacySoundStyle(4, 21).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath22:
                    Main.PlaySound(new LegacySoundStyle(4, 22).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath23:
                    Main.PlaySound(new LegacySoundStyle(4, 23).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath24:
                    Main.PlaySound(new LegacySoundStyle(4, 24).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath25:
                    Main.PlaySound(new LegacySoundStyle(4, 25).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath26:
                    Main.PlaySound(new LegacySoundStyle(4, 26).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath27:
                    Main.PlaySound(new LegacySoundStyle(4, 27).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath28:
                    Main.PlaySound(new LegacySoundStyle(4, 28).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath29:
                    Main.PlaySound(new LegacySoundStyle(4, 29).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath30:
                    Main.PlaySound(new LegacySoundStyle(4, 30).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath31:
                    Main.PlaySound(new LegacySoundStyle(4, 31).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath32:
                    Main.PlaySound(new LegacySoundStyle(4, 32).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath33:
                    Main.PlaySound(new LegacySoundStyle(4, 33).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath34:
                    Main.PlaySound(new LegacySoundStyle(4, 34).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath35:
                    Main.PlaySound(new LegacySoundStyle(4, 35).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath36:
                    Main.PlaySound(new LegacySoundStyle(4, 36).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath37:
                    Main.PlaySound(new LegacySoundStyle(4, 37).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath38:
                    Main.PlaySound(new LegacySoundStyle(4, 38).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath39:
                    Main.PlaySound(new LegacySoundStyle(4, 39).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath40:
                    Main.PlaySound(new LegacySoundStyle(4, 40).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath41:
                    Main.PlaySound(new LegacySoundStyle(4, 41).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath42:
                    Main.PlaySound(new LegacySoundStyle(4, 42).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath43:
                    Main.PlaySound(new LegacySoundStyle(4, 43).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath44:
                    Main.PlaySound(new LegacySoundStyle(4, 44).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath45:
                    Main.PlaySound(new LegacySoundStyle(4, 45).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath46:
                    Main.PlaySound(new LegacySoundStyle(4, 46).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath47:
                    Main.PlaySound(new LegacySoundStyle(4, 47).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath48:
                    Main.PlaySound(new LegacySoundStyle(4, 48).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath49:
                    Main.PlaySound(new LegacySoundStyle(4, 49).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath50:
                    Main.PlaySound(new LegacySoundStyle(4, 50).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath51:
                    Main.PlaySound(new LegacySoundStyle(4, 51).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath52:
                    Main.PlaySound(new LegacySoundStyle(4, 52).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath53:
                    Main.PlaySound(new LegacySoundStyle(4, 53).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath54:
                    Main.PlaySound(new LegacySoundStyle(4, 54).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath55:
                    Main.PlaySound(new LegacySoundStyle(4, 55).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath56:
                    Main.PlaySound(new LegacySoundStyle(4, 56).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath57:
                    Main.PlaySound(new LegacySoundStyle(4, 57).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath58:
                    Main.PlaySound(new LegacySoundStyle(4, 58).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath59:
                    Main.PlaySound(new LegacySoundStyle(4, 59).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath60:
                    Main.PlaySound(new LegacySoundStyle(4, 60).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath61:
                    Main.PlaySound(new LegacySoundStyle(4, 61).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_NPCDeath62:
                    Main.PlaySound(new LegacySoundStyle(4, 62).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item1:
                    Main.PlaySound(new LegacySoundStyle(2, 1).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item2:
                    Main.PlaySound(new LegacySoundStyle(2, 2).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item3:
                    Main.PlaySound(new LegacySoundStyle(2, 3).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item4:
                    Main.PlaySound(new LegacySoundStyle(2, 4).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item5:
                    Main.PlaySound(new LegacySoundStyle(2, 5).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item6:
                    Main.PlaySound(new LegacySoundStyle(2, 6).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item7:
                    Main.PlaySound(new LegacySoundStyle(2, 7).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item8:
                    Main.PlaySound(new LegacySoundStyle(2, 8).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item9:
                    Main.PlaySound(new LegacySoundStyle(2, 9).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item10:
                    Main.PlaySound(new LegacySoundStyle(2, 10).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item11:
                    Main.PlaySound(new LegacySoundStyle(2, 11).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item12:
                    Main.PlaySound(new LegacySoundStyle(2, 12).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item13:
                    Main.PlaySound(new LegacySoundStyle(2, 13).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item14:
                    Main.PlaySound(new LegacySoundStyle(2, 14).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item15:
                    Main.PlaySound(new LegacySoundStyle(2, 15).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item16:
                    Main.PlaySound(new LegacySoundStyle(2, 16).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item17:
                    Main.PlaySound(new LegacySoundStyle(2, 17).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item18:
                    Main.PlaySound(new LegacySoundStyle(2, 18).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item19:
                    Main.PlaySound(new LegacySoundStyle(2, 19).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item20:
                    Main.PlaySound(new LegacySoundStyle(2, 20).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item21:
                    Main.PlaySound(new LegacySoundStyle(2, 21).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item22:
                    Main.PlaySound(new LegacySoundStyle(2, 22).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item23:
                    Main.PlaySound(new LegacySoundStyle(2, 23).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item24:
                    Main.PlaySound(new LegacySoundStyle(2, 24).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item25:
                    Main.PlaySound(new LegacySoundStyle(2, 25).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item26:
                    Main.PlaySound(new LegacySoundStyle(2, 26).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item27:
                    Main.PlaySound(new LegacySoundStyle(2, 27).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item28:
                    Main.PlaySound(new LegacySoundStyle(2, 28).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item29:
                    Main.PlaySound(new LegacySoundStyle(2, 29).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item30:
                    Main.PlaySound(new LegacySoundStyle(2, 30).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item31:
                    Main.PlaySound(new LegacySoundStyle(2, 31).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item32:
                    Main.PlaySound(new LegacySoundStyle(2, 32).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item33:
                    Main.PlaySound(new LegacySoundStyle(2, 33).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item34:
                    Main.PlaySound(new LegacySoundStyle(2, 34).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item35:
                    Main.PlaySound(new LegacySoundStyle(2, 35).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item36:
                    Main.PlaySound(new LegacySoundStyle(2, 36).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item37:
                    Main.PlaySound(new LegacySoundStyle(2, 37).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item38:
                    Main.PlaySound(new LegacySoundStyle(2, 38).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item39:
                    Main.PlaySound(new LegacySoundStyle(2, 39).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item40:
                    Main.PlaySound(new LegacySoundStyle(2, 40).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item41:
                    Main.PlaySound(new LegacySoundStyle(2, 41).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item42:
                    Main.PlaySound(new LegacySoundStyle(2, 42).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item43:
                    Main.PlaySound(new LegacySoundStyle(2, 43).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item44:
                    Main.PlaySound(new LegacySoundStyle(2, 44).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item45:
                    Main.PlaySound(new LegacySoundStyle(2, 45).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item46:
                    Main.PlaySound(new LegacySoundStyle(2, 46).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item47:
                    Main.PlaySound(new LegacySoundStyle(2, 47).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item48:
                    Main.PlaySound(new LegacySoundStyle(2, 48).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item49:
                    Main.PlaySound(new LegacySoundStyle(2, 49).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item50:
                    Main.PlaySound(new LegacySoundStyle(2, 50).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item51:
                    Main.PlaySound(new LegacySoundStyle(2, 51).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item52:
                    Main.PlaySound(new LegacySoundStyle(2, 52).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item53:
                    Main.PlaySound(new LegacySoundStyle(2, 53).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item54:
                    Main.PlaySound(new LegacySoundStyle(2, 54).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item55:
                    Main.PlaySound(new LegacySoundStyle(2, 55).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item56:
                    Main.PlaySound(new LegacySoundStyle(2, 56).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item57:
                    Main.PlaySound(new LegacySoundStyle(2, 57).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item58:
                    Main.PlaySound(new LegacySoundStyle(2, 58).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item59:
                    Main.PlaySound(new LegacySoundStyle(2, 59).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item60:
                    Main.PlaySound(new LegacySoundStyle(2, 60).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item61:
                    Main.PlaySound(new LegacySoundStyle(2, 61).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item62:
                    Main.PlaySound(new LegacySoundStyle(2, 62).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item63:
                    Main.PlaySound(new LegacySoundStyle(2, 63).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item64:
                    Main.PlaySound(new LegacySoundStyle(2, 64).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item65:
                    Main.PlaySound(new LegacySoundStyle(2, 65).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item66:
                    Main.PlaySound(new LegacySoundStyle(2, 66).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item67:
                    Main.PlaySound(new LegacySoundStyle(2, 67).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item68:
                    Main.PlaySound(new LegacySoundStyle(2, 68).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item69:
                    Main.PlaySound(new LegacySoundStyle(2, 69).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item70:
                    Main.PlaySound(new LegacySoundStyle(2, 70).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item71:
                    Main.PlaySound(new LegacySoundStyle(2, 71).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item72:
                    Main.PlaySound(new LegacySoundStyle(2, 72).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item73:
                    Main.PlaySound(new LegacySoundStyle(2, 73).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item74:
                    Main.PlaySound(new LegacySoundStyle(2, 74).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item75:
                    Main.PlaySound(new LegacySoundStyle(2, 75).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item76:
                    Main.PlaySound(new LegacySoundStyle(2, 76).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item77:
                    Main.PlaySound(new LegacySoundStyle(2, 77).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item78:
                    Main.PlaySound(new LegacySoundStyle(2, 78).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item79:
                    Main.PlaySound(new LegacySoundStyle(2, 79).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item80:
                    Main.PlaySound(new LegacySoundStyle(2, 80).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item81:
                    Main.PlaySound(new LegacySoundStyle(2, 81).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item82:
                    Main.PlaySound(new LegacySoundStyle(2, 82).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item83:
                    Main.PlaySound(new LegacySoundStyle(2, 83).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item84:
                    Main.PlaySound(new LegacySoundStyle(2, 84).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item85:
                    Main.PlaySound(new LegacySoundStyle(2, 85).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item86:
                    Main.PlaySound(new LegacySoundStyle(2, 86).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item87:
                    Main.PlaySound(new LegacySoundStyle(2, 87).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item88:
                    Main.PlaySound(new LegacySoundStyle(2, 88).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item89:
                    Main.PlaySound(new LegacySoundStyle(2, 89).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item90:
                    Main.PlaySound(new LegacySoundStyle(2, 90).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item91:
                    Main.PlaySound(new LegacySoundStyle(2, 91).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item92:
                    Main.PlaySound(new LegacySoundStyle(2, 92).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item93:
                    Main.PlaySound(new LegacySoundStyle(2, 93).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item94:
                    Main.PlaySound(new LegacySoundStyle(2, 94).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item95:
                    Main.PlaySound(new LegacySoundStyle(2, 95).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item96:
                    Main.PlaySound(new LegacySoundStyle(2, 96).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item97:
                    Main.PlaySound(new LegacySoundStyle(2, 97).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item98:
                    Main.PlaySound(new LegacySoundStyle(2, 98).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item99:
                    Main.PlaySound(new LegacySoundStyle(2, 99).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item100:
                    Main.PlaySound(new LegacySoundStyle(2, 100).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item101:
                    Main.PlaySound(new LegacySoundStyle(2, 101).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item102:
                    Main.PlaySound(new LegacySoundStyle(2, 102).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item103:
                    Main.PlaySound(new LegacySoundStyle(2, 103).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item104:
                    Main.PlaySound(new LegacySoundStyle(2, 104).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item105:
                    Main.PlaySound(new LegacySoundStyle(2, 105).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item106:
                    Main.PlaySound(new LegacySoundStyle(2, 106).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item107:
                    Main.PlaySound(new LegacySoundStyle(2, 107).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item108:
                    Main.PlaySound(new LegacySoundStyle(2, 108).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item109:
                    Main.PlaySound(new LegacySoundStyle(2, 109).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item110:
                    Main.PlaySound(new LegacySoundStyle(2, 110).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item111:
                    Main.PlaySound(new LegacySoundStyle(2, 111).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item112:
                    Main.PlaySound(new LegacySoundStyle(2, 112).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item113:
                    Main.PlaySound(new LegacySoundStyle(2, 113).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item114:
                    Main.PlaySound(new LegacySoundStyle(2, 114).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item115:
                    Main.PlaySound(new LegacySoundStyle(2, 115).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item116:
                    Main.PlaySound(new LegacySoundStyle(2, 116).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item117:
                    Main.PlaySound(new LegacySoundStyle(2, 117).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item118:
                    Main.PlaySound(new LegacySoundStyle(2, 118).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item119:
                    Main.PlaySound(new LegacySoundStyle(2, 119).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item120:
                    Main.PlaySound(new LegacySoundStyle(2, 120).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item121:
                    Main.PlaySound(new LegacySoundStyle(2, 121).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item122:
                    Main.PlaySound(new LegacySoundStyle(2, 122).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item123:
                    Main.PlaySound(new LegacySoundStyle(2, 123).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item124:
                    Main.PlaySound(new LegacySoundStyle(2, 124).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                case Sounds.LegacySoundStyle_Item125:
                    Main.PlaySound(new LegacySoundStyle(2, 125).WithVolume(volume).WithPitchVariance(pitchVariance), x, y);
                    break;
                //case Sounds.LegacySoundStyle_BlizzardInsideBuildingLoop:
                //    Main.PlaySound(SoundID.CreateTrackable("blizzard_inside_building_loop", SoundType.Ambient))
                //    break;
                //case Sounds.LegacySoundStyle_BlizzardStrongLoop:
                //    break;
                //case Sounds.LegacySoundStyle_LiquidsHoneyWater:
                //    break;
                //case Sounds.LegacySoundStyle_LiquidsHoneyLava:
                //    break;
                //case Sounds.LegacySoundStyle_LiquidsWaterLava:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BallistaTowerShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_ExplosiveTrapExplode:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_FlameburstTowerShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_LightningAuraZap:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DefenseTowerSpawn:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyFireballShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyFireballImpact:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyFlameBreath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyFlyingCircleAttack:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyScream:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsySummon:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsyWindAttack:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DarkMageAttack:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DarkMageCastHeal:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DarkMageDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DarkMageHealImpact:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DarkMageHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DarkMageSummonSkeleton:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DrakinBreathIn:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DrakinDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DrakinHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DrakinShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinScream:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinBomberDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinBomberHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinBomberScream:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GoblinBomberThrow:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_JavelinThrowersAttack:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_JavelinThrowersDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_JavelinThrowersHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_JavelinThrowersTaunt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldExplosion:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldIgnite:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldIgniteLoop:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldScreamChargeLoop:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldFlyerChargeScream:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldFlyerDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_KoboldFlyerHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_LightningBugDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_LightningBugHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_LightningBugZap:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_OgreAttack:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_OgreDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_OgreGroundPound:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_OgreHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_OgreRoar:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_OgreSpit:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SkeletonDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SkeletonHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SkeletonSummoned:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WitherBeastAuraPulse:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WitherBeastCrystalImpact:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WitherBeastDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WitherBeastHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WyvernDeath:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WyvernHurt:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WyvernScream:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WyvernDiveDown:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_EtherianPortalDryadTouch:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_EtherianPortalIdleLoop:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_EtherianPortalOpen:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_EtherianPortalSpawnEnemy:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_CrystalCartImpact:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_DefeatScene:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_WinScene:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsysWrathShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BetsysWrathImpact:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BookStaffCast:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_BookStaffTwisterLoop:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GhastlyGlaiveImpactGhost:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_GhastlyGlaivePierce:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_MonkStaffGroundImpact:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_MonkStaffGroundMiss:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_MonkStaffSwing:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_PhantomPhoenixShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SonicBoomBladeSlash:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SkyDragonsFuryCircle:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SkyDragonsFuryShot:
                //    break;
                //case Sounds.LegacySoundStyle_DD2_SkyDragonsFurySwing:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundId), soundId, null);
            }
        }
    }
}
