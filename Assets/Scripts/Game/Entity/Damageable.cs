using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : Selectable
{
    [SerializeField] private SpriteRenderer healthBarPrefab;

    protected SpriteRenderer healthBar;

    new protected void Start()
    {
        SpawnIndicators();
    }

    new protected void SpawnIndicators()
    {
        base.SpawnIndicators();
        healthBar = Instantiate(healthBarPrefab, GameManager.SelectionSystem.IndicatorParent);
        healthBar.transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    new protected void DestroyIndicators()
    {
        base.DestroyIndicators();
        Destroy(healthBar.gameObject);
    }
}
