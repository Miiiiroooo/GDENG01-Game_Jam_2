using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingZombieBehavior : ZombieBehaviorTemplate
{
    public override void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        this.speed = 0.3f;
        this.baseHP = 7;
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
        DetermineWalkingAnimation();
        
        GameManager.Instance.GetZombieSpawnerRef().AddActiveZombie(this);
    }

    public override void Release()
    {
        if (this.isReleased)
            return;

        MeatBehavior meat = (MeatBehavior)GameManager.Instance.GetMeatPool().RequestPoolable();
        meat.InitLocation(this.transform.position);

        this.isReleased = true;
        this.transform.position = this.poolRef.transform.position;
        GameManager.Instance.GetZombieSpawnerRef().RemoveZombie(this);
    }

    private void DetermineWalkingAnimation()
    {
        System.Random rand = new System.Random();
        int randNum = rand.Next(0, 2);
        if (randNum == 0)
        {
            this.animator.SetTrigger("IsWalking1");
        }
        else if (randNum == 1)
        {
            this.animator.SetTrigger("IsWalking2");
        }
    }

    public override void IncreaseHP(string nextWave)
    {
        if (nextWave == "Wave 3" || nextWave == "Wave 5")
        {
            this.increaseHP++;
        }
        else if (nextWave == "Final Wave")
        {
            this.increaseHP += 2;
        }
    }
}
