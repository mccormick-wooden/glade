namespace Assets.Scripts.Interfaces
{
    public interface IWeapon
    {
        // Perhaps also should enforce a collider and rigidbody?
        float AttackDamage { get; }
        bool InUse { get; }

        bool isDPSType { get; }
    }
}
