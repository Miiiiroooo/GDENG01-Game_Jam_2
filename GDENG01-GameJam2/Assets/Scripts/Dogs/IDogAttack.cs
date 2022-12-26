using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDogAttack : MonoBehaviour
{
    public abstract void Attack(Transform target, Animator animator, AudioSource audioSource, float damage);
    
}
