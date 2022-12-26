using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleDogAttack : IDogAttack
{

    [SerializeField] private float intervalTime;
    [SerializeField] private int numberOfAttacks;
    [SerializeField] private Transform gunTip;


    //effects
    [SerializeField] private TrailRenderer tracerEffect;
    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private ParticleSystem bloodEffect;

    private DogBehaviour dog;

    private void Awake()
    {
        dog = GetComponent<DogBehaviour>();
    }

    public override void Attack(Transform target, Animator animator, AudioSource audioSource, float damage)
    {
        animator.SetTrigger("attack");
        
        StartCoroutine(AttackInterval(target, numberOfAttacks, audioSource, damage));
    }

    IEnumerator AttackInterval(Transform target, int attacksLeft, AudioSource audioSource, float damage)
    {
        audioSource.Play();

        attacksLeft -= 1;

        foreach (ParticleSystem flash in muzzleFlash)
        {
            flash.Emit(1);
        }

        float x, y, z;
        x = Random.Range(-0.2f, 0.2f);
        y = Random.Range(1.0f, 1.5f);
        z = Random.Range(-0.2f, 0.2f);
        Vector3 newTarget = new Vector3(target.position.x + x, target.position.y + y, target.position.z + z);

        var clone = Instantiate(tracerEffect, gunTip.position, Quaternion.identity);
        clone.AddPosition(newTarget);

        bloodEffect.transform.position = newTarget;
        bloodEffect.transform.forward = transform.position - target.position;
        bloodEffect.Play();

        ZombieBehaviorTemplate zombie = target.GetComponent<ZombieBehaviorTemplate>();
        zombie.OnTakeDamage(damage);

        if (zombie.IsDead())
        {
            dog.ClearTarget();
            dog.LookForTarget();

            yield break;
        }

        if (attacksLeft > 0)
        {
            yield return new WaitForSeconds(intervalTime);
            StartCoroutine(AttackInterval(target, attacksLeft, audioSource, damage));
        }

        
    }
}
