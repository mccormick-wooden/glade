using Assets.Scripts.Abstract;

public class AngryChestBump : BaseWeapon 
{


    protected override void Start()
    {
        TargetTags = new string[] { "Player" }; // I'm wondering if beacons should have a different tag?    
        InUse = true; // always be bumpin 
    }
}
