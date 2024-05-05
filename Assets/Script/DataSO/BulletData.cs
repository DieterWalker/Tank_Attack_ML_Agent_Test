using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Data/BulletData", order = 0)]
public class BulletData : ScriptableObject {
    
    public float speed = 10;
    public int damage = 5;
    public  float maxDistance= 10;
}

