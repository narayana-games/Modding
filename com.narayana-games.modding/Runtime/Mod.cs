using System;

namespace NarayanaGames.BeatTheRhythm.Modding {

    /// <summary>
    ///     This keeps the meta-data for each mod. Each mod-folder must have
    ///     one file named Mod.json which can be properly deserialized into
    ///     on instance of this class.
    /// </summary>
    [Serializable]
    public class Mod {
        /// <summary>
        ///     The key for the group, e.g. Arena-TrippyTunnels. All mods that
        ///     work with this item must use the exact same group key.
        /// </summary>
        public string groupKey = "";

        /// <summary>
        ///     Name of this mod. Must be unique within modCompatibilityKey.
        /// </summary>
        public string modName = "";

        /// <summary>
        ///     The current version of this mod.
        /// </summary>
        public string modVersion = "0.0.1";

        /// <summary>
        ///     Your name or gamer tag. Just make sure to be consistent with it.
        /// </summary>
        public string modAuthor = "";

        /// <summary>
        ///     Path to a small preview image that has 128x128. Could be png or jpg.
        /// </summary>
        public string modImage128x128 = "";

        /// <summary>
        ///     This should always be true - unless you are working on the mod and
        ///     changing files (like images/textures). Be aware that images do not
        ///     automatically reload but touching any of the files that link to
        ///     images will trigger reloading of the images as well.
        /// </summary>
        public bool useCache = true;
        
        /// <summary>
        ///     Which mod did you use to get started with this one? When you
        ///     copy a folder as template for your new mod, simply copy the
        ///     value you from modName here, and create a new name for your
        ///     new mod.
        /// </summary>
        public string basedOnMod = "";

        /// <summary>
        ///     Same as above, but this is the version of the template mod.
        /// </summary>
        public string basedOnModVersion = "";


        public void GetFrom(ModGroup group) {
            modName = group.pathToCurrentMod;
            groupKey = group.groupKey;
        }
        
    }
}
