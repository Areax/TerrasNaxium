using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragTarget : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Transform parentToReturnTo = null;
    public static bool attacking = false;
    public GameObject target;
    public GameObject canvas;
    //public GameObject card;
    GameObject actualtarget;

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (attacking)
        {
            parentToReturnTo = null;
            actualtarget = (GameObject)Object.Instantiate(target, eventData.position, transform.rotation);

            actualtarget.transform.SetParent(transform);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }


    }

    public void OnDrag(PointerEventData eventData)
    {
        if (attacking)
            actualtarget.transform.position = eventData.position;
            

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (attacking)
        {
            actualtarget.transform.SetParent(parentToReturnTo);
            if(parentToReturnTo == null)
               Destroy(actualtarget);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
            


    }

}