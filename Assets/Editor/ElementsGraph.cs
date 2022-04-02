using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class ElementsGraph : EditorWindow
{
    private ElementGraphView _graphView;
    private string _fileName = "New Narrative";

    [MenuItem("Graph/Elements Graph")]
    public static void OpenElementGraphWindow()
    {
        var window = GetWindow<ElementsGraph>();
        window.titleContent = new GUIContent("Elements Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        _graphView = new ElementGraphView
        {
            name = "Elements graph"
        };
        //_graphView.StretchToParentSize();
        VisualElementExtensions.StretchToParentSize(_graphView);
        rootVisualElement.Add(_graphView);
    }

    public void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File name: ");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();

        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save data" });
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load data" });

        var nodeCreateButton = new Button(() => _graphView.CreateNode("Element node"));
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Enter valid name", "OK");
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
