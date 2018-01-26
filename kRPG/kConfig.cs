using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Terraria;
using System.IO;
using Terraria.ModLoader;

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
		public static string configPath
        {
            get
            {
                return Main.SavePath + Path.DirectorySeparatorChar + "kRPG_Settings.json";
            }
        }
		public static string statsPath
        {
            get
            {
                return Main.SavePath + Path.DirectorySeparatorChar + "kRPG_Stats.json";
            }
        }

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
		internal static ConfigStats _stats=	new ConfigStats();
		public static ConfigStats stats {
			get {
				if(_stats==null) {
					_stats=	new ConfigStats();
					LoadConfig(statsPath,ref _stats);
				}
				return _stats;
			}
			private set {
				_stats=	value;
			}
		}

        public static void Initialize()
        {
            try
            {
                configLocal = new Config();
				stats = new ConfigStats();
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
                if (_configLocal == null) _configLocal = new Config();
                Save();

				_stats = new ConfigStats();
				LoadConfig(statsPath,ref _stats);
                if (_stats == null) _stats = new ConfigStats();
				SaveStats();
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
		public static void SaveStats()
		{
			Directory.CreateDirectory(Main.SavePath);
			File.WriteAllText(statsPath,JsonConvert.SerializeObject(stats,Formatting.Indented).Replace("  ","\t"));
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
		public class ConfigStats
		{
			public string lastStartVersion=		"1.1.1";
		}
    }
}
