using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Dubi.Circuit;

public class CircuitEventDrawerBase<T> : PropertyDrawer where T : CircuitEventBase
{
    Color warningColor = Color.yellow;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property, new GUIContent(label));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.objectReferenceValue != null)
        {
            SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
            T targetObject = serializedObject.targetObject as T;
            bool unlocked = targetObject.Unlocked;
            Color lockedColor = this.warningColor;
            lockedColor.a = 0.2f;

            if (!unlocked)
            {
                position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.DrawRect(position, lockedColor);
                position.height = EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, serializedObject.FindProperty("value"));
            if(EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                targetObject?.StateChanged();
            }

            if (!unlocked)
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.DrawRect(position, lockedColor);
                position.height = EditorGUIUtility.singleLineHeight;

                float width = position.width;
                Color cached = GUI.color;
                GUI.color = this.warningColor;
                position.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(position, "Locked by parent");
                position.width = width - EditorGUIUtility.labelWidth;
                position.x += EditorGUIUtility.labelWidth + 2.0f;
                GUI.color = cached;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, serializedObject.FindProperty("parent"), GUIContent.none);
                GUI.enabled = true;
            }            
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 2.0f + EditorGUIUtility.standardVerticalSpacing;

        if(property.objectReferenceValue != null)
        {
            SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
            T targetObject = serializedObject.targetObject as T;
            if(!targetObject.Unlocked)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}

[CustomPropertyDrawer(typeof(CircuitEventBool))]
public class CircuitEventDrawerBool : CircuitEventDrawerBase<CircuitEventBool>
{
}