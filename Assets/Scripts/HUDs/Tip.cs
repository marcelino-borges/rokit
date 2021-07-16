using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tip", menuName = "Rokit/Tips/Tip")]
public class Tip : ScriptableObject {
    [Header(" ")]
    [Header("stored in Resources folder.")]
    [Header("screens, sorted among all tips files")]
    [Header("This file will be shown in the loading")]
    [SerializeField]
    [TextArea(1, 2)]
    private string title;
    public string Title { get => title; set => title = value; }

    [SerializeField]
    [TextArea(5, 10)]
    private string body;
    public string Body { get => body; set => body = value; }

    [SerializeField]
    private Sprite image;
    public Sprite Image { get => image; set => image = value; }

}
