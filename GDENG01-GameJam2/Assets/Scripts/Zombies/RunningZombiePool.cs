using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningZombiePool : GameObjectPool
{
    [SerializeField] private List<APoolable> poolableObjectCopies;

    public override void SetPoolableReferenceInactive()
    {
        for (int i = 0; i < this.poolableObjectCopies.Count; i++)
        {
            this.poolableObjectCopies[i].gameObject.SetActive(false);
        }
    }


    public override void Initialize()
    {
        System.Random rand = new System.Random();
        int randNum = -1;

        for (int i = 0; i < this.maxPoolSize; i++)
        {
            randNum = rand.Next(0, poolableObjectCopies.Count);

            APoolable poolableObject = GameObject.Instantiate<APoolable>(this.poolableObjectCopies[randNum], this.poolableParent);
            poolableObject.Initialize();
            poolableObject.gameObject.SetActive(false);
            this.availableObjects.Add(poolableObject);
        }
    }
}
