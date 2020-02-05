// Kalciphoz's RPG Mod
//  Copyright (c) 2016, Kalciphoz's RPG Mod
// 
// 
// THIS SOFTWARE IS PROVIDED BY Kalciphoz's ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL FAIRFIELDTEK LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader;

namespace kRPG
{
    public class kConfig
    {
        //Make this fancier?
        private static Config _configLocal = new Config();

        private static Config _configServer = new Config();

        private static ConfigStats _stats = new ConfigStats();
        public static ClientConfig ClientSide => ConfigLocal.ClientSide;

        public static Config ConfigLocal
        {
            get
            {
                if (_configLocal != null)
                    return _configLocal;
                _configLocal = new Config();
                LoadConfig(ConfigPath, ref _configLocal);
                return _configLocal;
            }
            private set => _configLocal = value;
        }

        public static string ConfigPath => Main.SavePath + Path.DirectorySeparatorChar + "kRPG_Settings.json";

        public static Config ConfigServer
        {
            get => _configServer ?? (_configServer = new Config());
            private set => _configServer = value;
        }

        public static ConfigStats Stats
        {
            get
            {
                if (_stats != null)
                    return _stats;
                _stats = new ConfigStats();
                LoadConfig(StatsPath, ref _stats);

                return _stats;
            }
            private set => _stats = value;
        }

        public static string StatsPath => Main.SavePath + Path.DirectorySeparatorChar + "kRPG_Stats.json";

        public static void Initialize()
        {
            try
            {
                ConfigLocal = new Config();
                Stats = new ConfigStats();
                Load();
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath);

                _configLocal = new Config();
                LoadConfig(ConfigPath, ref _configLocal);
                if (_configLocal == null) _configLocal = new Config();
                Save();

                _stats = new ConfigStats();
                LoadConfig(StatsPath, ref _stats);
                if (_stats == null) _stats = new ConfigStats();
                SaveStats();
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        private static void LoadConfig<T>(string path, ref T config) where T : class
        {
            try
            {
                if (!File.Exists(path))
                    return;
                using (StreamReader reader = new StreamReader(path))
                {
                    config = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                }
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath);
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(ConfigLocal, Formatting.Indented).Replace("  ", "\t"));
            }
            catch (SystemException e)
            {
                ModLoader.GetMod("kRPG").Logger.InfoFormat(e.ToString());
            }
        }

        public static void SaveStats()
        {
            Directory.CreateDirectory(Main.SavePath);
            File.WriteAllText(StatsPath, JsonConvert.SerializeObject(Stats, Formatting.Indented).Replace("  ", "\t"));
        }

        public class ClientConfig
        {
            public bool ArpgMiniMap { get; set; } = false;
            public bool ManualInventory { get; set; } = false;
            public bool SmartInventory { get; set; } = false;
        }

        public class Config
        {
            public ClientConfig ClientSide { get; set; } = new ClientConfig();
        }

        public class ConfigStats
        {
            public string LastStartVersion { get; set; } = "1.1.1";
        }
    }
}