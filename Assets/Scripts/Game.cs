using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using static SlotBox;

public class Game : MonoBehaviour
{
    [Serializable]
    public struct SaveData
    {
        public int _coins;

        public int _lastBonusGetYear;
        public int _lastBonusGetMonth;
        public int _lastBonusGetDay;
        public int _lastBonusGetHour;
        public int _lastBonusGetMinute;
        public int _lastBonusGetSecond;

        public int _maxBetToLine;
        public int _currentBetToLine;

        public ExperienceHandler.SaveData _experienceSaveData;

        public SaveData(Game game)
        {
            DateTime dateTime = game._lastBonusGetDateTime;

            _lastBonusGetYear = dateTime.Year;
            _lastBonusGetMonth = dateTime.Month;
            _lastBonusGetDay = dateTime.Day;
            _lastBonusGetHour = dateTime.Hour;
            _lastBonusGetMinute = dateTime.Minute;
            _lastBonusGetSecond = dateTime.Second;
            
            _coins = game._player.Wallet.Amount;

            _maxBetToLine = game.MaxBetToLine;
            _currentBetToLine = game.BetToLine;

            _experienceSaveData = new ExperienceHandler.SaveData(game._player.Experience);
        }

        public void SetSavedDataTo(Game game)
        {
            game.MaxBetToLine = Mathf.Max(game.MaxBetToLine, _maxBetToLine);
            game.BetToLine = Mathf.Max(game.BetToLine, _currentBetToLine);
            game._player.Wallet.Add(_coins);
            game._lastBonusGetDateTime = new DateTime(_lastBonusGetYear, _lastBonusGetMonth, _lastBonusGetDay, _lastBonusGetHour, _lastBonusGetMinute, _lastBonusGetSecond);
            _experienceSaveData.SetSavedDataTo(game._player.Experience);
        }
    }

    [SerializeField] private float _multiplierExperienceForLevel = 1.85f;
    [SerializeField] private int _minExperienceForLevel = 1500;
    [SerializeField] private int _bonusPeriod = 12;
    [SerializeField] private int _bonusCoinsCount = 10000;
    [SerializeField] private int _minBetToLine = 5;
    [SerializeField] private SlotBox _slotBox;
    [SerializeField] private int _betToLine = 10;
    [SerializeField] private Inventory _playerInventory;


    public event Action<int> OnGainChanged;
    public event Action<int> OnBetChanged;
    public event Action<int> OnSpinExecuted;
    public event Action<int> OnSpinFinished;
    public event Action<CheckingSchemeResult, int> OnWinLine;
    public event Action<int> OnCoinsChanged
    {
        add { _player.Wallet.OnAmountChanged += value; }
        remove { _player.Wallet.OnAmountChanged -= value; }
    }
    public event Func<int, Task> OnLevelChanged
    {
        add { _player.Experience.OnLevelChanged += value; }
        remove { _player.Experience.OnLevelChanged -= value; }
    }
    public event Action<int> OnExperienceChanged
    {
        add { _player.Experience.OnExperienceChanged += value; }
        remove { _player.Experience.OnExperienceChanged -= value; }
    }
    public event Action<int> OnMaxBetToLineChanged;
    public event Action OnBonusIsReady;
    public event Action OnBonusGet;

    private DateTime _lastBonusGetDateTime;

    public int Bet => _slotBox.LinesCount * _betToLine;
    public int Gain 
    { 
        get => _gain;
        private set
        { 
            _gain = value;
            OnGainChanged?.Invoke(_gain);
        }
    }

    public bool IsStarted {  get; private set; }    
    public int Coins => _player.Wallet.Amount;
    public bool IsMaxBet() => _betToLine == MaxBetToLine;
    public bool IsMinBet() => _betToLine == _minBetToLine;
    public int LinesCount => _slotBox.LinesCount;
    public int BetToLine
    {
        get => _betToLine;
        private set
        {
            _betToLine = value;
            OnBetChanged?.Invoke(Bet);
        }
    }
    public int MaxBetToLine 
    {
        get => _maxBetToLine;
        private set
        {
            _maxBetToLine = value;
            OnMaxBetToLineChanged?.Invoke(_maxBetToLine);
        } 
    }
    public int CurrentLevel => _player.Experience.Level;
    public int Experience => _player.Experience.Experience;
    public int NextLevelExperience => _player.Experience.NextLevelExperience;
    public int CurrentLevelExperience => _player.Experience.CurrentLevelExperience;
    public Player Player => _player;
    public int BonusPeriod => _bonusPeriod;

    public void GetElapsedTimeBeforeBonus(out int hours, out int minutes)
    {
        DateTime timeToBonus = _lastBonusGetDateTime.AddHours(_bonusPeriod);
        TimeSpan timeSpan = (timeToBonus - DateTime.Now);
        hours = timeSpan.Hours; 
        minutes = timeSpan.Minutes;
    }

    private int _gain;
    private int _maxBetToLine;
    private bool _bonusIsReady;

    private Player _player;

    public bool BonusIsReady()
    {
        return _bonusIsReady; 
    }

    public void Awake()
    {
        _player = new Player(new Wallet(), new ExperienceHandler(GetNextLevelExperience), _playerInventory);

        MaxBetToLine = _minBetToLine * 4;

        _slotBox.OnWinLine += OnWinLineReaction;
        _slotBox.OnSpinExecuted += OnSpinExecutedPerformed;
        _slotBox.OnSpinFinished += OnSpinFinishedPerformed;
    }

    private void Update()
    {
        if (!_bonusIsReady)
        {
            double hours = (DateTime.Now - _lastBonusGetDateTime).TotalHours;
            if (hours >= _bonusPeriod) 
            {
                _bonusIsReady = true;
                OnBonusIsReady?.Invoke();
            }
        }
    }

    public void StartGame()
    {
        if (IsStarted)
        {
            throw new InvalidOperationException("Game is already Started!");
        }
        IsStarted = true;
    }

    public int GetBonus()
    {
        if (BonusIsReady())
        {
            _bonusIsReady = false;
            _lastBonusGetDateTime = DateTime.Now;
            _player.Wallet.Add(_bonusCoinsCount);
            OnBonusGet?.Invoke();
            return _bonusCoinsCount;
        }
        return 0;
    }

    public async void CheckAllSchemes()
    {
        await _slotBox.CheckAllSchemes();
    }

    public bool IsEnoughCointToSpin()
    {
        return _player.Wallet.HasEnough(Bet);
    }

    public bool IsReadyToSpin()
    {
        return _slotBox.IsReady;
    }

    public async Task Spin()
    {
        Debug.Log($"=======SPIN========  (BetToLine: {BetToLine})");
        if (_player.Wallet.TryToGet(Bet))
        {
            Gain = 0;
            await _slotBox.Spin();
            await _player.Experience.AddExperience(Bet);
        }
    }

    public void IncreaseMaxBetToLine(int amount)
    {
        if (amount > 0)
        { 
            MaxBetToLine += amount;
        }
    }

    public void IncreaseBet()
    {
        if (BetToLine < MaxBetToLine)
        {
            BetToLine *= 2;
        }
    }

    public void DecreaseBet()
    {
        if (BetToLine > _minBetToLine)
        {
            BetToLine /= 2;
        }
    }

    public void MaximizeBet()
    {
        BetToLine = MaxBetToLine;
    }

    //public Task OnLevelChangePerformed(int currentLevel)
    //{
    //    MaxBetToLine *= 2;
    //    return Task.CompletedTask;
    //}

    public void AddSlotItemSO(SlotItemSO slotItemSO)
    {
        _slotBox.AddSlotItemSO(slotItemSO);
    }

    public void AddSlotItemSOToColumnAt(int columnIndex, SlotItemSO slotItemSO)
    {
        _slotBox.AddSlotItemSOToColumnAt(columnIndex, slotItemSO);
    }

    private void OnWinLineReaction(SlotBox.CheckingSchemeResult result)
    {
        int lineGain = BetToLine * result.BetMultiplier * result.WinMultiplier;
        _player.Wallet.Add(lineGain);
        Gain += lineGain;
        OnWinLine?.Invoke(result, lineGain);
        Debug.Log($"{result.SchemeName} - {result.MatchedItemName} has matched ({result.MatchedItemsCount}) times. BetMul = {result.BetMultiplier} WinMul = {result.WinMultiplier} LineGain = {lineGain}");
    }

    private void OnSpinExecutedPerformed()
    {
        OnSpinExecuted?.Invoke(Bet);
    }

    private void OnSpinFinishedPerformed()
    {
        OnSpinFinished?.Invoke(Gain);
    }

    private int GetNextLevelExperience(int currentLevelExperience)
    {
        return _minExperienceForLevel + (int)(currentLevelExperience * 1.5f); 
    }
}
