# VR Camera Framelines
Useful tool to help you record in game footage while in your headset

Tested Version: Unity 5.4

## To Get Started

1. Find the script, VRCameraFramelines.

2. Find the SteamVR camera in the Unity editor.

3. Add the script to the SteamVR camera game object.

4. Modify settings as desired.

5. Hit play and start recording.

## Performance Note

__Gridlines__ is a more performance heavy option. In __Gridlines__ mode, a second camera is spawned at the exact position of the VR camera. This means the scene is being rendered twice. This is done because of the way Unity will render to the main displayer. So if you have heavy affects, make sure that you read through the settings and set up a new Main Display Camera.

__Frame Only__ is the performance light option, it spawns the framelines outside of what the main display can see, but it is still in the world. No second camera is generated, but you lose the ability to see a horizon in the middle of the view as well as gridlines.


## Settings

### Frame Type Setting
**GRIDLINES** will show in the VR headset, but not on the computer screen. This requires an extra camera, so it is less performant.
**FRAME ONLY** will show exactly what the player sees without another camera, but you lose the gridlines in the shot

### Frame Line Layer
The layer which all the frameline objects will be spawned into. Make sure to use an unused layer between 1 and 31. Note, the default layer can not be used as this makes zero sense.

### Ratio 
What aspect ratio the view is in. Limited at the moment the standard sizes

### Show Grid Lines
If you are familiar with the rule of thirds, these gridlines split the view into 9 different sections for helpful framing. Can turn the vertical and horizontal on and off independantly. In __Frame Only__ gridlines will never appear.

### Top Center and Bottom Horizons
If you have a crooked neck (like I do) this will help you keep the view level while filming. In __Frame Only__, the center horizon will never appear

### Show Center Reticle
A reticle to show the center of the screen. In __Frame Only__, the reticle will not appear.

### Blackout Outside Frame
Will add a mask to everything that the desktop view can not see to the VR headset. Useful if you want complete focus on what you are filming

### Components to Disable On Load
If you are in __Gridelines__ mode, you may need to disable components on the SteamVR camera to help with performance. For example, I had a water shader that was too costly to run twice, so I disabled it when using the __Gridelines__ option. This does nothing in __Frame Only__.

## Prefabs
### Display Camera Prefab
When in __Gridelines__ mode, an additional camera must be spawned to hide the headset's Framelines. This means if you have flares or other things attached to the regular player camera, you need to duplicate those and add them to a Main Display Camera prefab. If left null, will spawn the default Main Display Camera.

## License
MIT

## Github Link
https://github.com/KellanHiggins/VRCameraFramelines

