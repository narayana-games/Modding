using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
        ///     The key for the group, e.g. ArenaMod-TrippyTunnels. All mods that
        ///     work with this item must use the exact same group key.
        /// </summary>
        [FormerlySerializedAs("compatibleModsKey")]
        public string groupKey = "";

        /// <summary>
        ///     Optional override tag category (as defined on mod.io). Usually, the
        ///     first part of groupKey will be used.
        /// </summary>
        public string groupTagCategory = "";
        public string GroupTagCategory {
            get {
                if (!string.IsNullOrEmpty(groupTagCategory) || groupKey.IndexOf("-") < 0) {
                    return groupTagCategory;
                }

                return groupKey.Substring(0, groupKey.IndexOf("-"));
            }
        }
        
        /// <summary>
        ///     Optional override tag (as defined on mod.io). Usually, the
        ///     first part of groupKey will be used.
        /// </summary>
        public string groupTag = "";
        public string GroupTag {
            get {
                if (!string.IsNullOrEmpty(groupTag) || groupKey.IndexOf("-") < 0) {
                    return groupTag;
                }

                return groupKey.Substring(groupKey.IndexOf("-") + 1);
            }
        }
        
        /// <summary>
        ///     List of paths where mods for this group can be found.
        /// </summary>
        [SerializeField]
        private List<string> pathsForMods = new List<string>();

        private List<string> pathsForMods3rdParty = new List<string>();
        public void Push3rdPartyPaths(List<string> paths) {
            pathsForMods3rdParty.Clear();
            pathsForMods3rdParty.AddRange(paths);
        }
        
        public List<string> PathsForMods {
            get {
                List<string> completeList = new List<string>(pathsForMods);
                completeList.AddRange(pathsForMods3rdParty);
                return completeList;
            }
        }
        
        
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
