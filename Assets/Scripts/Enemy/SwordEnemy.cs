using Assets.Scripts.Abstract;
using UnityEngine;

public class SwordEnemy : BaseEnemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        weapon = GetComponentInChildren<BaseWeapon>();
        Utility.LogErrorIfNull(weapon, nameof(weapon), EnemyId);
        weapon.TargetTags = new string[] { "Player" };

        attackDelayTimeSeconds = 2.0f;

        desireToAttackPlayer = 4f;  //0.75f;
        desireToDefendBeacon = 4f;  //0.125f;
        desireToRunAndHeal = 1f;    //0.125f;
        desireToHealOthers = 0f;

        autoAttackPlayerDistanceToBeacon = 5f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateAnimations();
    }

    protected override void UpdateAnimations()
    {
        animator.SetFloat("Speed", velocityReporter.velocity.magnitude);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("MovementTree"))
        {
            weapon.InUse = false;

            if (isAttacking)
            {
                animator.SetBool("DoAttack", isAttacking);
                isAttacking = !isAttacking;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwordSlash"))
        {
            weapon.InUse = true;
            animator.SetBool("DoAttack", false);
        }
    }


    public void EmitFirstSwordSlash()
    {
        EventManager.TriggerEvent<SwordSwingEvent, Vector3, int>(transform.position, 0);
    }
}
