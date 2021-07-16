using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {    
    [Header("1. SET REFs IN INSPECTOR:", order = 0)]
    [Header("BOMB STUFF:", order = 1)] 
    public GameObject adjacentMarker;
    [Header("2. NOT SET IN INSPECTOR:", order = 2)]
    [Header("BOMB STUFF:", order = 3)]
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isColorBomb;
    public bool isSquareBomb;
    [Header("OTHER STUFF:", order = 4)]
    public int column;
    public int row;
    public bool isMatched = false;
    public float swipeAngle = 0;

    [Tooltip("The smaller the value, slower/smoother the movement")]
    public float transitionSmoothness;
    [HideInInspector]
    public GameGrid grid;
    [HideInInspector]
    public Match3Manager match3Manager;
    [HideInInspector]
    public bool isSpecialItem;
    [HideInInspector]
    public GameObject otherGem;

    private int previousColumn, previousRow;
    private float swipeResist = 1f;
    private int targetX, targetY;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;
    private Vector2 touchOrigin = -Vector2.one;
    private HintManager hintManager;
    private GameObject hintParticle = null;

    private bool canMove = true;

    void Awake() {
        match3Manager = GameObject.FindWithTag("Match3Manager").GetComponent<Match3Manager>();
    }

    void Start() {        
        grid = match3Manager.grid;
        hintManager = grid.GetComponent<HintManager>();
        transitionSmoothness = .15f;

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isSquareBomb = false;
    }

    //DEBUG ONLY
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            if (!isColorBomb && !isRowBomb && !isColumnBomb && !isSquareBomb)
                MakeRowBomb();
            if (isRowBomb)
                MakeColumnBomb();
            if (isColumnBomb)
                MakeSquareBomb();
            if (isSquareBomb)
                MakeColorBomb();
            
            //MakeSquareBomb();
            //MakeColorBomb();
            //MakeRowBomb();
            //MakeColumnBomb();
            //grid.PickGem(column, row);

        }
    }

    void Update() {
        //Mobile Input
        if (GameManager.canMakeMoves) {
            if (Input.touchCount > 0) {
                Touch touch = Input.touches[0];
                //When touch starts
                if (touch.phase == TouchPhase.Began) {
                    if (grid.currentState == GameState.move) {
                        //If there is a hint particle in the scene, destroys it when player touches the screen            
                        if (hintManager != null) {
                            hintManager.DestroyHint();
                        } 
                        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        //Destroys the gem if player has clicked the Pick powerup
                        if (match3Manager.activePowerUp == PowerUp.Pick) {
                            grid.PickGem(column, row);

                            //Adds this powerup to the list of used powerups
                            match3Manager.powerUpsUsed.Add(PowerUp.Pick);

                            //Debug.Log("Clicked the gem [" + column + "," + row + "] with power up PICK active.");
                        }
                        //Debug.Log("Clicked the gem [" + column + "," + row + "]");
                    }
                //When touch stops               
                } else if (touch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
                    if (grid.currentState == GameState.move) {
                        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if (hintParticle != null) {
                            Destroy(hintParticle);
                        }
                        CalculateAngle();
                    }
                }
            }
        }

        targetX = column;
        targetY = row;

        
        //Gem's movement logic
        if (Mathf.Abs(targetX - transform.position.x) > .1f) {
            //Moves the object towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            //transform.position = Vector2.Lerp(transform.position, tempPosition, transitionSmoothness);
            if (canMove) {
                gameObject.transform.DOMove(tempPosition, match3Manager.gemMovement_time).SetEase(match3Manager.gemMovement_CustomEase).OnComplete(OnCompleteMovement);
                canMove = false;
            }
            if (grid.allGemsInTheGrid[column, row] != this.gameObject) {
                grid.allGemsInTheGrid[column, row] = this.gameObject;
                grid.GetComponent<FindMatches>().FindAllMatches();
            }
        } else {
            //Directly sets the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1f) {
            //Moves the object towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            //transform.position = Vector2.Lerp(transform.position, tempPosition, transitionSmoothness);
            if (canMove) {
                gameObject.transform.DOMove(tempPosition, match3Manager.gemMovement_time).SetEase(match3Manager.gemMovement_CustomEase).OnComplete(OnCompleteMovement);
                canMove = false;
            }
            if (grid.allGemsInTheGrid[column, row] != this.gameObject) {
                grid.allGemsInTheGrid[column, row] = this.gameObject;
                grid.GetComponent<FindMatches>().FindAllMatches();
            }
        } else {
            //Directly sets the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
        
    }

    private void OnCompleteMovement() {
        canMove = true;
    }

    private void OnMouseDown() {
        //Debug.Log("Tag da gema clicada: " + this.gameObject.tag);
        if (hintParticle != null) {
            hintParticle = Instantiate(hintManager.hintParticle, this.transform.position, Quaternion.identity);
        }

        if (GameManager.canMakeMoves) {
            if (grid.currentState == GameState.move) {
                //If there is a hint particle in the scene, destroys it when player touches the screen            
                if (hintManager != null) {
                    hintManager.DestroyHint();
                } 

                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //Destroys the gem if player has clicked the Pick powerup
                if (match3Manager.activePowerUp == PowerUp.Pick) {
                    grid.PickGem(column, row);
                    //Adds this powerup to the list of used powerups
                    match3Manager.powerUpsUsed.Add(PowerUp.Pick);
                    //Debug.Log("Clicked the gem [" + column + "," + row + "] with power up PICK active.");
                }

                if(match3Manager.activePowerUp == PowerUp.Dynamite) {
                    grid.Dynamite(column, row);
                    //Adds this powerup to the list of used powerups
                    match3Manager.powerUpsUsed.Add(PowerUp.Dynamite);
                    //Debug.Log("Clicked the gem [" + column + "," + row + "] with power up DYNAMITE active.");
                }
            }
        }
    }

    private void OnMouseUp() {
        if (GameManager.canMakeMoves) {
            if (grid.currentState == GameState.move) {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (hintParticle != null) {
                    Destroy(hintParticle);
                }

                CalculateAngle();
            }
        }
    }

    /// <summary>
    /// Calculates the angle between the down/up click mouse events, to figure the direction of intended to the swipe
    /// </summary>
    private void CalculateAngle() {
        //Checks if the cursor's movement was enough to change (bigger than the minimum acceptable [swipeResist])
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist) {
            // The (*180/PI) is to convert the angle to the unity's angle units (0 to 180 anti-clockwise & 0 to -180 clockwise). 
            // E.g., there is not a 270 degree angle in unity
            grid.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MoveObjects();
            grid.currentGem = this;
        } else {
            grid.currentState = GameState.move;
        }
    }

    void MovePiecesActual(Vector2 direction) {
        //Plays movement sound
        SoundManagerScript.PlaySound("movement");
        otherGem = grid.allGemsInTheGrid[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (otherGem != null) {
            otherGem.GetComponent<Gem>().column += -1 * (int)direction.x;
            otherGem.GetComponent<Gem>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCoRoutine());
        } else {
            grid.currentState = GameState.move;
        }
    }

    /// <summary>
    /// Checks the angle/direction of the touch movement and swipes the position of the two gems
    /// </summary>
    private void MoveObjects() {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < grid.rows - 1) {
            //Right movement
            MovePiecesActual(Vector2.right);
        } else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0) {
            //Left movement
            MovePiecesActual(Vector2.left);
        } else if ((swipeAngle > 45 && swipeAngle <= 135) && row < grid.columns - 1) {
            //Up movement
            MovePiecesActual(Vector2.up);
        } else if (swipeAngle <= -45 && swipeAngle > -135 && row > 0) {
            //Down movement
            MovePiecesActual(Vector2.down);
        } else {
            grid.currentState = GameState.move;
        }        
    }
    
    /// <summary>
    /// Looks at the (left and right)/(up and down) gems and checks if they are the same, if so, sets both 3 as "matched"
    /// </summary>
    private void FindMatches() {
        //2x2 Matches (the reference, or start point, will be the bottom left gem
        GameObject upLeftGem = grid.allGemsInTheGrid[column, row + 1];
        GameObject upRightGem = grid.allGemsInTheGrid[column + 1, row + 1];
        GameObject bottomRightGem = grid.allGemsInTheGrid[column + 1, row];

        //if these gems exist/are valid
        if (upLeftGem != null & upRightGem != null && bottomRightGem != null) {
            if (upLeftGem.tag == this.gameObject.tag && upRightGem.tag == this.gameObject.tag && bottomRightGem.tag == this.gameObject.tag) {
                upLeftGem.GetComponent<Gem>().isMatched = true;
                upRightGem.GetComponent<Gem>().isMatched = true;
                bottomRightGem.GetComponent<Gem>().isMatched = true;
            }
        }
        if (row < (grid.columns - 1)) {
        }

        //Horizontal Matches
        if (column > 0 && column < grid.rows - 1) {
            //Get the objects in both sides in the horizontal
            GameObject leftGem = grid.allGemsInTheGrid[column - 1, row];
            GameObject rightGem = grid.allGemsInTheGrid[column + 1, row];
            
            if (leftGem != null && rightGem != null) {
                //If we have objects in both sides
                if ((leftGem.tag == this.gameObject.tag) && (rightGem.tag == this.gameObject.tag)) {
                    //if both objects have the same tag, then all 3 pieces set themselves as "matched"
                    leftGem.GetComponent<Gem>().isMatched = true;
                    rightGem.GetComponent<Gem>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        //Vertical Matches
        if (row > 0 && row < grid.columns - 1) {
            //Get the objects in both sides in the vertical
            GameObject upGem = grid.allGemsInTheGrid[column, row + 1];
            GameObject downGem = grid.allGemsInTheGrid[column, row - 1];

            if (upGem != null && downGem != null) {
                //If we have objects in both sides
                if ((upGem.tag == this.gameObject.tag) && (downGem.tag == this.gameObject.tag)) {
                    //if both objects have the same tag, then all 3 pieces set themselves as "matched"
                    upGem.GetComponent<Gem>().isMatched = true;
                    downGem.GetComponent<Gem>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    } 

    private void SpawnColorBombProjectile() {

    }

    /// <summary>
    /// Called by MoveObjects(), checks if the movements doesn't cause a match, move the pieces back to their places    
    /// </summary>    
    public IEnumerator CheckMoveCoRoutine() {
        //Color bomb's matching logic
        //This gem is a color bomb and the other one is the color to destroy
        if (isColorBomb) {
            if (grid.colorBombEffect != null) {
                //Instantiates the prefab of color bomb
                GameObject colorBombParticle = Instantiate(grid.colorBombEffect, otherGem.transform.position, Quaternion.identity);
                Destroy(colorBombParticle, grid.refillDelayTime); //waits .5s to destroy the particle
            } else {
                Debug.LogError("Variable colorBombEffect not assigned!");
            }

            List<Vector2> gemsMatchedByBomb = grid.GetComponent<FindMatches>().MatchGemsOfType(otherGem.tag);

            if (gemsMatchedByBomb != null) {
                grid.StartCoroutine(grid.SpawnColorBombProjectileCo(gemsMatchedByBomb));
                isMatched = true;  
            }

            SoundManagerScript.PlaySound("normalBomb");

        //The other gem is a color bomb and this piece has the color to destroy
        } else if (otherGem.GetComponent<Gem>().isColorBomb) {
            if (grid.colorBombEffect != null) {
                //Instantiates the prefab of color bomb
                GameObject colorBombParticle = Instantiate(grid.colorBombEffect, otherGem.transform.position, Quaternion.identity);
                Destroy(colorBombParticle, grid.refillDelayTime - 0.2f); //waits .5s to destroy the particle
            } else {
                Debug.LogError("Variable colorBombEffect not assigned!");
            }

            List<Vector2> gemsMatchedByBomb = grid.GetComponent<FindMatches>().MatchGemsOfType(this.gameObject.tag);
            
            if (gemsMatchedByBomb != null) {
                grid.StartCoroutine(grid.SpawnColorBombProjectileCo(gemsMatchedByBomb));
                isMatched = true;
                otherGem.GetComponent<Gem>().isMatched = true;
            }            

            SoundManagerScript.PlaySound("normalBomb");
        }

        yield return new WaitForSeconds(0.2f);//Default: 0.5f | Time before the game can responde to the gems' new positions || Mininum: 0.15f

        if (otherGem != null) {
            if(!isMatched && !otherGem.GetComponent<Gem>().isMatched) {
                //If both pieces are NOT matched, set their row/col info back to what it was before the last movement attempt
                otherGem.GetComponent<Gem>().row = row;
                otherGem.GetComponent<Gem>().column = column;
                row = previousRow;
                column = previousColumn;
                //Plays an error sound 
                SoundManagerScript.PlaySound("noMatch");
                yield return new WaitForSeconds(match3Manager.movementCooldownTime); //Time before player can move a gem again
                grid.currentGem = null;
                grid.currentState = GameState.move;
            } else {
                //If both are matched, destroy what is matched in the grid
                grid.DestroyMatches();
                match3Manager.DecreaseMovesLeft();
            }

            otherGem = null;
        }
    }

    /// <summary>
    /// Makes a bomb which can destroy all the gems at the same row
    /// </summary>
    public void MakeRowBomb() {
        if (!isColumnBomb && !isColorBomb && !isSquareBomb) {
            isRowBomb = true;

            GameObject bomb = null;

            //Setting the apropriate bomb prefab, according to the type of the gem
            switch(gameObject.tag) {
                case "Gem1":
                    bomb = grid.bombTypes.gem1_hor_bomb;
                    break;
                case "Gem2":
                    bomb = grid.bombTypes.gem2_hor_bomb;
                    break;
                case "Gem3":
                    bomb = grid.bombTypes.gem3_hor_bomb;
                    break;
                case "Gem4":
                    bomb = grid.bombTypes.gem4_hor_bomb;
                    break;
                case "Gem5":
                    bomb = grid.bombTypes.gem5_hor_bomb;
                    break;
                case "Gem6":
                    bomb = grid.bombTypes.gem6_hor_bomb;
                    break;
            }

            if(bomb != null) {
                GameObject horizontalBomb = Instantiate(bomb, transform.position, Quaternion.identity);
                horizontalBomb.transform.parent = this.transform;
                //Debug.Log("Row bomb instantiated");
            } else {
                Debug.LogError("Variable bomb not assigned!");
            }
        }
    }

    /// <summary>
    /// Makes a bomb which can destroy all the gems at the same column
    /// </summary>
    public void MakeColumnBomb() {
        if (!isRowBomb && !isColorBomb && !isSquareBomb) {
            isColumnBomb = true;

            GameObject bomb = null;

            //Setting the apropriate bomb prefab, according to the type of the gem
            switch (gameObject.tag) {
                case "Gem1":
                bomb = grid.bombTypes.gem1_vert_bomb;
                break;
                case "Gem2":
                bomb = grid.bombTypes.gem2_vert_bomb;
                break;
                case "Gem3":
                bomb = grid.bombTypes.gem3_vert_bomb;
                break;
                case "Gem4":
                bomb = grid.bombTypes.gem4_vert_bomb;
                break;                     
                case "Gem5":               
                bomb = grid.bombTypes.gem5_vert_bomb;
                break;                     
                case "Gem6":               
                bomb = grid.bombTypes.gem6_vert_bomb;
                break;
            }

            //Debug.Log("Bomb created from " + this.gameObject.tag);

            if (bomb != null) {
                GameObject horizontalBomb = Instantiate(bomb, transform.position, Quaternion.identity);
                horizontalBomb.transform.parent = this.transform;
                //Debug.Log("Column bomb instantiated");
            } else {
                //Debug.Log("Gem > MakeColumnBomb() > bomb not assigned (null).");
            }
        }
    }

    /// <summary>
    /// Makes a bomb which can destroy all the gems of the same type at the grid
    /// </summary>
    public void MakeColorBomb() {
        if (!isRowBomb && !isColumnBomb && !isSquareBomb) {
            //Set the flag to say that this is a color bomb
            isColorBomb = true;
            //Saves the reference to the color bomb object
            GameObject colorBomb = grid.bombTypes.colorBomb;
            //Checks if it's valid
            if (colorBomb != null) {
                //Instantiates the color bomb from the saved reference 
                GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
                //Parenting the color bomb to the original gem's transform
                color.transform.parent = this.transform;
                //Setting the color bomb with the correct tag
                this.gameObject.tag = "ColorBomb";
                this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
            } else {
                Debug.Log("Gem > MakeColorBomb() > colorBomb not assigned (null).");
            }
        }
    }

    /// <summary>
    /// Makes a bomb which can destroy a 3x3 square of gems
    /// </summary>
    public void MakeSquareBomb() {
        if (!isRowBomb && !isColumnBomb && !isColorBomb && !isSquareBomb) {
            //Set the flag to say that this is a square bomb
            isSquareBomb = true;

            //Creating the placeholder to the color bomb object
            GameObject sqrBomb = null;

            //Setting the apropriate bomb prefab, according to the type of the gem
            switch (gameObject.tag) {
                case "Gem1":
                sqrBomb = grid.bombTypes.gem1_sqr_bomb;
                break;
                case "Gem2":
                sqrBomb = grid.bombTypes.gem2_sqr_bomb;
                break;
                case "Gem3":
                sqrBomb = grid.bombTypes.gem3_sqr_bomb;
                break;
                case "Gem4":
                sqrBomb = grid.bombTypes.gem4_sqr_bomb;
                break;
                case "Gem5":
                sqrBomb = grid.bombTypes.gem5_sqr_bomb;
                break;
                case "Gem6":
                sqrBomb = grid.bombTypes.gem6_sqr_bomb;
                break;
            }

            if (sqrBomb != null) {
                //Instantiates the square bomb from the saved reference 
                GameObject squareBomb = Instantiate(sqrBomb, transform.position, Quaternion.identity);
                //Parenting the color bomb to the original gem's transform
                squareBomb.transform.parent = this.transform;
                Debug.Log("Square bomb instantiated");
            } else {
                Debug.Log("Gem > MakeSquareBomb() > sqrBomb not assigned (null).");
            }
        }
    }
}
