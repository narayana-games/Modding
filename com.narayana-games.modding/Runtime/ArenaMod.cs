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
        public List<ArenaModGameObject> moddableGameObjects = new List<ArenaModGameObject>();
        
        /// <summary>
        ///     Lighting mods for this arena.
        /// </summary>
        public List<ArenaModLight> moddableLights = new List<ArenaModLight>();

        public void ApplyTo(List<ModdableGameObject> gos, List<Light> lights, Material defaultSkyboxMat) {
            skybox.ApplyTo(defaultSkyboxMat);

            for (int i=0; i < Mathf.Min(moddableGameObjects.Count, gos.Count); i++) {
                moddableGameObjects[i].ApplyTo(gos[i]);
            }
            
            for (int i=0; i < Mathf.Min(moddableLights.Count, lights.Count); i++) {
                moddableLights[i].ApplyTo(lights[i]);
            }
        }
      
        public void GetFrom(List<ModdableGameObject> gos, List<Light> lights) {
            skybox.GetFrom();
            
            moddableGameObjects.Clear();
            foreach (ModdableGameObject go in gos) {
                ArenaModGameObject goMod = new ArenaModGameObject();
                moddableGameObjects.Add(goMod);
                goMod.GetFrom(go);
            }
            
            moddableLights.Clear();
            foreach (Light light in lights) {
                ArenaModLight lightMod = new ArenaModLight();
                moddableLights.Add(lightMod);
                lightMod.GetFrom(light);
            }
        }
    }

    [Serializable]
    public class ArenaModGameObject {
        public string name = "";
        public bool modThis = false;

        public bool overrideActive = false;
        
        // this does not work for all objects because some are
        // activated / deactivated by the game
        public bool isActive = true;
        
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        
        public List<ArenaModRenderer> moddableRenderers = new List<ArenaModRenderer>();

        public List<ArenaModGameObject> moddableChildren = new List<ArenaModGameObject>();
        
        private ArenaModGameObject original = null;

        public void ApplyTo(ModdableGameObject go) {
            if (original == null) {
                original = new ArenaModGameObject();
                original.GetFrom(go);
            }

            if (modThis) {
                if (overrideActive) {
                    go.gameObject.SetActive(isActive);
                }
                
                Transform t = go.transform;
                t.localPosition = position;
                t.localRotation = Quaternion.Euler(rotation);

                for (int i=0; i < Mathf.Min(moddableRenderers.Count, go.moddableRenderers.Count); i++) {
                    moddableRenderers[i].ApplyTo(go.moddableRenderers[i]);
                }
                
                for (int i=0; i < Mathf.Min(moddableChildren.Count, go.moddableChildren.Count); i++) {
                    moddableChildren[i].ApplyTo(go.moddableChildren[i]);
                }
            }
        }
        
        public void GetFrom(ModdableGameObject go) {
            name = go.name;
            modThis = true;

            isActive = go.gameObject.activeSelf;
            
            Transform t = go.transform; 
            position = t.localPosition;
            rotation = t.localRotation.eulerAngles;
            
            moddableRenderers.Clear();
            foreach (Renderer renderer in go.moddableRenderers) {
                ArenaModRenderer rendererMod = new ArenaModRenderer();
                rendererMod.GetFrom(renderer);
                moddableRenderers.Add(rendererMod);
            }

            moddableChildren.Clear();
            foreach (ModdableGameObject child in go.moddableChildren) {
                ArenaModGameObject goMod = new ArenaModGameObject();
                goMod.GetFrom(child);
                moddableChildren.Add(goMod);
            }
        }
    }

    [Serializable]
    public class ArenaModRenderer {
        public string name = "";
        public bool enabled = true;
        public string modelOverride = "";

        private ArenaModRenderer original = null;
        private Mesh mesh = null;

        public void ApplyTo(Renderer renderer) {
            if (original == null) {
                original = new ArenaModRenderer();
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
    public class ArenaModLight {
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

        private ArenaModLight original = null;
        
        public void ApplyTo(Light light) {
            if (original == null) {
                original = new ArenaModLight();
                original.GetFrom(light);
            }

            if (modThis) {
                //Debug.Log($"[Modding] setting light active: {isActive}");
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

                        gameStateLight.enabled = true;
                        gameStateLight.intensityWaiting = intensityWaiting;
                        gameStateLight.intensityPlaying = intensityPlaying;
                    } else {
                        if (gameStateLight != null) {
                            gameStateLight.enabled = false;
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
