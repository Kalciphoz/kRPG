using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Terraria;

namespace kRPG.Util
{
    public static class Utility
    {
        public static bool Contains<T>(this IEnumerable<T> e, T item)
        {
            List<T> values = e.ToList();
            return values.Contains(item);
        }

        public static TV Random<TK, TV>(this Dictionary<TK, TV> dictionary)
        {
            return dictionary.Values.ToList().Random();
        }

        public static T Random<T>(this IEnumerable<T> e)
        {
            List<T> values = e.ToList();
            return values[Main.rand.Next(values.Count)];
        }

        public static void CheckForUpdates()
        {
#pragma warning disable 162
            try
            {
                string url = @"http://raw.githubusercontent.com/FairfieldTekLLC/kRPG/master/kRPG_VersionInfo.json";
                WebClient client = new WebClient();
                Version currentVersion =kRPG.Mod.Version;
                client.DownloadStringCompleted += (sender, e) =>
                {
                    try
                    {
                        string text = e.Result;
                        VersionInfo versionInfo = JsonConvert.DeserializeObject<VersionInfo>(text);
                        Version latestVersion = new Version(versionInfo.version);
                        if (latestVersion > currentVersion)
                        {
                            //Public update available
                            Main.NewText("[c/cccccc:New version of] [c/ffdb00:KRPG] [c/cccccc:available]");
                            Main.NewTextMultiline("[c/cccccc:Summary:] " + versionInfo.summary, WidthLimit: 725);
                            Main.NewText("[c/cccccc:Get the update from Mod Browser]");
                        }
                        else if (latestVersion == currentVersion && new Version(kConfig.Stats.LastStartVersion) < currentVersion)
                        {
                            //After installing a public update
                            Main.NewText("[c/cccccc:KRPG is now up to date!]");
                            Main.NewTextMultiline("[c/cccccc:Summary changes:] " + versionInfo.summary, WidthLimit: 725);
                        }

                        kConfig.Stats.LastStartVersion = currentVersion.ToString();
                        kConfig.SaveStats();
                    }
                    catch
                    {
                        //
                    }
                };
                client.DownloadStringAsync(new Uri(url), url);
            }
            catch
            {
                //
            }
#pragma warning restore 162
        }
    }
}
