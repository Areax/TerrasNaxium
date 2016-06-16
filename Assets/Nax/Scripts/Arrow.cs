using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
     Vector3 pos1;
    Vector3 pos2;
    float objectHeight = 2.0f; // 2.0f for a cylinder, 1.0f for a cube

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            pos1 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.5f);
            pos1 = Camera.main.ScreenToWorldPoint(pos1);
            pos2 = pos1;
        }

        if (Input.GetMouseButton(0))
        {
            pos2 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 0.5f);
            pos2 = Camera.main.ScreenToWorldPoint(pos2);

        }

        if (pos2 != pos1)
        {
            Vector3 v3 = pos2 - pos1;
            transform.position = pos1 + (v3) / 2.0f;
            transform.localScale = new Vector3(transform.localScale.x, v3.magnitude / objectHeight, transform.localScale.z);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, v3);
        }
    }
}