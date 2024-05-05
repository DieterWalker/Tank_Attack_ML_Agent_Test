using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "Data/TurretData")]
public class TurretData : ScriptableObject {
    public GameObject bulletPrefab;
    public float reloadDelay = 1;
    public BulletData bulletData;
}
