//
// JEM.Unity DiscordRPC
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Globalization;
using JEM.Unity.DiscordRPC.Common;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JEM.Unity.DiscordRPC.Systems
{
    public enum JEMDiscordImageKey
    {
        UnityDefault
    }
    
    /// <summary>
    ///     Main discord controller. 
    /// </summary>
    public static class JEMDiscordController
    {
        private const string ApplicationId = "696168436556103771";
        private const string OptionalSteamId = "";

        /// <summary>
        ///     Initialize discord RPC controller.
        /// </summary>
        public static void Init(bool forced)
        {
            if (Application.isBatchMode)
            {
                Debug.Log($"Discord RPC can't be initialized in BathMode.");
                return; // ignore discord in batchmode!
            }

            //Debug.Log($"JEMDiscordController.Initialize isInitialized:{IsInitialized}");

            if (IsInitialized) return;
            IsInitialized = true;
            
            // Load configuration.
            JEMDiscordConfiguration.LoadConfiguration();
            // Hook the update
            EditorApplication.update += Update;
            
            // Load last initialization time.
            if (EditorPrefs.HasKey("jem.discordRPC.lastInitialization"))
                _lastInitializationTime = DateTime.Parse(EditorPrefs.GetString("jem.discordRPC.lastInitialization"),
                    CultureInfo.InvariantCulture);
            
            // To prevent spamming with discord RPC init calls we will only initialize discord controller again if there is at least
            //        one minute difference with previous init call.
            if (CanInitialize())
                InternalDiscordInitialize();
        }

        private static bool CanInitialize()
        {
            var config = JEMDiscordConfiguration.Resolve();
            if (!config.Enable)
                return false;
            
            var diff = DateTime.Now - _lastInitializationTime;
            return diff.Seconds >= config.RecompileTimeout;
        }

        internal static void InternalDiscordInitialize()
        {
            if (IsConnected)
                return;
            
            _handlers = new DiscordRpc.EventHandlers();
            
            Debug.Log("Discord: init");
            DiscordRpc.Initialize(ApplicationId, ref _handlers, true, OptionalSteamId);
            IsConnected = true;
            
            JEMDiscordUnityPresence.RefreshPresence();
        }
        
        /// <summary>
        ///     Shutdown discord controller.
        /// </summary>
        public static void Shutdown(bool dontSaveAnything)
        {
            //Debug.Log($"JEMDiscordController.Shutdown isInitialized:{IsInitialized} dontSaveAnything:{dontSaveAnything}");
            
            if (!IsInitialized) return;
            IsInitialized = false;
            
            // Disconnect update.
            EditorApplication.update -= Update;
            
            // Save timestamp.
            SaveTimestamp(dontSaveAnything);
            if (IsConnected)
            {
                Debug.Log("Discord: shutdown");
                DiscordRpc.Shutdown();
            }

            // Save last discord initialization time.
            EditorPrefs.SetString("jem.discordRPC.lastInitialization", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            IsConnected = false;
        }
        
        private static bool _wasWindowFocused;
        private static void Update()
        {
            if (!IsInitialized) return;
            if (!IsConnected)
            {
                // While discord is not yet connected, we will try again after one min.                            
                if (CanInitialize())
                    InternalDiscordInitialize();
                
                return;
            }
            // Hide unity presence if not focused.
            // TODO: Adjust timestamp after receiving focus again
            if (_wasWindowFocused == InternalEditorUtility.isApplicationActive) return;
            _wasWindowFocused = InternalEditorUtility.isApplicationActive;
            if (!JEMDiscordConfiguration.Resolve().ShowPresenceOnlyWhenActive)
                return;
            
            if (_wasWindowFocused)
            {
                JEMDiscordUnityPresence.RefreshPresence();
            }
            else
            {
                JEMDiscordUnityPresence.RefreshPresence(true);
            }
        }

        /// <summary>
        ///     Sets discord's rich presence the simplest way.
        /// </summary>
        public static void SetSimpleRPC(bool restartTimestamp, string stateStr, string detailsStr = "")
        {
            if (!IsInitialized) return;
            if (restartTimestamp || _lastTimestamp == 0)
                ResetTimestamp();
            
            SetFullRPC(restartTimestamp, new DiscordRpc.RichPresence()
            {
                state = stateStr,
                details = detailsStr,
                largeImageKey = GetImageName(JEMDiscordImageKey.UnityDefault),
                largeImageText = "",
            });
        }

        /// <summary>
        ///     Sets discord's rich presence using custom rpc data.
        /// </summary>
        public static void SetFullRPC(bool restartTimestamp, [NotNull] DiscordRpc.RichPresence richPresence)
        {
            if (richPresence == null) throw new ArgumentNullException(nameof(richPresence));
            //Debug.Log("Will update presence to " + richPresence);
            
            if (!IsInitialized) return;
            if (restartTimestamp || _lastTimestamp == 0)
                ResetTimestamp();

            if (!IsConnected)
                return;

            HasPresence = true;
            DiscordRpc.UpdatePresence(richPresence);
        }

        public static void ClearRPC()
        {
            if (!IsInitialized) return;
            if (!IsConnected)
                return;

            HasPresence = false;
            DiscordRpc.ClearPresence();
        }
        
        /// <summary>
        ///     Returns string key of given image we can use in our rich presence.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static string GetImageName(JEMDiscordImageKey key)
        {
            switch (key)
            {
                case JEMDiscordImageKey.UnityDefault:
                    return "icon_unity";
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
        
        public static void SetTimestamp(long newTimestamp)
        {
            _lastTimestamp = newTimestamp;
        }
        
        public static void ResetTimestamp()
        {
            _lastTimestamp = GetTimestamp();   
        }
        
        public static long GetTimestamp()
        {
            if (!_wasTimestampLoaded)
            {
                _wasTimestampLoaded = true;
                if (EditorPrefs.HasKey("jem.discordRPC.timestamp"))
                    return long.Parse(EditorPrefs.GetString("jem.discordRPC.timestamp"));
            }
            
            var ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000; // Convert windows ticks to seconds
            return ticks;
        }

        public static void SaveTimestamp(bool clear)
        {
            if (clear)
            {
                EditorPrefs.DeleteKey("jem.discordRPC.timestamp");
            }
            else
            {
                EditorPrefs.SetString("jem.discordRPC.timestamp", _lastTimestamp.ToString());
            }
        }

        internal static DateTime _lastInitializationTime;
        private static long _lastTimestamp;
        private static bool _wasTimestampLoaded;
        
        private static DiscordRpc.EventHandlers _handlers;
        
        /// <summary>
        ///     True when discord received content to draw.
        /// </summary>
        public static bool HasPresence { get; private set; }
        
        /// <summary>
        ///     Defines whether the discord rpc controller has been initialized or not.
        /// </summary>
        public static bool IsInitialized { get; private set; }
        
        /// <summary>
        ///     Defines whether we have connection with discord rpc or not.
        /// </summary>
        public static bool IsConnected { get; private set; }
    }
}