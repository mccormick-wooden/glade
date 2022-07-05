using PlayerBehaviors;

namespace PowerUps
{
    public class DamageIncreasePowerUp : BasePowerUp
    {
        private float summand = .1f;

        public DamageIncreasePowerUp()
        {
            Description = "Increase your damage output by 10%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.UpdateDamageModifier(summand);
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.UpdateDamageModifier(summand);
        }
    }
}
