﻿using System;
using HugsLib;
using HugsLib.Utils;
using PrepareLanding.Gui.World;
using PrepareLanding.Patches;
using Verse;

//TODO: general TODO -> translate all GUI strings.

namespace PrepareLanding
{
    public class PrepareLanding : ModBase
    {
        public PrepareLanding()
        {
            Logger.Message("In constructor.");
            if (Instance == null)
                Instance = this;
        }

        public static PrepareLanding Instance { get; private set; }

        public ModLogger ModLogger => Logger;

        public PrepareLandingUserData UserData { get; private set; }

        public WorldTileFilter TileFilter { get; private set; }

        public TileHighlighter TileHighlighter { get; private set; }

        public PrepareLandingWindow MainWindow { get; set; }


        /// <summary>
        ///     A unique identifier for your mod.
        ///     Valid characters are A-z, 0-9, -, no spaces.
        /// </summary>
        public override string ModIdentifier => "PrepareLanding";


        /// <summary>
        ///     Methods can register to this event to be called when definitions (Defs) have been loaded.
        /// </summary>
        public event Action OnDefsLoaded = delegate { };

        public event Action OnWorldGenerated = delegate { };

        public event Action OnWorldInterfaceOnGui = delegate { };

        public event Action OnWorldInterfaceUpdate = delegate { };

        public static void RemoveInstance()
        {
            Instance = null;
        }

        /// <summary>
        ///     Called during mod initialization.
        /// </summary>
        public override void Initialize()
        {
            Logger.Message("Initializing.");

            // initialize events
            PatchWorldInterfaceOnGui.OnWorldInterfaceOnGui += WorldInterfaceOnGui;
            PatchWorldInterfaceUpdate.OnWorldInterfaceUpdate += WorldInterfaceUpdate;

            UserData = new PrepareLandingUserData();

            // note: constructor should be called after the above events have been set
            TileHighlighter = new TileHighlighter();
        }

        /// <summary>
        ///     Called when the world map is generated.
        /// </summary>
        /// <remarks>This is not a RimWorld event. It is generated by an Harmony patch.</remarks>
        public void OnGenerateWorld()
        {
            // disable all tiles that are currently highlighted
            TileHighlighter.RemoveAllTiles();

            // new tile filter
            TileFilter = new WorldTileFilter(UserData);

            // call onto subscribers to tell them that the world has been generated.
            OnWorldGenerated.Invoke();
        }

        /// <summary>
        ///     Called after Initialize and when defs have been reloaded. This is a good place to inject defs.
        ///     Get your settings handles here, so that the labels will properly update on language change.
        ///     If the mod is disabled after being loaded, this method will STILL execute. Use ModIsActive to check.
        /// </summary>
        public override void DefsLoaded()
        {
            if (!ModIsActive)
            {
                Log.Message("[PrepareLanding] DefsLoaded: Mod is not active, bailing out.");
                return;
            }

            Logger.Message("DefsLoaded");

            OnDefsLoaded.Invoke();
        }

        /// <summary>
        ///     Called on each <see cref="RimWorld.WorldInterface" /> Gui event.
        /// </summary>
        public void WorldInterfaceOnGui()
        {
            OnWorldInterfaceOnGui.Invoke();
        }

        /// <summary>
        ///     Called on each <see cref="RimWorld.WorldInterface" /> update event.
        /// </summary>
        public void WorldInterfaceUpdate()
        {
            OnWorldInterfaceUpdate.Invoke();
        }
    }
}