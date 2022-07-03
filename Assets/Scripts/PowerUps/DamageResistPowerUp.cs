using PlayerBehaviors;

namespace PowerUps
{
    public class DamageResistPowerUp : BasePowerUp
    {
        public DamageResistPowerUp()
        {
            Description = "Increase your damage resistance by 10%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.damageResistanceModifier *= 1.1f;
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.damageResistanceModifier /= 1.1f;
        }
    }
}
