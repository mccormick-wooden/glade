using PlayerBehaviors;

namespace PowerUps
{
    public class DamageIncreasePowerUp : BasePowerUp
    {
        public DamageIncreasePowerUp()
        {
            Description = "Increase your damage output by 10%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.damageModifier *= 1.1f;
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.damageModifier /= 1.1f;
        }
    }
}
