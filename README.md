# VR Camera Framelines
Useful tool to help you record in game footage while in your headset. Created while filming devlogs for the game Windfall, A VR Sailing Experience.

Tested Version: Unity 2017.1.2

## To Get Started

1. Find the script, VRCameraFramelines.

2. Find the SteamVR camera in the Unity editor.

3. Add the script to the SteamVR camera game object.

4. Modify settings as desired.

5. Hit play and start recording.

## Version 1.1 - Whats New??

On Screen Notes - Notes can appear above, below and in the VR headset.
Camera Smoothing - Turn on camera smoothing, no more pukefest.
Custom Inspector - Easier to use customized inspector.

## Performance Note

__All Options__ is a more performance heavy option. In __All Options__ mode, a second camera is spawned at the exact position of the VR camera. This means the scene is being rendered twice. This is done because of the way Unity will render to the main displayer. So if you have heavy effects, make sure that you read through the settings and set up a new Main Display Camera.

__Performance__ is the light option, it spawns the framelines outside of what the main display can see, but it is still in the world. No second camera is generated, but you lose the ability to see a horizon in the middle of the view as well as gridlines. The rendered view is also lower quality on the screen.

## Settings

### Frame Type Setting
**ALL OPTIONS** will show in the VR headset, but not on the computer screen. This requires an extra camera, so it is less performant. The screen will be at display resolution (and so look better).
**PERFORMANCE** will show exactly what the player sees without another camera, but you lose the gridlines in the shot. The view will be lower resolution on the screen.

### Frame Line Layer
The layer which all the frameline objects will be spawned into. Make sure to use an unused layer between 8 and 31. Note, the default layers (0 - 7) can not be used and if selected will reset to the default layer of 29.

### Show Grid Lines (All Options only)
If you are familiar with the rule of thirds, these gridlines split the view into 9 different sections for helpful framing. Can turn the vertical and horizontal on and off independantly. In __Frame Only__ gridlines will never appear.

### Ratio 
What aspect ratio the view is in. Limited at the moment the standard sizes for screens.

### Blackout Outside Frame
Will add a mask to everything that the desktop view can not see to the VR headset. Useful if you want complete focus on what you are filming

### Top Center and Bottom Horizons
If you have a crooked neck (like I do) this will help you keep the view level while filming.

### Show Center Reticle (All Options only)
A reticle to show the center of the screen. In __Frame Only__, the reticle will not appear.

### Camera Smoothing (All Options only)
Smooths the camera out when you are recording. Removes a lot of the shake from your head bobbing around when recording. Lower values means a smooth move. Can be updated live.

### Top and Bottom Note
Shows little notes above and below the view in your headset. Useful if you want to remember to mention some things during your record

### Headset View Notes (All Options only)
Shows a list in the headset view. Useful if you want to go through a list of things in your recording and the Top and Bottom Notes do not cut it.

### Components to Disable On Load (All Options only)
You may need to disable components on the SteamVR camera to help with performance. For example, I had a water shader that was too costly to run twice, so I disabled it when using the __All Options__ option. Does not show in __Performance__.

### Display Camera Prefab (All Options only)
An additional camera must be spawned to hide the headset's Framelines. This means if you have flares or other things attached to the regular player camera, you need to duplicate those and add them to a Main Display Camera prefab. If left null, will spawn the default Main Display Camera.

## License
MIT

## Github Link
https://github.com/KellanHiggins/VRCameraFramelines