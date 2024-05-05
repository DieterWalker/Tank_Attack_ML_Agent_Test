using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRay : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake(){
        Vector3 direction = transform.position;
        Debug.DrawRay(transform.position, direction, Color.blue, 1f);
    }
}
