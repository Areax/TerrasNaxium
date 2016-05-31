using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NaxHandTargetParent : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragTarget.attacking)
        {
            if (eventData.pointerDrag == null)
                return;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(DragTarget.attacking)
        {
            if (eventData.pointerDrag == null)
                return;
        }


    }

    public void OnDrop(PointerEventData eventData)
    {

        if(DragTarget.attacking)
        {
            DragTarget d = eventData.pointerDrag.GetComponent<DragTarget>();

            if (d != null)
            {
                d.parentToReturnTo = this.transform;
            }
        }

    }


}
