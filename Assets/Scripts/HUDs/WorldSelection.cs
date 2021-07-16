using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSelection : MonoBehaviour {
    [SerializeField, Tooltip("Select the name of this world. If you can't find it, you might have forgotten to put this world's name in EnumCollection.Worlds.")]
    private Worlds worldName;

    public void loadLevel() {
        SoundManagerScript.PlayClickButton();
        GameManager.LoadLevel(worldName.ToString());
    }

    public void loadMainMenu() {
        SoundManagerScript.PlayClickButton();
        GameManager.LoadLevel("MainMenu");
    }

    public void loadWorldSelection() {
        SoundManagerScript.PlayClickButton();
        GameManager.LoadLevel("WorldSelection");
    }
}
