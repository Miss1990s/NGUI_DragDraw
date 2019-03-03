using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSpriteDepth : UIDragDropItem {
    //克隆出来的item不再克隆
    protected override void OnClone(GameObject original)
    {
        base.cloneOnDrag = false;
    }
    protected override void OnDragDropStart()
    {
        if (!draggedItems.Contains(this))
            draggedItems.Add(this);

        // Automatically disable the scroll view
        if (mDragScrollView != null) mDragScrollView.enabled = false;

        // Disable the collider so that it doesn't intercept events
        if (mButton != null) mButton.isEnabled = false;
        else if (mCollider != null) mCollider.enabled = false;
        else if (mCollider2D != null) mCollider2D.enabled = false;

        WidgetEditorRoot.StartEdit(transform.position);
    }

    protected override void OnDragDropMove(Vector2 delta)
    {
        base.OnDragDropMove(delta);
        WidgetEditorRoot.SynPosition(transform.position);
    }
    protected override void OnDragDropRelease(GameObject surface)
    {
        if (!cloneOnDrag)
        {

            // Is there a droppable container?
            DrawDragPanel container = surface ? NGUITools.FindInParents<DrawDragPanel>(surface) : null;

            if (container != null)
            {
                // Container found -- parent this object to the container
                mTrans.parent = container.transform;

                Vector3 pos = mTrans.localPosition;
                pos.z = 0f;
                mTrans.localPosition = pos;
            }
            else
            {
                NGUITools.Destroy(gameObject);
            }

            // Update the grid and table references
            mParent = mTrans.parent;
            mGrid = NGUITools.FindInParents<UIGrid>(mParent);
            mTable = NGUITools.FindInParents<UITable>(mParent);

            // Re-enable the drag scroll view script
            if (mDragScrollView != null)
                Invoke("EnableDragScrollView", 0.001f);

            // Notify the widgets that the parent has changed
            NGUITools.MarkParentAsChanged(gameObject);

            if (mTable != null) mTable.repositionNow = true;
            if (mGrid != null) mGrid.repositionNow = true;
        }
        else NGUITools.Destroy(gameObject);

        WidgetEditorRoot.EndEdit();
        // We're now done
        OnDragDropEnd();
    }
}
