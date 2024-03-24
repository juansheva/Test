using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text Scriptable", menuName = "ScriptableObjects/Text")]
public class TextScriptable : ScriptableObject
{
    [TextArea(3,10)]
    public string Text;

    public int Line = 0;
}
