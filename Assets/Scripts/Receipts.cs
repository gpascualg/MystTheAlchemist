using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;



[Serializable]
public class JSONReceipt
{
    public JSONComponentWithoutReceipt Final;
    public List<JSONComponentWithoutReceipt> Components;
    public string GUID;

    public ReceiptComponents Deserialize()
    {
        if (Components.Count == 0 || GUID == null || GUID.Length == 0)
        {
            return null;
        }

        Debug.Log($"Deserializing receipt {Final.Name}@{Components.Count} ({GUID})");
        var receipt = new ReceiptComponents()
        {
            Final = Final.Deserialize(GUID),
            Components = Components.ConvertAll(new Converter<JSONComponentWithoutReceipt, Component>(JSONComponentWithoutReceipt.Deserializer))
        };
        receipt.Final.ReceiptComponents = receipt;

        return receipt;
    }
}

public class ReceiptComponents
{
    public Component Final;
    public List<Component> Components;

    public static string ComponentName(Component component)
    {
        return component.Name;
    }

    public string GUID
    {
        get
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var data = new List<byte>(Final.BinaryGUID);
                foreach (var component in Components)
                {
                    data.AddRange(component.BinaryGUID);
                }

                var hash = mySHA256.ComputeHash(data.ToArray());
                return BitConverter.ToString(hash);
            }
        }
    }

    public JSONReceipt Serialize()
    {
        return new JSONReceipt()
        {
            Final = Final.SerializeWithoutReceipt(),
            Components = Components.ConvertAll(new Converter<Component, JSONComponentWithoutReceipt>(Component.SerializerWithoutReceipt)),
            GUID = GUID
        };
    }

    public static JSONReceipt Serializer(ReceiptComponents receipt)
    {
        return receipt.Serialize();
    }
}

[ExecuteInEditMode]
public class Receipts : MonoBehaviour
{
    [System.Serializable]
    class Node
    {
        public string Name;
        public string Guid;
        public Component AlchemicComponent;
        public List<Edge> Edges;
    }

    [System.Serializable]
    class Edge
    {
        public Node target;
        public string Guid;
    }

    [System.Serializable]
    class Receipt
    {
        public List<Node> Nodes = new List<Node>();
        public Node First;
    }

    public static Receipts Instance;

    private List<Receipt> receiptsGraph = new List<Receipt>();
    private Dictionary<Component, List<Receipt>> receiptsGraphByComponent = new Dictionary<Component, List<Receipt>>();
    
    public List<Component> Components = new List<Component>();
    public Dictionary<string, Component> ComponentsByName = new Dictionary<string, Component>();
    public Dictionary<ComponentType, List<Component>> ComponentsByType = new Dictionary<ComponentType, List<Component>>();

    private List<ReceiptComponents> receipts = new List<ReceiptComponents>();
    private Dictionary<string, ReceiptComponents> receiptsByName = new Dictionary<string, ReceiptComponents>();
    private Dictionary<string, ReceiptComponents> receiptsByGUID = new Dictionary<string, ReceiptComponents>();

    private void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        LoadConcoctions();
        LoadGraphs();
    }

    public ReceiptComponents FindReceipt(string GUID)
    {
        return receiptsByGUID.TryGetValue(GUID, out ReceiptComponents receipts) ? receipts : null;
    }

    public ReceiptComponents FindReceiptAt(int loc)
    {
        return receipts[loc];
    }

    public Component GetRandomComponentOfType(ComponentType type)
    {
        var components = ComponentsByType[type];
        return components[UnityEngine.Random.Range(0, components.Count)];
    }

    public void LoadGraphs()
    {
        receiptsGraph.Clear();
        receiptsGraphByComponent.Clear();

        receipts.Clear();
        receiptsByName.Clear();
        receiptsByGUID.Clear();

        var resources = Resources.LoadAll<ElementContainer>("");
        foreach (var graph in resources)
        {
            Receipt receipt = new Receipt();
            CreateNodes(graph, receipt);
            ConnectNodes(graph, receipt);
            if (receipt.First.AlchemicComponent == null)
            {
                Debug.LogError($"Invalid receipt {graph.name}");
                continue;
            }

            receiptsGraph.Add(receipt);
            if (receiptsGraphByComponent.ContainsKey(receipt.First.AlchemicComponent))
            {
                receiptsGraphByComponent[receipt.First.AlchemicComponent].Add(receipt);
            }
            else
            {
                receiptsGraphByComponent.Add(receipt.First.AlchemicComponent, new List<Receipt> { receipt });
            }

            ReceiptComponents receiptComponents = new ReceiptComponents()
            {
                Components = new List<Component>()
            };
            
            Node node = receipt.First;
            Edge edge = receipt.First.Edges[0];
            while (edge != null)
            {
                receiptComponents.Components.Add(node.AlchemicComponent);
                node = edge.target;
                edge = node.Edges.FirstOrDefault();
            }
            receiptComponents.Final = node.AlchemicComponent;
            receiptComponents.Final.ReceiptComponents = receiptComponents;

            receipts.Add(receiptComponents);
            receiptsByName.Add(receiptComponents.Final.Name, receiptComponents);
            receiptsByGUID.Add(receiptComponents.GUID, receiptComponents);

            Debug.Log($"Loaded receipt {receiptComponents.Final.Name} ({receiptComponents.GUID}) with {receiptComponents.Components.Count} ingredients");
        }
    }

    public void LoadConcoctions()
    {
        Components = Resources.LoadAll<Component>("AlchemicComponents").ToList();

        ComponentsByName.Clear();
        ComponentsByType.Clear();

        ComponentsByType.Add(ComponentType.Agent, new List<Component>());
        ComponentsByType.Add(ComponentType.Base, new List<Component>());
        ComponentsByType.Add(ComponentType.Potion, new List<Component>());
        ComponentsByType.Add(ComponentType.Flower, new List<Component>());

        foreach (var concoction in Components)
        {
            ComponentsByName.Add(concoction.Name, concoction);
            ComponentsByType[concoction.ComponentType].Add(concoction);
        }
    }

    private void CreateNodes(ElementContainer container, Receipt receipt)
    {
        foreach (var nodeData in container.nodeData)
        {
            Component alchemicComponent = null;
            if (ComponentsByName.ContainsKey(nodeData.DialogueText))
            {
                alchemicComponent = ComponentsByName[nodeData.DialogueText];
            }
            else
            {
                Debug.LogError($"Receipt {container.name} has invalid component {nodeData.DialogueText}");
            }

            receipt.Nodes.Add(new Node()
            {
                Name = nodeData.DialogueText,
                AlchemicComponent = alchemicComponent,
                Guid = nodeData.Guid,
                Edges = new List<Edge>()
            });
        }
    }

    private void ConnectNodes(ElementContainer container, Receipt receipt)
    {
        for (var i = 0; i < receipt.Nodes.Count; i++)
        {
            var conections = container.nodeLinks.Where(x => x.BaseNodeGuid == receipt.Nodes[i].Guid).ToList();
            for (var j = 0; j < conections.Count; j++)
            {
                var targetNodeGuid = conections[j].TargetNodeGuid;
                var targetNode = receipt.Nodes.First(x => x.Guid == targetNodeGuid);
                receipt.Nodes[i].Edges.Add(new Edge()
                {
                    target = targetNode,
                    Guid = targetNodeGuid
                });
            }
        }

        var guid = container.nodeLinks.Where(x => x.PortName == "Next").First().TargetNodeGuid;
        receipt.First = receipt.Nodes.First(x => x.Guid == guid);
    }

    public ReceiptComponents FindReceiptFromComponents(List<Component> components)
    {
        foreach (var component in components)
        {
            if (!receiptsGraphByComponent.ContainsKey(component))
            {
                continue;
            }

            foreach (var receipt in receiptsGraphByComponent[component])
            {
                bool invalid = false;
                HashSet<Component> usedComponents = new HashSet<Component>();

                Node node = receipt.First;
                Edge edge = receipt.First.Edges[0];
                while (edge != null)
                {
                    if (!components.Contains(node.AlchemicComponent))
                    {
                        invalid = true;
                        break;
                    }

                    if (usedComponents.Contains(node.AlchemicComponent))
                    {
                        invalid = true;
                        break;
                    }

                    usedComponents.Add(node.AlchemicComponent);

                    node = edge.target;
                    edge = node.Edges.FirstOrDefault();
                }

                if (!invalid && usedComponents.Count == components.Count)
                {
                    // Node is the last element
                    return receiptsByName[node.AlchemicComponent.Name];
                }
            }
        }

        return null;
    }
}
