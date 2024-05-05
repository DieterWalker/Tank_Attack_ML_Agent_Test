using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class HitFeedback : MonoBehaviour{
    public UnityEvent<GameObject> OnHit = new UnityEvent<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.TryGetComponent(out Bullet bullet)) {
           GameObject hitObjectName = gameObject;
           OnHit?.Invoke(hitObjectName);
        }
    }
}