using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Terraria;
using System.IO;

namespace kRPG
{
    public class kConfig
    {
        public static ClientConfig clientside
        {
            get
            {
                return configLocal.clientside;
            }
        }
        public static string configName = "kRPG_Settings.json";
        public static string configPath;

        //Make this fancier?
        internal static Config _configLocal = new Config();
        public static Config configLocal
        {
            get
            {
                if (_configLocal == null)
                {
                    _configLocal = new Config();
                    LoadConfig(configPath, ref _configLocal);
                }
                return _configLocal;
            }
            private set
            {
                _configLocal = value;
            }
        }
        internal static Config _configServer = new Config();
        public static Config configServer
        {
            get
            {
                if (_configServer == null)
                {
                    _configServer = new Config();
                }
                return _configServer;
            }
            private set
            {
                _configServer = value;
            }
        }

        public static void Initialize()
        {
            configPath = string.Concat(new object[] {
                Main.SavePath,
                Path.DirectorySeparatorChar,
                configName,
            });
            configLocal = new Config();
            Load();
        }
        public static void Load()
        {
            Directory.CreateDirectory(Main.SavePath);

            _configLocal = new Config();
            LoadConfig(configPath, ref _configLocal);
            Save();
        }
        private static void LoadConfig<T>(string path, ref T config) where T : class
        {
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        config = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                    }
                }
                catch { }
            }
        }
        public static void Save()
        {
            Directory.CreateDirectory(Main.SavePath);
            File.WriteAllText(configPath, JsonConvert.SerializeObject(configLocal, Formatting.Indented).Replace("  ", "\t"));
        }
        
        public class ClientConfig
        {
            public bool manualInventory = false;
            public bool arpgMinimap = false;
            public bool smartInventory = false;
        }
        public class Config
        {
            public ClientConfig clientside = new ClientConfig();
        }
    }
}
