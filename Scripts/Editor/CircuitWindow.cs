using Dubi.Circuit;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CircuitWindow : EditorWindow
{
    CircuitDragParent dragParent = null;

    [MenuItem("Dubi/Circuit Editor")]
    static void Init()
    {
        CircuitWindow window = (CircuitWindow)EditorWindow.GetWindow(typeof(CircuitWindow));
        window.Show();
        window.SetupRoot(window.rootVisualElement);
    }

    [UnityEditor.Callbacks.OnOpenAssetAttribute(1)]
    public static bool OnOpenAsset(int instanceID)
    {
        CircuitEventBase circuitEvent = EditorUtility.InstanceIDToObject(instanceID) as CircuitEventBase;
        if (circuitEvent != null)
        {
            CircuitWindow window = (CircuitWindow)EditorWindow.GetWindow(typeof(CircuitWindow));
            window.Show();
            window.SetupRoot(window.rootVisualElement, circuitEvent);          

            return true;
        }

        return false;
    }

    public void SetupRoot(VisualElement root, CircuitEventBase focusOn = null)
    {
        ScriptableObject gridProperties = Resources.Load<ScriptableObject>("Circuit Event System Grid Properties");
        SerializedObject serializedGridProperties = new SerializedObject(gridProperties);
        SerializedProperty gridEditorProperties = serializedGridProperties.FindProperty("gridProperties");

        this.dragParent = new CircuitDragParent(gridEditorProperties);
        root.Add(this.dragParent);

        (string, CircuitEventBool)[] boolEvents = LoadEvents<CircuitEventBool>();
        foreach((string folder, CircuitEventBool e) boolEvent in boolEvents)
        {
            CircuitEventNodeBool node = new CircuitEventNodeBool(this.dragParent);
            node.BindCircuitEvent(boolEvent.folder, boolEvent.e);
            this.dragParent.AddDragElement(node);                
        }  
    }

    (string, T)[] LoadEvents<T>() where T : ScriptableObject
    {
        List<(string, T)> assets = new List<(string, T)>();
        string typeString = typeof(T).ToString().Replace("UnityEngine.", "");
        string[] guids = AssetDatabase.FindAssets("t:" + typeString);
        foreach(string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        
            string folder = assetPath[..(assetPath.LastIndexOf("/") + 1)];           

            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
                assets.Add((folder, asset));
        }

        return assets.ToArray();
    }   
}
