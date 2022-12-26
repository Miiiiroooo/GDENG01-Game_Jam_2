using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum ZombieType
{
    Unknown,
    Walking,
    Running
}

public abstract class ZombieBehaviorTemplate : APoolable
{
    [SerializeField] protected float HP;
    [SerializeField] protected float baseHP;
    [SerializeField] protected float increaseHP;
    [SerializeField] protected float speed;
    [SerializeField] protected Vector3 targetLocation;

    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;

    [SerializeField] protected bool isDead = false;
    [SerializeField] protected bool isReleased = false;

    void Update()
    {
        if (!this.gameObject.activeSelf)
            return;

        CheckIfAgentReachedDestination();
        CheckIfDeathAnimationIsFinished();
    }

    public void SetDestination(Vector3 target)
    {
        this.targetLocation = target;
    }

    public virtual void OnTakeDamage(float damage)
    {
        if (damage < 0 || this.HP <= 0)
            return;

        this.HP -= damage;
        if (this.HP <= 0 && !isDead)
            OnDeath();
    }

    protected virtual void OnDeath()
    {
        this.agent.isStopped = true;
        this.agent.speed = 0.0f;
        this.animator.SetTrigger("IsDead");
        this.isDead = true;
    }

    public virtual void IncreaseHP(string nextWave)
    {
        if (nextWave == "Wave 3" || nextWave == "Wave 5" || nextWave == "Final Wave")
        {
            this.increaseHP++;
        }
    }

    protected virtual void CheckIfAgentReachedDestination()
    {
        if (this.transform.position.z <= targetLocation.z)
        {
            GameManager.Instance.UpdateGameState(GameStates.GameLose);
        }
    }

    protected virtual void CheckIfDeathAnimationIsFinished()
    {
        AnimatorStateInfo currentState = this.animator.GetCurrentAnimatorStateInfo(0);

        if (!currentState.IsName("Zombie_Death_Back_Mid_1"))
            return;

        if (currentState.normalizedTime >= 0.60f)
        {
            this.poolRef.ReleasePoolable(this);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}
