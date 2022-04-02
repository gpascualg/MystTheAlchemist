using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AlchemicComponent", order = 1)]
public class Component : ScriptableObject
{
    public string Name;
    public string SpriteName;
    public string Description;
    public int RestoresSeconds = 0;
}
