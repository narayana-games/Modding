using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {
    public class ModFileIO {
        
        public enum PathBase {
            Absolute,
            StreamingAssets,
            PersistentDataPath
        }

        public static string GetFullPath(PathBase pathBase, string path) {
            switch (pathBase) {
                case PathBase.Absolute: 
                    return path;
                case PathBase.StreamingAssets:
                    return $"{Application.streamingAssetsPath}/{path}";
                case PathBase.PersistentDataPath:
                    return $"{Application.persistentDataPath}/{path}";
            }

            return path;
        }
        
        public static void SaveConfig(string basePath, object obj, string fileName, bool fileHasFullPath = false) {
            string path = fileName;
            if (!fileHasFullPath) {
                path = Path.Combine(basePath, $"{fileName}.json");
            }

            FileInfo file = new FileInfo(path);
            if (file.Directory == null) {
                Debug.LogError($"<b>[Modding-FileIO]</b> Could not save to '{path}'");
                return;
            }

            if (!file.Directory.Exists) {
                file.Directory.Create();
            }

            string json = JsonUtility.ToJson(obj, true);
            using (StreamWriter writer = File.CreateText(file.FullName)) {
                writer.Write(json);
            }

            Debug.Log($"<b>[Modding-FileIO]</b> Saved to {file.FullName}");
        }

        public List<Mod> LoadMods(PathBase pathBase, ModGroup modGroup) {
            List<Mod> mods = new List<Mod>();
            foreach (string path in modGroup.PathsForMods) {
                string fullPath = path;
                if (!Path.IsPathRooted(path)) {
                    fullPath = GetFullPath(pathBase, fullPath);
                }
                DirectoryInfo dirInfo = new DirectoryInfo(fullPath);
                FileInfo[] modFiles = dirInfo.GetFiles("Mod.json", SearchOption.AllDirectories);
                foreach (FileInfo modFile in modFiles) {
                    Mod mod = LoadFile<Mod>(modFile.FullName);
                    if (mod.groupKey.Equals(modGroup.groupKey)) {
                        mod.PathToMod = modFile.Directory.FullName;
                        mods.Add(mod);
                    }
                }
            }

            return mods;
        }
        
        public T LoadConfig<T>(string basePath, string fileName, out string path) where T : new() {
            path = Path.Combine(basePath, $"{fileName}.json");
            return LoadFile<T>(path);
        }

        public bool LastLoadWasSuccessful { get; private set; }
        
        private T LoadFile<T>(string path) where T : new() {
            T container;
            string json = null;
            LastLoadWasSuccessful = false;
            // there is a copy of this above
            if (File.Exists(path)) {
                try {
                    using (StreamReader reader = File.OpenText(path)) {
                        json = reader.ReadToEnd();
                    }

                    container = JsonUtility.FromJson<T>(json);
                    LastLoadWasSuccessful = true;
                } catch (Exception exc) {
                    string backupFileName = $"{path}_corrupt_{DateTime.Now:yyyy-MM-dd_HHmm}.json";
                    container = new T();

                    try {
                        using (StreamWriter writer = File.CreateText(backupFileName)) {
                            writer.Write(json);
                        }

                        using (StreamWriter writer = File.CreateText($"{backupFileName}.error.txt")) {
                            writer.Write($"Reading from Json-file '{path}' failed with exception: {exc}");
                        }
                    } catch { }
                }
            } else {
                container = new T();
            }

            return container;
        }

        private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

        public static Texture2D LoadTexture(string basePath, string fileName, bool useCache, bool isSkybox = false) {
            string path = Path.Combine(basePath, fileName);
            if (useCache && textureCache.ContainsKey(path)) {
                Debug.Log($"<b>[Modding-FileIO]</b> Cache hit for {fileName}");
                return textureCache[path];
            }

            Texture2D texture = isSkybox
                ? new Texture2D(2, 2, TextureFormat.RGB24, false)
                : new Texture2D(2, 2);

            try {
                if (File.Exists(path)) {
                    byte[] fileData = File.ReadAllBytes(path);
                    texture.LoadImage(fileData);
                    if (useCache) {
                        textureCache[path] = texture;
                        Debug.Log($"<b>[Modding-FileIO]</b> Put {fileName} into cache");
                    } else {
                        Debug.Log($"<b>[Modding-FileIO]</b> Loaded {fileName} without caching");
                    }
                } else {
                    Debug.LogError($"<b>[Modding-FileIO]</b> There is no file at: '{path}'");
                }
            } catch (Exception exc) {
                Debug.LogException(exc);
            }

            return texture;
        }

    }
}
