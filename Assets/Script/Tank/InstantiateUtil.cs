using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instant : MonoBehaviour{
    public GameObject objectToInstantiate;

    public void InstantiateObject(){
        Instantiate(objectToInstantiate);
    }
}
