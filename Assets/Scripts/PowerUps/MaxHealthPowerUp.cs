using PlayerBehaviors;

namespace PowerUps
{
    public class MaxHealthPowerUp : BasePowerUp
    {
        private float summand = 0.15f;

        public MaxHealthPowerUp()
        {
            Description = "Increase your maximum health by 15%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.UpdateMaxHealthModifier(summand);
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.UpdateMaxHealthModifier(-summand);
        }
    }
}
