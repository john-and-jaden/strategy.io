﻿using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : Interactable
{
    [SerializeField] private float softCollisionRadius = 0.5f;
    [SerializeField] protected float moveSpeed = 1f;

    protected UnitState state;
    public UnitState State { get { return state; } }

    private Vector3 targetPos;
    public Vector3 TargetPos
    {
        get { return targetPos; }
        set { targetPos = value; }
    }

    private Collider2D[] softCollisionTargets;

    protected override void Awake()
    {
        health = maxHealth;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        targetPos = transform.position;
        state = UnitState.IDLE;
    }

    protected virtual void Update()
    {
        if (state == UnitState.RELOCATING) UpdateRelocate();
    }

    void FixedUpdate()
    {
        // Shift out of the way of moving units
        if (state == UnitState.IDLE)
        {
            Vector3 shiftDir = Vector3.zero;

            // Implement a soft collision radius
            softCollisionTargets = Physics2D.OverlapCircleAll(transform.position, softCollisionRadius);
            foreach (Collider2D col in softCollisionTargets)
            {
                Vector3 colPos = col.transform.position;
                Vector3 colDir = (transform.position - colPos).normalized;
                shiftDir = (shiftDir + colDir).normalized;
            }

            transform.position += shiftDir * moveSpeed * Time.deltaTime;
        }
    }

    public void Relocate(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        state = UnitState.RELOCATING;
    }

    public virtual void Interact(Vector3 targetPos)
    {
        Relocate(targetPos);
    }

    protected void UpdateRelocate()
    {
        float targetDistSqr = Vector3.SqrMagnitude(transform.position - targetPos);
        if (targetDistSqr < GameManager.UnitSystem.GroupRadiusSqr)
        {
            state = UnitState.IDLE;
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    protected bool IsEnemy(Interactable target)
    {
        return target.PlayerId != playerId && target.PlayerId != -1;
    }
}
