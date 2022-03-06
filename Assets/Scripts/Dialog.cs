using System;
using UnityEngine;

[Serializable]
public class Dialog
{
    [SerializeField] public Sprite avatar;
    [SerializeField] [TextArea(3, 5)] public string sentence;
}
