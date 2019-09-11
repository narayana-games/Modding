using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace NarayanaGames.BeatTheRhythm.Modding {

    public class ArenaModController : MonoBehaviour {

        [Header("Moddable Items")]
        public Material moddableSkybox = null;
        public List<Light> moddableLights = new List<Light>();
        
        [FormerlySerializedAs("moddableNoteSources")]
        public List<ModdableGameObject> moddableGameObjects = new List<ModdableGameObject>();
        
        [Header("File Location of Mod Configuration")]
        public ModFileIO.PathBase pathBase = ModFileIO.PathBase.StreamingAssets;

        [SerializeField]
        private string defaultPathForMods = "";

        public string DefaultPathForMods {
            get { return ModFileIO.GetFullPath(pathBase, defaultPathForMods); }
        }
        
        [FormerlySerializedAs("activeMod")]
        [Header("Loaded From File System")]
        public ModGroup modGroup = new ModGroup();

        public Mod mod;
        
        [FormerlySerializedAs("config")]
        public ArenaMod arenaMod;
        private ArenaMod arenaOrig = new ArenaMod();

        private Scene myScene;

        public void SetScene(Scene scene) {
            myScene = scene;
        }

        
        private static ArenaModController instance;

        public static ArenaModController Instance {
            get {
                return instance;
            }
            set {
                instance = value;
            }
        }

        public void Awake() {
            if (moddableSkybox == null) {
                moddableSkybox = new Material(Shader.Find("Skybox/Panoramic"));
            }
            
            arenaOrig.GetFrom(
                moddableGameObjects,
                moddableLights);
            
        }

        public void OnEnable() {
            instance = this;

            InitializeActiveModLoading();
        }

        public void OnDisable() {
            instance = null;
            
            activeModConfigWatcher?.Dispose();
            activeModConfigWatcher = null;
            
            arenaModLoader.DisposeWatcher();
        }

        public UnityEvent onModChanged = new UnityEvent();

        private bool initialLoad = true;
        
        private FileSystemWatcher activeModConfigWatcher;

        private bool invokeOnModChanged = false;
        private ModLoader<ModGroup> modGroupLoader = new ModLoader<ModGroup>();
        private ModLoader<Mod> modLoader = new ModLoader<Mod>();
        private ModLoader<ArenaMod> arenaModLoader = new ModLoader<ArenaMod>();

        private void InitializeActiveModLoading() {
            modGroupLoader.BasePath = DefaultPathForMods;
            modGroupLoader.onModLoaded = OnModGroupLoaded;
            modGroupLoader.LoadFile();
        }

        private void OnModGroupLoaded(ModGroup newModGroup) {
            try {
                Debug.Log("<b>[Modding]</b> ==================== Mod Group (Re-)loaded ====================");
                Debug.Log($"<b>[Modding]</b> LoadActiveModConfig: Loaded '{modGroupLoader.FullFilePath}', " 
                          + $"new mod: {newModGroup.pathToCurrentMod}, " 
                          + $"last mod: {modGroup.pathToCurrentMod}");

                string oldMod = modGroup.pathToCurrentMod;
                modGroup = newModGroup;

                if (CheckBasePath()) {
                    
                    if (modGroup.isGroupActive) {
                        if (initialLoad || modGroup.pathToCurrentMod != oldMod) {
                            initialLoad = false;
                            ModChanged();
                        } else {
                            Debug.LogWarning("<b>[Modding]</b> Current Mod was not changed!");
                        }
                    } else {
                        Debug.Log("<b>[Modding]</b> Mod is not active: Restoring Default");
                        arenaMod = arenaOrig;
                        arenaModLoader.ModChanged = true;
                    }
                } else {
                    WriteErrorToFile("ActiveModConfig", $"pathToCurrentMod '{newModGroup.FullBasePath}' does not seem to exist!");
                }
            } catch (Exception exc) {
                Debug.LogException(exc);
            }
        }

        public void WriteErrorToFile(string fileName, string errorMsg) {
            Debug.LogError($"<b>[Modding]</b> {errorMsg}");
            
            string errorFileName =
                $"{DefaultPathForMods}/{fileName}_error_{DateTime.Now:yyyy-MM-dd_HHmm}.txt";

            try {
                using (StreamWriter writer = File.CreateText(errorFileName)) {
                    writer.Write(errorMsg);
                }
            } catch { }
        }
        

        public void Update() {
            if (invokeOnModChanged) {
                invokeOnModChanged = false;
                onModChanged.Invoke();
            }

            if (arenaModLoader.ModChanged) {
                if (myScene != SceneManager.GetActiveScene()) {
                    return;
                }
                
                arenaModLoader.ModChanged = false;
                Debug.Log("<b>[Modding]</b> Applying ArenaMod changes");
                arenaMod.skybox.LoadTextures(moddableSkybox, mod.useCache);
                arenaMod.ApplyTo(
                    moddableGameObjects,
                    moddableLights,
                    arenaOrig.skybox.SkyboxMaterial
                );
            }
        }

        private void ModChanged() {
            try {
                invokeOnModChanged = true;
                arenaModLoader.DisposeWatcher();
                arenaModLoader.BasePath = modGroup.FullBasePath;
                arenaModLoader.onModLoaded = loadedMod => {
                    arenaMod = loadedMod;
                };
                arenaModLoader.LoadFile();

                modLoader.DisposeWatcher();
                modLoader.BasePath = modGroup.FullBasePath;
                modLoader.onModLoaded = loadedMod => {
                    mod = loadedMod;
                };
                modLoader.LoadFile();
            } catch (Exception exc) {
                Debug.LogException(exc);
            }
        }

        [ContextMenu("Save ActiveModConfig")]
        public void SaveActiveModConfig() {
            string path = $"{DefaultPathForMods}/ModGroup.json"; 
            ModFileIO.SaveConfig(modGroup.FullBasePath, modGroup, path, true);
        }
        
        [ContextMenu("Save Setup into Config")]
        public void SaveSetupIntoConfig() {
            mod.GetFrom(modGroup);
            arenaMod.GetFrom(
                moddableGameObjects,
                moddableLights);
        }
        
        [ContextMenu("Save Config to File")]
        public void SaveConfig() {
            CheckBasePath();
            
            ModFileIO.SaveConfig(modGroup.FullBasePath, mod, "Mod");
            ModFileIO.SaveConfig(modGroup.FullBasePath, arenaMod, "ArenaMod");
        }

        private ModFileIO modFileIO = new ModFileIO();
        
        public List<Mod> LoadMods() {
            return modFileIO.LoadMods(pathBase, modGroup);
        }

        public void ActivateMod(string pathToMod) {
            modGroup.pathToCurrentMod = pathToMod;
            initialLoad = true;
            SaveActiveModConfig();
        }

        public bool CheckBasePath() {
            return modGroup.SetBasePath(Path.IsPathRooted(modGroup.pathToCurrentMod)
                ? ""
                : DefaultPathForMods);
        }
    }
}
