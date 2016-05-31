using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NaxDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public bool empt = false;

	public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log ("OnPointerEnter");
        if (eventData.pointerDrag == null)
            return;
        NaxDraggable d = eventData.pointerDrag.GetComponent<NaxDraggable>();
        if (d != null)
            d.placeholderParent = this.transform;
	}

	public void OnPointerExit(PointerEventData eventData) {
        //Debug.Log ("OnPointerExit");
        if (eventData.pointerDrag == null)
            return;
        NaxDraggable d = eventData.pointerDrag.GetComponent<NaxDraggable>();
        if (d != null && d.placeholderParent == this.transform)
        {
            d.placeholderParent = d.parentToReturnTo;

        }
        

    }

    void Update()
    {
        if (transform.childCount == 0)
           empt = false;
    }


	public void OnDrop(PointerEventData eventData) {

		NaxDraggable d = eventData.pointerDrag.GetComponent<NaxDraggable>();

		if(d != null && empt == false) {
			d.parentToReturnTo = this.transform; 
			empt = true;
		}

	}
		

}
