using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBehaviour : MonoBehaviour
{
    [Header("Template")]
    public DogTemplate template;

    //components
    Animator animator;
    AudioSource audioSource;
    IDogAttack attack;
    [SerializeField] private SphereCollider detector;

    [Space(20)]
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask zombieMask;

    //Dog Config
    public string dogName;
    public float attackCooldown;
    public float attackDamage;
    public float attackSpeed;
    private float range;
    private AudioClip attackSound;

    [Header("Dog Config")]
    [SerializeField] private float currentTime;
    public int attackUpgradeCost;
    public int attackSpeedUpgradeCost;

    private void Awake()
    {
        dogName = template.dogName;
        attackCooldown = template.attackCooldown;
        attackDamage = template.attackDamage;
        attackSpeed = template.attackSpeed;
        range = template.range;
        attackSound = template.attackSound;
        detector.radius = range;

        attackUpgradeCost = template.attackUpgradeCost;
        attackSpeedUpgradeCost = template.attackSpeedUpgradeCost;

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        attack = GetComponent<IDogAttack>();
        target = null;

        audioSource.clip = attackSound;

    }

    private void Update()
    {
        //LookForTarget();

        if (target != null)
        {
            LookAtTarget();
            currentTime += Time.deltaTime * attackSpeed;

            if (currentTime >= attackCooldown)
            {
                attack.Attack(target, animator, audioSource, attackDamage);
                currentTime = 0;
            }
        }

        HandleAnimations();
    }

    private void LookAtTarget()
    {
        transform.LookAt(target);
    }

    private void Attack()
    {
        
    
    }

    private void HandleAnimations()
    {
        if (target == null)
        {
            animator.SetBool("hasTarget", false);
        }
        else if (target != null)
        {
            animator.SetBool("hasTarget", true);
        }
    }
     
    public void LookForTarget()
    {
        if (target == null)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, Vector3.up, 0, zombieMask);

            float minDistance = 999f;
            foreach (RaycastHit hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                bool isZombieDead = hit.transform.GetComponent<ZombieBehaviorTemplate>().IsDead();

                if (distance < minDistance && !isZombieDead)
                {
                    minDistance = distance;
                    target = hit.transform;
                }
            }
        }
        
    }

    public void ClearTarget()
    {
        target = null;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Zombie")
        {
            LookForTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Zombie")
        {
            target = null;
            LookForTarget();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, template.range);
    }



}
