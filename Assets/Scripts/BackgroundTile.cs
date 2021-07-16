using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour {
    [Header("ASSIGN:")]
    public Sprite backgroundActive;
    public Sprite backgroundInactive;
    [Header("MAY ASSIGN A NEW VALUE:")]
    public int hitPoints = 1;
    public SpriteRenderer sprite;

    public void TakeDamage(int damage) {
        hitPoints -= damage;
        //MakeLighter();
    }

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if(hitPoints <= 0) {
            Destroy(this.gameObject);
        }
    }

    void MakeLighter() {
        //take the current color
        Color color = sprite.color;
        //Get the current color's alpha value and cut it in half
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }

    public void ActivateTile(float timeInSeconds) {
        StartCoroutine(ChangeSprite(timeInSeconds));
    }

    private IEnumerator ChangeSprite(float time) {
        //sprite.sprite = backgroundActive;
        yield return new WaitForSeconds(time);
        //sprite.sprite = backgroundInactive;
    }


    
}

