using PlayerBehaviors;

namespace PowerUps
{
    public class DamageResistPowerUp : BasePowerUp
    {
        private float scalar = 1.1f;

        public DamageResistPowerUp()
        {
            Description = "Increase your damage resistance by 10%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.ScaleDamageResistanceModifier(scalar);
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.ScaleDamageResistanceModifier(scalar);
        }
    }
}
