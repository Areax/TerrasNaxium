using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class NaxClick : MonoBehaviour {

    //will return to where the card is put
	public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;
    GameObject placeholder = null;




	public void OnBeginDrag(PointerEventData eventData){
        Debug.Log("Dragging initiated");
            //placeholder is used for making sure the parent will have a "placeholder" object to come back to
            placeholder = new GameObject();
            placeholder.transform.SetParent(this.transform.parent);
            LayoutElement le = placeholder.AddComponent<LayoutElement>();
            le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
            le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            //placeholder is now where the card left the area
            placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

            parentToReturnTo = this.transform.parent;
            placeholderParent = parentToReturnTo;
            this.transform.SetParent(this.transform.parent.parent);

            GetComponent<CanvasGroup>().blocksRaycasts = false;

	}

	public void  OnDrag(PointerEventData eventData){
            this.transform.position = eventData.position;
            //if the drop area has the empt component (Drop Zones)
            if (placeholderParent.GetComponent<NaxDropZone>())
            {
                //make sure the placeholder will only be placed if it's hovering over an empty drop area
                NaxDropZone script = placeholderParent.GetComponent<NaxDropZone>();
                if (placeholder.transform.parent != placeholderParent && script.empt == false)
                    placeholder.transform.SetParent(placeholderParent);
            }
            //otherwise you're free to place the placeholder in the hand
            else if (placeholder.transform.parent != placeholderParent)
                placeholder.transform.SetParent(placeholderParent);

            int newSiblingIndex = placeholderParent.childCount;

        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }

            placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}
		
	public void  OnEndDrag(PointerEventData eventData){
            this.transform.SetParent(parentToReturnTo);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder);

	}
}