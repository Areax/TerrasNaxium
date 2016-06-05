using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NaxHand : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {


	public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag == null)
            return;
        NaxDraggable d = eventData.pointerDrag.GetComponent<NaxDraggable>();
        if (d != null)
            d.placeholderParent = this.transform;
	}

	public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerDrag == null)
            return;
        NaxDraggable d = eventData.pointerDrag.GetComponent<NaxDraggable>();
        if (d != null && d.placeholderParent == this.transform)
            d.placeholderParent = d.parentToReturnTo;
    }


    public void OnDrop(PointerEventData eventData)
    {
        NaxDraggable d = eventData.pointerDrag.GetComponent<NaxDraggable>();
        d.parentToReturnTo = this.transform;
    }
}
