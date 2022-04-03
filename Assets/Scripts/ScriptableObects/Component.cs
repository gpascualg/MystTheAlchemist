using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ComponentType
{
    Agent = 0,
    Base = 1,
    Potion = 2
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AlchemicComponent", order = 1)]
public class Component : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public GameObject Prefab;
    public string Description;
    public int RestoresSeconds = 0;
    public float Threshold;
    public ComponentType ComponentType;
}
