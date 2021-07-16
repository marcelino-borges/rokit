using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionTextUI : MonoBehaviour
{
    public TextMeshProUGUI versionText;

    private void Awake()
    {
        versionText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        versionText.text = "v" + Application.version;
    }
}
