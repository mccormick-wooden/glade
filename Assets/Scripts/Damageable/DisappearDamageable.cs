using Assets.Scripts.Abstract;

namespace Assets.Scripts.Damageable
{
    public class DisappearDamageable : BaseDamageable
    {
        protected override void Die()
        {
            gameObject.SetActive(false);
            base.Die();
        }
    }
}
