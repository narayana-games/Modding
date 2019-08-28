using System;
using System.Collections.Generic;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {

    /// <summary>
    ///     A mod for a given arena. Currently, arena mods are only supported
    ///     for the arenas Trippy Tunnels and Zero Distraction. Many of the
    ///     arenas in Holodance / Beat the Rhythm are not really suited for
    ///     modding but we'll eventually also support building your own arenas
    ///     directly in Unity, and those can use the same modding framework
    ///     to support modding them.
    /// </summary>
    [Serializable]
    public class ArenaMod {

        public SkyboxMod skybox = new SkyboxMod();

        /// <summary>
        ///     Game object mods for this arena.
        /// </summary>
        public List<StageConfigNoteSource> moddableGameObjects = new List<StageConfigNoteSource>();
        
        /// <summary>
        ///     Lighting mods for this arena.
        /// </summary>
        public List<StageConfigLight> moddableLights = new List<StageConfigLight>();

        public void ApplyTo(List<ModdableGameObject> noteSources, List<Light> lights, Material defaultSkyboxMat) {
            skybox.ApplyTo(defaultSkyboxMat);

            for (int i=0; i < Mathf.Min(moddableGameObjects.Count, noteSources.Count); i++) {
                moddableGameObjects[i].ApplyTo(noteSources[i]);
            }
            
            for (int i=0; i < Mathf.Min(moddableLights.Count, lights.Count); i++) {
                moddableLights[i].ApplyTo(lights[i]);
            }
        }
      
        public void GetFrom(List<ModdableGameObject> noteSources, List<Light> lights) {
            skybox.GetFrom();
            
            moddableGameObjects.Clear();
            foreach (ModdableGameObject noteSource in noteSources) {
                StageConfigNoteSource noteSourceMod = new StageConfigNoteSource();
                noteSourceMod.GetFrom(noteSource);
                moddableGameObjects.Add(noteSourceMod);
            }
            
            moddableLights.Clear();
            foreach (Light light in lights) {
                StageConfigLight lightMod = new StageConfigLight();
                lightMod.GetFrom(light);
                moddableLights.Add(lightMod);
            }
        }
    }

    [Serializable]
    public class StageConfigNoteSource {
        public string name = "";
        public bool modThis = false;
        
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        
        public List<RendererConfig> moddableRenderers = new List<RendererConfig>();

        private StageConfigNoteSource original = null;

        public void ApplyTo(ModdableGameObject noteSource) {
            if (original == null) {
                original = new StageConfigNoteSource();
                original.GetFrom(noteSource);
            }

            if (modThis) {
                Transform t = noteSource.transform; 
                t.localPosition = position;
                t.localRotation = Quaternion.Euler(rotation);

                for (int i=0; i < Mathf.Min(moddableRenderers.Count, noteSource.moddableRenderers.Count); i++) {
                    moddableRenderers[i].ApplyTo(noteSource.moddableRenderers[i]);
                }
            }
        }
        
        public void GetFrom(ModdableGameObject noteSource) {
            name = noteSource.name;
            modThis = true;
            
            Transform t = noteSource.transform; 
            position = t.localPosition;
            rotation = t.localRotation.eulerAngles;
            
            moddableRenderers.Clear();
            foreach (Renderer renderer in noteSource.moddableRenderers) {
                RendererConfig rendererMod = new RendererConfig();
                rendererMod.GetFrom(renderer);
                moddableRenderers.Add(rendererMod);
            }
        }
    }

    [Serializable]
    public class RendererConfig {
        public string name = "";
        public bool enabled = true;
        public string modelOverride = "";

        private RendererConfig original = null;
        private Mesh mesh = null;

        public void ApplyTo(Renderer renderer) {
            if (original == null) {
                original = new RendererConfig();
                original.GetFrom(renderer);
            }

            renderer.enabled = enabled;
            if (mesh != null) {
                renderer.GetComponent<MeshFilter>().mesh = mesh;
            } else if (!string.IsNullOrEmpty(modelOverride)) {
                Debug.LogError($"Not loading '{modelOverride}' because loading models is not implemented, yet!");
            }
        }

        public void GetFrom(Renderer renderer) {
            name = renderer.name;
            enabled = renderer.enabled;
            mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
        }
    }


    [Serializable]
    public class StageConfigLight {
        public string name = "";
        public bool modThis = false;
        public bool isActive = true;
        
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;

        public LightType type = LightType.Directional;
        public float range = 10;
        public float spotAngle = 30;
        
        public Color color = Color.white;
        public float intensity = 0.5F;
        public LightShadows shadows = LightShadows.None;

        public bool dependentOnGameState = false;
        public float intensityWaiting = 0.5F;
        public float intensityPlaying = 0.5F;

        private StageConfigLight original = null;
        
        public void ApplyTo(Light light) {
            if (original == null) {
                original = new StageConfigLight();
                original.GetFrom(light);
            }

            if (modThis) {
                light.gameObject.SetActive(isActive);
                light.enabled = isActive;
                if (isActive) {
                    
                    Transform t = light.transform; 
                    t.localPosition = position;
                    t.localRotation = Quaternion.Euler(rotation);
                    
                    light.type = type;
                    light.range = range;
                    light.spotAngle = spotAngle;
                    light.color = color;
                    light.intensity = intensity;
                    light.shadows = shadows;

                    GameStateLight gameStateLight = light.GetComponent<GameStateLight>();
                    if (dependentOnGameState) {
                        if (gameStateLight == null) {
                            Debug.LogWarning($"Light '{light.name}' cannot depend on Game State; ignored!");
                            return;
                        }

                        gameStateLight.intensityWaiting = intensityWaiting;
                        gameStateLight.intensityPlaying = intensityPlaying;
                    } else {
                        if (gameStateLight != null) {
                            gameStateLight.enabled = true;
                        }
                    }
                }
            }
        }

        public void GetFrom(Light light) {
            name = light.name;
            modThis = true;
            isActive = light.gameObject.activeSelf;

            Transform t = light.transform; 
            position = t.localPosition;
            rotation = t.localRotation.eulerAngles;
            
            type = light.type;
            range = light.range;
            spotAngle = light.spotAngle;
            color = light.color;
            intensity = light.intensity;
            shadows = light.shadows;

            GameStateLight gameStateLight = light.GetComponent<GameStateLight>();
            dependentOnGameState = gameStateLight != null && gameStateLight.enabled;
            if (dependentOnGameState) {
                intensityWaiting = gameStateLight.intensityWaiting;
                intensityPlaying = gameStateLight.intensityPlaying;
            }
        }
    }
}
