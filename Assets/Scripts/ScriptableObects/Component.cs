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
    Potion = 2,
    Flower = 3
}


[System.Serializable]
public class JSONComponent
{
    public string Name;
    public string TransmogName;
    public int Type;
    public JSONReceipt Receipt;
    public int RestoresSeconds;
    public float Threshold;

    public static Component Deserializer(JSONComponent component)
    {
        return component.Deserialize();
    }

    public Component Deserialize(string receiptGuid = "")
    {
        Debug.Log($"Deserializing component {Name}");

        var components = Receipts.Instance.ComponentsByName;
        if (components.ContainsKey(Name))
        {
            return components[Name];
        }

        Component asset = ScriptableObject.CreateInstance<Component>();
        asset.Name = Name;
        asset.TransmogName = TransmogName;
        asset.Sprite = Receipts.Instance.ComponentsByName[TransmogName].Sprite;
        asset.ComponentType = (ComponentType)Type;
        asset.ReceiptComponents = Receipt.Deserialize();
        asset.RestoresSeconds = RestoresSeconds;
        asset.Threshold = Threshold;
        return asset;
    }
}

[System.Serializable]
public class JSONComponentWithoutReceipt
{
    public string Name;
    public string TransmogName;
    public int Type;
    public int RestoresSeconds;
    public float Threshold;

    public static Component Deserializer(JSONComponentWithoutReceipt component)
    {
        return component.Deserialize();
    }

    public Component Deserialize(string receiptGuid = "")
    {
        Debug.Log($"Deserializing component {Name}");

        var components = Receipts.Instance.ComponentsByName;
        if (components.ContainsKey(Name))
        {
            return components[Name];
        }

        Component asset = ScriptableObject.CreateInstance<Component>();
        asset.Name = Name;
        asset.TransmogName = TransmogName;
        asset.Sprite = Receipts.Instance.ComponentsByName[TransmogName].Sprite;
        asset.ComponentType = (ComponentType)Type;
        asset.ReceiptComponents = null;
        asset.RestoresSeconds = RestoresSeconds;
        asset.Threshold = Threshold;
        return asset;
    }
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AlchemicComponent", order = 1)]
public class Component : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public Sprite NameSprite;
    public GameObject Prefab;
    public string Description;
    public int RestoresSeconds = 0;
    public float Threshold;
    public ComponentType ComponentType;

    public string TransmogName { get; set; }
    public ReceiptComponents ReceiptComponents { get; set; }

    public static JSONComponent Serializer(Component component)
    {
        return component.Serialize();
    }

    public static JSONComponentWithoutReceipt SerializerWithoutReceipt(Component component)
    {
        return component.SerializeWithoutReceipt();
    }

    public JSONComponent Serialize()
    {
        return new JSONComponent()
        {
            Name = Name,
            TransmogName = TransmogName,
            Type = (int)ComponentType,
            Receipt = ReceiptComponents?.Serialize(),
            RestoresSeconds = RestoresSeconds,
            Threshold = Threshold
        };
    }

    public JSONComponentWithoutReceipt SerializeWithoutReceipt()
    {
        return new JSONComponentWithoutReceipt()
        {
            Name = Name,
            TransmogName = TransmogName,
            Type = (int)ComponentType,
            RestoresSeconds = RestoresSeconds,
            Threshold = Threshold
        };
    }

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
