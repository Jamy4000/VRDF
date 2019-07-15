## CORE Package
- [x] SetupVR Systems
- [x] Input Systems
- [x] Raycasting Systems
- [x] VRInteractions Systems
- [x] Haptic Effect
- [x] Fading Effect
- [x] LaserPointer Systems
- [x] Controllers Models Setup Systems
- [x] ButtonActionChoser Systems

### OPTIONAL
- [ ] Redo LaserPointer using mesh generation, like DrawMesh, and a simple plane (Welcome to my life).
- [ ] Redo Controllers Setup using RenderMesh SharedComponent.
- [ ] NOT MANDATORY, need to check if it make sense : Rewrite Input Systems with Touch and Click differentiation (instead of only BaseInputCapture component), and create seperate Systems. 
- [ ] When CameraFade is done, destroy entity. Recreate it when event is raised.
- [ ] Create editor script for EnumFlagsField which handle the multiple object editing. I basically need to display a property field, and check afterward by hand the answer given by the user to then redisplay the right enum.


## MOVE AROUND Package
- [ ] Fly Away Systems
- [ ] SBS Teleport Systems
- [ ] Curve Teleport Systems

### OPTIONAL
- [ ] LongRange Teleport Systems



## GAZE Package
- [ ] Core Systems
- [ ] Reticle Systems

### OPTIONAL




## UI Package
I'm not sure I'm gonna take care of that, as long as Unity didn't switch to the new version.
We're working using Events anyway, so it should be fine.