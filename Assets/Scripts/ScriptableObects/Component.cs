using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
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

    public ReceiptComponents ReceiptComponents { get; set; }

    public string GUID
    {
        get
        {
            return BitConverter.ToString(BinaryGUID);
        }
    }

    public byte[] BinaryGUID
    {
        get
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                return mySHA256.ComputeHash(Encoding.ASCII.GetBytes(Name));
            }
        }
    }
}
