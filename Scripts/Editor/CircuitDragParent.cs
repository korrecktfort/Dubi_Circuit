using Dubi.Circuit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CircuitDragParent : DragParent
{
    float bezierThickness = 5.0f;
    float tangentStrength = 0.5f;
    Color bezierColor = Color.white;

    CircuitEventNodeBool[] boolNodes = new CircuitEventNodeBool[0];

    VisualElement drawStartElementLeft, drawStartElementRight;
        
    public VisualElement DrawStartElementLeft { get => drawStartElementLeft; set => drawStartElementLeft = value; }
    public VisualElement DrawStartElementRight { get => drawStartElementRight; set => drawStartElementRight = value; }

    public CircuitDragParent(SerializedProperty property) : base(property)
    {
        base.OnDrawHandles += DrawConnectionHandles;
        base.OnMouseUp += ClearDrawingHandles;
    }

    public void ClearDrawingHandles(MouseUpEvent e)
    {
        this.drawStartElementLeft = null;
        this.drawStartElementRight = null;
    }    

    void DrawConnectionHandles()
    {        
        float tStrength = base.pixelsPerUnit * this.tangentStrength;

        if(this.drawStartElementRight != null)
        {
            VisualElement e = this.drawStartElementRight;
            Vector2 p0 = base.gridMousePos;
            Vector2 p1 = base.ElementCenterToGrid(e);
            Vector2 t0 = p0 + Vector2.right * tStrength;
            Vector2 t1 = p1 + Vector2.left * tStrength;

            Handles.DrawBezier(p0, p1, t0, t1, this.bezierColor, null, this.bezierThickness);
        }

        if (this.drawStartElementLeft != null)
        {
            VisualElement e = this.drawStartElementLeft;
            Vector2 p0 = base.ElementCenterToGrid(e);
            Vector2 p1 = base.gridMousePos;            
            Vector2 t0 = p0 + Vector2.right * tStrength;
            Vector2 t1 = p1 + Vector2.left * tStrength;

            Handles.DrawBezier(p0, p1, t0, t1, this.bezierColor, null, this.bezierThickness);

        }

        foreach (CircuitEventNodeBool node in this.boolNodes)
        {
            (bool valid, Vector2 worldLeft, Vector2 worldRight) connection = node.GetParentConnection();
            if (connection.valid)
            {
                Vector2 p0 = base.WorldToGrid(connection.worldLeft);
                Vector2 p1 = base.WorldToGrid(connection.worldRight);               
                Vector2 t0 = p0 + Vector2.left * tStrength;
                Vector2 t1 = p1 + Vector2.right * tStrength;

                Handles.DrawBezier(p0, p1, t0, t1, this.bezierColor, null, this.bezierThickness);
            }
        }
    }

    public override void AddDragElement(DragElement dragElement)
    {
        base.AddDragElement(dragElement);

        switch (dragElement)
        {
            case CircuitEventNodeBool:
                List<CircuitEventNodeBool> boolList = this.boolNodes.ToList();
                boolList.Add(dragElement as CircuitEventNodeBool);
                this.boolNodes = boolList.ToArray();
                break;
        }       
    }

    public void CenterOn(CircuitEventBase circuitEvent)
    {
        base.FocusOnPosition(ElementCenterToGrid(circuitEvent.node));
    }
}
