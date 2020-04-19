//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.IO;
using UnityEngine;

namespace JEM.Unity.DiscordRPC.Systems
{
    [Serializable]
    public sealed class JEMDiscordConfiguration
    {
        /// <summary>
        ///     Relative path to the configuration file.
        /// </summary>
        public const string ConfigFile = "JEM/JEMDiscordRPC.json";

        /// <summary>
        ///     Defines whether or not JEMDiscord should be enabled.
        /// </summary>
        public bool Enable = true;

        /// <summary>
        ///     When true, RPC is shown only when unity application is fully focused.
        /// </summary>
        public bool ShowPresenceOnlyWhenActive = false;

        /// <summary>
        ///     Defines time in second of how much time need to pass to reinitialize rpc again after code recompilation.
        /// </summary>
        public int RecompileTimeout = 30;
        
        public static void LoadConfiguration()
        {
            if (File.Exists(ConfigFile))
                Loaded = JsonUtility.FromJson<JEMDiscordConfiguration>(File.ReadAllText(ConfigFile));
            else
            {
                Debug.Log($"{ConfigFile} was missing. Creating new.");
                Loaded = new JEMDiscordConfiguration();
            }
        }

        public static void SaveConfiguration()
        {
            if (Loaded == null)
                return;
            
            File.WriteAllText(ConfigFile, JsonUtility.ToJson(Loaded));
        }

        public static JEMDiscordConfiguration Resolve()
        {
            if (Loaded == null)
                LoadConfiguration();
            
            return Loaded;
        }
        
        public static JEMDiscordConfiguration Loaded { get; private set; }
    }
}