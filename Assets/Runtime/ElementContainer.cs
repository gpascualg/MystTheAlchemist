using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class ElementContainer : ScriptableObject
{
    public List<ElementNodeLinkData> nodeLinks = new List<ElementNodeLinkData>();
    public List<ElementNodeData> nodeData = new List<ElementNodeData>();
    public Vector2 TargetNodeGuid;
}
