using PlayerBehaviors;
using UnityEngine;

public abstract class BasePowerUp
{
    public string Description { get; protected set; }


    public virtual void ApplyPowerUp()
    {
        Utility.LogErrorIfNull(PlayerStats.Instance, "PlayerStats singleton",
            "PowerUps need a PlayerStats in the scene to work properly");
        if (PlayerStats.Instance == null) return;

        PlayerStats.Instance.AddToPowerUps(this);
    }

    // PlayerStats or enemy specific classes could have (de)buffs that also repeal an existing power up as a consequence.
    // This method is effectively the inverse of `ApplyPowerUp`
    public virtual void RepealPowerUp()
    {
        Utility.LogErrorIfNull(PlayerStats.Instance, "PlayerStats singleton",
            "PowerUps need a PlayerStats in the scene to work properly");
    }
}
