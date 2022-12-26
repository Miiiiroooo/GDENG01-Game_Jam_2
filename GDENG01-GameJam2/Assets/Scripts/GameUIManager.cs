using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    #region FIELDS
    public static GameUIManager Instance { get; private set; }

    //Game Config
    [SerializeField] private bool canOpenShop = false;

    [Header("DogStand")]
    [SerializeField] private DogStand dogstand;

    [Header("Dogs")]
    [SerializeField] private GameObject basicDog;
    [SerializeField] private GameObject rifleDog;
    [SerializeField] private GameObject shotgunDog;

    //UI
    [Header("UI")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text waveDisplayText;

    [Header("Upgrade UI Components")]
    [SerializeField] private Image dogImage;
    [SerializeField] private TMP_Text dogName;
    [SerializeField] private TMP_Text attackDamage;
    [SerializeField] private TMP_Text attackSpeed;
    [SerializeField] private TMP_Text attackCostText;
    [SerializeField] private TMP_Text attackSpeedCostText;

    [Header("Upgrade Config")]
    [SerializeField] private float attackIncrement;
    [SerializeField] private float attackSpeedIncrement;
    [SerializeField] private float costMultiplier;

    [Header("Dog Costs")]
    [SerializeField] private int basicDogCost = 50;
    [SerializeField] private int rifleDogCost = 100;
    [SerializeField] private int shotgunDogCost = 200;

    [Header("Costs Text")]
    [SerializeField] private TMP_Text basicDogCostText;
    [SerializeField] private TMP_Text rifleDogCostText;
    [SerializeField] private TMP_Text shotgunDogCostText;

    [Header("Tutorial Texts")]
    [SerializeField] private string[] tutorialTexts;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialGameObject;
    [SerializeField] private Animation tutorialTextAnimation;
    [SerializeField] private float currentTime;
    [SerializeField] private float textDuration;
    [SerializeField] private int textsIndex = -1;

    enum tutorialStates
    {
        first,
        buyDog,
        UpgradeDog
    }

    [Header("Game Over UI")]
    [SerializeField] private string[] gameOverStrings;
    [SerializeField] private TMP_Text gameOverText;

    [Header("Wave Display Components")]
    [SerializeField] private Animator waveDisplayAnimator;
    [SerializeField] private AudioSource waveDisplayAudio;
    #endregion

    // UNITY METHODS
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

        EventBroadcaster.Instance.AddObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
    }

    private void Start()
    {
        DisableAllWindows();
        waveDisplayText.gameObject.SetActive(false);
        moneyText.gameObject.SetActive(true);

        basicDogCostText.text = basicDogCost.ToString();
        rifleDogCostText.text = rifleDogCost.ToString();
        shotgunDogCostText.text = shotgunDogCost.ToString();
        moneyText.text = GameManager.Instance.GetMoney().ToString();

    }

    private void OnDisable()
    {
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canOpenShop && !shopUI.activeSelf && dogstand != null && dogstand.dog == null)
        {
            OpenShopUI();
     
        }
        else if (Input.GetKeyDown(KeyCode.E) && canOpenShop && !upgradeUI.activeSelf && dogstand != null && dogstand.dog != null)
        {
            OpenUpgradeUI();
   
        }
        else if (Input.GetKeyDown(KeyCode.E) && (shopUI.activeSelf || upgradeUI.activeSelf))
        {
            CloseShopUI();
            CloseUpgradeUI();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && (GameManager.Instance.GetCurrentGameState() == GameStates.RestPhase || GameManager.Instance.GetCurrentGameState() == GameStates.Gameplay))
        {
            OnPause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.GetCurrentGameState() == GameStates.Pause)
        {
            OnResume();
        }

        HandleTutorial();
    }

    // Delegates
    private void OnChangeGameState()
    {
        GameStates currentState = GameManager.Instance.GetCurrentGameState();

        switch (currentState)
        {
            case GameStates.Tutorial:
                break;
            case GameStates.RestPhase:
                break;
            case GameStates.Gameplay:
                break;
            case GameStates.Shopping:
                break;
            case GameStates.GameWin:
                OnGameWin();
                break;
            case GameStates.GameLose:
                OnGameLose();
                break;
            case GameStates.Pause:
                break;
            default:
                break;
        }
    }

    // UI-related Methods
    public void OpenShopUI()
    {
        shopUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseShopUI()
    {
        shopUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dogImage.sprite = dogstand.dog.template.dogImage;
        dogName.text = dogstand.dog.dogName;
        attackDamage.text = dogstand.dog.attackDamage.ToString();
        attackSpeed.text = dogstand.dog.attackSpeed.ToString();
        attackCostText.text = dogstand.dog.attackUpgradeCost.ToString();
        attackSpeedCostText.text = dogstand.dog.attackSpeedUpgradeCost.ToString();

    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetCanOpenShop(bool value)
    {
        canOpenShop = value;
    }

    public void SetCurrentDogStand(DogStand newStand)
    {
        dogstand = newStand;
    }

    public void ClearDogStand()
    {
        dogstand = null;
    }

    public void BuyBasicDog()
    {
        if (dogstand == null || dogstand.dog != null)
        {
            return;
        }

        if (GameManager.Instance.GetMoney() >= basicDogCost)
        {
            GameManager.Instance.UpdateMoney(-basicDogCost);

            GameObject dog = Instantiate(basicDog, dogstand.spawnPoint.position, Quaternion.identity, dogstand.spawnPoint);
            dogstand.dog = dog.GetComponent<DogBehaviour>();
        }

        CloseShopUI();
    }

    public void BuyRifleDog()
    {
        if (dogstand == null || dogstand.dog != null)
        {
            return;
        }

        if (GameManager.Instance.GetMoney() >= rifleDogCost)
        {
            GameManager.Instance.UpdateMoney(-rifleDogCost);

            GameObject dog = Instantiate(rifleDog, dogstand.spawnPoint.position, Quaternion.identity, dogstand.spawnPoint);
            dogstand.dog = dog.GetComponent<DogBehaviour>();
        }

        CloseShopUI();
    }

    public void BuyShotgunDog()
    {
        if (dogstand == null || dogstand.dog != null)
        {
            return;
        }

        if (GameManager.Instance.GetMoney() >= shotgunDogCost)
        {
            GameManager.Instance.UpdateMoney(-shotgunDogCost);

            GameObject dog = Instantiate(shotgunDog, dogstand.spawnPoint.position, Quaternion.identity, dogstand.spawnPoint);
            dogstand.dog = dog.GetComponent<DogBehaviour>();
        }

        CloseShopUI();
    }

    public void UpdateMoneyText()
    {
        moneyText.text = GameManager.Instance.GetMoney().ToString();
    }

    public void UpgradeAttackDamage()
    {
        int cost = dogstand.dog.attackUpgradeCost;
        if (GameManager.Instance.GetMoney() >= cost)
        {
            GameManager.Instance.UpdateMoney(-cost);

            dogstand.dog.attackDamage += attackIncrement;
            dogstand.dog.attackDamage = RoundFloatToNearestTens(dogstand.dog.attackDamage);
            attackDamage.text = dogstand.dog.attackDamage.ToString();

            dogstand.dog.attackUpgradeCost = dogstand.dog.attackUpgradeCost + (int)(dogstand.dog.attackUpgradeCost * costMultiplier);
            attackCostText.text = dogstand.dog.attackUpgradeCost.ToString();
        }

    }

    public void UpgradeAttackSpeed()
    {
        int cost = dogstand.dog.attackSpeedUpgradeCost;
        if (GameManager.Instance.GetMoney() >= cost)
        {
            GameManager.Instance.UpdateMoney(-cost);

            dogstand.dog.attackSpeed += attackSpeedIncrement;
            attackSpeed.text = dogstand.dog.attackSpeed.ToString();

            dogstand.dog.attackSpeedUpgradeCost = dogstand.dog.attackSpeedUpgradeCost + (int)(dogstand.dog.attackSpeedUpgradeCost * costMultiplier);
            attackSpeedCostText.text = dogstand.dog.attackSpeedUpgradeCost.ToString();
        }
    }

    private float RoundFloatToNearestTens(float input)
    {
        float finalValue = 0.0f;
        int tempInteger = (int)(input * 10f + 0.2f); 
        finalValue = (float)tempInteger / 10f; 

        return finalValue;
    }

    public void ShowTutorialText(int index)
    {
        tutorialGameObject.SetActive(true);
        tutorialText.text = tutorialTexts[index];
        tutorialTextAnimation.Play();
    }

    public void CloseTutorialText()
    {
        tutorialGameObject.SetActive(false);
    }

    private void HandleTutorial()
    {
        if (GameManager.Instance.GetCurrentGameState() == GameStates.Tutorial)
        {
            currentTime += Time.deltaTime;

            if (currentTime > 1 && !tutorialGameObject.activeSelf)
            {
                textsIndex++;
                if (textsIndex == tutorialTexts.Length)
                {
                    GameManager.Instance.UpdateGameState(GameStates.Gameplay);
                }
                else
                {
                    ShowTutorialText(textsIndex);
                }
            }
        }

        if (tutorialGameObject.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                CloseTutorialText();
                currentTime = 0;
            }
        }
    }

    public void OnDisplayWaveName(string waveName)
    {
        this.waveDisplayText.gameObject.SetActive(true);
        this.waveDisplayText.text = waveName;
        this.waveDisplayAnimator.SetTrigger("Displaying");
        this.waveDisplayAudio.Play();
        StartCoroutine(CheckIfWaveDisplayIsFinished());
    }

    private IEnumerator CheckIfWaveDisplayIsFinished()
    {
        AnimatorStateInfo currentState = this.waveDisplayAnimator.GetCurrentAnimatorStateInfo(0);

        while (currentState.normalizedTime < 1.0f)
        {
            currentState = this.waveDisplayAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        this.waveDisplayAnimator.SetTrigger("Inactive");
        this.waveDisplayText.gameObject.SetActive(false);
    }

    private void DisableAllWindows()
    {
        shopUI.SetActive(false);
        upgradeUI.SetActive(false);
        gameOverUI.SetActive(false);
        tutorialGameObject.SetActive(false);
        pauseMenuUI.SetActive(false);
        moneyText.gameObject.SetActive(false);
    }

    private void OnGameWin()
    {
        DisableAllWindows();
        this.gameOverUI.SetActive(true);
        this.gameOverText.text = this.gameOverStrings[0];
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnGameLose()
    {
        DisableAllWindows();
        this.gameOverUI.SetActive(true);
        this.gameOverText.text = this.gameOverStrings[1];
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnPause()
    {
        DisableAllWindows();
        this.pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.UpdateGameState(GameStates.Pause);
    }

    public void OnResume()
    {
        DisableAllWindows();
        this.moneyText.gameObject.SetActive(true);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.UpdateGameState(GameManager.Instance.GetPrevGameState());
    }

    public void ReturnToMainMenu()
    {
        DisableAllWindows();
        SceneLoader.Instance.LoadLevel(SceneNames.MAIN_MENU_SCENE);
    }
}
