using System;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {

    [Serializable]
    public class TunnelMod {
        public enum StateKey : int {
            Normal = 0,
            ComboX2 = 1,
            ComboX3 = 2,
            ComboX4 = 3,
            ComboLong = 4
        }
        
        public string activateState = "";
        public bool useCache = true;
        public TunnelState normal = new TunnelState();
        public TunnelState comboX2 = new TunnelState();
        public TunnelState comboX3 = new TunnelState();
        public TunnelState comboX4 = new TunnelState();
        public TunnelState comboLong = new TunnelState();

        private TunnelState[] states = null;
        public TunnelState[] States {
            get {
                if (states == null) {
                    states = new[] { normal, comboX2, comboX3, comboX4, comboLong };                    
                }

                return states;
            }
        }
        
        public void LoadTextures() {
            ModGroup activeMod = ArenaModController.Instance.modGroup;
            foreach (TunnelState state in States) {
                foreach (TunnelLayer layer in state.Layers) {
                    if (!string.IsNullOrEmpty(layer.texture)) {
                        layer.Texture =
                            ModFileIO.LoadTexture(
                                activeMod.FullBasePath,
                                layer.texture, 
                                useCache);
                    }
                }
            }
        }
    }

    [Serializable]
    public class TunnelState {
        public float transitionTime = 0.3F;
        
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;

        public int segments = 32;
        
        public float stretch = 0;

        public TunnelLayer layer1 = new TunnelLayer();
        public TunnelLayer layer2 = new TunnelLayer();
        public TunnelLayer layer3 = new TunnelLayer();
        
        private TunnelLayer[] layers = null;
        public TunnelLayer[] Layers {
            get {
                if (layers == null) {
                    layers = new[] { layer1, layer2, layer3 };                    
                }

                return layers;
            }
        }
        
    }

    [Serializable]
    public class TunnelLayer {
        public float alpha = 0.5F;
        public float speed = 0.5F;
        public float rotation = 1;
        public float twist = 1;
        public float exposure = 0.7F;
        public string texture = string.Empty;

        public Texture2D Texture { get; set; }
    }
}
