using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBombProjectile : MonoBehaviour {

    public Vector2 targetPosition;
    private float speed = 15f;
    public bool canMove;

    private void Start() {
        //Debug.Log("Projectile spawned!");
        //Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update() {
        if (targetPosition != null && ((Vector2)transform.position != targetPosition)) {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            //if (transform.position != targetObject.transform.position) {
            //    transform.position = Vector2.Lerp(transform.position, targetObject.transform.position, time);
            //}
            //Debug.Log("targetObject = NOT null");
        } else {
            Destroy(gameObject);
        }
    }
}
