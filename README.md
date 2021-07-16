#ABOUT
---------------

#DEV GUIDE FOR CUSTOMIZATION

####BOMBS:
	**ROW/COLUM BOMBS**
	- CONDITION: Matching 4 or 7 equal gems in the same column or row
	- Verification of how many gems matched at an specific player movement: Scripts > GameGrid > DestroyMatchesAt()
	- Instantiating the bombs: Scripts > Gem > MakeRowBomb() & MakeColumnBomb()
	- Deciding whether it's a row or column bomb: Scripts > FindMatches > CheckBombs()
	**COLOR BOMBS**
	- CONDITION: Matching 5 or 8 equal gems in the same column or row
	- Scripts > Gem> CheckMoveCoRoutine(), CheckToMakeNewBombs(), HasFiveGemsInSingleColumnOrRow()
	**ADJACENT BOMBS**
	- CONDITION: Matching 5 or 8 equal gems in other shapes (T or L)
	- Scripts > Gem> CheckMoveCoRoutine(), CheckToMakeNewBombs(), HasFiveGemsInSingleColumnOrRow()
	
	
####CAMERA SETTINGS:
	- Assets > Scripts > CameraScaler or click MainCamera object and set the CameraScaler script public variables

####GAME OVER
	- Assets > Scripts > GameManager
	- Everytime it decreases the variable "movesLeft" it calls "checkGameOver()"
	- If movesLeft <= 0, then it sets the variable "isGameOver" to true
	- The script "GameOverMenu", attached to the canvas, checks every frame (method Update()) the state of GameManager's "isGameOver", so the menu can show up or not

####GRID 
	**SIZE & INSTANTIATED OBJECTS NAMES**
	- Assets > Scripts > GameGrid
	**BLANK SPACES**
	- Click on the object GameGrid in scene, go to its inspector and insert in the array **GridLayout** elements of type "Blank" at the desired positions
	**BREAKABLE TILES**
	- Assets > Scripts > GameGrid
	- You can set both the breakable tiles prefab and new elements into the "Board Layout" array, setting these new elements with the type "Breakable"

####**MAKING A NEW LEVEL**:
	- Put the GameGrid, SoundManager, GameManager, HockeyAppAndroid, AppCenter and MatchFinder prefabs in the scene, at (x,y) = (0,0);
	- Create a Canvas and put the prefabs PlayerHUD, PauseMenu and GameOverMenu;
	- Set the objects referenced publicly in the prefabs scripts;
	- Set the background's layer to "background";
	- Put the CameraScaler script in the camera object and set the "cameraOffset" public variable with the MainCamera (camera's Z position must be set -5 or bigger, so that the gems can be viewed by the camera);
	- Remember to assign all the public variables of these GameObjects in inspector;
	- After creating the new level, make sure to put it's name in the EnumCollection.cs > Levels

####**MAKING A NEW WORLD**:
	- Go to the gameObject: CanvasMenu > MainMenu > BottomPanel > Scroll-Snap > Viewport > Content.
	- Each one of these objects into the Content, using numbers as name, is a world in the scroll view.
	- To create a new world, you just need to duplicate the last object (greater number) and rename it to the correct number in the order.
	- Go the Scripts > EnumCollection and put the new world's name in the enum "Levels".
	- Click the new world's object, go to the inspector and set it's actual name in the "WorldSelection" script (if you did the last step, you'll find it's name here).
	- Then, in the same place, you'll need to assign this same world's object in it's Button component onClick() event (click on the "+", assign the object, click in the function dropdown, go to WorldSelection > LoadLevel()).
	

####MATCHING OBJECTS:
	- The code will only match objects with the same tag. Click on each object's prefab and add/check the tag set.
	- Check the fucking TAGs in the objects prefabs!!

####NEW GEMS:
	- Check TAGs
	
####NOTIFICATIONS:
	- Scripts > GameManager > OnApplicationQuit();
	- Scripts > Match3Manager > DecreaseMovesLeft().

####OBJECTS SIZE/SPACING:
	- LARGER OBJECT | SHORTER SPACING: decrease the "Pixels Per Unit" property at sprite's inspector.
	- SHORTER OBJECT | LARGER SPACING: increase the "Pixels Per Unit" property at sprite's inspector.

####PARTICLES:
	- When match destroys the gem: Scripts > GameGrid > DestroyMatchesAt()

####Reset the grid with new set of gems
	- Scripts > GameGrid > Method ResetGrid()
	
####Score system:
	- Score increase will be greater at each chainned matches made;
	- StreakValue increased at each match in Scripts > GameGrid > FillGridCoRoutine();
	- Value per matched gem in variable BasePieceValue at GameGrid script;
	- DestroyMatchAt() adding the score;
	- Match3Manager > AddScore().
	**STARS GOALS**:
		- Stars references: starsGoals_UIImage;
		- AddScore() - Checking if reached score enough to set the stars.

####SMOOTHNESS OF THE MOVE
	- Assets > Scripts > **Obj.csharp**
	- Look for the two Lerp functions in the Update() and change the time parameter of it.

####SOUNDS:
	- MATCH: Scripts > Match3Manager > DestroyMatches()
	- STAR: Scripts > Match3Manager > CheckForStar();
	
####SPECIAL ITEM:	
	- Scripts > GameGrid > RefillGrid();
	- Scripts > SpecialItem;
	- Sorted when refilling the grid;
	- The script sorts an index number to choose the gem and if this number * 100 can be divided by 9 and if the grid doesn't have one already
	
	
####STARS:
	- Check the section **Score system**.

####SWIPE CHARACTERISTICS (lerp speed, change positions, swipe angles, click detection):
	- Assets > Scripts > **Obj.csharp**

####TIMINGS
	**BEFORE MOVE BACK WHEN THERE'S NO MATCH**
	- Assets > Scripts > **Obj.csharp**
	- Method CheckMoveCoRoutine()
	- Change the parameter of WaitForSeconds()
	**RESPONSE TO MOVEMENTS (check if matched or not)**
	- Scripts > Gem > CheckMoveCoRoutine() 
	**BEFORE THE GEMS DECREASE THEIR ROWS WHEN THE BELOW ONES ARE DESTROYED
	- Scripts > GameGrid > DecreaseRowCoRoutine()
	**BEFORE THE GRID CAN INSTATIATE NEW GEMS**
	- Scripts > GameGrid > DecreaseRowCoRoutine()

####VICTORY

**OBS:** the grid is rotated 90 degress on Z

### ARTS GUIDANCE:
	- **OBS:** Always go to the texture's inspector and choose the correct Max Size to that texture.
	**Resolutions**:	
	- Gems: 512 x 512px
	- Large Images: match 900px width
	
###FIREBASE:
	- I have imported the packages for database, analytics, messaging.
	- Installed Firebase CLI to use the Firebase Functions to execute server codes to send delayed notifications with the Firebase Cloud Messaging (FCM)
	- Instalar Firebase CLI: https://firebase.google.com/docs/cli
	- Instalar SDK Admin: https://firebase.google.com/docs/admin/setup?authuser=0#add_the_sdk (pegar nome do BANCO DE DADOS no console do Firebase > Configurações do Projeto > Contas de serviço


