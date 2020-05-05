//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Unity.DiscordRPC.Systems;
using UnityEditor;
using UnityEngine;

namespace JEM.Unity.DiscordRPC.Editor
{
    public sealed class JEMDiscordConfigurationWindow : EditorWindow
    {
        private void OnEnable()
        {
            // Apply Title
            titleContent = new GUIContent("JEM DiscordRPC Configuration");

            // Load config,
            JEMDiscordConfiguration.LoadConfiguration();
        }

        private void OnInspectorUpdate() => Repaint();
        
        private void OnGUI()
        {
            var cfg = JEMDiscordConfiguration.Loaded;

            GUILayout.Label("Settings", EditorStyles.boldLabel);
            cfg.Enable = EditorGUILayout.Toggle("Enable RPC", cfg.Enable);
            cfg.ShowPresenceOnlyWhenActive =
                EditorGUILayout.Toggle("Show only when window focused", cfg.ShowPresenceOnlyWhenActive);

            cfg.RecompileTimeout = EditorGUILayout.IntSlider("Recompile Timeout", cfg.RecompileTimeout, 10, 360);

            EditorGUILayout.Space();
            GUI.enabled = false;
            GUILayout.Label("Presence Status", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("HasPresence", JEMDiscordController.HasPresence.ToString());
            if (!JEMDiscordController.IsConnected)
            {
                var diff = DateTime.Now - JEMDiscordController._lastInitializationTime;
                EditorGUILayout.LabelField("Will connect in",$"{cfg.RecompileTimeout - diff.Seconds:0.00}");   
            }
            else
            {
                EditorGUILayout.LabelField("Connected");
            }
            EditorGUILayout.LabelField("_drawState", JEMDiscordUnityPresence._drawState.ToString());
            EditorGUILayout.LabelField("_sceneName", JEMDiscordUnityPresence._sceneName);
            EditorGUILayout.LabelField("_inPlayMode", JEMDiscordUnityPresence._inPlayMode.ToString());
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save Configuration"))
            {
                JEMDiscordConfiguration.SaveConfiguration();
            }

            if (GUILayout.Button("Refresh Presence"))
            {
                JEMDiscordUnityPresence.RefreshPresence();
            }

            if (GUILayout.Button("Clear Presence"))
            {
                JEMDiscordUnityPresence.RefreshPresence(true);
            }
        }

        [MenuItem("JEM/JEM DiscordRPC", priority = 100)]
        public static void ShowWindow()
        {
            var activeWindow = GetWindow<JEMDiscordConfigurationWindow>(true, "JEM", true);
            activeWindow.maxSize = new Vector2(580, 420);
            activeWindow.minSize = activeWindow.maxSize;
        }
    }
}