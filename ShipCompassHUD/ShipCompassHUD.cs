using System.IO;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

using ShipCompassHUD;
using ShipCompassHUD.Components;

namespace ShipCompassHUD
{
    public class ShipCompassHUD : ModBehaviour
    {
        public static ShipCompassHUD Instance;
        public GameObject _CompassInit;

        public AssetBundle _markersBundle;

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            ModHelper.Console.WriteLine($"Look upon my Works, ye Mighty, and despair! Flight Path Vectors is loaded...", MessageType.Success);

            new Harmony("SpatialX.ShipCompassHUD").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            _markersBundle = AssetBundle.LoadFromFile(Path.Combine(ModHelper.Manifest.ModFolderPath, "assets/hudmarkers"));
        }


        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            SpawnOnStart();
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }



        public void SpawnOnStart()
        {
            if (_CompassInit == null)
            {
                _CompassInit = new GameObject("ShipCompass");
                _CompassInit.AddComponent<CompassInit>();
                Instance.ModHelper.Events.Unity.FireInNUpdates(_CompassInit.GetComponent<CompassInit>().Start, 10);
            }
            else
            {
                _CompassInit.GetComponent<CompassInit>().Initialized = false;
            }
        }


        public void LogForStupids(string PrintIT)
        {
            ModHelper.Console.WriteLine(PrintIT, MessageType.Success);
        }


        public GameObject LoadAsset(string path)
        {
            return (GameObject)_markersBundle.LoadAsset(path);
        }

        public override void Configure(IModConfig config)
        {
            ConfigureAlarmFirstStep();
        }

        public void ConfigureAlarmFirstStep()
        {
            var ShowMark = ModHelper.Config.GetSettingsValue<bool>("Enable Flight Path Vectors HUD");
            var MarkSize = ModHelper.Config.GetSettingsValue<float>("Markers Size");
            var RotSensi = ModHelper.Config.GetSettingsValue<float>("Auto Rotation Sensibility");
            var HorizonKind = ModHelper.Config.GetSettingsValue<string>("Artificial Horizon");

            var ShowAlt = ModHelper.Config.GetSettingsValue<bool>("Enable Rate of Closure HUD");
            var ShowSpd = ModHelper.Config.GetSettingsValue<bool>("Enable Surface Speed HUD");
            var ShowOrb = ModHelper.Config.GetSettingsValue<bool>("Enable True Speed HUD");
            var ShowAcc = ModHelper.Config.GetSettingsValue<bool>("Enable Acceleration HUD");
            var ShowOrbSpd = ModHelper.Config.GetSettingsValue<bool>("Enable Orbit Speed HUD");

            var AccUnit = ModHelper.Config.GetSettingsValue<string>("Acceleration Units");
            var ShowConsole = ModHelper.Config.GetSettingsValue<bool>("Enable Trajectory Console (broken)");
            

            if (_CompassInit != null)
            {
                if (_CompassInit.GetComponent<CompassInit>() != null)
                {
                    _CompassInit.GetComponent<CompassInit>().ConfigureInTwoStep(ShowMark, MarkSize, HorizonKind, RotSensi, ShowAlt, ShowSpd, ShowOrb, ShowAcc, ShowOrbSpd, AccUnit, ShowConsole);
                }
            }
        }

    }

}
