using PlayerBehaviors;

namespace PowerUps
{
    public class DamageResistPowerUp : BasePowerUp
    {
        private float summand = .1f;

        public DamageResistPowerUp()
        {
            Description = "Increase your damage resistance by 10%";
        }

        public override void ApplyPowerUp()
        {
            base.ApplyPowerUp();
            PlayerStats.Instance.UpdateDamageResistanceModifier(summand);
        }

        public override void RepealPowerUp()
        {
            PlayerStats.Instance.UpdateDamageResistanceModifier(-summand);
        }
    }
}
