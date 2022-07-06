using Assets.Scripts.Interfaces;

public class CrystalWeapon : IWeapon
{
    private float CrystalDefaultDamage = 5f;
    public float AttackDamage { get; }
    public bool InUse { get => false; }
    public bool isDPSType { get => false; }

    public CrystalWeapon(float damage)
    {
        AttackDamage = damage;
    }

    public CrystalWeapon()
    {
        AttackDamage = CrystalDefaultDamage;
    }

}
