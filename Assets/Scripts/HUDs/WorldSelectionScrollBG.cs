using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSelectionScrollBG : MonoBehaviour {
    public float scrollSpeed;
    public float tileSizeZ;

    private Vector2 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        transform.position = startPosition + Vector2.right * newPosition;
    }
}
