using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunDogAttack : IDogAttack
{
    [SerializeField] private float delay;
    [SerializeField] private Transform[] gunTip;

    //effects
    [SerializeField] private TrailRenderer tracerEffect;
    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private ParticleSystem bloodEffect;

    [SerializeField] private DogBehaviour dog;

    private void Awake()
    {
        dog = GetComponent<DogBehaviour>();
    }

    public override void Attack(Transform target, Animator animator, AudioSource audioSource, float damage)
    {
        animator.SetTrigger("attack");
        audioSource.PlayDelayed(delay);

        StartCoroutine(AttackInterval(target, audioSource, damage));


    }

    IEnumerator AttackInterval(Transform target, AudioSource audioSource, float damage)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
        foreach (ParticleSystem flash in muzzleFlash)
        {
            flash.Emit(1);
        }

        float x, y, z;
        x = Random.Range(-0.2f, 0.2f);
        y = Random.Range(1.0f, 1.5f);
        z = Random.Range(-0.2f, 0.2f);
        Vector3 newTarget = new Vector3(target.position.x + x, target.position.y + y, target.position.z + z);

        var clone = Instantiate(tracerEffect, gunTip[0].position, Quaternion.identity);
        clone.AddPosition(newTarget);

        var clone1 = Instantiate(tracerEffect, gunTip[1].position, Quaternion.identity);
        clone1.AddPosition(newTarget);

        //var blood = Instantiate(bloodEffect, new Vector3(target.position.x, target.position.y + 1, target.position.z), Quaternion.identity);
        bloodEffect.transform.position = newTarget;
        bloodEffect.transform.forward = transform.position - target.position;
        bloodEffect.Play();

        ZombieBehaviorTemplate zombie = target.GetComponent<ZombieBehaviorTemplate>();
        zombie.OnTakeDamage(damage);

        if (zombie.IsDead())
        {
            dog.ClearTarget();
            dog.LookForTarget();
        }


    }
}
