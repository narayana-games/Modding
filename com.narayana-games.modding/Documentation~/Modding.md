# Modding Holodance / Beat the Rhythm

This guide applies to the VR rhythm games 
[Holodance](https://store.steampowered.com/app/422860/Holodance/) 
and 
[Beat the Rhythm](https://store.steampowered.com/app/781200/Beat_the_Rhythm_VR/)

## Getting Started

Currently, Holodance only has support for modding in the Arenas
[Trippy Tunnels](https://www.youtube.com/watch?v=dLQy7N1rClw) and *Zero Distraction*.

To get started, check out:

```<pathToSteam>\Steam\steamapps\common\Holodance\Holodance_Data\StreamingAssets\ArenaMods\TrippyTunnels\```

Any mods you put into that folder will immediately show up in the game 
(once you have entered the Arena that they were designed for).

Mods that you have downloaded from [mod.io](https://holodance.mod.io/) (inside the game), will show up in:

```C:\Users\<YourName>\AppData\LocalLow\narayana games\Holodance\modio-187\_installedMods\<someID>```

You can also add additional folders for your convenience in the file `ModGroup.json`. There is one
`ModGroup.json` for each group of mods. Currently, we only have *ArenaMods*, and each arena that
supports modding has one `ModGroup.json`. We'll eventually also have full arenas (all full arenas
will be one other mod group), avatars (also one mod group for all avatars), weapons (one group for
each mechanic), orbs (one group) and maybe color schemes.

Here is my `ModGroup.json` file of *ZeroDistraction* (located in `...\StreamingAssets\ArenaMods\ZeroDistraction\ModGroup,json`,
see above for full path to folder `StreamingAssets\`):

```
{
    "isGroupActive": true,
    "pathToCurrentMod": "C:\\Users\\LONG_PATH\\modio-187\\_installedMods\\41752_48294",
    "groupKey": "ArenaMod-ZeroDistraction",
    "groupTagCategory": "",
    "groupTag": "",
    "pathsForMods": [
        "ArenaMods/ZeroDistraction",
        "C:\\GameDev\\GitHub\\Modding\\ModExamples\\Holodance\\ArenaMods\\ZeroDistraction"
    ]
}
```

You'll notice that I have added a path to the local clone of this repository, for easy testing.
You'll also notice that `ArenaMods/ZeroDistraction` is a relative path. It's relative to:
`<pathToSteam>\Steam\steamapps\common\Holodance\Holodance_Data\StreamingAssets\`.
And you'll also notice that `pathToCurrentMod` points to a path that isn't included in `pathsForMods`.
The reason is that the paths of mod.io (and later Steam Workshop) are fixed and can never be changed,
and are therefore implicitly added during runtime instead of putting them into the configuration file.

`groupTagCategory` and `groupTag` can be empty if the `groupKey` is comprised of a 
category (`ArenaMods`) and tag (`ZeroDistraction`), separated by minus (`-`).

## Creating your first mod

Get the [Texture Kit](https://holodance.mod.io/texture-kit) mod from mod.io. This changes everything,
and also has a few textures and skyboxes for you to play with.

In the mod-folder, there are four important files:

1. `Mod.json` - this has the the meta-data for your mod
2. `ArenaMod.json` - this has generic overrides, like the skybox, lights, or specific objects
3. `TunnelMod.json` - this is specific to Trippy Tunnels and lets you override the tunnel effect
4. `preview128x96.png` - the preview image used for this mod in-game

*Texture Kit* also has two folders:

1. `Skyboxes/` - several skyboxes, some of which are already used in the game; you can use these for your own mods
2. `TunnelTextures/` - several textures that you can use for the tunnel effects; this also has the [Genetica](http://spiralgraphics.biz/genetica.htm) source files so if you have Genetica, you can alter those textures according to your likes there

Obviously, your mods should only include the assets that you actually use, but the purpose of *Texture Kit* is to give you something to play with.

### Mod.json

Here's what this looks like:

```
{
    "groupKey": "ArenaMod-TrippyTunnels",
    "modName": "Texture Kit",
    "modVersion": "1.0.0",
    "modAuthor": "jashan",
    "modImage128x96": "preview128x96.png",
    "useCache": true,
    "basedOnMod": "ShapeShift",
    "basedOnModVersion": "1.0.0"
}
```

Most of this should be pretty straightforward: Don't mess with `groupKey` or your mod won't show 
up for Trippy Tunnels. Currently, the only two valid values for `groupKey` are:

1. `ArenaMod-TrippyTunnels`: For all mods for the Arena Trippy Tunnels in Holodance
2. `ArenaMod-ZeroDistraction`: For all mods for the Arena Zero Distraction in Holodance

`modName` is how your mod shows up in the game, `modVersion` currently doesn't show up, and neither does `modAuthor`.
But eventually, they will.

`modImage128x96` is an 128x96 image, preferably png (but jpg should also work). This shows up in game.

`useCache` this is an important one for authoring: When you make changes to texture files, these won't show
up when you keep this set to `true`. So, for authoring, when your tweaking textures, `false` is preferable.
Just don't forget to set this to `true` before publishing, or your mod will cause performance hiccups each time
it is loaded.

`basedOnMod` and `basedOnVersion` are there to keep track of the mods origin. If you use TextureKit
as a template for your own mod, you'd obviously put this here:

```
    "basedOnMod": "Texture Kit",
    "basedOnModVersion": "1.0.0"
```

### ArenaMod.json

This is a little more complex. We'll take it section by section:

```
    "skybox": {
        "skyboxTexture": "Skyboxes/PitchBlack.png",
        "skyboxTintColor": {
            "r": 0.5,
            "g": 0.5,
            "b": 0.5,
            "a": 1.0
        },
        "skyboxExposure": 1.0,
        "skyboxRotation": 0.0,
        "skyboxMapping": 1.0,
        "skyboxImageType": 0.0,
        "skyboxMirrorOnBack": 0.0,
        "skybox3DLayout": 0.0
    },
```

The important thing to know is that `skyboxTexture` can be empty. Then you keep the default skybox.
If you do override the skybox, `skyboxTintColor`, `skyboxExposure` and `skyboxRotation` should
be very straightforward to understand. For the remaining ones, please consult
[Unity Skybox Panoramic Shader](https://github.com/Unity-Technologies/SkyboxPanoramicShader). In particular,
the first section of the [shader source code](https://github.com/Unity-Technologies/SkyboxPanoramicShader/blob/master/Skybox-PanoramicBeta.shader) will be helpful.

For more details, check out the [source code of ModSkybox.cs](https://github.com/narayana-games/Modding/blob/master/com.narayana-games.modding/Runtime/SkyboxMod.cs)

The section `moddableGameObjects` lets you change game objects in the scene that were prepared for
modding. In Trippy Tunnels, this is currently `FlyingNotesSource (1) - (0, 0) - 14m`, and most important
thing you can do here is enable or disable the renderers for the "screen" in the background:

```
            "moddableRenderers": [
                {
                    "name": "FlyingNotesSourceRectangle",
                    "enabled": false,
                    "modelOverride": ""
                },
                {
                    "name": "FlyingNotesSourceActive",
                    "enabled": false,
                    "modelOverride": ""
                }
            ]
```

Also important to know: `modelOverride` is not implemented, yet. So don't waste your time trying to
change anything there.

The section `moddableLights` lets you change lights that were prepared for modding.
Best way to learn how this works is having a look at 
[Arena Mod Source Code](https://github.com/narayana-games/Modding/blob/master/com.narayana-games.modding/Runtime/ArenaMod.cs),
in particular the class `StageConfigLight`, and also Unity documentation: 
[Unity Manual: The Light Inspector](https://docs.unity3d.com/Manual/class-Light.html).

What you won't find there is how `dependentaOnGameState`, `intensityWaiting` and `intensityPlaying`
work. But that's easy to explain: You may have noticed that in Retro Clubbing, the light dims once you
start playing a song. That's done by setting `dependentOnGameState` to `true` and having different values
for `intensityWaiting` and `intensityPlaying`.

### PostProcessingMod.json

Lets you change the postprocessing settings for the arena. We only use 
[Bloom](https://github.com/Unity-Technologies/PostProcessing/wiki/Bloom) 
and [Color Grading](https://github.com/Unity-Technologies/PostProcessing/wiki/Color-Grading).
Also, we only expose a subset of the actual parameters. If you'd like to change a parameter that
we haven't included, yet, [please let us know](https://discord.gg/Holodance).

Here's a version of `ArenaMods/ZeroDistraction/MaximumDistraction/`
with some parts omitted to make it a little easier to read (those values are designed to 
increase distraction, so don't consider them an artistic recommendation):

```
{
    "modThis": true,
    "activateState": "",
    "useCache": true,
    "normal": {
        "transitionTime": 0.3,
        "bloom": {
            "intensity": 4.0,
            "threshold": 0.8,
            "softKnee": 0.4,
            "diffusion": 7.2
        },
        "colorGrading": {
            "temperature": 0.0,
            "tint": 0.0,
            "exposure": 0.5,
            "colorFilter": {
                "r": 1.0,
                "g": 1.0,
                "b": 1.0,
                "a": 1.0
            },
            "hueShift": 0.0,
            "saturation": 30.0,
            "contrast": 0.0
        }
    },
    "comboX2": {
        "transitionTime": 0.3,
        "bloom": { /* omitted */ },
        "colorGrading": { /* omitted */ }
    },
    "comboX3": {
        "transitionTime": 0.3,
        /* omitted */
    },
    "comboX4": {
        "transitionTime": 0.3,
        /* omitted */
    },
    "comboLong": {
        "transitionTime": 0.3,
        /* omitted */
    }
}
```

`activateState` is for development and creating screenshots. Make sure to keep this
empty when publishing. While working on your mod, put in `normal`, `comboX2`, `comboX3`, `comboX4`, or `comboLong`
to activate these states. You may also want to use a short `transitionTime` during testing but you can make
this several seconds for the higher combos (for `comboX2`, in particular, setting this to a high value may result
in that state never being fully reached because the player only has to catch 8 more orbs to move on to `comboX3`).

### TunnelMod.json

This is a big one. As the name may make obvious, this one will only work in the Arena *Trippy Tunnels*,
and it mods the appearance of the tunnel.
I'll try to keep it short, anyways. Here's a version of `ArenaMods/TrippyTunnels/TextureKit/`
with some parts omitted to make it a little easier to read:

```
{
    "activateState": "",
    "normal": {
        "transitionTime": 0.2,
        "position": {
            "x": 0.0,
            "y": 4.8,
            "z": 0.0
        },
        "rotation": {
            "x": 0.0,
            "y": 0.0,
            "z": 0.0
        },
        "scale": {
            "x": 10.0,
            "y": 10.0,
            "z": 800.0
        },
        "segments": 32,
        "stretch": 0.0,
        "layer1": {
            "alpha": 1.0,
            "speed": 0.2,
            "rotation": 0.0,
            "twist": 0.0,
            "exposure": 0.7,
            "texture": "TunnelTextures/Organic02_Purple.png"
        },
        "layer2": {
            "alpha": 0.3,
            "speed": -0.3,
            "rotation": 0.16,
            "twist": 0.13,
            "exposure": 0.3,
            "texture": "TunnelTextures/Fractal01.png"
        },
        "layer3": {
            "alpha": 0.2,
            "speed": -0.2,
            "rotation": -0.6,
            "twist": -0.5,
            "exposure": 0.2,
            "texture": "TunnelTextures/Fractal05.png"
        }
    },
    "comboX2": {
        "transitionTime": 1.0,
        "position": { "x": 0.0, "y": 3.5, "z": 0.0 },
        "rotation": { "x": 0.0, "y": 0.0, "z": 0.0
        },
        "scale": { "x": 12.0, "y": 8.0, "z": 800.0 },
        "segments": 32,
        "stretch": 0.0,
        "layer1": {
            "alpha": 0.8,
            "speed": -0.5,
            "rotation": 1.0,
            "twist": 1.0,
            "exposure": 0.8,
            "texture": "TunnelTextures/AlwaysWatching.png"
        },
        "layer2": { /* omitted */ },
        "layer3": { /* omitted */ }
    },
    "comboX3": {
        "transitionTime": 2.0,
         /* omitted */ 
    },
    "comboX4": {
        "transitionTime": 10.0,
         /* omitted */ 
    },
    "comboLong": {
        "transitionTime": 30.0,
         /* omitted */ 
    }
}
```

See section PostProcessingMod.json for how `activateState` and `transitionTime` work.

Be very careful with changing `position` and `rotation`. This could be very disorienting for players if you
do something extreme.

`segments` can be an interesting effect if you put in a low value, like 3 (which results in a triangular tunnel),
or 4 (which results in a square tunnel). You may use `rotation.z` to rotate the tunnel to something reasonable when
it has these sharp edges (like how it's done in `comboLong` of `Texture Kit` and also the mod `Shape Shift`
that `Texture Kit` was based on).

`stretch` is best played with to figure out what it does. It, um, stretches the texture on the tunnel.

Each tunnel has three layers. You could set `alpha` to 0.0 to practically remove the layer. Be aware that
setting `texture` to an empty string will result in the default texture for that layer being loaded.
The same applies to `speed`, `rotation`, `twist` and `exposure` ... but those are actually pretty
easy to understand simply by their names (in this context, `rotation` is obviously the rotation speed,
not a fixed rotation).

#### Pro Tip

To offset a little of the discomfort that these tunnels can easily cause in VR, I usually have `speed` and
`rotation` of the different layers set in opposite directions. Together with a skybox that offers a good
reference for the eye, the tunnel should be tolerable for most people. Reducing `alpha` and/or `exposure`
can also help.

## Sharing Mods

We use mod.io so you can share your mods with other players, regardless of platform:

[Holodance on mod.io](https://holodance.mod.io/)

Eventually, we'll put an uploader right into the game but that will probably after 
we have completed the beatmap editor.
