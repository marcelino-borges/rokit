using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour {
    [Header("ASSIGN:")]
    public Transform cameraOffset;

    [Header("DON'T TOUCH UNLESS NECESSARY:")]
    private float aspectRatio;
    private const float ASPECT_RATIO_18x9 = 0.5625f;
    private const float ASPECT_RATIO_16x9 = 0.5f;
    private float padding;
    private const float PADDING_18x9 = 1.25f;
    private const float PADDING_16x9 = .1f;
    private float YOffset = -1;
    private float XOffset = 0;

    private GameGrid grid;

    // Start is called before the first frame update
    void Start() {
        grid = GameObject.FindWithTag("Grid").GetComponent<GameGrid>();
        if(grid != null) {
            //Handling 16:9 and 18:9 aspect ratios
            if(ScreenRate() > 0.5f) {
                //16x9
                padding = PADDING_16x9;
                aspectRatio = ASPECT_RATIO_16x9;
            } else {
                //18x9
                padding = PADDING_18x9;
                aspectRatio = ASPECT_RATIO_18x9;
            }
            //Debug.Log("Padding: " + padding + " | Aspect Ratio: " + aspectRatio);
            RepositionCamera(grid.rows - 1, grid.columns - 1);
        }
    }

    private float ScreenRate() {
        //Debug.Log("Screen.Width: " + (float)Screen.width + " | Screen.Height: " + (float)Screen.height);
        //Debug.Log("ScreenRate: " + (float)Screen.width / Screen.height);
        return (float)Screen.width / Screen.height;
    }

    private void RepositionCamera(float x, float y) {
        Vector3 tempPosition = new Vector3(x/2 + XOffset, y/2 + YOffset, cameraOffset.position.z);
        transform.position = tempPosition;
        if (grid.rows >= grid.columns) {
            Camera.main.orthographicSize = (grid.rows / 2 + padding) / aspectRatio;
        } else {
            Camera.main.orthographicSize = grid.columns / 2 + padding / aspectRatio;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
