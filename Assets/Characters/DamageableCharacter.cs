using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Interfaces;
using TMPro;

namespace Assets.Characters
{
    public class DamageableCharacter : MonoBehaviour, IDamageable
    {
        //public GameObject healthText;
        public bool disableSimulation = false;
        public bool canTurnInvincible = false;
        public float invincibilityTime = 0.25f;
        private float invincibleTimeElapsed = 0f;

        Rigidbody2D rb;
        Collider2D physicsCollider;
        private Stats stats;

        public float _health;
        public float Health
        {
            set
            {
                //if (value < _health)
                //{
                //    //RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
                //    //textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                //    //Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                //    //textTransform.SetParent(canvas.transform);
                //}

                _health = value;


                if (_health <= 0)
                {
                    //animator.SetBool("IsAlive", false);
                    //isAlive = false;
                    Targetable = false;
                }
            }
            get
            {
                return _health;
            }
        }

        bool _targetable = true;
        public bool Targetable
        {
            get
            {
                return _targetable;
            }
            set
            {
                _targetable = value;
                if (disableSimulation)
                {
                    rb.simulated = false;
                }
                physicsCollider.enabled = value;
            }
        }

        public bool _invincible = false;
        public bool Invincible 
        { 
            get => _invincible; 
            set 
            {
                _invincible = value;
                if(_invincible == true)
                {
                    invincibleTimeElapsed = 0f;
                }
            } 
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            physicsCollider = GetComponent<Collider2D>();
            stats = GetComponent<Stats>();
            Health = stats.health;

            //Debug.Log("[DamCharacter] health:" + Health);
        }

        public void OnHit(float damage, Vector2 knockback)
        {
            if(!Invincible)
            {
                Health -= damage;

                // Apply force to the enemy
                rb.AddForce(knockback);

                if(canTurnInvincible)
                {
                    // Activate invincibility and timer
                    Invincible = true;
                }
            }
        }

        void IDamageable.OnHit(float damage)
        {
            if(!Invincible)
            {
                Health -= damage;

                if (canTurnInvincible)
                {
                    // Activate invincibility and timer
                    Invincible = true;
                }
            }
        }

        void IDamageable.OnObjectDestroyed()
        {
            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            if(Invincible)
            {
                invincibleTimeElapsed += Time.deltaTime;

                if(invincibleTimeElapsed > invincibilityTime)
                {
                    Invincible = false;
                }
            }
        }
    }
}
