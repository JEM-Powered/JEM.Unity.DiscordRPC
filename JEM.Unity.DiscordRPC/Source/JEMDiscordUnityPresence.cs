//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Unity.DiscordRPC.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JEM.Unity.DiscordRPC
{
    /// <summary>
    ///     Discord presence helper.
    /// </summary>
    public static class JEMDiscordUnityPresence
    {
        private enum State
        {
            DrawScene,
            DrawPrefab
        }
        
        public static void ReportSceneActivation(string sceneName)
        {
            if (_sceneName == sceneName)
                return;
            _sceneName = sceneName;
            _drawState = State.DrawScene;

            RefreshPresence();
        }

        public static void ReportPlayModeActivation(bool activeState)
        {
            if (_inPlayMode == activeState)
                return;
            
            _inPlayMode = activeState;

            RefreshPresence();
        }

        public static void RefreshPresence()
        {
            var scene = ResolveSceneName();
            var presence = new DiscordRpc.RichPresence();
            presence.details = $"Developing {Application.productName}";
            
            string str;
            if (_inPlayMode)
                str = $"In playmode ({scene})";
            else
            {
                switch (_drawState)
                {
                    case State.DrawScene:
                        str = $"Editing {scene}";
                        break;
                    case State.DrawPrefab:
                        str = "N/A";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            presence.state = str;
            
            presence.largeImageKey = JEMDiscordController.GetImageName(JEMDiscordImageKey.UnityDefault);
            presence.largeImageText = Application.unityVersion;
                
            JEMDiscordController.SetFullRPC(false, presence);
        }

        private static string ResolveSceneName()
        {
            // When no sceneName is available try to return name of currently active.
            if (string.IsNullOrEmpty(_sceneName))
            {
                var scene = SceneManager.GetActiveScene().name;
                if (string.IsNullOrEmpty(scene))
                    scene = "Untitled"; // If we can't resolve name of the scene, it's most likely a new unsaved scene.
                return scene;
            }

            return _sceneName;
        }

        private static State _drawState;
        private static string _sceneName;
        private static bool _inPlayMode;
    }
}