using System;
using Assets.Scripts.Abstract;

public class MushroomExplosionWeapon : BaseWeapon
{
    private string[] validCollideTags = new string[] { "Player" };
    DateTime creationTime;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        TargetTags = new string[] { "Player" };
        InUse = true;
        creationTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (DateTime.Now > creationTime.AddSeconds(1))
        {
            Destroy(gameObject);
        }
    }
}
