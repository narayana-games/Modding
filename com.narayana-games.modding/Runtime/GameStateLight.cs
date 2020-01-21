using System;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {

    public class GameStateLight : MonoBehaviour {

        [Range(0, 8)]
        public float intensityWaiting = 1F;

        [Range(0, 8)]
        public float intensityPlaying = 0F;

        [Range(0, 8)]
        public float intensitySpecialEvent = 0F;

        public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Light myLight;

        private float startTime;
        private float fadeTime;
        private float startIntensity;
        private float endIntensity;

        public void Awake() {
            //Debug.Log($"{this.name}.GameStateLight.Awake() called", this);
            if (myLight == null) {
                myLight = GetComponent<Light>();
            }
        }

        // public void OnEnable() {
        //     Debug.Log($"{this.name}.GameStateLight.OnEnable() called", this);
        // }
        //
        // public void OnDisable() {
        //     Debug.Log($"{this.name}.GameStateLight.OnDisable() called", this);
        // }
        
        public void StartPlaying(float fadeTime) {
            StartTransition(fadeTime, myLight.intensity, intensityPlaying);
        }

        public void StartWaiting(float fadeTime) {
            StartTransition(fadeTime, myLight.intensity, intensityWaiting);
        }

        public void StartSpecialEvent(float fadeTime) {
            StartTransition(fadeTime, myLight.intensity, intensitySpecialEvent);
        }

        private void StartTransition(float fadeTime, float startIntensity, float endIntensity) {
            // Debug.Log(
            //     $"{this.name}.GameStateLight.StartTransition({fadeTime}, {startIntensity}, {endIntensity}) called,"
            //     +" isActiveAndEnabled={isActiveAndEnabled}", this);
            
            
            startTime = Time.realtimeSinceStartup;

            this.fadeTime = fadeTime;
            this.startIntensity = startIntensity;
            this.endIntensity = endIntensity;
            this.enabled = true;
            if (!isActiveAndEnabled || fadeTime < float.Epsilon) {
                myLight.intensity = endIntensity;
                myLight.enabled = myLight.intensity > 0.0001F;
                this.enabled = false;
            }
        }

        public void Update() {
            float timeFrac = (Time.realtimeSinceStartup - startTime) / fadeTime;
            if (timeFrac < 1) {
                timeFrac = transitionCurve.Evaluate(timeFrac);

                myLight.intensity = Mathf.Lerp(startIntensity, endIntensity, timeFrac);

                if (!myLight.enabled) {
                    //Debug.Log($"{this.name}.GameStateLight.Update(): ENABLED Light");
                    myLight.enabled = true;
                }
            } else {
                myLight.intensity = endIntensity;
                if (myLight.intensity < 0.0001F) {
                    // Debug.Log($"{this.name}.GameStateLight.Update(): DISABLED Light");
                    myLight.enabled = false;
                }
                this.enabled = false;
            }
        }
    }
}
