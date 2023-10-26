using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Interfaces;

public class EnemyAutoAttack : MonoBehaviour
{
    public Collider2D skillCollider;
    private float _attackDamage = 1;
    public float AttackDamage
    {
        get { return _attackDamage; }
        set { _attackDamage = value; }
    }
    Vector2 rightAttackOffset;

    private void Start()
    {
        rightAttackOffset = transform.position;
    }

    public void AttackRight()
    {
        //print("[Slime] Attack Right");
        skillCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        //print("[Slime] Attack Left");
        skillCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        skillCollider.enabled = false;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //if(collision.tag == "Enemy")
    //    //{
    //    //    // Deal damage to the enemy
    //    //    Enemy enemy = collision.GetComponent<Enemy>();

    //    //    if(enemy != null)
    //    //    {
    //    //        enemy.Health -= damage;
    //    //    }
    //    //}

    //    Debug.Log("[slime] hitting");

    //    IDamageable damageableObject = collision.GetComponent<IDamageable>();
    //    if (damageableObject != null)
    //    {
    //        damageableObject.OnHit(AttackDamage);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[EnemyAutoAttack] No damageableObject");
    //    }
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            Debug.Log("Get into here");
            damageableObject.OnHit(AttackDamage);
        }
        else
        {
            Debug.LogWarning("[EnemyAutoAttack] No damageableObject");
        }
    }
}
