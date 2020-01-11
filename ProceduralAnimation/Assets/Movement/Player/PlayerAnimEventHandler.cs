using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    private PlayerHitbox playerHitbox;

    [SerializeField] private AnimHandler animHandler;

    private void Start()
    {
        playerHitbox = hitbox.GetComponent<PlayerHitbox>();
    }

    // Hitbox
    public void AEDeactivateHitbox()
    {
        hitbox.SetActive(false);
    }

    public void AEActivateHitbox()
    {
        hitbox.SetActive(true);
        playerHitbox.SetDamage(animHandler.GetCurrentCombo());
    }

    // Attack State
    public void AEListenForAttack()
    {
        animHandler.attackState = 1;
    }

    public void AELeaveAttacking()
    {
        animHandler.StopAttacking();
    }

    public void AELeaveAttackState()
    {
        animHandler.LeaveAttackState();
    }
}
