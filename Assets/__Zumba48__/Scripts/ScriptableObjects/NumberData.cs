using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NmberData", menuName = "Scriptable Objects/Number Data", order = 1)]
public class NumberData : ScriptableObject
{

    public Color[] NumberColors;

    public Color DefaultForOtherColorsHigherNumbers;


}
