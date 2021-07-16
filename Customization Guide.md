#PROJECT CUSTOMIZATION GUIDE

####GRID SIZE | INSTANTIATED OBJECTS NAMES:
- Assets > Scripts > **Grid.csharp**

####OBJECTS SIZE/SPACING:
- LARGER OBJECT | SHORTER SPACING: decrease the "Pixels Per Unit" property at sprite's inspector.
- SHORTER OBJECT | LARGER SPACING: increase the "Pixels Per Unit" property at sprite's inspector.

####SWIPE CHARACTERISTICS (lerp speed, change positions, swipe angles, click detection):
- Assets > Scripts > **Obj.csharp**

####MATCHING OBJECTS:
- The code will only match objects with the same tag. Click on each object's prefab and add/check the tag set.
- Check the fucking TAGs in the objects prefabs!!

####TIME BEFORE MOVE BACK WHEN THERE'S NO MATCH
- Assets > Scripts > **Obj.csharp**
- Method CheckMoveCoRoutine()
- Change the parameter of WaitForSeconds();

####SMOOTHNESS OF THE MOVE
- Assets > Scripts > **Obj.csharp**
- Look for the two Lerp functions in the Update() and change the time parameter of it.


