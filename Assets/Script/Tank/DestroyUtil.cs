using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyUtil : MonoBehaviour
{
    public void DestroyHelper(){
        gameObject.SetActive(false);
        // Destroy(gameObject);
    }
}
