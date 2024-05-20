using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIPanel : PanelUI
{
    private const string SETTINGS_BUTTON_NAME = "settings-button";
    private const string SPIN_BUTTON_NAME = "spin-button";
    private const string COINS_LABEL_NAME = "coins-label";
    private const string GAIN_LABEL_NAME = "gain-label";
    private const string INCREASE_BET_BUTTON_NAME = "increase-bet-button";
    private const string DECREASE_BET_BUTTON_NAME = "decrease-bet-button";
    private const string MAX_BET_BUTTON_NAME = "max-bet-button";
    private const string INFO_BUTTON_NAME = "info-button";
    private const string STATISTICS_BUTTON_NAME = "statistics-button";
    private const string BET_BUTTON_NAME = "bet-button";
    private const string EXPERIENCE_PROGRESS_BAR_NAME = "experience-progress-bar";
    private const string SHOP_BUTTON_NAME = "shop-button";
    private const string BONUS_BUTTON_NAME = "bonus-button";
    private const string ANIMATION_STYLE_NAME = "button-animation";
    private const string ANIMATION_STYLE_NAME2 = "button-animation2";


    public event Action OnSettingsButtonClicked;
    public event Action OnSpinButtonClicked;
    public event Action OnIncreaseBetButtonClicked;
    public event Action OnDecreaseBetButtonClicked;
    public event Action OnBetButtonClicked;
    public event Action OnMaxBetButtonClicked;
    public event Action OnInfoButtonClicked;
    public event Action OnStatisticsButtonClicked;
    public event Action OnShopButtonClicked;
    public event Action OnBonusButtonClicked;

    public Func<bool> IsMaxBet;
    public Func<bool> IsMinBet;
    public Func<bool> IsReadyToSpin;
    public Func<bool> IsEnoughCoinsToSpin;

    public bool IsLocked { get; private set; }

    private int _coinsAmount;
    private int _gainAmount;
    private int _betAmount;
    private float _experienceProgress;
    private bool _bonusButtonAnimationEnabled;
    private float _animationTimer;

    private void Update()
    {
        if (GetRoot() != null)
        {
            Button bonusButton = GetElement<Button>(BONUS_BUTTON_NAME);
            if (_bonusButtonAnimationEnabled || bonusButton.ClassListContains(ANIMATION_STYLE_NAME))
            {
                if (_animationTimer <= 0)
                {
                    _animationTimer = 1f;
                    bonusButton.ToggleInClassList(ANIMATION_STYLE_NAME);
                }
                else
                {
                    _animationTimer -= Time.deltaTime;
                }
            }
        }
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        Refresh();
    }

    public void SetBonusButtonAnimationEnabled(bool enabled)
    {
        _bonusButtonAnimationEnabled = enabled;
    }

    public void SetCoinsAmount(int amount) 
    {
        _coinsAmount = amount;
        if (GetRoot() != null)
        {
            GetElement<Label>(COINS_LABEL_NAME).text = amount.ToString();
            Refresh();
        }
    }

    public void SetGainAmount(int amount)
    {
        _gainAmount = amount;
        if (GetRoot() != null)
        {
            GetElement<Label>(GAIN_LABEL_NAME).text = amount.ToString();
        }
    }

    public void SetBetAmount(int amount)
    {
        _betAmount = amount;
        if (GetRoot() != null)
        {
            GetElement<Button>(BET_BUTTON_NAME).text = amount.ToString();
            Refresh();
        }
    }

    public void SetExperienceProgress(float value)
    {
        _experienceProgress = value;
        if (GetRoot() != null)
        {
            GetElement<ProgressBar>(EXPERIENCE_PROGRESS_BAR_NAME).value = value;
        }
    }

    public void Refresh()
    {
        if (GetRoot() != null)
        {
            SetBonusButtonAnimationEnabled(_bonusButtonAnimationEnabled);

            bool isReadyToSpin = IsReadyToSpin == null || IsReadyToSpin();
            bool isEnoughCoins = IsEnoughCoinsToSpin == null || IsEnoughCoinsToSpin();
            bool isLocked = IsLocked;

            bool spinButtonEnabled = (isReadyToSpin) && (isEnoughCoins) && !isLocked;


            SetElementEnabled(SPIN_BUTTON_NAME, spinButtonEnabled);
            SetElementEnabled(DECREASE_BET_BUTTON_NAME, (IsMinBet == null || !IsMinBet()) && !IsLocked);
            SetElementEnabled(MAX_BET_BUTTON_NAME, (IsMaxBet == null || !IsMaxBet()) && !IsLocked);
            SetElementEnabled(INCREASE_BET_BUTTON_NAME, (IsMaxBet == null || !IsMaxBet()) && !IsLocked);

            GetElement<Label>(COINS_LABEL_NAME).text = _coinsAmount.ToString();
            GetElement<Label>(GAIN_LABEL_NAME).text = _gainAmount.ToString();
            GetElement<Button>(BET_BUTTON_NAME).text = _betAmount.ToString();
            GetElement<ProgressBar>(EXPERIENCE_PROGRESS_BAR_NAME).value = _experienceProgress;
        }
    }

    protected override void OnBind()
    {
        Refresh();
        RegisterCallbacks();
    }

    protected override void OnUnbind()
    {
    }

    private void RegisterCallbacks()
    {
        RegisterOnClickCallback(SETTINGS_BUTTON_NAME, OnSettingsButtonClicked);
        RegisterOnClickCallback(INFO_BUTTON_NAME, OnInfoButtonClicked);
        RegisterOnClickCallback(STATISTICS_BUTTON_NAME, OnStatisticsButtonClicked);
        RegisterOnClickCallback(SPIN_BUTTON_NAME, OnSpinButtonClicked);
        RegisterOnClickCallback(BET_BUTTON_NAME, OnBetButtonClicked);
        RegisterOnClickCallback(MAX_BET_BUTTON_NAME, OnMaxBetButtonClicked);
        RegisterOnClickCallback(INCREASE_BET_BUTTON_NAME, OnIncreaseBetButtonClicked);
        RegisterOnClickCallback(DECREASE_BET_BUTTON_NAME, OnDecreaseBetButtonClicked);
        RegisterOnClickCallback(SHOP_BUTTON_NAME, OnShopButtonClicked);
        RegisterOnClickCallback(BONUS_BUTTON_NAME, OnBonusButtonClicked);
    }
}
