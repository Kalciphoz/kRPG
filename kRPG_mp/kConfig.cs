using System;
using Newtonsoft.Json;
using Terraria;
using System.IO;
using Terraria.ModLoader;

namespace kRPG_mp
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
            try
            {
                configPath = string.Concat(new object[] {
                    Main.SavePath,
                    Path.DirectorySeparatorChar,
                    configName,
                });
                configLocal = new Config();
                Load();
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath);

                _configLocal = new Config();
                LoadConfig(configPath, ref _configLocal);
                Save();
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        private static void LoadConfig<T>(string path, ref T config) where T : class
        {
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        config = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                    }
                }
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Main.SavePath);
                File.WriteAllText(configPath, JsonConvert.SerializeObject(configLocal, Formatting.Indented).Replace("  ", "\t"));
            }
            catch (SystemException e)
            {
                ErrorLogger.Log(e.ToString());
            }
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
