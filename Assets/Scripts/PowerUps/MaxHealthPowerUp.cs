using PlayerBehaviors;

namespace PowerUps
{
    public class MaxHealthPowerUp : BasePowerUp
    {
        private float scalar = 1.15f;

        public MaxHealthPowerUp()
        {
            Description = "Increase your maximum health by 15%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.ScaleMaxHealthModifier(scalar);
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.ScaleMaxHealthModifier(1 / scalar);
        }
    }
}
