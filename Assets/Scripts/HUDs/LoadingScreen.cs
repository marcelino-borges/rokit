using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    [Tooltip("Image (UI element in hierarchy) shown beside the tip's text.")]
    public Image image_UIComponent;
    [Tooltip("Text (UI element in hierarchy) that will hold the tip's title.")]
    public Text tipTitle_UIComponent;
    [Tooltip("Text (UI element in hierarchy) that will hold the tip's body.")]
    public Text tipBody_UIComponent;

    private const string TIPS_PATH = "Tips";
    private Object[] tipsAvailable;

    void Awake() {
        //Retrieving all tips files in Resources (it was getting a null array when used this line in Start())
        tipsAvailable = Resources.LoadAll(TIPS_PATH, typeof(Tip));
    }

    public void SetTipData() {
        //If there is a list of tips
        if (tipsAvailable != null) {
            //Sort an index to choose a tip in the array
            int indexToSortTip = Random.Range(0, tipsAvailable.Length);

            Tip sortedTip = (Tip)tipsAvailable[indexToSortTip];

            tipTitle_UIComponent.text = sortedTip.Title;
            tipBody_UIComponent.text = sortedTip.Body;
            image_UIComponent.sprite = sortedTip.Image;

            //Debug.Log("sortedTip.title = " + sortedTip.Title);

        } else {
            Debug.LogError("Array tipsAvailable cannot be null.");
        }
    }
}
