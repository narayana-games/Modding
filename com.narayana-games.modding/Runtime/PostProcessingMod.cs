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
            ComboLong = 4,
            InObstacle = 5,
            LowEnergy = 6
        }
        
        public bool modThis = true;
        
        public string activateState = "";
        public PostProcessingState normal = new PostProcessingState();
        public PostProcessingState comboX2 = new PostProcessingState();
        public PostProcessingState comboX3 = new PostProcessingState();
        public PostProcessingState comboX4 = new PostProcessingState();
        public PostProcessingState comboLong = new PostProcessingState();
        public PostProcessingState inObstacle = new PostProcessingState();
        public PostProcessingState lowEnergy = new PostProcessingState();

        private PostProcessingState[] states = null;
        public PostProcessingState[] States {
            get {
                if (lowEnergy.IsDefault) {
                    lowEnergy = inObstacle.Copy();
                }

                if (states == null) {
                    states = new[] { normal, comboX2, comboX3, comboX4, comboLong, inObstacle, lowEnergy };                    
                }
                
                return states;
            }
        }
    }

    [Serializable]
    public class PostProcessingState {
        [Header("General")]
        public float transitionTime = 0.3F;

        [Space]
        public Bloom bloom = new Bloom();

        [Serializable]
        public class Bloom {
            // see https://github.com/Unity-Technologies/PostProcessing/wiki/Bloom
            // for how all of these work
            
            public float intensity = 1F;

            public float threshold = 0.8F;

            // maybe add Clamp?
            [Range(0, 1)]
            public float softKnee = 0.5F;

            [Range(0, 5)]
            public float diffusion = 5F;
        }

        [Space]
        public ColorGrading colorGrading = new ColorGrading();
        
        [Serializable]
        public class ColorGrading {
            // see https://github.com/Unity-Technologies/PostProcessing/wiki/Color-Grading
            // for how all of these work

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

        public bool IsDefault {
            get {
                return Mathf.Abs(transitionTime - 0.3F) < float.Epsilon

                       && Mathf.Abs(bloom.intensity - 1.0F) < float.Epsilon
                       && Mathf.Abs(bloom.threshold - 0.8F) < float.Epsilon
                       && Mathf.Abs(bloom.softKnee - 0.5F) < float.Epsilon
                       && Mathf.Abs(bloom.diffusion - 5F) < float.Epsilon

                       && Mathf.Abs(colorGrading.temperature - 0F) < float.Epsilon
                       && Mathf.Abs(colorGrading.tint - 0F) < float.Epsilon
                       && Mathf.Abs(colorGrading.exposure - 0.5F) < float.Epsilon
                       // ignore colorFilter
                       && Mathf.Abs(colorGrading.hueShift - 0F) < float.Epsilon
                       && Mathf.Abs(colorGrading.saturation - 0F) < float.Epsilon
                       && Mathf.Abs(colorGrading.contrast - 0F) < float.Epsilon;
            }
        }

        public PostProcessingState Copy() {
            PostProcessingState copy = new PostProcessingState();
            copy.transitionTime = this.transitionTime;
            
            copy.bloom.intensity = this.bloom.intensity;
            copy.bloom.threshold = this.bloom.threshold;
            copy.bloom.softKnee = this.bloom.softKnee;
            copy.bloom.diffusion = this.bloom.diffusion;

            copy.colorGrading.temperature = this.colorGrading.temperature;
            copy.colorGrading.tint = this.colorGrading.tint;
            copy.colorGrading.exposure = this.colorGrading.exposure;
            copy.colorGrading.colorFilter = this.colorGrading.colorFilter;
            copy.colorGrading.hueShift = this.colorGrading.hueShift;
            copy.colorGrading.saturation = this.colorGrading.saturation;
            copy.colorGrading.contrast = this.colorGrading.contrast;
            
            return copy;
        }
    }
}
