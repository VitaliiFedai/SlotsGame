using CustomSlotsElements;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class UIManager : MonoBehaviour
{
    private const string WILD_X2_COLUMN_2_ITEM_NAME = "Wild_x2_Column2";
    private const string WILD_X2_COLUMN_4_ITEM_NAME = "Wild_x2_Column4";
    private const string INCREASE_MAX_BET_ITEM_NAME = "IncreaseMaxBet";
    private const float INCREASE_MAX_BET_PRICE_MULTIPLIER = 2.0f;
    private const int COLUMN_2_INDEX = 1;
    private const int COLUMN_4_INDEX = 3;
    private const int INCREASE_MAX_BET_REQUIRED_LEVEL_STEPS = 3;


    private SettingsPanel _settingsPanel;
    private GameUIPanel _gameUIPanel;
    private GameStatisticsPanel _gameStatisticsPanel;
    private MultiplierInfoSheet _multipliersPanel;
    private CheckingSchemeSheet _checkingSchemePanel;
    private LevelUpWindow _levelUpWindow;
    private BetTooltip _betTooltip;
    private ShopWindow _shopWindow;
    private BonusWindow _bonusWindow;
    private MessageWindow _messageWindow;

    [SerializeField] private Game _game;
    [SerializeField] private Saver _saver;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private RecordsHandler _recordsHandler;
    [SerializeField] private Shop _shop;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _game.CheckAllSchemes();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _saver.Delete();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            _game.Player.Wallet.Add(1000000);
            _saver.Save();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _game.Player.Experience.AddExperience(10000);
            _saver.Save();
        }

    }

    private void Awake()
    {
        AssignAllPanels();
        BindGameUIPanel();
        BindSettingsPanel();
        BindMultipliersPanel();
        BindCheckingSchemePanel();
        BindGameStatisticsPanel();
        BindLevelUpWindow();
        BindShopWindow();
        BindBonusWindow();
        BindRecordsHandler();
        BindGame();
        BindBetTooltip();
        BindMessageWindow();
        _saver.OnLoadGame += OnLoadGame;
    }

    private void OnDestroy()
    {
        UnbindGameUIPanel();
        UnbindSettingsPanel();
        UnbindMultipliersPanel();
        UnbindCheckingSchemePanel();
        UnbindGameStatisticsPanel();
        UnbindLevelUpWindow();
        UnbindShopWindow();
        UnbindBonusWindow();
        UnbindRecordsHandler();
        UnbindGame();
        UnbindMessageWindow();
        _saver.OnLoadGame -= OnLoadGame;
    }

    private void OnBackButtonClicked()
    {
        _settingsPanel.enabled = false;
        _multipliersPanel.enabled = false;
        _gameStatisticsPanel.enabled = false;
        _checkingSchemePanel.enabled = false;
        _bonusWindow.enabled = false;
        _shopWindow.enabled = false;
    }

    private void OnSettingsBackButtonClicked()
    {
        OnBackButtonClicked();
        _saver.Save();
    }

    private void OnShopElementClick(ShopItemElement element)
    {
        if (_shop.CanBuy(element.ItemSO))
        {
            _shop.BuyItem(element.ItemSO);
            _soundManager.PlayClickSound();
            SlotItemSO slotItemSO = element.ItemSO._slotItemSO;
            if (slotItemSO != null)
            {
                if (slotItemSO._isWild)
                {
                    int columnIndex = element.ItemSO.name == WILD_X2_COLUMN_2_ITEM_NAME ? COLUMN_2_INDEX : COLUMN_4_INDEX;
                    _game.AddSlotItemSOToColumnAt(columnIndex, slotItemSO);
                }
                else
                {
                    _game.AddSlotItemSO(slotItemSO);
                }
                _shopWindow.RefreshElement(element.ItemSO);
            }
            else
            {
                if (element.ItemSO.name == INCREASE_MAX_BET_ITEM_NAME)
                {
                    _game.IncreaseMaxBetToLine(_game.MaxBetToLine);
                    _shop.SetPrice(element.ItemSO, (int)(element.Price * INCREASE_MAX_BET_PRICE_MULTIPLIER));
                    _shop.SetRequiredLevel(element.ItemSO, element.RequiredLevel + INCREASE_MAX_BET_REQUIRED_LEVEL_STEPS);
                }
            }
            _saver.Save();
        }
    }

    private void OnRecordChange(IRecord receord)
    {
        _gameStatisticsPanel.Refresh();
    }

    private async void OnSpinButtonClicked()
    {
        _gameUIPanel.SetLocked(true);
        await _game.Spin();
        _gameUIPanel.SetLocked(!_game.IsReadyToSpin());
    }

    private void OnWinLine(SlotBox.CheckingSchemeResult result, int gain)
    {
        _soundManager.PlayCoinsDropSound();
    }

    private void OnPlaytableNextTabButtonClicked()
    {
        _checkingSchemePanel.enabled = true;
        _multipliersPanel.enabled = false;
    }

    private void OnPlaytablePrevTabButtonClicked()
    {
        _checkingSchemePanel.enabled = false;
        _multipliersPanel.enabled = true;
    }

    private async Task OnLevelChanged(int currentLevel)
    {
        _gameStatisticsPanel.SetLevel(currentLevel);
        _shopWindow.SetCurrentLevel(currentLevel);
        if (_game.IsStarted)
        {
            _soundManager.PlayPopUpSound();
            await _levelUpWindow.Show(currentLevel, _game.MaxBetToLine);
        }
        OnExperienceChanged(_game.Experience);
        _saver.Save();
    }

    private void OnExperienceChanged(int currentExperience)
    {
        float progress = Mathf.InverseLerp(_game.CurrentLevelExperience, _game.NextLevelExperience, _game.Experience);
        _gameUIPanel.SetExperienceProgress(progress);
    }

    private void OnBetButtonClicked()
    {
        _betTooltip.SetBetToLine(_game.BetToLine);
        _betTooltip.ShowWithAnim();
    }

    private void OnChangedShopItem(ItemSO itemSO, int value)
    {
        _shopWindow.RefreshElement(itemSO);
    }

    private void AssignAllPanels()
    {
        _gameUIPanel = FindObjectOfType<GameUIPanel>();
        _settingsPanel = FindObjectOfType<SettingsPanel>();
        _multipliersPanel = FindObjectOfType<MultiplierInfoSheet>();
        _checkingSchemePanel = FindObjectOfType<CheckingSchemeSheet>();
        _gameStatisticsPanel = FindObjectOfType<GameStatisticsPanel>();
        _levelUpWindow = FindObjectOfType<LevelUpWindow>();
        _betTooltip = FindObjectOfType<BetTooltip>();
        _shopWindow = FindObjectOfType<ShopWindow>();
        _bonusWindow = FindObjectOfType<BonusWindow>();
        _messageWindow = FindObjectOfType<MessageWindow>();
    }

    private void BindGameUIPanel()
    {
        _gameUIPanel.SetCoinsAmount(_game.Coins);
        _gameUIPanel.SetBetAmount(_game.Bet);
        _gameUIPanel.SetGainAmount(_game.Gain);

        _gameUIPanel.OnSettingsButtonClicked += _settingsPanel.Show;
        _gameUIPanel.OnInfoButtonClicked += _multipliersPanel.Show;
        _gameUIPanel.OnStatisticsButtonClicked += _gameStatisticsPanel.Show;
        _gameUIPanel.OnShopButtonClicked += _shopWindow.Show;
//        _gameUIPanel.OnBonusButtonClicked += _bonusWindow.Show;
        _gameUIPanel.OnBonusButtonClicked += OnBonusWindowClick;


        _gameUIPanel.OnIncreaseBetButtonClicked += _game.IncreaseBet;
        _gameUIPanel.OnDecreaseBetButtonClicked += _game.DecreaseBet;
        _gameUIPanel.OnMaxBetButtonClicked += _game.MaximizeBet;
        _gameUIPanel.OnSpinButtonClicked += OnSpinButtonClicked;
        _gameUIPanel.OnBetButtonClicked += OnBetButtonClicked;

        _gameUIPanel.IsMaxBet = _game.IsMaxBet;
        _gameUIPanel.IsMinBet = _game.IsMinBet;
        _gameUIPanel.IsReadyToSpin = _game.IsReadyToSpin;
        _gameUIPanel.IsEnoughCoinsToSpin = _game.IsEnoughCointToSpin;

        _gameUIPanel.OnClick += _soundManager.PlayClickSound;
        _gameUIPanel.Refresh();
        _gameUIPanel.enabled = true;
        _gameUIPanel.SetBonusButtonAnimationEnabled(_game.BonusIsReady());
    }

    private void UnbindGameUIPanel()
    {
        _gameUIPanel.OnSettingsButtonClicked -= _settingsPanel.Show;
        _gameUIPanel.OnInfoButtonClicked -= _multipliersPanel.Show;
        _gameUIPanel.OnStatisticsButtonClicked -= _gameStatisticsPanel.Show;
        _gameUIPanel.OnShopButtonClicked -= _shopWindow.Show;
//        _gameUIPanel.OnBonusButtonClicked -= _bonusWindow.Show;
        _gameUIPanel.OnBonusButtonClicked -= OnBonusWindowClick;

        _gameUIPanel.OnIncreaseBetButtonClicked -= _game.IncreaseBet;
        _gameUIPanel.OnDecreaseBetButtonClicked -= _game.DecreaseBet;
        _gameUIPanel.OnMaxBetButtonClicked -= _game.MaximizeBet;
        _gameUIPanel.OnSpinButtonClicked -= OnSpinButtonClicked;
        _gameUIPanel.OnBetButtonClicked -= OnBetButtonClicked;

        _gameUIPanel.IsMaxBet = null;
        _gameUIPanel.IsMinBet = null;
        _gameUIPanel.IsReadyToSpin = null;
        _gameUIPanel.IsEnoughCoinsToSpin = null;

        _gameUIPanel.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindSettingsPanel()
    {
        _settingsPanel.OnBackButtonClicked += OnSettingsBackButtonClicked;
        _settingsPanel.OnClick += _soundManager.PlayClickSound;
        _settingsPanel.enabled = false;
    }

    private void UnbindSettingsPanel()
    {
        _settingsPanel.OnBackButtonClicked -= OnSettingsBackButtonClicked;
        _settingsPanel.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindMultipliersPanel()
    {
        _multipliersPanel.OnBackButtonClicked += OnBackButtonClicked;
        _multipliersPanel.OnNextTabButtonClicked += OnPlaytableNextTabButtonClicked;
        _multipliersPanel.OnClick += _soundManager.PlayClickSound;
        _multipliersPanel.enabled = false;
    }

    private void UnbindMultipliersPanel()
    {
        _multipliersPanel.OnBackButtonClicked -= OnBackButtonClicked;
        _multipliersPanel.OnNextTabButtonClicked -= OnPlaytableNextTabButtonClicked;
        _multipliersPanel.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindGame()
    {
        _game.OnCoinsChanged += _gameUIPanel.SetCoinsAmount;
        _game.OnCoinsChanged += _shopWindow.SetMoneyAmount;
        _game.OnGainChanged += _gameUIPanel.SetGainAmount;
        _game.OnBetChanged += _gameUIPanel.SetBetAmount;
        _game.OnWinLine += OnWinLine;
        _game.OnLevelChanged += OnLevelChanged;
        _game.OnExperienceChanged += OnExperienceChanged;
        _game.OnMaxBetToLineChanged += OnMaxBetToLineChanged;
        _game.OnBonusIsReady += OnBonusIsReady;
    }

    private void UnbindGame()
    {
        _game.OnCoinsChanged -= _gameUIPanel.SetCoinsAmount;
        _game.OnCoinsChanged -= _shopWindow.SetMoneyAmount;
        _game.OnGainChanged -= _gameUIPanel.SetGainAmount;
        _game.OnBetChanged -= _gameUIPanel.SetBetAmount;
        _game.OnWinLine -= OnWinLine;
        _game.OnLevelChanged -= OnLevelChanged;
        _game.OnExperienceChanged -= OnExperienceChanged;
        _game.OnMaxBetToLineChanged -= OnMaxBetToLineChanged;
        _game.OnBonusIsReady -= OnBonusIsReady;
    }

    private void OnMaxBetToLineChanged(int amount)
    {
        _gameStatisticsPanel.SetMaxBetToLine(amount);
        _gameUIPanel.Refresh();
    }

    private void OnBonusIsReady()
    {
        _gameUIPanel.SetBonusButtonAnimationEnabled(true);
        Debug.Log("UIManager:  BonusIsReady!");
    }

    private void BindCheckingSchemePanel()
    {
        _checkingSchemePanel.OnBackButtonClicked += OnBackButtonClicked;
        _checkingSchemePanel.OnPrevTabButtonClicked += OnPlaytablePrevTabButtonClicked;
        _checkingSchemePanel.OnClick += _soundManager.PlayClickSound;
        _checkingSchemePanel.enabled = false;
    }

    private void UnbindCheckingSchemePanel()
    {
        _checkingSchemePanel.OnBackButtonClicked -= OnBackButtonClicked;
        _checkingSchemePanel.OnPrevTabButtonClicked -= OnPlaytablePrevTabButtonClicked;
        _checkingSchemePanel.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindGameStatisticsPanel()
    {
        _gameStatisticsPanel.OnBackButtonClicked += OnBackButtonClicked;
        _gameStatisticsPanel.OnClick += _soundManager.PlayClickSound;
        _gameStatisticsPanel.SetLevel(_game.CurrentLevel);
        _gameStatisticsPanel.SetMaxBetToLine(_game.MaxBetToLine);
        _gameStatisticsPanel.enabled = false;
    }

    private void UnbindGameStatisticsPanel()
    {
        _gameStatisticsPanel.OnBackButtonClicked -= OnBackButtonClicked;
        _gameStatisticsPanel.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindLevelUpWindow()
    {
        _levelUpWindow.OnClick += _soundManager.PlayClickSound;
        _levelUpWindow.enabled = false;
    }

    private void UnbindLevelUpWindow()
    {
        _levelUpWindow.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindMessageWindow()
    {
        _messageWindow.OnClick += _soundManager.PlayClickSound;
        _messageWindow.enabled = false;
    }

    private void UnbindMessageWindow()
    {
        _messageWindow.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindShopWindow() 
    {
        _shop.StartSession(_game.Player);
        _shop.OnChangedPrice += OnChangedShopItem;
        _shop.OnChangedRequiredLevel += OnChangedShopItem;

        _shopWindow.OnBackButtonClicked += OnBackButtonClicked;
        _shopWindow.OnClick += _soundManager.PlayClickSound;
        _shopWindow.OnElementClick += OnShopElementClick;

        _shopWindow.enabled = false;

        _shopWindow.SetItems(_shop.ItemsInfo);
        _shopWindow.SetCurrentLevel(_game.CurrentLevel);
        _shopWindow.SetMoneyAmount(_game.Coins);
    }

    private void UnbindShopWindow() 
    {
        _shopWindow.OnBackButtonClicked -= OnBackButtonClicked;
        _shopWindow.OnClick -= _soundManager.PlayClickSound;
        _shopWindow.OnElementClick -= OnShopElementClick;

        _shop.OnChangedPrice -= OnChangedShopItem;
        _shop.OnChangedRequiredLevel -= OnChangedShopItem;
        _shop.StopSession();
    }

    private void BindBonusWindow()
    {
        _bonusWindow.OnBackButtonClicked += OnBackButtonClicked;
        _bonusWindow.OnClick += _soundManager.PlayClickSound;
        _bonusWindow.enabled = false;
    }

    private async void OnBonusWindowClick()
    {
        if (_game.BonusIsReady())
        {
            _gameUIPanel.SetBonusButtonAnimationEnabled(false);
            int bonus = _game.GetBonus();
            if (!_messageWindow.enabled)
            {
                _soundManager.PlayPopUpSound();
                _soundManager.PlayCoinsDropSound();
                await _messageWindow.Show($"You got {bonus} Coins! \n\r Come again after {_game.BonusPeriod} hours to get more!");
            }
        }
        else
        {
            if (!_messageWindow.enabled)
            {
                int hours;
                int minutes;
                _game.GetElapsedTimeBeforeBonus(out hours, out minutes);
                await _messageWindow.Show($"Wait {hours}:{minutes} hours to get a new bonus.");
            }
        }
    }

    private void UnbindBonusWindow()
    {
        _bonusWindow.OnBackButtonClicked -= OnBackButtonClicked;
        _bonusWindow.OnClick -= _soundManager.PlayClickSound;
    }

    private void BindRecordsHandler()
    {
        _recordsHandler.OnChange += OnRecordChange;
    }

    private void UnbindRecordsHandler()
    {
        _recordsHandler.OnChange -= OnRecordChange;
    }

    private void BindBetTooltip()
    {
        _betTooltip.SetBetToLine(_game.BetToLine);
        _betTooltip.enabled = false;
    }

    private void OnLoadGame()
    {
        _shopWindow.SetItems(_shop.ItemsInfo);
        _shopWindow.Refresh();
    }
}
