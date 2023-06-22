using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class DragParent : PersistentGridElement
{
    public DragElement currentDragging = null;
    public Vector2 dragOffset = Vector2.zero;    

    public DragParent(SerializedProperty property) : base(property)
    {
        base.style.height = new StyleLength(StyleKeyword.Auto);
        base.style.width = new StyleLength(StyleKeyword.Auto);
        base.style.flexGrow = new StyleFloat(1.0f);              

        base.OnMouseMove += MouseMove;
        base.OnMouseUp += MouseUp;
        base.OnMouseLeave += MouseLeave;
    }   

    public void MouseMove(MouseMoveEvent e)
    {
        this.currentDragging?.SetPosition(base.pixelsPerUnit * base.gridMousePos - this.dragOffset);        
    }

    public void MouseUp(MouseUpEvent e)
    {
        this.currentDragging = null;
    }

    public void MouseLeave(MouseLeaveEvent e)
    {
        this.currentDragging = null;
    }

    public virtual void AddDragElement(DragElement dragElement)
    {
        base.AddToElementsContainer(dragElement);        
    }
}