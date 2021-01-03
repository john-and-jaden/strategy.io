using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit
{
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float maxAttackDist = 2f;

    private Interactable assignedEnemy;

    new protected void Update()
    {
        if (state == UnitState.ATTACKING) UpdateAttack();
        base.Update();
    }

    private void UpdateAttack()
    {
        if (assignedEnemy == null) return;

        Vector3 enemyPos = assignedEnemy.transform.position;
        float enemyDistSqr = Vector3.SqrMagnitude(transform.position - enemyPos);
        if (enemyDistSqr < maxAttackDist * maxAttackDist)
        {
            assignedEnemy.TakeDamage(attackRate * damageAmount * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyPos, moveSpeed * Time.deltaTime);
        }
    }

    public override void Interact(Vector3 targetPos)
    {
        List<Interactable> hoveredInteractables = GameManager.SelectionSystem.HoverTargets;
        if (hoveredInteractables.Count > 0 && hoveredInteractables[0].playerId != playerId && hoveredInteractables[0].playerId != -1)
        {
            assignedEnemy = hoveredInteractables[0];
            state = UnitState.ATTACKING;
        }
        else
        {
            StopAttacking();
            base.Interact(targetPos);
        }
    }

    private void StopAttacking()
    {
        if (assignedEnemy == null) return;
        // assignedEnemy.RemoveDestroyedListener(HandleResourceDestruction);
        assignedEnemy = null;
    }
}
