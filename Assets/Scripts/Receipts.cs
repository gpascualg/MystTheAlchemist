using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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

    [SerializeField]
    private List<Receipt> receipts = new List<Receipt>();

    private Dictionary<Component, List<Receipt>> receiptsByComponent = new Dictionary<Component, List<Receipt>>();
    
    [SerializeField]
    public List<Component> Components = new List<Component>();

    public Dictionary<string, Component> ComponentsByName = new Dictionary<string, Component>();


    private void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        LoadConcoctions();
        LoadGraphs();
    }

    public void LoadGraphs()
    {
        receipts.Clear();
        receiptsByComponent.Clear();

        var resources = Resources.LoadAll<ElementContainer>("");
        foreach (var graph in resources)
        {
            Receipt receipt = new Receipt();
            CreateNodes(graph, receipt);
            ConnectNodes(graph, receipt);
            if (receipt.First.AlchemicComponent == null)
            {
                Debug.LogError($"Invalid receipt {graph.name}");
                return;
            }

            receipts.Add(receipt);

            if (receiptsByComponent.ContainsKey(receipt.First.AlchemicComponent))
            {
                receiptsByComponent[receipt.First.AlchemicComponent].Add(receipt);
            }
            else
            {
                receiptsByComponent.Add(receipt.First.AlchemicComponent, new List<Receipt> { receipt });
            }
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
