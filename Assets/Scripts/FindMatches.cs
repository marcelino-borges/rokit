using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour {

    private GameGrid grid;
    [HideInInspector]
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<GameGrid>();
    }

    private List<GameObject> IsAdjacentBomb(Gem gem1, Gem gem2, Gem gem3) {
        List<GameObject> currentGems = new List<GameObject>();

        if (gem1.isSquareBomb) {
            currentMatches.Union(GetAdjacentGems(gem1.column, gem1.row));
        }

        if (gem2.isSquareBomb) {
            currentMatches.Union(GetAdjacentGems(gem2.column, gem2.row));
        }

        if (gem3.isSquareBomb) {
            currentMatches.Union(GetAdjacentGems(gem3.column, gem3.row));
        }

        return currentGems;
    }

    private List<GameObject> IsRowBomb(Gem leftGem, Gem currentGem, Gem rightGem) {
        List<GameObject> currentGems = new List<GameObject>();

        if (leftGem.isRowBomb) {
            currentMatches.Union(GetRowGems(leftGem.row));
        }

        if (currentGem.isRowBomb) {
            currentMatches.Union(GetRowGems(currentGem.row));
        }

        if (rightGem.isRowBomb) {
            currentMatches.Union(GetRowGems(rightGem.row));
        }

        return currentGems;
    }

    private List<GameObject> IsColumnBomb(Gem upGem, Gem currentGem, Gem downGem) {
        List<GameObject> currentGems = new List<GameObject>();

        if (upGem.isColumnBomb) {
            currentMatches.Union(GetColumnGems(upGem.column));
        }

        if (currentGem.isColumnBomb) {
            currentMatches.Union(GetColumnGems(currentGem.column));
        }

        if (downGem.isColumnBomb) {
            currentMatches.Union(GetColumnGems(downGem.column));
        }

        return currentGems;
    }

    private void AddToListAndMatch(GameObject gem) {
        //Adds the matched gem to the list of matches
        if (!currentMatches.Contains(gem)) {
            currentMatches.Add(gem);
        }
        //Set it to "matched"
        gem.GetComponent<Gem>().isMatched = true;
        
    }

    private void GetNearbyGems(GameObject gem1, GameObject gem2, GameObject gem3) {
        AddToListAndMatch(gem1);
        AddToListAndMatch(gem2);
        AddToListAndMatch(gem3);
    }

    public void FindAllMatches() {
        StartCoroutine(FindAllMatchesCoRoutine());
    }

    private IEnumerator FindAllMatchesCoRoutine() {
        //yield return new WaitForSeconds(.1f); //OBS: Values greater than .1f don't let the gem move to matching positions
        //OLD: yield return null;

        for (int i = 0; i < grid.rows; i++) {
            for (int j = 0; j < grid.columns; j++) {

                GameObject currentGem = grid.allGemsInTheGrid[i, j];
                if (currentGem != null) {
                    Gem currentGemScript = currentGem.GetComponent<Gem>();

                    //columns
                    if (i > 0 && i < grid.rows - 1) {
                        GameObject leftGem = grid.allGemsInTheGrid[i - 1, j];
                        GameObject rightGem = grid.allGemsInTheGrid[i + 1, j];

                        if (leftGem != null && rightGem != null) {
                            Gem leftGemScript = leftGem.GetComponent<Gem>();
                            Gem rightGemScript = rightGem.GetComponent<Gem>();

                            if (leftGem != null && rightGem != null) {
                                //If the left and right gems exist, then:
                                if (leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag) {
                                    //If left and right gems have the same tag of the current gem, then:

                                    currentMatches.Union(IsRowBomb(leftGemScript, currentGemScript, rightGemScript));
                                    currentMatches.Union(IsColumnBomb(leftGemScript, currentGemScript, rightGemScript));
                                    currentMatches.Union(IsAdjacentBomb(leftGemScript, currentGemScript, rightGemScript));
                                    GetNearbyGems(leftGem, currentGem, rightGem);
                                }
                            }
                        }
                    }

                    //rows                    
                    if (j > 0 && j < (grid.columns - 1)) {
                        GameObject upGem = grid.allGemsInTheGrid[i, j + 1];
                        GameObject downGem = grid.allGemsInTheGrid[i, j - 1];

                        if (upGem != null && downGem != null) {
                            Gem upGemScript = upGem.GetComponent<Gem>();
                            Gem downGemScript = downGem.GetComponent<Gem>();

                            if (upGem != null && downGem != null) {
                                //If the up and down gems exists, then:
                                if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag) {
                                    //Checks if all the 3 gems (current, up and down) have the same tag

                                    currentMatches.Union(IsColumnBomb(upGemScript, currentGemScript, downGemScript));
                                    currentMatches.Union(IsRowBomb(upGemScript, currentGemScript, downGemScript));
                                    currentMatches.Union(IsAdjacentBomb(upGemScript, currentGemScript, downGemScript));
                                    GetNearbyGems(upGem, currentGem, downGem);
                                }
                            }
                        }
                    }                    
                }

            }//for2
        }//for1
        yield return null;
    }

    /// <summary>
    /// Sets all gems of the type passed as parameter to "matched" (used by color bomb)
    /// </summary>    
    public List<Vector2> MatchGemsOfType(string type) {
        List<Vector2> matchedGems = new List<Vector2>();

        for (int i = 0; i < grid.rows; i++) {
            for (int j = 0; j < grid.columns; j++) {
                //Check if that piece exist
                if (grid.allGemsInTheGrid[i, j] != null) {
                    if (grid.allGemsInTheGrid[i, j].tag == type) {
                        grid.allGemsInTheGrid[i, j].GetComponent<Gem>().isMatched = true;
                        GameObject gem = grid.allGemsInTheGrid[i, j];
                        matchedGems.Add(gem.transform.position);
                    }
                }
            }
        }
        return matchedGems;
    }

    /// <summary>
    /// Gets all the gems around (3x3 square)
    /// </summary>
    private List<GameObject> GetAdjacentGems(int column, int row) {
        List<GameObject> gems = new List<GameObject>();

        //Loops through the square (3x3) around the current gem
        for (int i = (column - 1); i <= (column + 1); i++) {
            for (int j = (row - 1); j <= (row + 1); j++) {
                if(i >= 0 && i < grid.rows && j >= 0 && j < grid.columns) {
                    //Protecting the code of NullPointException for to access non-existing gems outside the board (when our current gem is at the bounds of the grid)
                    if (grid.allGemsInTheGrid[i, j] != null) {
                        //Keeping the special item out of it
                        if (grid.allGemsInTheGrid[i, j].tag != "SpecialItem") {
                            gems.Add(grid.allGemsInTheGrid[i, j]);
                            grid.allGemsInTheGrid[i, j].GetComponent<Gem>().isMatched = true;
                        }
                    }
                }
            }
        }
        return gems;
    }


    /// <summary>
    /// Gets all the gems in a column
    /// </summary>
    private List<GameObject> GetColumnGems(int column) {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < grid.columns; i++) {
            //Getting rid of NullPointException's
            if (grid.allGemsInTheGrid[column, i] != null) {
            //Keeping the special item out of it
                if (grid.allGemsInTheGrid[column, i].tag != "SpecialItem") {
                    Gem gem = grid.allGemsInTheGrid[column, i].GetComponent<Gem>();
                    if (gem.isRowBomb) {
                        gems.Union(GetRowGems(i)).ToList();
                    }

                    gems.Add(grid.allGemsInTheGrid[column, i]);
                    gem.isMatched = true;
                }
            }
        }
        
        return gems;
    }
    /// <summary>
    /// Gets all the gems in a row
    /// </summary>
    private List<GameObject> GetRowGems(int row) {
        List<GameObject> gems = new List<GameObject>();
        for (int i = 0; i < grid.rows; i++) {        
            //Getting rid of NullPointException's
            if (grid.allGemsInTheGrid[i, row] != null) {
                //Keeping the special item out of it
                if (grid.allGemsInTheGrid[i, row].tag != "SpecialItem") {
                    Gem gem = grid.allGemsInTheGrid[i, row].GetComponent<Gem>();
                    if (gem.isColumnBomb) {
                        gems.Union(GetColumnGems(i)).ToList();
                    }
                    gems.Add(grid.allGemsInTheGrid[i, row]);
                    gem.isMatched = true;
                }
            }
        }
        return gems;
    }

    public void CheckBombs(MatchType matchType) {
        
        //Has the player moved something?
        if (grid.currentGem != null) {
            //Is the gem he moved matched?
            if (grid.currentGem.isMatched && grid.currentGem.tag == matchType.color) {
                //Make it unmatched
                grid.currentGem.isMatched = false;
                
                //Deciding what kind of line bomb to make
                float swipeAngle = grid.currentGem.swipeAngle;
                if ((swipeAngle > -45 && swipeAngle <= 45) || (swipeAngle <= -135 || swipeAngle > 135)) {
                    //If the move was horizontal (left or right), makes a row bomb:
                    grid.currentGem.MakeRowBomb();
                } else {
                    grid.currentGem.MakeColumnBomb();
                }
                
            } else if (grid.currentGem.otherGem != null) {
                //Is the other gem matched?
                Gem otherGem = grid.currentGem.otherGem.GetComponent<Gem>();
                if (otherGem.isMatched && otherGem.tag == matchType.color) {
                    //If the other gem is "matched"
                    otherGem.isMatched = false;
                    
                    //Deciding what kind of bomb to make
                    float swipeAngle = otherGem.swipeAngle;
                    if ((swipeAngle > -45 && swipeAngle <= 45) || (swipeAngle <= -135 || swipeAngle > 135)) {
                        //If the move was horizontal (left or right), makes a row bomb:
                        otherGem.MakeRowBomb();
                    } else {
                        otherGem.MakeColumnBomb();
                    } 
                   
                }
            }
        }        
    }

}
