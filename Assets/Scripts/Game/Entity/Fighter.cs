using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fighter : Unit
{
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float maxAttackDist = 2f;
    [SerializeField] private float autoAttackRadius = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private Interactable assignedEnemy;
    private bool canAttack = true;
    private WaitForSeconds cooldownWaitForSeconds;

    void Awake()
    {
        cooldownWaitForSeconds = new WaitForSeconds(attackCooldown);
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
        if (hoveredInteractables.Count > 0 && hoveredInteractables[0].PlayerId != playerId && hoveredInteractables[0].PlayerId != -1)
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

        Vector3 enemyPos = assignedEnemy.transform.position;
        float enemyDistSqr = Vector3.SqrMagnitude(transform.position - enemyPos);
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
            transform.position = Vector2.MoveTowards(transform.position, enemyPos, moveSpeed * Time.deltaTime);
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
        List<Interactable> interactablesInAutoAttackRadius = Physics2D.OverlapCircleAll(transform.position, autoAttackRadius).Select(collider => collider.gameObject.GetComponent<Interactable>()).Where(interactable => interactable.PlayerId != playerId && interactable.PlayerId != -1).ToList();
        Interactable closestEnemy = Helper.GetNearestInteractable(interactablesInAutoAttackRadius, transform.position);
        if (closestEnemy != null)
        {
            Attack(closestEnemy);
        }
    }

    private void HandleEnemyDeath()
    {
        StopAttacking();
    }
}
