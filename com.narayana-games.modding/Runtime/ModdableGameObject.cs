using System.Collections.Generic;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Modding {

    public class ModdableGameObject : MonoBehaviour {
        [Tooltip("Renderers that could be modded")]
        public List<Renderer> moddableRenderers = new List<Renderer>();
    
        [Tooltip("Children Game Objects that could be modded")]
        public List<ModdableGameObject> moddableChildren = new List<ModdableGameObject>();
    }
}
