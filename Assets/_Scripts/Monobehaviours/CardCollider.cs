using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollider : MonoBehaviour
{
    CardParent cardParent;
    private void Awake()
    {
        cardParent = transform.parent.GetComponent<CardParent>();
    }

    private void OnMouseDown()
    {
        cardParent.MouseDown();
    }
    private void OnMouseDrag()
    {
        cardParent.MouseDrag();
    }
    private void OnMouseUp()
    {
        cardParent.MouseUp();
    }
}
