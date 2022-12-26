using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GameStates
{
    Unknown,
    Tutorial,
    RestPhase,
    Gameplay,
    Shopping,
    GameWin,
    GameLose,
    Pause,
}

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    // FIELDS
    [SerializeField] private int money;
    [SerializeField] private GameStates currentState = GameStates.Unknown;
    [SerializeField] private GameStates prevState = GameStates.Unknown;
    [SerializeField] private ZombieSpawner zombieSpawner = null;
    [SerializeField] private GameObjectPool meatDropPool = null;

    // METHODS
    void Start()
    {
        this.currentState = GameStates.Tutorial;

        if (this.meatDropPool != null)
            this.meatDropPool.Initialize();
    }

    public int GetMoney()
    {
        return money;
    }

    public void UpdateMoney(int amount)
    {
        money += amount;

        if (money < 0)
            money = 0;
        else if (money > 99999)
            money = 99999;

        GameUIManager.Instance.UpdateMoneyText();
    }

    public GameStates GetCurrentGameState()
    {
        return this.currentState;
    }

    public GameStates GetPrevGameState()
    {
        return this.prevState;
    }

    public void UpdateGameState(GameStates newState)
    {
        if (newState == this.currentState)
            return;

        this.prevState = this.currentState;
        this.currentState = newState;

        CheckGameOver();
        EventBroadcaster.Instance.PostEvent(EventNames.ON_CHANGE_GAME_STATE);
    }

    public ZombieSpawner GetZombieSpawnerRef()
    {
        return this.zombieSpawner;
    }

    public GameObjectPool GetMeatPool()
    {
        return this.meatDropPool;
    }

    private void CheckGameOver()
    {
        if (this.currentState == GameStates.GameWin || this.currentState == GameStates.GameLose)
        {
            Time.timeScale = 0;
        }
    }
}
