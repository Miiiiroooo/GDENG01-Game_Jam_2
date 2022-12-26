using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    // FIELDS
    [SerializeField] private int currentWaveIndex = -1;

    [SerializeField] private string currentWaveName = "";
    [SerializeField] private List<ZombieWaveTemplate> waveList = new List<ZombieWaveTemplate>();
    [SerializeField] private List<Transform> targetLocationList = new List<Transform>();
    [SerializeField] private List<APoolable> activeZombiesList = new List<APoolable>();

    [SerializeField] private GameObjectPool walkingZombiePool;
    [SerializeField] private GameObjectPool runningZombiePool;

    [SerializeField] private bool isStopUpdating = false;
    [SerializeField] private bool hasUpdatedPoolableObjectsForNextWave = false;
    [SerializeField] private bool hasDisplayedWaveName = false;


    // UNITY METHODS
    void Awake()
    {
        // init wave
        currentWaveIndex = 0;
        currentWaveName = waveList[currentWaveIndex].WaveName;
        foreach (ZombieWaveTemplate wave in waveList)
            wave.FullReset();

        // init object pooling
        if (walkingZombiePool != null)
            walkingZombiePool.Initialize();
        if (runningZombiePool != null)
            runningZombiePool.Initialize();

        // init event-broadcasting
        EventBroadcaster.Instance.AddObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
    }

    private void OnDisable()
    {
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
    }

    void Update()
    {
        GameStates currentGameState = GameManager.Instance.GetCurrentGameState();
        if (currentGameState == GameStates.GameWin || currentGameState == GameStates.GameLose || currentGameState == GameStates.Pause || currentGameState == GameStates.Tutorial)
            return;

        if (this.isStopUpdating)
            return;


        ZombieWaveTemplate currentWave = this.waveList[currentWaveIndex];
        if (!this.hasDisplayedWaveName)
        {
            GameUIManager.Instance.OnDisplayWaveName(currentWaveName);
            this.hasDisplayedWaveName = true;
        }

        if (currentWave.ElapsedWaveTime < currentWave.WaveDuration)
        {
            foreach (MiniZombieWaveTemplate miniWave in currentWave.MiniWaveList)
            {
                float waveTime = currentWave.ElapsedWaveTime;
                float miniWaveStartTime = miniWave.StartSpawnTimeInPercentage * currentWave.WaveDuration;

                if (miniWave.SpawnType == SpawnType.SingleBurst && !miniWave.IsSingleBurstDone && waveTime > miniWaveStartTime)
                {
                    SpawnZombie(miniWave.ZombieType, miniWave.SpawnCount);
                    miniWave.IsSingleBurstDone = true;
                }
                else if (miniWave.SpawnType == SpawnType.SpawnOverTime)
                {
                    float miniwaveEndTime = miniWave.EndSpawnTimeInPercentage * currentWave.WaveDuration;

                    if (waveTime > miniWaveStartTime && waveTime < miniwaveEndTime)
                    {
                        if (miniWave.ElapsedSpawnTime > miniWave.SpawnTicks)
                        {
                            SpawnZombie(miniWave.ZombieType, miniWave.SpawnCount);

                            miniWave.RestartElapsedTime(false);
                        }

                        miniWave.AddElapsedTime(Time.deltaTime);
                    }
                }
            }

            currentWave.AddElapsedTime(Time.deltaTime);
        }
        else if (currentWave.ElapsedWaveTime >= currentWave.WaveDuration && this.activeZombiesList.Count == 0)
        {
            GameManager.Instance.UpdateGameState(GameStates.RestPhase);
            currentWave.AddElapsedTime(Time.deltaTime);
            UpdatePoolableObjectsForNextWave();

            if (currentWave.ElapsedWaveTime >= currentWave.WaveDuration + currentWave.RestDuration)
            {
                this.hasUpdatedPoolableObjectsForNextWave = false;
                this.hasDisplayedWaveName = false;
                this.currentWaveIndex++;

                if (this.currentWaveIndex >= this.waveList.Count)
                {
                    GameManager.Instance.UpdateGameState(GameStates.GameWin);
                }
                else
                {
                    currentWaveName = waveList[currentWaveIndex].WaveName;
                    GameManager.Instance.UpdateGameState(GameStates.Gameplay);
                }
            }
        }
    }

    // DELEGATES
    private void OnChangeGameState()
    {
        GameStates currentState = GameManager.Instance.GetCurrentGameState();

        switch (currentState)
        {
            case GameStates.Tutorial:
                this.isStopUpdating = true;
                break;
            case GameStates.RestPhase:
                this.isStopUpdating = false;
                break;
            case GameStates.Gameplay:
                this.isStopUpdating = false;
                break;
            case GameStates.Shopping:
                break;
            case GameStates.GameWin:
                this.isStopUpdating = true;
                break;
            case GameStates.GameLose:
                this.isStopUpdating = true;
                break;
            case GameStates.Pause:
                this.isStopUpdating = true;
                break;
            default:
                break;
        }
    }

    // OTHER METHODS
    private void SpawnZombie(ZombieType type, int count)
    {
        if (walkingZombiePool == null || runningZombiePool == null)
            return;

        if (count == 1)
        {
            APoolable poolable = null;

            if (type == ZombieType.Walking)
                poolable = this.walkingZombiePool.RequestPoolable();
            else if (type == ZombieType.Running)
                poolable = this.runningZombiePool.RequestPoolable();

            NavMeshAgent agentComponent = poolable.GetComponent<NavMeshAgent>();
            if (agentComponent != null)
            {
                System.Random rand = new System.Random();
                int randNum = rand.Next(0, this.targetLocationList.Count);

                agentComponent.SetDestination(targetLocationList[randNum].position);
                poolable.GetComponent<ZombieBehaviorTemplate>().SetDestination(targetLocationList[randNum].position);
            }
        }
        else if (count > 1)
        {
            APoolable[] poolableList = null;

            if (type == ZombieType.Walking)
                poolableList = this.walkingZombiePool.RequestPoolableBatch(count);
            else if (type == ZombieType.Running)
                poolableList = this.runningZombiePool.RequestPoolableBatch(count);

            for (int i = 0; i < poolableList.Length; i++)
            {
                NavMeshAgent agentComponent = poolableList[i].GetComponent<NavMeshAgent>();
                if (agentComponent != null)
                {
                    System.Random rand = new System.Random();
                    int randNum = rand.Next(0, this.targetLocationList.Count);

                    agentComponent.SetDestination(targetLocationList[randNum].position);
                    poolableList[i].GetComponent<ZombieBehaviorTemplate>().SetDestination(targetLocationList[randNum].position);
                }
            }
        }
    }

    private void UpdatePoolableObjectsForNextWave()
    {
        if (this.hasUpdatedPoolableObjectsForNextWave || this.currentWaveIndex + 1 >= this.waveList.Count)
            return;

        string nextWave = this.waveList[this.currentWaveIndex + 1].WaveName;

        APoolable[] walkingZombieList = this.walkingZombiePool.RequestAllAvailablePoolableObjectsWhileInactive();
        foreach (APoolable poolable in walkingZombieList)
        {
            ZombieBehaviorTemplate zombie = (ZombieBehaviorTemplate)poolable;
            zombie.IncreaseHP(nextWave);
        }

        APoolable[] runningZombieList = this.runningZombiePool.RequestAllAvailablePoolableObjectsWhileInactive();
        foreach (APoolable poolable in runningZombieList)
        {
            ZombieBehaviorTemplate zombie = poolable.GetComponent<ZombieBehaviorTemplate>();
            zombie.IncreaseHP(nextWave);
        }

        this.hasUpdatedPoolableObjectsForNextWave = true;
    }

    public void AddActiveZombie(APoolable zombie)
    {
        if (this.activeZombiesList.Contains(zombie))
            return;
    
        this.activeZombiesList.Add(zombie);
    }

    public void RemoveZombie(APoolable zombie)
    {
        if (!this.activeZombiesList.Contains(zombie))
            return;

        this.activeZombiesList.Remove(zombie);
    }

    public string GetCurrentWaveName()
    {
        return this.currentWaveName;
    }
}
