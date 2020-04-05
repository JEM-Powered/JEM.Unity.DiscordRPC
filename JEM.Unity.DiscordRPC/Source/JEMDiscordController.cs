//
// JEM.Unity DiscordRPC
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Unity.DiscordRPC.Common;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace JEM.Unity.DiscordRPC
{
    public enum JEMDiscordImageKey
    {
        UnityDefault
    }
    
    public static class JEMDiscordController
    {
        private const string ApplicationId = "696168436556103771";
        private const string OptionalSteamId = "";
        
        /// <summary>
        ///     Initialize discord controller.
        /// </summary>
        public static void Init()
        {
            if (Application.isBatchMode)
            {
                Debug.Log($"Discord RPC can't be initialized in BathMode.");
                return; // ignore discord in batchmode!
            }

            if (IsInitialized) return;
            IsInitialized = true;
            
            //Debug.Log("Discord: init");
            
            _handlers = new DiscordRpc.EventHandlers();
            // _handlers.readyCallback += ReadyCallback;
            // _handlers.disconnectedCallback += DisconnectedCallback;
            // _handlers.errorCallback += ErrorCallback;

            DiscordRpc.Initialize(ApplicationId, ref _handlers, true, OptionalSteamId);

            // Hook the update
            EditorApplication.update += Update;
        }
        
        /// <summary>
        ///     Shutdown discord controller.
        /// </summary>
        public static void Shutdown(bool dontSaveAnything)
        {
            if (!IsInitialized) return;
            IsInitialized = false;
            
            // Save timestamp.
            SaveTimestamp(dontSaveAnything);
            
            // Disconnect update.
            EditorApplication.update -= Update;
            
            // JEMLogger.Log("Discord: shutdown", "DISCORD");
            DiscordRpc.Shutdown();
        }
        
        // private static void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
        // {
        //     Debug.Log($"Discord: connected to {connectedUser.username}#{connectedUser.discriminator}: {connectedUser.userId}");
        // }
        //
        // private static void DisconnectedCallback(int errorCode, string message)
        // {
        //     Debug.Log($"Discord: disconnect {errorCode}: {message}");
        // }
        //
        // private static void ErrorCallback(int errorCode, string message)
        // {
        //     Debug.Log($"Discord: error {errorCode}: {message}");
        // }

        private static bool _wasWindowFocused;
        private static void Update()
        {
            if (!IsInitialized) return;
            
            // Hide unity presence if not focused.
            // TODO: Adjust timestamp after receiving focus again
            if (_wasWindowFocused != UnityEditorInternal.InternalEditorUtility.isApplicationActive)
            {
                _wasWindowFocused = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
                if (_wasWindowFocused)
                {
                    JEMDiscordUnityPresence.RefreshPresence();
                }
                else
                {
                    DiscordRpc.ClearPresence();
                }
            }

            // no need for callbacks?
            // DiscordRpc.RunCallbacks();
        }

        /// <summary>
        ///     Sets discord's rich presence the simplest way.
        /// </summary>
        public static void SetSimpleRPC(bool restartTimestamp, string stateStr, string detailsStr = "")
        {
            if (!IsInitialized) return;
            if (restartTimestamp || _lastTimestamp == 0)
                ResetTimestamp();
        
            _lastPresence = new DiscordRpc.RichPresence
            {
                state = stateStr,
                details = detailsStr,
                largeImageKey = GetImageName(JEMDiscordImageKey.UnityDefault),
                largeImageText =  "",
                
                startTimestamp = _lastTimestamp
            };
        
            DiscordRpc.UpdatePresence(_lastPresence);
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

            _lastPresence = richPresence;
            _lastPresence.startTimestamp = _lastTimestamp;
            
            DiscordRpc.UpdatePresence(_lastPresence);
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
        
        private static long _lastTimestamp;
        private static bool _wasTimestampLoaded;
        
        private static DiscordRpc.RichPresence _lastPresence;
        private static DiscordRpc.EventHandlers _handlers;
        
        /// <summary>
        ///     Defines whether the discord rpc controller has been initialized or not.
        /// </summary>
        public static bool IsInitialized { get; private set; }
    }
}