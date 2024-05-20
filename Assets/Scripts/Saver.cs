using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Saver : MonoBehaviour
{
    [Serializable]
    private struct SaveData
    {
        public Game.SaveData _gameSaveData;
        public SoundManager.SaveData _soundSaveData;
        public List<RecordSaveData> _recordsSaveData;
        public Inventory.SaveData _shopInventorySaveData;
        public SlotBox.SaveData _slotBoxSaveData;

        public SaveData(Game game, SoundManager soundManager, RecordsHandler recordsHandler, Inventory shopInventory, SlotBox slotBox)
        {
            _gameSaveData = new Game.SaveData(game);
            _soundSaveData = new SoundManager.SaveData(soundManager);
            _recordsSaveData = new List<RecordSaveData>(recordsHandler.GetSaveData());
            _shopInventorySaveData = new Inventory.SaveData(shopInventory);
            _slotBoxSaveData = new SlotBox.SaveData(slotBox);
        }

        public void Restore(Game game, SoundManager soundManager, RecordsHandler recordsHandler, Inventory shopInventory, SlotBox slotBox)
        {
            _gameSaveData.SetSavedDataTo(game);
            _soundSaveData.SetSavedDataTo(soundManager);
            recordsHandler.LoadFromSavedData(_recordsSaveData);
            _shopInventorySaveData.SetSavedDataTo(shopInventory);
            _slotBoxSaveData.SetSavedDataTo(slotBox);
        }
    }

    private const string SAVE_RECORD_NAME = "SlotsSaveGame";

    [SerializeField] private Game _game;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private RecordsHandler _recordsHandler;
    [SerializeField] private Inventory _shopInventory;
    [SerializeField] private SlotBox _slotBox;


    public event Action OnLoadGame;

    private bool _isLoading;

    private void Start()
    {
        if (TryToLoad())
        {
            Debug.Log("Game Loaded successfully");
        }

        _game.OnCoinsChanged += OnGameChanged;
        _game.OnExperienceChanged += OnGameChanged;
        _recordsHandler.OnChange += OnRecordsChanged;
        _game.StartGame();
    }

    private void OnDestroy()
    {
        _game.OnCoinsChanged -= OnGameChanged;
        _game.OnExperienceChanged -= OnGameChanged;
        _recordsHandler.OnChange -= OnRecordsChanged;
    }

    public void Save()
    {
        if (!_isLoading)
        { 
            SaveData saveData = new SaveData(_game, _soundManager, _recordsHandler, _shopInventory, _slotBox);
            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_RECORD_NAME, json);

            Debug.Log($"Game Saved. {json}");
        }
    }

    public void Delete()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("SaveFile Deleted.");
    }

    public bool TryToLoad()
    {
        if (PlayerPrefs.HasKey(SAVE_RECORD_NAME))
        {
            try
            {
                _isLoading = true;
                string json = PlayerPrefs.GetString(SAVE_RECORD_NAME);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                saveData.Restore(_game, _soundManager, _recordsHandler, _shopInventory, _slotBox);

                Debug.Log($"Game Loaded   {json}");

                OnLoadGame?.Invoke();
                _isLoading = false;
                return true;
            }
            catch
            {
                _isLoading = false;
                //PlayerPrefs.DeleteAll();
                Debug.Log("Error on load. SaveFile Deleted.");
                return false;
            }
        }
        else
        {
            Debug.Log("Saved game was not found.");
        }
        return false;
    }

    private void OnRecordsChanged(IRecord record)
    {
        Save();
    }

    private void OnGameChanged(int amount)
    {
        Save();
    }

}