using Assets.Scripts.Abstract;

public class AngryChestBump : BaseWeapon 
{
    protected override void Start()
    {
        InUse = true; // always be bumpin 
    }
}
