using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private int timeDelay = 0;
    [SerializeField] private bool isFreeze = false;
    private void Update(){
        timeDelay++; 
        if (isFreeze == false && timeDelay == 300 ){
            isFreeze = true;
            timeDelay = 0;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        if (isFreeze == true && timeDelay == 200 ){
            isFreeze = false;
            timeDelay = 0;
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
        }
    }
}
