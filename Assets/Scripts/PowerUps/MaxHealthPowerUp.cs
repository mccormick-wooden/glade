using PlayerBehaviors;

namespace PowerUps
{
    public class MaxHealthPowerUp : BasePowerUp
    {
        public MaxHealthPowerUp()
        {
            Description = "Increase your maximum health by 25%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.maxHealthModifier *= 1.25f;
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.maxHealthModifier /= 1.25f;
        }
    }
}
