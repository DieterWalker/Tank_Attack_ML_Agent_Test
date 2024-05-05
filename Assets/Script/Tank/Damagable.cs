using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour{
    [SerializeField] private int MaxHealth = 100;
    [SerializeField] private int Health;

    public int CheckHealth{
        get{ return Health;}
        set{
            Health = value;
            OnHealthChange?.Invoke((float)Health / MaxHealth);
        }
    }

    [SerializeField] private UnityEvent OnDead;
    [SerializeField] private UnityEvent<float> OnHealthChange;
    [SerializeField] private UnityEvent OnHit, OnHeal;

    private void  Start(){
        CheckHealth = MaxHealth;
    }

    internal void Hit(int damagePoints){
        CheckHealth -= damagePoints;
        if (CheckHealth <= 0){
            OnDead?.Invoke();
        } else {
            OnHit?.Invoke();
        }
    }

    public void Heal(int healthBoost){
        CheckHealth += healthBoost;
        CheckHealth += Mathf.Clamp(Health, 0, MaxHealth);
        OnHeal?.Invoke();
    }
}
