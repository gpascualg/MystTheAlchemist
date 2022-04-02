using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class ElementGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public ElementGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        VisualElementExtensions.StretchToParentSize(grid);

        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter node)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private Port GeneratePort(ElementsGraphNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private ElementsGraphNode GenerateEntryPointNode()
    {
        var node = new ElementsGraphNode
        {
            title = "START",
            GUID = System.Guid.NewGuid().ToString(),
            ElementText = "Entrypoint",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateElementNode(nodeName));
    }

    public ElementsGraphNode CreateElementNode(string nodeName)
    {
        var elementsNode = new ElementsGraphNode
        {
            title = nodeName,
            GUID = System.Guid.NewGuid().ToString(),
            ElementText = nodeName,
            EntryPoint = false
        };

        var inputPort = GeneratePort(elementsNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input 0";
        elementsNode.inputContainer.Add(inputPort);

        var buttonIn = new Button(() => { AddChoicePortIn(elementsNode); });
        buttonIn.text = "New input";
        elementsNode.titleContainer.Add(buttonIn);

        var buttonOut = new Button(() => { AddChoicePortOut(elementsNode); });
        buttonOut.text = "New output";
        elementsNode.titleContainer.Add(buttonOut);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => { elementsNode.ElementText = evt.newValue; elementsNode.title = evt.newValue; });

        textField.SetValueWithoutNotify(elementsNode.title);
        elementsNode.mainContainer.Add(textField);

        elementsNode.RefreshExpandedState();
        elementsNode.RefreshPorts();
        elementsNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return elementsNode;
    }

    public void AddChoicePortOut(ElementsGraphNode elementNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(elementNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outpuPortCount = elementNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice { outpuPortCount} " : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName,
        };

        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(""));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemoveOutPort(elementNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName =  choicePortName;

        elementNode.outputContainer.Add(generatedPort);
        elementNode.RefreshPorts();
        elementNode.RefreshExpandedState();
    }

    private void RemoveOutPort(ElementsGraphNode elementNode, Port generadPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generadPort.portName && x.output.node == generadPort.node);

        if (targetEdge.Any()) {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        elementNode.outputContainer.Remove(generadPort);
        elementNode.RefreshPorts();
        elementNode.RefreshExpandedState();
    }

    public void AddChoicePortIn(ElementsGraphNode elementNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(elementNode, Direction.Input);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var inpuPortCount = elementNode.inputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Input { inpuPortCount} " : overriddenPortName;

        generatedPort.portName = choicePortName;


        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName,
        };

        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(""));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemoveInPort(elementNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);


        elementNode.inputContainer.Add(generatedPort);
        elementNode.RefreshPorts();
        elementNode.RefreshExpandedState();
    }

    private void RemoveInPort(ElementsGraphNode elementNode, Port generadPort)
    {
        var targetEdge = edges.ToList().Where(x => x.input.portName == generadPort.portName && x.input.node == generadPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.output.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        elementNode.inputContainer.Remove(generadPort);
        elementNode.RefreshPorts();
        elementNode.RefreshExpandedState();
    }

}
