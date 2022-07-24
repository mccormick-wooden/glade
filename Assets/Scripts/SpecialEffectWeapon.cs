using Assets.Scripts.Abstract;

public class SpecialEffectWeapon : BaseWeapon
{
    private readonly string[] _specialEffectTargetTags = new string[] { "Enemy", "Damageable" };

    protected override void Start()
    {
        InUse = true;
        isDPSType = false;
        TargetTags = _specialEffectTargetTags;
    }
}
