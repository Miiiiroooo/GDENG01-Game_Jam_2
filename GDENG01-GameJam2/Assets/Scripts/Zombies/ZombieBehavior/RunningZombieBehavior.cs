using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunningZombieBehavior : ZombieBehaviorTemplate
{
    public override void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        this.speed = 1.5f;
        this.baseHP = 6;
        this.increaseHP = 0;
        this.targetLocation = Vector3.zero;
        this.agent.speed = this.speed;
        this.isDead = false;
        this.isReleased = false;
        this.HP = this.baseHP + this.increaseHP;
    }

    public override void OnActivate()
    {
        this.HP = this.baseHP + this.increaseHP;
        this.isReleased = false;
        this.isDead = false;
        this.agent.isStopped = false;
        this.agent.SetDestination(targetLocation);
        this.agent.speed = speed;
        this.animator.SetTrigger("IsRunning");

        GameManager.Instance.GetZombieSpawnerRef().AddActiveZombie(this);
    }

    public override void Release()
    {
        if (this.isReleased)
            return;

        MeatBehavior meat = (MeatBehavior)GameManager.Instance.GetMeatPool().RequestPoolable();
        if (meat != null)
            meat.InitLocation(this.transform.position);

        this.isReleased = true;
        this.transform.position = this.poolRef.transform.position;
        GameManager.Instance.GetZombieSpawnerRef().RemoveZombie(this);
    }
}
