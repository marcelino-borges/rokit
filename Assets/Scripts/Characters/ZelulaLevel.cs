using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZelulaLevel : MonoBehaviour {
    private Animator animator;

    // Start is called before the first frame update
    void Start() {
        animator = gameObject.GetComponent<Animator>();
    }

    /// <summary>
    /// Character makes the celebration anim
    /// </summary>
    public void MakeMatchAnimation() {
        StartCoroutine(CelebrateMatchCoRoutine());
    }
    /// <summary>
    /// The character sorts and play one if the celebration anims
    /// </summary>
    private IEnumerator CelebrateMatchCoRoutine() {
        //Sorting the animation by a number
        int sortedAnim = Random.Range(0, 2);
        //Playing the animation according to the sorted number
        switch (sortedAnim) {
            case 0:
            animator.Play("CelebrateMatch1");
            yield return new WaitForSeconds(.7f);
            break;
            case 1:
            animator.Play("CelebrateMatch2");
            yield return new WaitForSeconds(.4f);
            break;
        }
        //Debug.Log("Sorted anim zelula: " + sortedAnim);
        animator.Play("Idle");
    }

    /// <summary>
    /// Character makes the celebration anim (when earned a star or wins the game)
    /// </summary>
    public void MakeStarOrVictoryAnim() {
        StartCoroutine(CelebrateStarOrVictoryCoRoutine());
    }

    /// <summary>
    /// Sets the animator's bool, MadeMatch, to the current value and waits 1s to turn it to false
    /// </summary>
    private IEnumerator CelebrateStarOrVictoryCoRoutine() {         
        animator.Play("CelebrateVictory");
        yield return new WaitForSeconds(.9f);
        animator.Play("Idle");
    }
}
