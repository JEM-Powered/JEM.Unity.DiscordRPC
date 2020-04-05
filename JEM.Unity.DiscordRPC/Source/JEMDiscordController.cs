//
// JEM.Unity DiscordRPC
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Unity.DiscordRPC.Common;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.Unity.DiscordRPC
{
    [InitializeOnLoad]
    public class JEMDiscordController
    {
        static JEMDiscordController()
        {
            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                // // Clear cache.
                // if (!Application.isPlaying)
                // {
                //     SceneObjectReferenceDatabase.ExternalObjectReferences = null;
                //     SceneObjectReferenceDatabase.CachedObjects.Clear();
                // }
                //
                // IsOpeningScene = false;
            };
            
            EditorApplication.playModeStateChanged += change =>
            {
                switch (change)
                {
                    case PlayModeStateChange.EnteredEditMode:
                        // IsEditor = true;
                        break;
                    case PlayModeStateChange.ExitingEditMode:
                    case PlayModeStateChange.EnteredPlayMode:
                    case PlayModeStateChange.ExitingPlayMode:
                        // IsEditor = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(change), change, null);
                }
            };

            Debug.Log("Hello, World");
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
        }

        private static bool _updateCompilationNumberOnce;
        private static void OnAssemblyCompilationStarted(string str)
        {
            if (_updateCompilationNumberOnce) return;
            _updateCompilationNumberOnce = true;
            Debug.Log("Compile");
        }
        
        //
        // internal const string ApplicationId = "696168436556103771";
        // internal const string OptionalSteamId = "";
        //
        // private DiscordRpc.RichPresence _presence;
        // private DiscordRpc.EventHandlers _handlers;
        //
        // private void Start()
        // {
        //     if (Application.isBatchMode) return; // ignore discord in batchmode!
        //     // JEMLogger.Log("Discord: init", "DISCORD");
        //     _handlers = new DiscordRpc.EventHandlers();
        //     _handlers.readyCallback += ReadyCallback;
        //     _handlers.disconnectedCallback += DisconnectedCallback;
        //     _handlers.errorCallback += ErrorCallback;
        //     _handlers.joinCallback += JoinCallback;
        //     _handlers.spectateCallback += SpectateCallback;
        //     _handlers.requestCallback += RequestCallback;
        //     DiscordRpc.Initialize(ApplicationId, ref _handlers, true, OptionalSteamId);
        // }
        //
        // private void OnDestroy()
        // {
        //     if (EventTrigger.Entry.IsBachMode) return; // ignore discord in batchmode!
        //     if (Instance == null)
        //         return;
        //
        //     // JEMLogger.Log("Discord: shutdown", "DISCORD");
        //     DiscordRpc.Shutdown();
        // }
        //
        // private void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
        // {
        //     JEMLogger.Log($"Discord: connected to {connectedUser.username}#{connectedUser.discriminator}: {connectedUser.userId}", "DISCORD");
        // }
        //
        // private void DisconnectedCallback(int errorCode, string message)
        // {
        //     JEMLogger.Log($"Discord: disconnect {errorCode}: {message}", "DISCORD");
        // }
        //
        // private void ErrorCallback(int errorCode, string message)
        // {
        //     JEMLogger.Log($"Discord: error {errorCode}: {message}", "DISCORD");
        // }
        //
        // private void JoinCallback(string secret)
        // {
        //     JEMLogger.Log($"Discord: join ({secret})", "DISCORD");
        // }
        //
        // private void SpectateCallback(string secret)
        // {
        //     JEMLogger.Log($"Discord: spectate ({secret})", "DISCORD");
        // }
        //
        // private void RequestCallback(ref DiscordRpc.DiscordUser request)
        // {
        //     JEMLogger.Log($"Discord: join request {request.username}#{request.discriminator}: {request.userId}", "DISCORD");
        // }
        //
        // private void Update()
        // {
        //     if (EventTrigger.Entry.IsBachMode) return; // ignore discord in batchmode!
        //     DiscordRpc.RunCallbacks();
        // }
        //
        // internal static void SetPresenceToString(bool restartTimestamp, string strState)
        // {
        //     if (EventTrigger.Entry.IsBachMode) return; // ignore discord in batchmode!
        //     if (restartTimestamp)
        //         _lastTimestamp = GetTimestamp();
        //
        //     Instance._presence = new DiscordRpc.RichPresence
        //     {
        //         details = strState,
        //         state = string.Empty,
        //         largeImageKey = "image_large",
        //
        //         startTimestamp = _lastTimestamp
        //     };
        //
        //     DiscordRpc.UpdatePresence(Instance._presence);
        // }
        //
        // internal static void CollectAndSendInGamePresence(bool restartTimestamp)
        // {
        //     if (EventTrigger.Entry.IsBachMode) return; // ignore discord in batchmode!
        //     try
        //     {
        //         if (restartTimestamp)
        //             _lastTimestamp = GetTimestamp();
        //
        //         Instance._presence = new DiscordRpc.RichPresence
        //         {
        //             details = "Playing: " + NetworkShared.ReceivedServerMeta.GamemodeName,
        //             state = GetFixedMapName(),
        //             largeImageKey = "image_large",
        //
        //             partySize = PlayerEntity.Players.Count,
        //             partyMax = NetworkShared.ReceivedServerMeta.ServerSize,
        //
        //             startTimestamp = _lastTimestamp
        //         };
        //
        //         DiscordRpc.UpdatePresence(Instance._presence);
        //     }
        //     catch (Exception e)
        //     {
        //         throw new InvalidOperationException("An unexcepted error occurred while updating discord presence.", e);
        //     }
        // }
        //
        // private static string GetFixedMapName()
        // {
        //     var n = NetworkShared.ReceivedServerMeta.MapName;
        //     if (n.StartsWith("m_"))
        //         n = n.Remove(0, 2); // remove m_ from the name
        //     return n;
        // }
        //
        // private static long GetTimestamp()
        // {
        //     long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
        //     ticks /= 10000000; // Convert windows ticks to seconds
        //     return ticks;
        // }
        //
        // private static long _lastTimestamp;
    }
}