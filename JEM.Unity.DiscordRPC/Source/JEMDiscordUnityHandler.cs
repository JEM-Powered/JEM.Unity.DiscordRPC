//
// JEM.Unity DiscordRPC
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JEM.Unity.DiscordRPC
{
    [InitializeOnLoad]
    internal class JEMDiscordUnityHandler
    {
        static JEMDiscordUnityHandler()
        {
            // Handle scene change.
            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                if (!Application.isPlaying)
                    JEMDiscordUnityPresence.ReportSceneActivation(scene.name);
            };
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (Application.isPlaying)
                    JEMDiscordUnityPresence.ReportSceneActivation(scene.name);
            };
            EditorSceneManager.newSceneCreated += (scene, setup, mode) =>
            {
                if (!Application.isPlaying)
                    JEMDiscordUnityPresence.ReportSceneActivation(scene.name);
            };
            
            // Handle playmode change.
            EditorApplication.playModeStateChanged += change =>
            {
                switch (change)
                {
                    case PlayModeStateChange.EnteredEditMode:
                        JEMDiscordUnityPresence.ReportPlayModeActivation(false);
                        break;
                    case PlayModeStateChange.ExitingEditMode:
                    case PlayModeStateChange.EnteredPlayMode:
                    case PlayModeStateChange.ExitingPlayMode:
                        JEMDiscordUnityPresence.ReportPlayModeActivation(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(change), change, null);
                }
            };
            
            EditorApplication.quitting += () =>
            {
                // Shutdown discord and don't save anything for next session.
                JEMDiscordController.Shutdown(true);
            };
            
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;

            JEMDiscordController.Init();
            JEMDiscordUnityPresence.RefreshPresence();
        }
 
        private static bool _updateCompilationNumberOnce;
        private static void OnAssemblyCompilationStarted(string str)
        {
            if (_updateCompilationNumberOnce) return;
            _updateCompilationNumberOnce = true;

            // Always shutdown controller when assemblies are being recompiled.
            // NOTE: Save last timestamp to restore it in the next session.
            JEMDiscordController.Shutdown(false);
        }
    }
}