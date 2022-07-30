using Assets.Scripts.Interfaces;
using UnityEngine;

public class TreeSpirit : MonoBehaviour
{
    /// <summary>
    /// The playerGameObjectRootName must be the name of the physical player model.
    /// </summary>
    [SerializeField]
    private string playerModelGameObjectRootName = "PlayerModel";

    private GameObject playerModel;
    private IDamageable playerDamageable;
    private GameObject ent;
    private Animator animator;
    private GladeHealthManager gladeHealthManager;
    
    private void Awake()
    {
        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));

        playerDamageable = playerModel.GetComponent<IDamageable>();
        Utility.LogErrorIfNull(playerDamageable, nameof(playerDamageable));
        playerDamageable.IsHealable = true;

        ent = GameObject.Find("Ent");
        Utility.LogErrorIfNull(ent, nameof(ent));

        animator = GetComponent<Animator>();
        Utility.LogErrorIfNull(animator, nameof(animator));

        gladeHealthManager = GameObject.Find("GladeHealthManager").GetComponent<GladeHealthManager>();
        Utility.LogErrorIfNull(gladeHealthManager, nameof(gladeHealthManager));
    }

    private void Update()
    {
        OrientTowardsPlayer();

        if (playerDamageable.CurrentHp <= playerDamageable.MaxHp / 2)
            HealPlayer();

        if (gladeHealthManager.CurrentHP <= gladeHealthManager.MaxHP / 2)
            HealGlade();
    }

    private void OrientTowardsPlayer()
    {
        var lookAtThis = new Vector3 { x = playerModel.transform.position.x, y = transform.position.y, z = playerModel.transform.position.z };
        transform.LookAt(lookAtThis);
        ent.transform.LookAt(lookAtThis);
    }

    private void HealPlayer()
    {
        playerDamageable.Heal(playerDamageable.MaxHp - playerDamageable.CurrentHp);
        animator.SetTrigger("Heal");
    }

    private void HealGlade()
    {
        gladeHealthManager.UpdateGladeHealth(100);
        animator.SetTrigger("Heal");
    }
}
