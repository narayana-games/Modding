using System;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {
    [Serializable]
    public class SkyboxMod {
        /// <summary>
        ///     Path to a skybox texture that works with Unity's built in
        ///     Skybox/Panorama shader. Can be png or jpg.
        /// </summary>
        public string skyboxTexture = "";
        
        /// <summary>For internal use.</summary>
        public Material SkyboxMaterial { get; set; }
        
        public Color skyboxTintColor = Color.gray;
        
        // 0-8
        public float skyboxExposure = 1F;
        
        // 0-360
        public float skyboxRotation = 0F;
        
        // 0: 6 Frames Layout | 1: Latitude Longitude Layout
        public float skyboxMapping = 1F;
        
        // !!! Only with Latitude Longitude Layout !!!
        // 0: 360 degrees | 1: 180 degrees
        public float skyboxImageType = 0F;
        
        // 0: no | 1: yes
        public float skyboxMirrorOnBack = 0F;
        
        // 0: None | 1: Side by Side | 2: Over Under
        public float skybox3DLayout = 0F;

        private bool isOriginalSkybox = false;
        
        public void ApplyTo(Material defaultSkyboxMat) {
            if (SkyboxMaterial != null) {
                ApplySkybox();
            } else {
                RenderSettings.skybox = defaultSkyboxMat;
                Debug.Log($"Restored default Skybox: {defaultSkyboxMat}");
            }
            DynamicGI.UpdateEnvironment();
        }
        
        private void ApplySkybox() {
            if (!isOriginalSkybox) {
                SkyboxMaterial.SetColor("_Tint", skyboxTintColor);
                SkyboxMaterial.SetFloat("_Exposure", skyboxExposure);
                SkyboxMaterial.SetFloat("_Rotation", skyboxRotation);
                SkyboxMaterial.SetFloat("_Mapping", skyboxMapping);
                SkyboxMaterial.SetFloat("_ImageType", skyboxImageType);
                SkyboxMaterial.SetFloat("_Layout", skybox3DLayout);
            }

            RenderSettings.skybox = SkyboxMaterial;
            Debug.Log($"Updated skybox to: {SkyboxMaterial}");
        }

        public void GetFrom() {
            skyboxTexture = string.Empty; // that's the default ;-)
            SkyboxMaterial = RenderSettings.skybox;
            isOriginalSkybox = true;
        }
        
        public void LoadTextures(Material moddableSkybox, bool useCache) {
            ModGroup activeMod = ArenaModController.Instance.modGroup;
            if (!string.IsNullOrEmpty(skyboxTexture)) {
                Texture2D skyboxTexture2D 
                    = ModFileIO.LoadTexture(activeMod.FullBasePath, skyboxTexture, useCache, true);
                
                SkyboxMaterial = new Material(moddableSkybox);
                SkyboxMaterial.SetTexture("_MainTex", skyboxTexture2D);
                SkyboxMaterial.name = skyboxTexture;
            }
        }
        
    }
}
