using System;
using System.IO;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {
    public class ModLoader<T> where T : new() {
        private FileSystemWatcher fileWatcher;

        public delegate void SetMod(T mod);

        public SetMod onModLoaded;
        
        public string BasePath { get; set; }

        private string fullFilePath;
        public string FullFilePath { get { return fullFilePath; } }
        
        public bool ModChanged { get; set; }

        public void LoadFile() {
            string fileName = typeof(T).Name;
            onModLoaded(ModFileIO.LoadConfig<T>(BasePath, fileName, out fullFilePath));
            Debug.Log($"<b>[Modding-Loader]</b> Loaded '{fileName}' from '{FullFilePath}'");
            ModChanged = true;

            if (fileWatcher == null) {
                var fi = new FileInfo(FullFilePath);
                fileWatcher =
                    new FileSystemWatcher(fi.DirectoryName, fi.Name) {
                        NotifyFilter = NotifyFilters.LastWrite
                    };
                fileWatcher.Changed += (o, args) => OnFileChanged();
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        private void OnFileChanged() {
            // this happens in a separate thread
            // => exceptions won't show up if not caught and explicitly logged
            try {
                LoadFile();
            } catch (Exception exc) {
                Debug.LogException(exc);
            }
        }

        public void DisposeWatcher() {
            fileWatcher?.Dispose();
        }
    }
}
