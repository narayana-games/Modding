using System;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {

    [Serializable]
    public class PostProcessingMod {
        public enum StateKey : int {
            Normal = 0,
            ComboX2 = 1,
            ComboX3 = 2,
            ComboX4 = 3,
            ComboLong = 4
        }
        
        public bool modThis = true;
        
        public string activateState = "";
        public bool useCache = true;
        public PostProcessingState normal = new PostProcessingState();
        public PostProcessingState comboX2 = new PostProcessingState();
        public PostProcessingState comboX3 = new PostProcessingState();
        public PostProcessingState comboX4 = new PostProcessingState();
        public PostProcessingState comboLong = new PostProcessingState();

        private PostProcessingState[] states = null;
        public PostProcessingState[] States {
            get {
                if (states == null) {
                    states = new[] { normal, comboX2, comboX3, comboX4, comboLong };                    
                }

                return states;
            }
        }
    }

    [Serializable]
    public class PostProcessingState {
        [Header("General")]
        public float transitionTime = 0.3F;

        // see https://github.com/Unity-Technologies/PostProcessing/wiki/Bloom
        // for how all of these work

        [Header("Bloom")]
        public float intensity = 1F;
        public float threshold = 0.8F;
        // maybe add Clamp?
        [Range(0, 1)]
        public float softKnee = 0.5F;
        [Range(0, 5)]
        public float diffusion = 5F;
        
        // see https://github.com/Unity-Technologies/PostProcessing/wiki/Color-Grading
        // for how all of these work
        
        [Space]
        [Header("White Balance")]
        
        [Range(-100, 100)]
        public float temperature = 0F;
        
        [Range(-100, 100)]
        public float tint = 0F;
        
        
        [Header("Tone")]
        public float exposure = 0.5F;
        public Color colorFilter = Color.white;
        
        [Range(-100, 100)]
        public float hueShift = 0F;
     
        [Range(-100, 100)]
        public float saturation = 0F;

        [Range(-100, 100)]
        public float contrast = 0F;
        
    }

}
