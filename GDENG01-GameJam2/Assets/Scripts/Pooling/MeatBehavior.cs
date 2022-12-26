using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBehavior : APoolable
{
    [SerializeField] private float riseDuration;
    [SerializeField] private float elapsedRising;
    [SerializeField] private float riseSpeed;

    [SerializeField] private float fallDuration;
    [SerializeField] private float elapsedFalling;
    [SerializeField] private float fallSpeed;

    [SerializeField] private float lifeTimeDuration;
    [SerializeField] private float elapsedLifeTime;

    [SerializeField] private bool isRising;
    [SerializeField] private int baseMeatValue;

    void Update()
    {
        if (!this.gameObject.activeSelf)
            return;


        if (this.isRising)
        {
            if (this.elapsedRising > this.riseDuration)
            {
                this.isRising = false;
                this.elapsedRising = 0;
            }
            else
            {
                Vector3 currentPos = this.transform.position;
                float deltaY = currentPos.y + (riseSpeed * Time.deltaTime);
                Vector3 newPos = new Vector3(currentPos.x, deltaY, currentPos.z);
                this.transform.position = newPos;

                this.elapsedRising += Time.deltaTime;
            }
        }
        else
        {
            if (this.elapsedFalling > this.fallDuration)
            {
                this.isRising = true;
                this.elapsedFalling = 0;
            }
            else
            {
                Vector3 currentPos = this.transform.position;
                float deltaY = currentPos.y - (fallSpeed * Time.deltaTime);
                Vector3 newPos = new Vector3(currentPos.x, deltaY, currentPos.z);
                this.transform.position = newPos;

                this.elapsedFalling += Time.deltaTime;
            }
        }

        if (this.elapsedLifeTime < this.lifeTimeDuration)
        {
            this.elapsedLifeTime += Time.deltaTime;
        }
        else
        {
            this.poolRef.ReleasePoolable(this);
        }
    }

    public override void Initialize()
    {
        this.riseDuration = 1.0f;
        this.elapsedRising = 0.0f;
        this.riseSpeed = 0.1f;

        this.fallDuration = 1.0f;
        this.elapsedFalling = 0.0f;
        this.fallSpeed = 0.1f;

        this.lifeTimeDuration = 30f;
        this.elapsedLifeTime = 0.0f;

        this.isRising = true;
        this.baseMeatValue = 25;
    }

    public override void OnActivate()
    {
        this.elapsedLifeTime = 0.0f;
    }

    public override void Release()
    {
        this.elapsedRising = 0;
        this.elapsedFalling = 0;
        this.transform.position = this.poolRef.transform.position;
    }

    public void OnPickUp()
    {
        DetermineMeatValue();
        this.poolRef.ReleasePoolable(this);
    }

    private void DetermineMeatValue()
    {
        string currentWave = GameManager.Instance.GetZombieSpawnerRef().GetCurrentWaveName();
        int overallMeatValue = this.baseMeatValue;

        if (currentWave == "Wave 3")
        {
            overallMeatValue += 5;
        }
        else if (currentWave == "Wave 4")
        {
            overallMeatValue += 10;
        }
        else if (currentWave == "Wave 5")
        {
            overallMeatValue += 15;
        }
        else if (currentWave == "Wave 6")
        {
            overallMeatValue += 20;
        }
        else if (currentWave == "Final Wave")
        {
            overallMeatValue += 25;
        }

        GameManager.Instance.UpdateMoney(overallMeatValue);
    }

    public void InitLocation(Vector3 zombiePos)
    {
        Vector3 newPos = new Vector3(zombiePos.x, 0.3f, zombiePos.z);
        this.transform.position = newPos;
    }
}
