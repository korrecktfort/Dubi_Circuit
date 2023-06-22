using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Dubi.Circuit;
using UnityEditor.UIElements;
using UnityEditor;
using System;

namespace Dubi.Circuit
{
    public abstract class CircuitEventNode<T> : DragElement
    {
        CircuitDragParent circuitDragParent = null;
        CircuitEvent<T> circuitEvent = null;
        SerializedObject serializedObject = null;

        TextElement label = new TextElement();
        TextElement pathLabel = new TextElement();

        Toggle receiveEvent = new Toggle();

        PropertyField valueProperty = new PropertyField();

        VisualElement main = new VisualElement();
        VisualElement left = new VisualElement();
        VisualElement center = new VisualElement();
        VisualElement right = new VisualElement();
        VisualElement leftConnector = new VisualElement();
        VisualElement rightConnector = new VisualElement();
        VisualElement topBar = new VisualElement();
        VisualElement topBarTop = new VisualElement();        

        Button select = new Button();

        TextElement lockedText = new TextElement();

        public CircuitEventNode(DragParent dragParent) : base(dragParent)
        {
            base.styleSheets.Add(Resources.Load<StyleSheet>("CircuitEventNode"));

            this.receiveEvent.bindingPath = "lockable";
            this.label.bindingPath = "m_Name";
            this.valueProperty.bindingPath = "value";

            AddToClassList("root");
            this.topBar.AddToClassList("topBar");

            this.topBarTop.AddToClassList("topBar__top");
            this.topBar.Add(this.topBarTop);

            this.topBarTop.Add(this.receiveEvent);

            this.pathLabel.AddToClassList("pathLabel");
            this.topBar.Add(this.pathLabel);

            this.label.AddToClassList("label");
            this.topBarTop.Add(this.label);

            this.select.text = "s";
            this.select.tooltip = "Select Circuit Event Asset";
            this.select.AddToClassList("select");
            this.topBarTop.Add(this.select);

            Add(this.topBar);
            Add(this.main);

            this.main.AddToClassList("main");
            this.main.AddToClassList("denyDrag");
            Add(this.leftConnector);
            Add(this.rightConnector);

            this.leftConnector.AddToClassList("connector");
            this.leftConnector.AddToClassList("connector--left");

            this.rightConnector.AddToClassList("connector");
            this.rightConnector.AddToClassList("connector--right");

            this.left.AddToClassList("left");

            this.lockedText.AddToClassList("locked-text");
            this.lockedText.text = "Locked";
            this.center.Add(this.lockedText);

            this.center.Add(this.valueProperty);
            this.center.AddToClassList("center");

            this.right.AddToClassList("right");

            this.main.Add(this.left);
            this.main.Add(this.center);
            this.main.Add(this.right);

            this.leftConnector.SendToBack();
            this.leftConnector.AddToClassList("denyDrag");
            this.rightConnector.SendToBack();
            this.rightConnector.AddToClassList("denyDrag");

            this.receiveEvent.tooltip = "Tick and assign to lock by other event";

            this.receiveEvent.RegisterValueChangedCallback((e) =>
            {
                this.leftConnector.style.display = new StyleEnum<DisplayStyle>(e.newValue ? DisplayStyle.Flex : DisplayStyle.None);
            });

            this.leftConnector.RegisterCallback<MouseDownEvent>((e) => 
            {
                switch (e.button)
                {
                    case 0:
                        this.circuitDragParent.DrawStartElementRight = this.leftConnector;
                        break;
                    case 1:
                        RemoveParent();
                        break;
                }
            });

            this.leftConnector.RegisterCallback<MouseUpEvent>((e) => 
            {
                switch (e.button)
                {
                    case 0:
                        VisualElement drawStartLeft = this.circuitDragParent.DrawStartElementLeft;
                        if (drawStartLeft != null)
                        {
                            CircuitEventNode<T> otherNode = QForParent<CircuitEventNode<T>>(drawStartLeft);
                            if (otherNode is CircuitEventNode<T> && otherNode != this)
                                this.SetParent(otherNode);
                        }

                        this.circuitDragParent.ClearDrawingHandles(null);
                        break;

                    case 1:
                        break;
                }
            });

            this.rightConnector.RegisterCallback<MouseDownEvent>((e) => 
            {
                switch (e.button)
                {
                    case 0:
                        this.circuitDragParent.DrawStartElementLeft = this.rightConnector;
                        break;
                    case 1:
                        foreach (CircuitEvent<T> child in this.circuitEvent.Children)
                            ((CircuitEventNode<T>)child.node).RemoveParent(this);
                        break;
                }
            });

            this.rightConnector.RegisterCallback<MouseUpEvent>((e) => 
            {
                switch (e.button)
                {
                    case 0:
                        VisualElement drawStartRight = this.circuitDragParent.DrawStartElementRight;
                        if (drawStartRight != null)
                        {
                            CircuitEventNode<T> otherNode = QForParent<CircuitEventNode<T>>(drawStartRight);
                            if (otherNode is CircuitEventNode<T> && otherNode != this)
                            {
                                otherNode.SetParent(this);
                            }
                        }

                        this.circuitDragParent.ClearDrawingHandles(null);
                        break;

                    case 1:
                        break;
                }
            });

            this.receiveEvent.RegisterValueChangedCallback((e) => 
            {
                if (!this.receiveEvent.value)
                {
                    RemoveParent();
                }

                DisplayLockedState();
            });
        }

        public void UpdateChildrenDisplayLockedState()
        {
            foreach(CircuitEvent<T> child in this.circuitEvent.Children)
            {
                CircuitEventNode<T> node = child.node as CircuitEventNode<T>;
                node.DisplayLockedState();
                node.UpdateChildrenDisplayLockedState();
            }
        }

        public void DisplayLockedState()
        {
            bool unlocked = this.circuitEvent.Unlocked;

            if(unlocked)
            {
                this.lockedText.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                this.center.RemoveFromClassList("center--locked");               
            }
            else
            {
                this.lockedText.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                this.center.AddToClassList("center--locked");                
            }
        }
     
        public void BindCircuitEvent(string folder, CircuitEvent<T> circuitEvent)
        {
            circuitEvent.node = this;
            this.circuitEvent = circuitEvent;
            
            SerializedObject serializedObject = new SerializedObject(circuitEvent);
            base.InjectNodePosProperty(serializedObject.FindProperty("nodePos"));

            this.pathLabel.text = folder;

            this.receiveEvent.Bind(serializedObject);
            this.label.Bind(serializedObject);
            this.valueProperty.Bind(serializedObject);

            this.select.clickable = new Clickable(() => 
            {
                Selection.activeObject = circuitEvent;
            });

            this.valueProperty.RegisterValueChangeCallback((e) =>
            {                
                this.circuitEvent.StateChanged();
                UpdateChildrenDisplayLockedState();                
            });

            DisplayLockedState();

            this.serializedObject = serializedObject;
        }

        public void SetParent(CircuitEventNode<T> parentNode)
        {
            if(this.circuitEvent.Parent == null)
            {                
                SerializedObject parent = parentNode.serializedObject;

                this.serializedObject.FindProperty("parent").objectReferenceValue = parent.targetObject;
                this.serializedObject.ApplyModifiedProperties();

                SerializedProperty children = parent.FindProperty("children");
                children.arraySize++;
                children.serializedObject.ApplyModifiedProperties();

                children.GetArrayElementAtIndex(children.arraySize - 1).objectReferenceValue = this.serializedObject.targetObject;
                children.serializedObject.ApplyModifiedProperties();

                DisplayLockedState();
                UpdateChildrenDisplayLockedState();
            }
        }

        void RemoveParent()
        {
            if (this.circuitEvent.Parent != null)
                RemoveParent(this.circuitEvent.Parent.node as CircuitEventNode<T>);
        }

        public void RemoveParent(CircuitEventNode<T> parentNode)
        {
            if(this.circuitEvent.Parent == parentNode.circuitEvent)
            {
                SerializedObject parent = parentNode.serializedObject;

                SerializedProperty children = parent.FindProperty("children");
                for (int i = children.arraySize - 1; i >= 0 ; i--)
                {
                    CircuitEvent<T> cEvent = children.GetArrayElementAtIndex(i).objectReferenceValue as CircuitEvent<T>;
                    if (cEvent.Parent == parentNode.circuitEvent)
                    {
                        children.DeleteArrayElementAtIndex(i);
                        children.serializedObject.ApplyModifiedProperties();                        
                    }
                }

                this.serializedObject.FindProperty("parent").objectReferenceValue = null;
                this.serializedObject.ApplyModifiedProperties();

                DisplayLockedState();
                UpdateChildrenDisplayLockedState();
            }
        }

        public Vector2 WorldPosLeftConnector()
        {
            return this.leftConnector.LocalToWorld(this.leftConnector.contentRect.center);
        }

        public Vector2 WorldPosRightConnector()
        {
            return this.rightConnector.LocalToWorld(this.rightConnector.contentRect.center);
        }

        public (bool, Vector2, Vector2) GetParentConnection()
        {
            if(this.circuitEvent.Parent != null)
            {
                CircuitEventNode<T> parentNode = this.circuitEvent.Parent.node as CircuitEventNode<T>;
                if(parentNode != null)
                    return (true, WorldPosLeftConnector(), parentNode.WorldPosRightConnector());
            }

            return (false, Vector2.zero, Vector2.zero);
        }

        protected override void SetDragParent(DragParent dragParent)
        {
            base.SetDragParent(dragParent);

            if (dragParent is CircuitDragParent)
                this.circuitDragParent = dragParent as CircuitDragParent;
        }

        protected U QForParent<U>(VisualElement element) where U : CircuitEventNode<T>
        {
            if (element != null && element.parent != null)
            {
                if (element.parent is U)
                {
                    return element.parent as U;
                }
                else
                {
                    return QForParent<U>(element.parent);
                }
            }

            return null;
        }        
    }   
}