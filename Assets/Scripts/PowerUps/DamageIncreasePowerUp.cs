using PlayerBehaviors;

namespace PowerUps
{
    public class DamageIncreasePowerUp : BasePowerUp
    {
        private float scalar = 1.1f;

        public DamageIncreasePowerUp()
        {
            Description = "Increase your damage output by 10%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.ScaleDamageModifier(scalar);
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.ScaleDamageModifier(scalar);
        }
    }
}
