using UnityEngine;
using System.Collections;

public class CardCreation : MonoBehaviour {

	
	private GameObject tooltip;


	// Use this for initialization
	void Start () {
		
		tooltip = GameObject.Find ("Tooltip");
	}

	public void CosntructDataString()
	{
	}

	// Update is called once per frame
	void Update () {
		tooltip.transform.position = tooltip.transform.parent.position;
	}
}
