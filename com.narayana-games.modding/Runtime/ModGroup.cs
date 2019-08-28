using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Serialization;

namespace NarayanaGames.BeatTheRhythm.Modding {

    /// <summary>
    ///     A group of mods of which only one can be active at any given
    ///     time. This could be a mod for a specific arena, or an override
    ///     for a specific weapon or orb.
    /// </summary>
    [Serializable]
    public class ModGroup {
        /// <summary>
        ///     Is this mod group currently active? If not, the default data
        ///     of the current build is used. 
        /// </summary>
        public bool isGroupActive = true; 
        
        /// <summary>
        ///     Path to the currently active mod. Only applied to game if
        ///     isGroupActive is set to true. Paths must be either relative
        ///     to StreamingAssets, or absolute.
        /// </summary>
        public string pathToCurrentMod = "";
        
        /// <summary>
        ///     The key for the group, e.g. Arena-TrippyTunnels. All mods that
        ///     work with this item must use the exact same group key.
        /// </summary>
        [FormerlySerializedAs("compatibleModsKey")]
        public string groupKey = "";
        
        /// <summary>
        ///     List of paths where mods for this group can be found.
        /// </summary>
        public List<string> pathsForMods = new List<string>();

        private string basePath = string.Empty;

        public bool SetBasePath(string newBasePath) {
            this.basePath = newBasePath;
            return Directory.Exists(FullBasePath);
        }
        
        public string FullBasePath {
            get { return Path.Combine(basePath, pathToCurrentMod); }
        }
    }
}
