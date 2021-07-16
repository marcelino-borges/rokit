using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{

    private GameGrid grid;
    public float hintDelay = 15;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;

    // Start is called before the first frame update
    void Start() {
        grid = FindObjectOfType<GameGrid>();
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update() {
        hintDelaySeconds -= Time.deltaTime;
        if(hintDelaySeconds <= 0 && currentHint == null) {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    /// <summary>
    /// Find all the possible matches in the board
    /// </summary>
    /// <returns></returns>
    private List<GameObject> FindAllMatches() {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < grid.rows; i++) {
            for (int j = 0; j < grid.columns; j++) {
                if (grid.allGemsInTheGrid[i, j] != null) {
                    if (i < grid.rows - 1) {
                        if (grid.SwitchAndCheck(i, j, Vector2.right)) {
                            possibleMoves.Add(grid.allGemsInTheGrid[i, j]);
                        }
                    }

                    if (j < grid.columns - 1) {
                        if (grid.SwitchAndCheck(i, j, Vector2.up)) {
                            possibleMoves.Add(grid.allGemsInTheGrid[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    /// <summary>
    /// Pick one match randomly
    /// </summary>    
    private GameObject PickOneRandomly() {
        List<GameObject> possibleMoves = new List<GameObject>();

        possibleMoves = FindAllMatches();

        if(possibleMoves.Count > 0) {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }

        return null;
    }

    /// <summary>
    /// Create the hint behind the sorted match
    /// </summary>
    private void MarkHint() {
        GameObject move = PickOneRandomly();
        if(move != null) {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Destroy the hint
    /// </summary>
    public void DestroyHint() {
       if(currentHint != null) {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }

}
