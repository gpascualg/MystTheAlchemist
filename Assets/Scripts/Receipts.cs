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
    public string Name;
    public List<string> Components;
    public string GUID;
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
            Name = Final.Name,
            Components = Components.ConvertAll(new Converter<Component, string>(ComponentName)),
            GUID = GUID
        };
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

            receipts.Add(receiptComponents);
            receiptsByName.Add(receiptComponents.Final.name, receiptComponents);
            receiptsByGUID.Add(receiptComponents.GUID, receiptComponents);

            Debug.Log($"Loaded receipt {receiptComponents.Final.Name} ({receiptComponents.GUID}) with {receiptComponents.Components.Count} ingredients");
        }
    }

    public void LoadConcoctions()
    {
        Components = Resources.LoadAll<Component>("AlchemicComponents").ToList();

        ComponentsByName.Clear();
        foreach (var concoction in Components)
        {
            ComponentsByName.Add(concoction.Name, concoction);
        }
    }

    private void CreateNodes(ElementContainer container, Receipt receipt)
    {
        Debug.Log("creating nodes");
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

            Debug.Log("adding node");
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
        Debug.Log("creating nodes");
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
}
