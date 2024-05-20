using System; 

public class Player 
{
    public Wallet Wallet { get; private set; }
    public ExperienceHandler Experience { get; private set; }
    public Inventory Inventory { get; private set; }    

    public Player(Wallet wallet, ExperienceHandler experience, Inventory inventory)
    {
        if (wallet == null || experience == null || inventory == null)
        {
            throw new ArgumentNullException($"Wallet: {wallet} Experience: {experience} Inventory: {inventory}");
        }

        Wallet = wallet;    
        Experience = experience;
        Inventory = inventory;
    }
}
