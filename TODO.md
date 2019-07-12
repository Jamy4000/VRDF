## CORE Package
- [x] SetupVR Systems
- [ ] Input Systems : Done, need to remove rest of the ref for old input events
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
- [ ] Rewrite Input Systems without Right/Left differentiation, as we get the Input in all cases.
- [ ] Rewrite Input Systems with Touch and Click differentiation (instead of only BaseInputCapture component), and create seperate Systems. NOT MANDATORY, need to check if it make sense
- [ ] When CameraFade is done, destroy entity. Recreate it when event is raised.


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