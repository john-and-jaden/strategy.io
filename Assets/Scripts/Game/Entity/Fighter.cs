using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Fighter : Unit
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float maxAttackDist = 1f;
    [SerializeField] private float autoAttackRadius = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask attackMask;

    private Interactable assignedEnemy;
    private bool canAttack = true;
    private WaitForSeconds cooldownWaitForSeconds;

    new void Awake()
    {
        cooldownWaitForSeconds = new WaitForSeconds(attackCooldown);
        base.Awake();
    }

    new protected void Update()
    {
        if (state == UnitState.ATTACKING) UpdateAttack();
        else if (state == UnitState.IDLE) SeekNearbyEnemies();
        base.Update();
    }

    public override void Interact(Vector3 targetPos)
    {
        List<Interactable> hoveredInteractables = GameManager.SelectionSystem.GetHoverTargetsOfType<Interactable>();
        if (hoveredInteractables.Count > 0 && IsEnemy(hoveredInteractables[0]))
        {
            Attack(hoveredInteractables[0]);
        }
        else
        {
            StopAttacking();
            base.Interact(targetPos);
        }
    }

    private void Attack(Interactable enemy)
    {
        assignedEnemy = enemy;
        assignedEnemy.AddDeathListener(HandleEnemyDeath);
        state = UnitState.ATTACKING;
    }

    private void StopAttacking()
    {
        if (assignedEnemy == null) return;
        assignedEnemy.RemoveDeathListener(HandleEnemyDeath);
        assignedEnemy = null;
        state = UnitState.IDLE;
    }

    private void UpdateAttack()
    {
        if (assignedEnemy == null) return;

        float enemyDistSqr = Utils.GetSqrDistance(this, assignedEnemy);
        if (enemyDistSqr < maxAttackDist * maxAttackDist)
        {
            if (canAttack)
            {
                StartCoroutine(AttackCooldown());
                assignedEnemy.TakeDamage(damageAmount);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, assignedEnemy.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return cooldownWaitForSeconds;
        canAttack = true;
    }

    private void SeekNearbyEnemies()
    {
        Interactable enemy = Utils.GetNearest<Interactable>(transform.position, autoAttackRadius, attackMask, (t) => IsEnemy(t));
        if (enemy != null)
        {
            Attack(enemy);
        }
    }

    private void HandleEnemyDeath()
    {
        StopAttacking();
    }
}
