using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Interfaces;

public class TriggeredAttack : MonoBehaviour
{
    public Collider2D skillCollider;
    private float _attackDamage = 1;
    public float AttackDamage
    {
        get { return _attackDamage; }
        set { _attackDamage = value; }
    }
    public float knockbackForce = 5000f;
    Vector2 rightAttackOffset;

    private void Start()
    {
        rightAttackOffset = transform.position;
    }

    public void AttackRight()
    {
        //print("Attack Right");
        skillCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        //print("Attack Left");
        skillCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack()
    {
        skillCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.tag == "Enemy")
        //{
        //    // Deal damage to the enemy
        //    Enemy enemy = collision.GetComponent<Enemy>();

        //    if(enemy != null)
        //    {
        //        enemy.Health -= damage;
        //    }
        //}

        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            Vector3 parentPosition = gameObject.GetComponentInParent<Transform>().position;
            Vector2 direction = (Vector2)(collision.gameObject.transform.position - parentPosition).normalized;
            Vector2 knockback = direction * knockbackForce;

            //collision.SendMessage("OnHit", damage, knockback);
            damageableObject.OnHit(AttackDamage, knockback);
        }
        else
        {
            Debug.LogWarning("[SkillAttack] No damageableObject");
        }
    }
}
