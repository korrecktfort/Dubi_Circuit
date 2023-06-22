using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragElement : VisualElement
{
    protected SerializedProperty nodePos = null;
    protected DragParent dragParent = null;

    public Vector2 NodePos => this.nodePos.vector2Value;

    public DragElement(DragParent dragParent)
    {
        SetDragParent(dragParent);

        base.style.position = new StyleEnum<Position>(Position.Absolute);
        SetPosition(Vector2.zero);

        RegisterCallback<MouseDownEvent>((e) => 
        {
            VisualElement target = e.target as VisualElement;          

            if(!target.ClassListContains("denyDrag") && dragParent.currentDragging != this)
            {
                dragParent.currentDragging = this;
                dragParent.dragOffset = e.localMousePosition;
            }
        });

        RegisterCallback<MouseUpEvent>((e) => 
        {
            if (dragParent.currentDragging == this)
                dragParent.currentDragging = null;
        });

        base.MarkDirtyRepaint();
    }

    protected virtual void SetDragParent(DragParent dragParent)
    {
        this.dragParent = dragParent;
    }

    public void InjectNodePosProperty(SerializedProperty nodePosProp)
    {
        this.nodePos = nodePosProp;
        SetPosition(nodePosProp.vector2Value);
    }

    public void SetPosition(Vector2 gridPosition)
    {
        base.style.left = new StyleLength(gridPosition.x);
        base.style.top = new StyleLength(gridPosition.y);

        if(this.nodePos != null)
        {
            this.nodePos.vector2Value = gridPosition;
            this.nodePos.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }    
    }
}
