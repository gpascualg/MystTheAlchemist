using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private ElementGraphView _targetGraphView;
    private ElementContainer _container;
    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<ElementsGraphNode> Nodes => _targetGraphView.nodes.ToList().Cast<ElementsGraphNode>().ToList();

    public static GraphSaveUtility GetInstance(ElementGraphView targetGraphView) {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string filename)
    {
        Debug.Log("Saving graph");
        if (!Edges.Any()) return;
        var elementContainer = ScriptableObject.CreateInstance<ElementContainer>();
        var connectedPorts = Edges.Where(x => x.input.node != null ).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            Debug.Log("Saving link");
            var outputNode = connectedPorts[i].output.node as ElementsGraphNode;
            var inputNode = connectedPorts[i].input.node as ElementsGraphNode;

            elementContainer.nodeLinks.Add(new ElementNodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            }); 
        }
        foreach (var elementNode in Nodes.Where(node => !node.EntryPoint))
        {
            Debug.Log("Saving node");
            elementContainer.nodeData.Add(new ElementNodeData
            {
                Guid = elementNode.GUID,
                DialogueText = elementNode.ElementText,
                Position = elementNode.GetPosition().position
            });
        }
        AssetDatabase.CreateAsset(elementContainer, $"Assets/Resources/{filename}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string filename) { 
        _container = Resources.Load<ElementContainer>(filename);

        if (_container == null)
        {
            EditorUtility.DisplayDialog("File not found", "Enter valid file", "OK");
            return;
        }

        ClearGrapgh();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGrapgh()
    {
        Debug.Log("clearing graph");
        Nodes.Find(x => x.EntryPoint).GUID = _container.nodeLinks[0].BaseNodeGuid;
        foreach(var node in Nodes)
        {
            Debug.Log("deleting node");
            if (node.EntryPoint) continue;
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
        }
    }

    private void CreateNodes()
    {
        Debug.Log("creating nodes");
        foreach(var nodeData in _container.nodeData)
        {
            Debug.Log("adding node");
            var tmpNode = _targetGraphView.CreateElementNode(nodeData.DialogueText);
            tmpNode.GUID = nodeData.Guid;
            _targetGraphView.AddElement(tmpNode);

            var nodePorts = _container.nodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePortOut(tmpNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        Debug.Log("connecting nodes");
        for(var i = 0; i < Nodes.Count; i++)
        {
            var conections = _container.nodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID ).ToList();
            for (var j = 0; j < conections.Count; j++)
            {
                var targetNodeGuid = conections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                targetNode.SetPosition(new Rect(_container.nodeData.First(x => x.Guid == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
            }

        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tmpEdge = new Edge { 
            output = output,
            input = input
        };

        tmpEdge.input.Connect(tmpEdge);
        tmpEdge.output.Connect(tmpEdge);
        _targetGraphView.Add(tmpEdge);
    }
}
