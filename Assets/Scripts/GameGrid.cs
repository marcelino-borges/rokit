using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] //Put serializable here so Unity can know that this class can be shown in the inspector (in the public array I'm using above)
public class TileType {
    public int rowIndex, columnIndex;
    public TileKind tileKind;
}

[System.Serializable] //Put serializable here so Unity can know that this class can be shown in the inspector (in the public array I'm using above)
public class MatchType {
    public int type;
    public string color;
}

[System.Serializable]
public struct BombTypes {
    public GameObject gem1_hor_bomb;
    public GameObject gem1_sqr_bomb;
    public GameObject gem1_vert_bomb;
    public GameObject gem2_hor_bomb;
    public GameObject gem2_sqr_bomb;
    public GameObject gem2_vert_bomb;
    public GameObject gem3_hor_bomb;
    public GameObject gem3_sqr_bomb;
    public GameObject gem3_vert_bomb;
    public GameObject gem4_hor_bomb;
    public GameObject gem4_sqr_bomb;
    public GameObject gem4_vert_bomb;
    public GameObject gem5_hor_bomb;
    public GameObject gem5_sqr_bomb;
    public GameObject gem5_vert_bomb;
    public GameObject gem6_hor_bomb;
    public GameObject gem6_sqr_bomb;
    public GameObject gem6_vert_bomb;
    public GameObject colorBomb;
}

public class Mission {
    public MissionType missionType;
    public bool isCompleted;    

    public Mission(MissionType missionType) {
        this.missionType = missionType;
        isCompleted = false;
    }

    public override bool Equals(object obj) {
        if (obj == null) return false;
        Mission objAsPart = obj as Mission;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public bool Equals(Mission other) {
        if (other == null) return false;
        return (this.missionType.Equals(other.missionType));
    }

    public bool Equals(MissionType otherMissionType) {
        return (this.missionType == otherMissionType);
    }
}

public class GameGrid : MonoBehaviour {

    [Header("ASSIGN CREATING NEW LEVEL:", order = 0)]
    [Header("> GRID DIMENSIONS AND LAYOUT:", order = 1)]
    //Grid dimensions
    [Tooltip("Number of rows the grid will have.")]
    public int rows;
    [Tooltip("Number of columns the grid will have.")]
    public int columns;
    [Tooltip("What element each position in the grid will have (when not a gem).")]
    public TileType[] gridPositionsLayout;

    [Header("> GEMS, TILES, BOMBS, ITEMS:", order = 1)]
    //Tile object
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject frozenRockTilePrefab;
    public GameObject specialItemPrefab;
    //Character animated in the level
    public GameObject levelCharacter;
    //All the Dots available in the project, for sorting
    public GameObject[] gemsAvailable;
    public BombTypes bombTypes;

    [Header("REFILL GRID:")]
    //Offset to be used by the gems when falling down to the grid
    [Tooltip("Height where the filling gem will drop from")]
    public int dropInOffset;
    [Tooltip("Time to spawn the next filling gem")]
    public float refillDelayTime = 0.5f; //Default: 0.5f

    [Header("PARTICLES:")]
    [Tooltip("The big and single star")]
    public GameObject destroyEffect;
    [Tooltip("The explosion of small stars")]
    public GameObject starsExplosionEffect;
    [Tooltip("The explosion of broken Pink pieces of gem")]
    public GameObject gem1Explosion_pink;
    [Tooltip("The explosion of broken Green pieces of gem")]
    public GameObject gem2Explosion_green;
    [Tooltip("The explosion of broken Blue pieces of gem")]
    public GameObject gem3Explosion_blue;
    [Tooltip("The explosion of broken Yellow pieces of gem")]
    public GameObject gem4Explosion_yellow;
    [Tooltip("The explosion of broken Red pieces of gem")]
    public GameObject gem5Explosion_red;
    [Tooltip("The explosion of broken Purple pieces of gem")]
    public GameObject gem6Explosion_purple;
    [Tooltip("The explosion of small bright pieces of gem")]
    public GameObject gemExplosionDust;
    [Tooltip("Row bomb particle")]
    public GameObject rowBombEffect;
    [Tooltip("Column bomb particle")]
    public GameObject columnBombEffect;
    [Tooltip("Square bomb particle")]
    public GameObject squereBombEffect;
    [Tooltip("Prefab of the projectile that will be spawned in the direction of the gems to be destroyed by a color bomb")]
    public GameObject projectilePrefab;
    [Tooltip("Lightning effects particle")]
    public GameObject lightningEffect;
    [Tooltip("Color bomb fireworks effects particle")]
    public GameObject colorBombFireworksEffect;
    [Tooltip("Row bomb particle")]
    public GameObject colorBombEffect;

    [Header("OTHERS:")]
    public Transform mainCanvasTransform;
    
    [HideInInspector]
    public Match3Manager match3Manager;
    //Array to keep track of all the OBJECTS in the grid
    [HideInInspector]
    public GameObject[,] allGemsInTheGrid;
    [HideInInspector]
    public GameObject[,] allTilesInTheGrid;
    [HideInInspector]
    public GameState currentState = GameState.move;
    [HideInInspector]
    public Gem currentGem;
    [HideInInspector]
    public MatchType matchType;
    //Array to keep track of all the TILES in the grid
    [HideInInspector]
    public bool[,] blankSpaces;

    private BackgroundTile[,] breakableTiles;
    private BackgroundTile[,] frozenRockTiles;
    private FindMatches findMatches;
    private int qtyBombsMatched;
    private GameObject specialItemInstantiated;
    private bool hasSpawnedSpecialItem;

    private void Awake() {
        match3Manager = GameObject.FindWithTag("Match3Manager").GetComponent<Match3Manager>();
    }

    void Start() {
        breakableTiles = new BackgroundTile[rows, columns];
        frozenRockTiles= new BackgroundTile[rows, columns];
        qtyBombsMatched = 0;
        findMatches = this.GetComponent<FindMatches>();
        blankSpaces = new bool[rows, columns];
        allGemsInTheGrid = new GameObject[rows, columns];
        allTilesInTheGrid = new GameObject[rows, columns];
        //Calling the function to set up the grid
        SetUpGrid();
        GameManager.canMakeMoves = true;
        hasSpawnedSpecialItem = false;
        //Debug.Log("SpawnPoint original position = " + Camera.main.ScreenToWorldPoint(match3Manager.uIElements.movesLeft_UIText.transform.position));        
    }

    public void GenerateBlankSpaces() {
        for (int i = 0; i < gridPositionsLayout.Length; i++) {
            if (gridPositionsLayout[i].tileKind == TileKind.Blank) {
                blankSpaces[gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex] = true;
            }
        }
    }

    public void GenerateBreakableTiles() {
        //Look at all the tiles in the layout
        for (int i = 0; i < gridPositionsLayout.Length; i++) {
            //If the tile is a "jelly" tile
            if (gridPositionsLayout[i].tileKind == TileKind.Breakable) {
                //Incrementing the counting of breakable tiles
                match3Manager.quantityBreakable++;
                //Create a "jelly" tile at that position
                Vector2 tempPosition = new Vector2(gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                //Making all the tile objects created to be children (parented) to the GameGrid object
                tile.transform.parent = this.transform;
                //Setting a proper name to all the tiles
                tile.name = "(" + gridPositionsLayout[i].rowIndex + "," + gridPositionsLayout[i].columnIndex + ") - BreakableTile";
                breakableTiles[gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex] = tile.GetComponent<BackgroundTile>();
            }
        }
        //Show the breakable on UI as mission, if the level designer chooses to use them in the level
        if (match3Manager.quantityBreakable > 0) {
            match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().breakableTilesUI.SetActive(true);
            match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().breakableTilesCheckUI.SetActive(false);
            match3Manager.missionsInLevel.Add(new Mission(MissionType.Breakable));
        }
    }

    public void GenerateFrozenRockTiles() {
        //Look at all the tiles in the layout
        for (int i = 0; i < gridPositionsLayout.Length; i++) {
            if(allTilesInTheGrid[gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex] == null) {
                //If a tile is a "FrozenRock" tile
                if (gridPositionsLayout[i].tileKind == TileKind.FrozenRock) {
                    //Incrementing the counting of frozen rock tiles
                    match3Manager.quantityFrozenRocks++;
                    //Create a "FrozenRock" tile at that position
                    Vector2 tempPosition = new Vector2(gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex);
                    GameObject frozenRockTile = Instantiate(frozenRockTilePrefab, tempPosition, Quaternion.identity);
                    frozenRockTile.transform.parent = this.transform;
                    //Setting a proper name to all the tiles
                    frozenRockTile.name = "(" + gridPositionsLayout[i].rowIndex + "," + gridPositionsLayout[i].columnIndex + ") - FrozenRock";
                    frozenRockTiles[gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex] = frozenRockTile.GetComponent<BackgroundTile>();

                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                    //Making all the tile objects created to be children (parented) to the GameGrid object
                    backgroundTile.transform.parent = this.transform;
                    //Setting a proper name to all the tiles
                    backgroundTile.name = "(" + gridPositionsLayout[i].rowIndex + "," + gridPositionsLayout[i].columnIndex + ") - BackgroundTile";
                    allTilesInTheGrid[gridPositionsLayout[i].rowIndex, gridPositionsLayout[i].columnIndex] = backgroundTile;
                }
            }
        }
        //Show the frozen rocks on UI as mission if the level designer chooses to use them in the level
        if (match3Manager.quantityFrozenRocks > 0) {
            match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().frozenRocksUI.SetActive(true);
            match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().frozenRocksCheckUI.SetActive(false);
            match3Manager.missionsInLevel.Add(new Mission(MissionType.FrozenRock));
        }
    }

    /// <summary>
    /// Makes the first grid setup of gems
    /// </summary>
    private void SetUpGrid() {
        //Generating the blank spaces refered in this class inspector
        GenerateBlankSpaces();
        //Generating breakeable tiles (jelly)
        GenerateBreakableTiles();
        //Generating frozen rock tiles
        GenerateFrozenRockTiles();
        //Creating grid with the info about width, height and tile background passed in the public inspector
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (!blankSpaces[i, j] && !frozenRockTiles[i,j]) {
                    //If in this current position there is not a blank space, then instantiate the gem
                    Vector2 tempPosition = new Vector2(i, j + dropInOffset);

                    if (allTilesInTheGrid[i, j] == null) {
                        //Instanting the background tile                    
                        GameObject backgroundTile = Instantiate(tilePrefab, tempPosition - new Vector2(0, dropInOffset), Quaternion.identity) as GameObject;

                        //Making all the tile objects created to be children (parented) to the GameGrid object
                        backgroundTile.transform.parent = this.transform;
                        //Setting a proper name to all the tiles
                        backgroundTile.name = "(" + i + "," + j + ") - BackgroundTile";
                        allTilesInTheGrid[i, j] = backgroundTile;
                    }

                    //Sorting a dot
                    int indexSortedGem = Random.Range(0, gemsAvailable.Length);

                    //While the sorted piece is making a match, another one will be sorted, untill it finds one that doesn't generate a match
                    int iterations = 0; //Limiting the quantity of iterations inside this loop, to avoid an infinite loop
                    while (MatchesAt(i, j, gemsAvailable[indexSortedGem]) && iterations <= 100) {
                        indexSortedGem = Random.Range(0, gemsAvailable.Length);
                        iterations++;
                    }

                    iterations = 0;

                    //Instantiating the sorted gem
                    GameObject gem = Instantiate(gemsAvailable[indexSortedGem], tempPosition, Quaternion.identity);
                    gem.GetComponent<Gem>().row = j;
                    gem.GetComponent<Gem>().column = i;
                    gem.transform.parent = this.transform;
                    gem.name = "(" + i + "," + j + ") - " + gem.tag;
                    allGemsInTheGrid[i, j] = gem;
                }
            }
        }
    }

    /// <summary>
    /// Points out if an object makes an horizontal or a vertical match at an specific place in the grid
    /// </summary>
    private bool MatchesAt(int column, int row, GameObject piece) {
        if (column > 1 && row > 1) {
            if (allGemsInTheGrid[column - 1, row] != null && allGemsInTheGrid[column - 2, row] != null) {
                if (allGemsInTheGrid[column - 1, row].tag == piece.tag && allGemsInTheGrid[column - 2, row].tag == piece.tag) {
                    //if the 2 pieces at the left have the same tag, then inform true to match
                    return true;
                }
            }
            if (allGemsInTheGrid[column, row - 1] != null && allGemsInTheGrid[column, row - 2] != null) {
                if (allGemsInTheGrid[column, row - 1].tag == piece.tag && allGemsInTheGrid[column, row - 2].tag == piece.tag) {
                    //if the 2 pieces at the bottom have the same tag, then inform true to match
                    return true;
                }
            }
        } else if (column <= 1 || row <= 1) {
            //Covering the bottom-left/edge cases
            if (row > 1) {
                if (allGemsInTheGrid[column, row - 1] != null && allGemsInTheGrid[column, row - 2] != null) {
                    if (allGemsInTheGrid[column, row - 1].tag == piece.tag && allGemsInTheGrid[column, row - 2].tag == piece.tag) {
                        return true;
                    }
                }
            }
            if (column > 1) {
                if (allGemsInTheGrid[column - 1, row] != null && allGemsInTheGrid[column - 2, row] != null) {
                    if (allGemsInTheGrid[column - 1, row].tag == piece.tag && allGemsInTheGrid[column - 2, row].tag == piece.tag) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Finds out if we have 5 gems in a column or in a row
    /// </summary>
    private MatchType HowManyGemsInSingleColumnOrRow() {
        //Make a caopy of the current matches
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";

        //Cycle through all of match copy and decide if a bomb needs to be made
        for (int i = 0; i < matchCopy.Count; i++) {
            //Store this gem
            Gem thisGem = matchCopy[i].GetComponent<Gem>();

            string color = matchCopy[i].tag;

            int column = thisGem.column;
            int row = thisGem.row;
            int columnMatch = 0;
            int rowMatch = 0;

            //Cycle through the rest of the gems and compare
            for (int j = 0; j < matchCopy.Count; j++) {
                //Store the next gem
                Gem nextGem = matchCopy[j].GetComponent<Gem>();

                if (nextGem == thisGem) {
                    continue;
                }

                if (nextGem.column == thisGem.column && nextGem.tag == color) {
                    columnMatch++;
                }

                if (nextGem.row == thisGem.row && nextGem.tag == color) {
                    rowMatch++;
                }
            }
            //Return 1 if column bomb
            if (columnMatch == 4 || rowMatch == 4) {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            //Return 2 if adjacent
            else if (columnMatch == 2 && rowMatch == 2) {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            //Return 3 if column or row match
            else if (columnMatch == 3 || rowMatch == 3) {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    private void CheckToMakeNewBombs() {
        //How many gems are in findMatches currentMatches?
        if (findMatches.currentMatches.Count > 3) {
            //What type of match?
            MatchType typeOfMatch = HowManyGemsInSingleColumnOrRow();
            //If a color bomb
            if (typeOfMatch.type == 1) {
                //Is the object valid?
                if (currentGem != null && currentGem.isMatched && currentGem.tag == typeOfMatch.color) {
                    //Does the current gem existes  and is matched                                        
                    currentGem.isMatched = false;
                    currentGem.MakeColorBomb();
                } else {
                    //if it is NOT matched
                    if (currentGem.otherGem != null) {
                        //If the current gem's other gem exists
                        Gem otherGem = currentGem.otherGem.GetComponent<Gem>();
                        if (otherGem.isMatched && otherGem.tag == typeOfMatch.color) {
                            //If the other gem is matched                                
                            otherGem.isMatched = false;
                            otherGem.MakeColorBomb();
                        }
                    }
                }
            }
            //If an adjacent bomb
            else if (typeOfMatch.type == 2) {
                //Is the object valid? Is it matched?
                if (currentGem != null && currentGem.isMatched && currentGem.tag == typeOfMatch.color) {
                    currentGem.isMatched = false;
                    currentGem.MakeSquareBomb();
                    //If the current gem is NOT matched
                } else if (currentGem.otherGem != null) {
                    Gem otherGem = currentGem.otherGem.GetComponent<Gem>();
                    if (otherGem.isMatched && otherGem.tag == typeOfMatch.color) {
                        //But if the current gem's other gem is matched                            
                        otherGem.isMatched = false;
                        otherGem.MakeSquareBomb();
                    }
                }
            }
            //If row or column bomb
            else if (typeOfMatch.type == 3) {
                findMatches.CheckBombs(typeOfMatch);
            }
        }
    }

    /// <summary>
    /// Looks at every gem in the grid after matches are made and refills the grid with gems at a null array position of the grid
    /// </summary>
    private void RefillGrid() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (allGemsInTheGrid[i, j] == null && !blankSpaces[i, j] && !frozenRockTiles[i, j]) {
                    Vector2 tempPosition = new Vector2(i, j + dropInOffset);
                    //Sorts a gem to be spawned
                    int indexSortedGem = Random.Range(0, gemsAvailable.Length);

                    int maxIterations = 0;

                    while (MatchesAt(i, j, gemsAvailable[indexSortedGem]) && maxIterations < 100) {
                        maxIterations++;
                        indexSortedGem = Random.Range(0, gemsAvailable.Length);
                    }

                    if (match3Manager.spawnSpecialItem) {
                        int randomMultiplier = Random.Range(0, 20);
                        //Debug.Log("Sorted gem index: " + indexSortedGem + ". " + indexSortedGem + " * " + randomMultiplier + " = " + indexSortedGem * randomMultiplier + ". Rest = " + ((indexSortedGem * randomMultiplier) % 9));

                        //Checking if the sorted index * 100 is divided by 9 (sorting method to instantiate the special item)
                        //If it is, instantiates an special item and the grid hasn't an special item already   
                        if (((indexSortedGem * randomMultiplier) % 9) == 0 && specialItemInstantiated == null && !hasSpawnedSpecialItem) {
                            //Debug.Log("----------------------- Special item sorted!!!");
                            specialItemInstantiated = Instantiate(specialItemPrefab, tempPosition, Quaternion.identity);
                            //Saves the spawned gem to the grid's array
                            allGemsInTheGrid[i, j] = specialItemInstantiated;
                            //Parenting the new object to the grid's game object
                            specialItemInstantiated.transform.parent = this.transform;
                            //Naming it properly
                            specialItemInstantiated.name = "SpecialItem";

                            specialItemInstantiated.GetComponent<Gem>().row = j;
                            specialItemInstantiated.GetComponent<Gem>().column = i;

                            match3Manager.missionsInLevel.Add(new Mission(MissionType.SpecialItem));

                            hasSpawnedSpecialItem = true;

                            //If it isn't
                            // TODO: Simplificar esses 2 else com o SpawnGem()
                        } else {
                            SpawnGem(i, j, indexSortedGem, tempPosition);
                        }
                    } else {
                        SpawnGem(i, j, indexSortedGem, tempPosition);
                    }
                }
            }
        }
    }

    private void SpawnGem(int i, int j, int sortedIndex, Vector2 tempPosition) {
        //Spawns the sorted gem at this [i, j] position
        GameObject gem = Instantiate(gemsAvailable[sortedIndex], tempPosition, Quaternion.identity);
        //Saves the spawned gem to the grid's array
        allGemsInTheGrid[i, j] = gem;
        //Parenting the new object to the grid's game object
        gem.transform.parent = this.transform;
        gem.name = "(" + i + "," + j + ") - " + gem.tag;

        gem.GetComponent<Gem>().row = j;
        gem.GetComponent<Gem>().column = i;
    }

    /// <summary>
    /// Called by FillGridCoRoutine(), destroys the "matched" gems in the grid
    /// </summary>
    public void DestroyMatches() {
        //BOMBS
        //How many gems are in the matched gems list from the script FindMatches
        qtyBombsMatched = findMatches.currentMatches.Count;

        if (qtyBombsMatched >= 4) {
            //If player matches 4 or 7 bombs, then creates a line bomb:
            CheckToMakeNewBombs();
        }

        findMatches.currentMatches.Clear();

        //Plays match sound
        SoundManagerScript.PlaySound("match");

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                //If the gem's game object is NOT null and is not a special item, destroy it
                if (allGemsInTheGrid[i, j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCoRoutine());
    }

    /// <summary>
    /// Destroys the pieces at a given position
    /// </summary>   
    private void DestroyMatchesAt(int column, int row) {
        if (allGemsInTheGrid[column, row].GetComponent<Gem>().isMatched) {

            //Instantiates the particle system of explosion
            //GameObject particle = Instantiate(destroyEffect, allGemsInTheGrid[column, row].transform.position, Quaternion.identity);
            //Destroy(particle, refillDelay); //waits .5s to destroy the particle

            //-------------------------
            //--- ROW BOMB ------------
            //-------------------------

            //If it's a row or column bomb, spawn the proper particles
            if (allGemsInTheGrid[column, row].GetComponent<Gem>().isRowBomb) {
                if (rowBombEffect != null) {
                    SpawnLineBombAt(rowBombEffect, column, row, Quaternion.identity);
                    
                    //Vibrate device
                    #if UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif  
                } else {
                    Debug.LogError("Variable rowBombEffect not assigned!");
                }

            //-------------------------
            //--- COLUMN BOMB ---------
            //-------------------------

            } else if (allGemsInTheGrid[column, row].GetComponent<Gem>().isColumnBomb) {
                if (columnBombEffect != null) {
                    SpawnLineBombAt(columnBombEffect, column, row, Quaternion.Euler(0, 0, 90));
                    
                    //Vibrate device
                    #if UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif
                } else {
                    Debug.LogError("Variable columnBombEffect not assigned!");
                }


            //-------------------------
            //--- SQUARE BOMB ---------
            //-------------------------

            } else if (allGemsInTheGrid[column, row].GetComponent<Gem>().isSquareBomb) {
                if (squereBombEffect != null) {
                    SpawnSimpleParticlesAt(squereBombEffect, column, row, refillDelayTime);
                    SpawnSimpleParticlesAt(lightningEffect, column, row, refillDelayTime);
                    
                    //Vibrate device
                    #if UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif
                } else {
                    Debug.LogError("Variable squereBombEffect not assigned!");
                }
            }

            //-------------------------
            //--- COLOR BOMB ----------
            //-------------------------

            if (allGemsInTheGrid[column, row].GetComponent<Gem>().isColorBomb) {
                //Spawning the Dust particles (generic color - white)
                SpawnSimpleParticlesAt(colorBombEffect, column, row, refillDelayTime);
                SpawnSimpleParticlesAt(colorBombFireworksEffect, column, row, 1.5f);
                
                //Vibrate device
                #if UNITY_ANDROID
                    Handheld.Vibrate();
                #endif
            }

            //Deciding the correct coloured particle to spawn, according to the gem's tag/type
            switch (allGemsInTheGrid[column, row].tag) {
                case "Gem1":
                //If PINK GEM
                    SpawnSimpleParticlesAt(gem1Explosion_pink, column, row, refillDelayTime);                
                    break;
                case "Gem2":
                //If GREEN GEM
                    SpawnSimpleParticlesAt(gem2Explosion_green, column, row, refillDelayTime);
                    break;
                case "Gem3":
                //If BLUE GEM
                    SpawnSimpleParticlesAt(gem3Explosion_blue, column, row, refillDelayTime);
                    break;
                case "Gem4":
                //If YELLOW GEM
                    SpawnSimpleParticlesAt(gem4Explosion_yellow, column, row, refillDelayTime);
                    break;
                case "Gem5":
                //If RED GEM
                    SpawnSimpleParticlesAt(gem5Explosion_red, column, row, refillDelayTime);
                    break;
                case "Gem6":
                //If PURPLE GEM
                    SpawnSimpleParticlesAt(gem6Explosion_purple, column, row, refillDelayTime);
                    break;
            }

            //Spawning the Dust particles (generic color - white)
            SpawnSimpleParticlesAt(gemExplosionDust, column, row, refillDelayTime);

            //Spawning the stars particles (generic color - white)
            SpawnSimpleParticlesAt(starsExplosionEffect, column, row, refillDelayTime);

            DamageFrozenRock(column, row);
            DamageBreakableTile(column, row);

            //Make the background tile active (brighter)
            allTilesInTheGrid[column, row].GetComponent<BackgroundTile>().ActivateTile(refillDelayTime / 2);
            /*
            GameObject gemValueText = CreateText(allGemsInTheGrid[column, row].transform.position.x, allGemsInTheGrid[column, row].transform.position.y, match3Manager.gemBaseValue.ToString());

            if (gemValueText != null) {
                //Debug.Log("gemValueText válida");
                gemValueText = Instantiate(gemValueText, allGemsInTheGrid[column, row].transform.position, Quaternion.identity);
                gemValueText.transform.SetParent(mainCanvasTransform.transform);
                //Destroy(gemValueText, refillDelay);
            } else {
                Debug.LogError("Variable gemValueText not assigned!");
            }
            */
            //if the gem at that position is set as "matched", destroys the game object 
            //and sets its position at the array as null
            Destroy(allGemsInTheGrid[column, row]);

            allGemsInTheGrid[column, row] = null;

            //Score
            match3Manager.AddScore(match3Manager.gemBaseValue * match3Manager.streakValue);
        }
    }

    /// <summary>
    /// Spawns colored particles at an specific point and rotation in grid, to simulate a gem has broken into pieces
    /// </summary>
    private void SpawnSimpleParticlesAt(GameObject particle, int column, int row, float time) {
        if (particle != null) {
            GameObject obj = Instantiate(particle, allGemsInTheGrid[column, row].transform.position, Quaternion.identity);
            Destroy(obj, time); //waits .5s to destroy the particle
        } else {
            Debug.LogError("Variable " + AuxiliarScripts.GetVariableName(() => particle) + " not assigned!");
        }
    }
    
    /// <summary>
    /// Spawns a line bomb (row or column) at an specific point and rotation in grid
    /// </summary>
    private void SpawnLineBombAt(GameObject bomb, int column, int row, Quaternion rotation) {
        if (bomb != null) {
            //Make the background tile active (brighter)
            allTilesInTheGrid[column, row].GetComponent<BackgroundTile>().ActivateTile(refillDelayTime - .2f);
            //Spawning the row bomb effect
            GameObject lineBombParticle = Instantiate(bomb, allGemsInTheGrid[column, row].transform.position, rotation);
            //Destroying the row bomb effect
            Destroy(lineBombParticle, refillDelayTime);
            //Playing the proper sound
            SoundManagerScript.PlaySound("lineBomb");
        } else {
            Debug.LogError("Variable " + AuxiliarScripts.GetVariableName(() => bomb) + " not assigned!");
        }
    }

    public GameObject CreateText(float x, float y, string text_to_print) {
        if (mainCanvasTransform != null) {
            GameObject UItextGO = new GameObject("Text2");

            RectTransform trans = UItextGO.AddComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(x, y);

            Text text = UItextGO.AddComponent<Text>();
            text.text = text_to_print;
            text.fontSize = 25;
            text.font = Resources.Load<Font>("Fonts/Soft Marshmallow");
            text.color = new Color(255, 255, 255, 255);
            UItextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);

            return UItextGO;
        } else {
            Debug.LogError("Variable gemValueText not assigned!");
        }

        Debug.Log("CreateText() -> Variable mainCanvasTransform not assigned.");
        return null;
    }

    /// <summary>
    /// Damages/brakes a frozen rock at an specific point in grid
    /// </summary>
    private void DamageFrozenRock(int column, int row) {
        if(column > 0) {
            if(frozenRockTiles[column - 1, row]) {
                frozenRockTiles[column - 1, row].TakeDamage(1);
                //And remove it from the array
                if (frozenRockTiles[column - 1, row].hitPoints <= 0) {
                    frozenRockTiles[column - 1, row] = null;
                    if(match3Manager.quantityFrozenRocks > 0)
                        match3Manager.quantityFrozenRocks--;
                }
            }
        }
        //if (column < columns - 1) {
        if (column < rows - 1) {
            if (frozenRockTiles[column + 1, row]) {
                frozenRockTiles[column + 1, row].TakeDamage(1);
                //And remove it from the array
                if (frozenRockTiles[column + 1, row].hitPoints <= 0) {
                    frozenRockTiles[column + 1, row] = null;
                    if (match3Manager.quantityFrozenRocks > 0)
                        match3Manager.quantityFrozenRocks--;
                }
            }
        }
        if (row > 0) {
            if (frozenRockTiles[column, row - 1]) {
                frozenRockTiles[column, row - 1].TakeDamage(1);
                //And remove it from the array
                if (frozenRockTiles[column, row - 1].hitPoints <= 0) {
                    frozenRockTiles[column, row - 1] = null;
                    if (match3Manager.quantityFrozenRocks > 0)
                        match3Manager.quantityFrozenRocks--;
                }
            }
        }
        if (row < columns - 1) {
            if (frozenRockTiles[column, row + 1]) { //This index should be [column, row + 1], but it was finding index out of bounds at this line
                frozenRockTiles[column, row + 1].TakeDamage(1);
                //And remove it from the array
                if (frozenRockTiles[column, row + 1].hitPoints <= 0) {
                    frozenRockTiles[column, row + 1] = null;
                    if (match3Manager.quantityFrozenRocks > 0)
                        match3Manager.quantityFrozenRocks--;
                }
            }
        }

        //If we have no more frozen rocks
        if(match3Manager.quantityFrozenRocks <= 0) {
            //if the ui image is active
            if(match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().frozenRocksUI.activeInHierarchy) {
                //activate the check image
                match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().frozenRocksCheckUI.SetActive(true);
                //Call the animation
                match3Manager.uIElements.playerHud.GetComponent<Animator>().Play("frozen_rocks_earned");
                //If we have a list of missions initialized
                //And if we DONT have the frozen rock mission added
                if(match3Manager.missionsInLevel != null && match3Manager.missionsInLevel.Contains(new Mission(MissionType.FrozenRock))) {
                    //Add it to the list of missions
                    match3Manager.missionsInLevel.Find(x => x.missionType == MissionType.FrozenRock).isCompleted = true;
                }
            }
        }
    }

    private void DamageBreakableTile(int column, int row) {
        //Does the tile need to break?
        //If it does, give it 1 of damage
        if (breakableTiles[column, row] != null) {
            breakableTiles[column, row].TakeDamage(1);
            //And remove it from the array
            if (breakableTiles[column, row].hitPoints <= 0) {
                breakableTiles[column, row] = null;
                if (match3Manager.quantityBreakable > 0)
                    match3Manager.quantityBreakable--;

                //If we have no more breakable tiles
                if (match3Manager.quantityBreakable <= 0) {
                    //if the ui image is active
                    if (match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().breakableTilesUI.activeInHierarchy) {
                        //activate the check image
                        match3Manager.uIElements.playerHud.GetComponent<PlayerHud>().breakableTilesCheckUI.SetActive(true);
                        //Call the animation
                        match3Manager.uIElements.playerHud.GetComponent<Animator>().Play("breakable_earned");

                        //If we have a list of missions initialized
                        //And if we have a breakable mission already added to the list
                        if (match3Manager.missionsInLevel != null && match3Manager.missionsInLevel.Contains(new Mission(MissionType.Breakable))) {
                            //Find this mission and set it's status to true
                            match3Manager.missionsInLevel.Find(x => x.missionType == MissionType.Breakable).isCompleted = true;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Delayed routine called by DestroyMaches() that looks for empty positions and decreases the gem's row number
    /// </summary> 
    public IEnumerator DecreaseRowCoRoutine() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (allGemsInTheGrid[i, j] == null && !blankSpaces[i, j] && !frozenRockTiles[i, j]) {
                    //loop the space above to the top of the column
                    for (int k = j + 1; k < columns; k++) {
                        if (allGemsInTheGrid[i, k] != null) {
                            //move that gem to this empty space
                            allGemsInTheGrid[i, k].GetComponent<Gem>().row = j;
                            //sets that position to be null
                            allGemsInTheGrid[i, k] = null;
                            //breaks out of the loop
                            break;
                        }
                    }
                }
            }
        }
        yield return null; //Default: 0.5f
        StartCoroutine(FillGridCoRoutine());
    }

    /// <summary>
    /// Delayed routine that calls RefillGrid() method to spawn new gems at empty positions
    /// </summary>
    private IEnumerator FillGridCoRoutine() {
        //yield return new WaitForSeconds(refillDelayTime);
        RefillGrid();
        //yield return new WaitForSeconds(refillDelayTime);

        while (MatchesOnGrid()) {
            match3Manager.streakValue += .3f;
            DestroyMatches();
            yield break;
        }

        //findMatches.currentMatches.Clear();
        currentGem = null;
        yield return new WaitForSeconds(refillDelayTime);

        //Check if we got a deadlock in the grid - COMMENTED BECAUSE WE HAVE A POWER UP TO SHUFFLE THE GRID IN THESE CASES
        if (IsDeadLocked()) {
            Debug.Log("Deadlocked!");

            //If player gets a deadlock and has already used all his power ups
            if(match3Manager.powerUpsUsed.Count >= match3Manager.howManyPowerUps) {
                match3Manager.isGameOver = true;
                match3Manager.CallGameOverRoutines();
                currentState = GameState.wait;
            }
            ShuffleGrid();
        }
        currentState = GameState.move;
        match3Manager.streakValue = 1;
    }

    /// <summary>
    /// Returns true when a "matched" object is found and false when not found
    /// </summary>    
    private bool MatchesOnGrid() {
        findMatches.FindAllMatches();
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (allGemsInTheGrid[i, j] != null) {
                    if (allGemsInTheGrid[i, j].GetComponent<Gem>().isMatched) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void SwitchPieces(int column, int row, Vector2 direction) {
        //Take the second gem and save it in a holder
        GameObject holder = allGemsInTheGrid[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //Switching the first gem to be the second position
        allGemsInTheGrid[column + (int)direction.x, row + (int)direction.y] = allGemsInTheGrid[column, row];
        //Set the first gem to be the second gem
        allGemsInTheGrid[column, row] = holder;
    }
    /// <summary>
    /// Check for matches virtually
    /// </summary>   
    private bool CheckForMatches() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (allGemsInTheGrid[i, j] != null) {
                    //Make sure that one and two to the right are in the grid
                    if (i < rows - 2) {
                        //Check if the gems to the right and 2 to the right exist
                        if (allGemsInTheGrid[i + 1, j] != null && allGemsInTheGrid[i + 2, j] != null) {
                            if ((allGemsInTheGrid[i + 1, j].tag == allGemsInTheGrid[i, j].tag) && (allGemsInTheGrid[i + 2, j].tag == allGemsInTheGrid[i, j].tag)) {
                                return true;
                            }
                        }
                    }
                    //Make sure that one and two above are in the grid
                    if (j < columns - 2) {
                        //Check if the gems above exist
                        if (allGemsInTheGrid[i, j + 1] != null && allGemsInTheGrid[i, j + 2] != null) {
                            if (allGemsInTheGrid[i, j + 1].tag == allGemsInTheGrid[i, j].tag && allGemsInTheGrid[i, j + 2].tag == allGemsInTheGrid[i, j].tag) {
                                return true;
                            }
                        }
                    }

                }
            }
        }
        return false;
    }
    /// <summary>
    /// Switch positions virtually
    /// </summary>
    public bool SwitchAndCheck(int column, int row, Vector2 direction) {
        SwitchPieces(column, row, direction);
        if (CheckForMatches()) {
            SwitchPieces(column, row, direction);
            return true;
        }

        SwitchPieces(column, row, direction);
        return false;
    }

    /// <summary>
    /// Checks if there is a deadlock in the grid
    /// </summary>   
    private bool IsDeadLocked() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (allGemsInTheGrid[i, j] != null) {
                    if (i < rows - 1) {
                        if (SwitchAndCheck(i, j, Vector2.right)) {
                            return false;
                        }
                    }

                    if (j < columns - 1) {
                        if (SwitchAndCheck(i, j, Vector2.up)) {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Shuffles all the gems in the grid
    /// </summary>
    public void ShuffleGrid() {
        //Create a list of game objects
        List<GameObject> newGrid = new List<GameObject>();
        //Add every piece to this list
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (allGemsInTheGrid[i, j] != null) {
                    newGrid.Add(allGemsInTheGrid[i, j]);
                }
            }
        }
        //For every spot on the grid...
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                //If this spot shouldn't be blank
                if (!blankSpaces[i, j] && !frozenRockTiles[i, j]) {
                    //Pick a random number
                    int pieceToUse = Random.Range(0, newGrid.Count);

                    //While the sorted piece is making a match, another one will be sorted, untill it finds one that doesn't generate a match
                    int maxIterations = 0; //Limiting the quantity of iterations inside this loop, to avoid an infinite loop
                    while (MatchesAt(i, j, newGrid[pieceToUse]) && maxIterations <= 100) {
                        pieceToUse = Random.Range(0, newGrid.Count);
                        maxIterations++;
                    }

                    //Make a container for the piece
                    Gem piece = newGrid[pieceToUse].GetComponent<Gem>();

                    maxIterations = 0;

                    //Assign the column and row to the piece
                    piece.column = i;
                    piece.row = j;
                    //Fill in the gems array with this new piece
                    allGemsInTheGrid[i, j] = newGrid[pieceToUse];
                    //Remove it from the list
                    newGrid.Remove(newGrid[pieceToUse]);
                }
            }
        }
        //Check if it's still deadlocked
        if (IsDeadLocked()) {
            ShuffleGrid();
        }

        //Debug.Log("Grid Shuffled");
    }

    /// <summary>
    /// Destroys all the gems and background tiles instantiated in the scene
    /// </summary>
    public void DestroyAllGems() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {

                if (allGemsInTheGrid[i, j] != null) {
                    Destroy(allGemsInTheGrid[i, j]);
                }

            }
        }
    }

    /// <summary>
    /// Starts fresh the grid of gems again
    /// </summary>
    public void ResetGrid() {
        DestroyAllGems();
        SetUpGrid();
    }
    
    public void PickGem(int column, int row) {
        if (allGemsInTheGrid[column, row] != null) {
            DestroySingleGem(column, row);

            SoundManagerScript.PlaySound("pickGem");
            
            match3Manager.activePowerUp = PowerUp.None;            

            StartCoroutine(DecreaseRowCoRoutine());
        } else {
            Debug.Log("Tried to pick (power up) a null gem");
        }
    }

    public void Dynamite(int column, int row) {
        // TODO: put a destruction sound
        SoundManagerScript.PlaySound("dynamite");

        //Iterating through all the gems in a fixed column
        for (int i = 0; i < columns; i++) {
            if (allGemsInTheGrid[column, i] != null) {
                DestroySingleGem(column, i);
            }
        }
        //Iterating through all the gems in a fixed row
        for (int i = 0; i < rows; i++) {
            if (allGemsInTheGrid[i, row] != null) {
                DestroySingleGem(i, row);
            }
        }
        //After it all, clear the active power tup variable        
        match3Manager.activePowerUp = PowerUp.None;
        
        //Decreases gems row
        StartCoroutine(DecreaseRowCoRoutine());        
    }

    public void DestroySingleGem(int column, int row) {
        //Instantiates the particle system of explosion
        GameObject particle = Instantiate(destroyEffect, allGemsInTheGrid[column, row].transform.position, Quaternion.identity);
        Destroy(particle, .5f); //waits .5s to destroy the particle

        GameObject particle2 = Instantiate(starsExplosionEffect, allGemsInTheGrid[column, row].transform.position, Quaternion.identity);
        Destroy(particle2, .5f); //waits .5s to destroy the particle

        //Destroy the gem
        Destroy(allGemsInTheGrid[column, row]);

        //and sets its position at the array as null
        allGemsInTheGrid[column, row] = null;
    }

    /// <summary>
    /// Spawn projectiles when a color bomb is used, having the sent list of game objects as targets
    /// </summary>
    public IEnumerator SpawnColorBombProjectileCo(List<Vector2> gems) {
        //Spawn a projectile for each one of the gems found        
        foreach (Vector2 gemPosition in gems) {
            //Wait a frame
            yield return .4f;
            //Instantiates the projectile prefab
            GameObject projectile = Instantiate(projectilePrefab, levelCharacter.transform.position, Quaternion.identity) as GameObject;            
                        
            if (projectile != null) {
                //Set the projectile target position as the gem position
                projectile.GetComponent<ColorBombProjectile>().targetPosition = gemPosition;                
            }

            //Vibrate device
            #if UNITY_ANDROID
                Handheld.Vibrate();
            #endif
            SoundManagerScript.PlaySound("projectile");
        }

    }

    

}

