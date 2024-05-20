using System;
using System.Threading.Tasks;
using UnityEngine;

public class ExperienceHandler 
{
    [Serializable]
    public struct SaveData
    {
        public int Level;
        public int Experience;
        public int NextLevelExperience;
        public int CurrentLevelExperience;

        public SaveData(ExperienceHandler experienceHandler)
        {
            Level = experienceHandler.Level;
            Experience = experienceHandler.Experience;
            NextLevelExperience = experienceHandler.NextLevelExperience;
            CurrentLevelExperience = experienceHandler.CurrentLevelExperience;
        }

        public void SetSavedDataTo(ExperienceHandler experienceHandler)
        {
            experienceHandler.NextLevelExperience = NextLevelExperience;
            experienceHandler.CurrentLevelExperience = CurrentLevelExperience;
            experienceHandler.Level = Level;
            experienceHandler.Experience = Experience;
        }
    }

    public event Func<int, Task> OnLevelChanged;
    public event Action<int> OnExperienceChanged;

    public int Experience 
    { 
        get => _experience;
        private set
        { 
            _experience = value;
            OnExperienceChanged?.Invoke(Experience);
        }
    }
    public int Level 
    {
        get => _level;
        private set
        {
            _level = value;
            OnLevelChanged?.Invoke(_level);
        }
    }

    public int NextLevelExperience { get; private set; }
    public int CurrentLevelExperience { get; private set; }
    public float CurrentProgress => Mathf.InverseLerp(CurrentLevelExperience, NextLevelExperience, Level);

    private Func<int, int> GetNextLevelExperience;

    private int _experience;
    private int _level;

    public ExperienceHandler(Func<int, int> getNextLevelExperience)
    {
        Experience = 0;
        Level = 1;
        CurrentLevelExperience = 0;
        GetNextLevelExperience = getNextLevelExperience;
        NextLevelExperience = GetNextLevelExperience(CurrentLevelExperience);
        OnExperienceChanged = null;
        OnLevelChanged = null;
    }

    public async Task AddExperience(int amount)
    { 
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }
        Experience += amount;
        while (Experience >= NextLevelExperience) 
        {
            Level++;
            CurrentLevelExperience = NextLevelExperience;
            NextLevelExperience = GetNextLevelExperience(CurrentLevelExperience);
            if (NextLevelExperience <= CurrentLevelExperience)
            {
                throw new Exception($"{nameof(NextLevelExperience)} ({NextLevelExperience}) is not greater than {nameof(CurrentLevelExperience)} (CurrentLevelValue)! It must not be so!");
            }
            await Task.WhenAll(OnLevelChanged?.Invoke(Level));
        }
    }
}
