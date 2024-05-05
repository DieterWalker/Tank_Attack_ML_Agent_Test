using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class CaculatingEnemy : MonoBehaviour{
    [SerializeField] private float enemyUnit = 5;
    public UnityEvent<float> OnEnemyChange = new UnityEvent<float>();
    public UnityEvent<float> OnEnemyStart = new UnityEvent<float>();
    public UnityEvent OnEnemyEnd = new UnityEvent();

    private void Awake(){
        OnEnemyStart?.Invoke(enemyUnit);
    }

    public void EnemyDeath(float newEnemyUnit){
        enemyUnit = enemyUnit - newEnemyUnit;
        OnEnemyChange?.Invoke(enemyUnit);
        if (enemyUnit == 0){
            OnEnemyEnd?.Invoke();
        }
    }
}
