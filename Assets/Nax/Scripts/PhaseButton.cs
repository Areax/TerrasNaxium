using UnityEngine;
using System.Collections;


public class PhaseButton : MonoBehaviour 
{
	public void OnGUI()
		{
			if (GUI.Button(new Rect(25f, 25f , 150f , 30f), "Next Phase"))
			{
            NaxDraggable.moveable = false;
			DragTarget.attacking = true;
			}

			if (GUI.Button(new Rect (25f, 65f, 150f, 30f), "Refresh Server List"))
			{
			NaxDraggable.moveable = true;
			DragTarget.attacking = false;
			}
		}
	}
