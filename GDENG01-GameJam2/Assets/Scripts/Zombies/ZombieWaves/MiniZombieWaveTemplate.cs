using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpawnType
{
    Unknown,
    SpawnOverTime,
    SingleBurst
}

[System.Serializable]
public class MiniZombieWaveTemplate
{
    #region FIELDS
    [SerializeField] private SpawnType spawnType;
    [SerializeField] private ZombieType zombieType;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnTicks;
    [SerializeField] private float startSpawnTimeInPercentage; // relative to the overall duration of the wave
    [SerializeField] private float endSpawnTimeInPercentage; // relative to the overall duration of the wave
    [SerializeField] private float elapsedSpawnTime;
    [SerializeField] private bool isSingleBurstDone;
    #endregion

    #region PROPERTIES
    public SpawnType SpawnType
    {
        get { return this.spawnType; }
        private set { this.spawnType = value; }
    }

    public ZombieType ZombieType
    {
        get { return this.zombieType; }
        private set { this.zombieType = value; }
    }

    public int SpawnCount
    {
        get { return this.spawnCount; }
        private set { this.spawnCount = value; }
    }

    public float SpawnTicks
    {
        get { return this.spawnTicks; }
        private set { this.spawnTicks = value; }
    }

    public float StartSpawnTimeInPercentage
    {
        get { return this.startSpawnTimeInPercentage; }
        private set { this.startSpawnTimeInPercentage = value; }
    }

    public float EndSpawnTimeInPercentage
    {
        get { return this.endSpawnTimeInPercentage; }
        private set { this.endSpawnTimeInPercentage = value; }
    }
    
    public float ElapsedSpawnTime
    {
        get { return this.elapsedSpawnTime; }
        private set { this.elapsedSpawnTime = value; }
    }

    public bool IsSingleBurstDone
    {
        get { return this.isSingleBurstDone; }
        set { this.isSingleBurstDone = value; }
    }
    #endregion

    public void AddElapsedTime(float increase)
    {
        this.ElapsedSpawnTime += increase;
    }

    public void RestartElapsedTime(bool isFullReset)
    {
        this.ElapsedSpawnTime = isFullReset ? 100 : 0;
    }

    public void RestartSingleBurst()
    {
        if (this.spawnType != SpawnType.SingleBurst)
            return;

        this.isSingleBurstDone = false;
    }
}
