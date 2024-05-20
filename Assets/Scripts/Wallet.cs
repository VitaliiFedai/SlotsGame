using System;

public class Wallet
{
    public event Action<int> OnAmountChanged;

    public int Amount 
    { 
        get => _amount; 
        private set 
        { 
            _amount = value;
            OnAmountChanged?.Invoke(_amount);
        }
    }

    private int _amount;


    public bool HasEnough(int amount)
    { 
        return Amount >= amount;
    }

    public bool TryToGet(int amount)
    { 
        if (HasEnough(amount)) 
        {
            Amount -= amount;
            return true;
        }
        return false;
    }

    public void Add(int amount)
    {
        Amount += amount;
    }

    internal void Reset()
    {
        Amount = 0;
    }
}
