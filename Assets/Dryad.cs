using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Interfaces;
using Spine.Unity;
using Spine;
using UnityEngine.SceneManagement;

public class Dryad : MonoBehaviour, IDamageable
{
    float secondAttacking = 0.0f;

    public bool disableSimulation = false;
    public bool canTurnInvincible = false;
    public float invincibilityTime = 0.25f;
    private float invincibleTimeElapsed = 0f;

    public float hitTime = 1f;
    private float hitTimeElapsed = 0f;

    public float deadTime = 1f;
    private float deadTimeElapsed = 0f;

    public float atkRange = 0.5f;
    public float damage = 1.0f;
    bool isDamageable = false;

    public int expValue = 10;

    Collider2D physicsCollider;
    private Stats stats;

    //public float damage = 1;
    [SerializeField] FloatingHealthBar healthBar;

    public DetectionZone detectionZone;
    public AttackZone attackZone;
    public float moveSpeed = 500f;
    private bool canMove = true;

    public EnemyAutoAttack attackHitBox;
    //private Collider2D attackCollider;

    Rigidbody2D rb;
    //public Collider2D enemyCollider;

    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;
    bool isFlipped = false;

    private bool _isAttack = false;
    public bool IsAttack
    {
        get { return _isAttack; }
        set
        {
            if (_isAttack == false && value == true)
            {
                TrackEntry trackEntry = animationState.SetAnimation(0, "attack/melee/normal-attack", true);
                trackEntry.Start += OnSpineAnimationStart;
                trackEntry.Complete += OnSpineAnimationComplete;
                Attack();
                //skeletonAnimation.AnimationState.SetAnimation(0, "attack/melee/goo-split", true);
                //attackHitBox.enabled = true;
            }
            if (_isAttack == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "action/move-forward", true);
                attackHitBox.StopAttack();
            }
            _isAttack = value;
        }
    }

    private bool _isHit = false;
    public bool IsHit
    {
        get { return _isHit; }
        set
        {
            if (_isHit == false && value == true)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "defense/hit-by-normal", false);
                hitTimeElapsed = 0f;
                secondAttacking = 0f;
            }
            if (_isHit == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "action/move-forward", true);
            }
            _isHit = value;
        }
    }

    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
        set
        {
            if (_isDead == false && value == true)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "defense/hit-die", false);
                deadTimeElapsed = 0f;
            }
            _isDead = value;
        }
    }

    public float _health;
    public float Health
    {
        set
        {
            if (value < _health)
            {
                IsHit = true;
            }

            _health = value;


            if (_health <= 0)
            {
                IsDead = true;
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
            if (_invincible == true)
            {
                invincibleTimeElapsed = 0f;
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;

        //skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        skeletonAnimation.AnimationState.SetAnimation(0, "action/move-forward", true);

        physicsCollider = GetComponent<Collider2D>();
        stats = GetComponent<Stats>();
        Health = stats.health;
        damage = stats.damage;
        isDamageable = false;

        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(Health, stats.health);

        //attackCollider = attackHitBox.GetComponent<Collider2D>();
        attackHitBox.enabled = false;
        attackHitBox.AttackDamage = stats.damage;
    }

    public void OnSpineAnimationComplete(TrackEntry trackEntry)
    {
        //Debug.Log("[Slime] complete");
        isDamageable = true;
    }

    public void OnSpineAnimationStart(TrackEntry trackEntry)
    {
        //Debug.Log("[Slime] start");
        isDamageable = false;
    }

    //private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
    //{
    //    // This method is called when the animation completes
    //    string animationName = trackEntry.Animation.Name;

    //    // Check the animation name if you need to take specific actions && Check if this is the event at the end of your loop
    //    // Do something specific for this animation
    //    // Perform your action or call your method here
    //    // This event is triggered at the end of your loop for "YourAnimationName"
    //    if (animationName == "attack/melee/bounce" && e.Data.Name == "EndLoopEventName")
    //    {
    //        Debug.Log("[Slime] add animation event");
    //        //Destroy(gameObject);
    //    }
    //}

    public void Flip(bool flip)
    {
        // Get the local scale of the GameObject
        Vector3 localScale = transform.localScale;

        // Flip the X scale to -1 if flip is true, or 1 if flip is false
        localScale.x = flip ? -0.16f : 0.16f;

        // Apply the new local scale
        transform.localScale = localScale;
    }

    private void Update()
    {
        if (attackZone.detectedObjs.Count > 0)
        {
            IsAttack = true;
            Vector3 localScale = transform.localScale;
            if (localScale.x < 0)
                attackHitBox.AttackLeft();
            else if (localScale.x > 0)
                attackHitBox.AttackRight();
            canMove = false;
        }
        else
        {
            IsAttack = false;
            canMove = true;
        }

        if (secondAttacking > 0)
        {
            secondAttacking -= Time.deltaTime;
            if (secondAttacking <= 0)
            {
                //APPLY DAMAGE TO NEAREST AXIE
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    float distance = Vector3.Distance(player.transform.position, transform.position);
                    //Debug.Log(distance);
                    if (distance < atkRange)
                    {
                        IDamageable comp = player.GetComponent<IDamageable>();
                        if (comp != null && !comp.Invincible)
                        {
                            comp.OnHit(damage);
                        }
                    }
                }
                secondAttacking = 0;
            }
        }
        healthBar.UpdateHealthBar(_health, stats.health);

    }
    void Attack()
    {
        secondAttacking = 0.75f;
    }

    private void LateUpdate()
    {
        isDamageable = false;
    }

    private void FixedUpdate()
    {
        if (Targetable && detectionZone.detectedObjs.Count > 0)
        {
            // Calculate direction to target object
            Vector2 direction = (detectionZone.detectedObjs[0].transform.position - transform.position).normalized;

            // Move toward detected object
            if (canMove)
            {
                rb.AddForce(direction * moveSpeed * Time.deltaTime);
            }

            // Check if the input direction is negative (left) or positive (right)
            direction = (detectionZone.detectedObjs[0].transform.position - transform.position).normalized;
            if (direction.x < 0)
            {
                isFlipped = false;
            }
            else if (direction.x > 0)
            {
                isFlipped = true;
            }
            Flip(isFlipped);
        }


        if (Invincible)
        {
            invincibleTimeElapsed += Time.deltaTime;

            if (invincibleTimeElapsed > invincibilityTime)
            {
                Invincible = false;
            }
        }

        if (IsHit)
        {
            hitTimeElapsed += Time.deltaTime;

            if (hitTimeElapsed > hitTime)
            {
                IsHit = false;
            }
        }

        if (IsDead)
        {
            deadTimeElapsed += Time.deltaTime;

            if (deadTimeElapsed > deadTime)
            {
                Application.Quit();
            }
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    IDamageable damageable = collision.collider.GetComponent<IDamageable>();

    //    if (damageable != null)
    //    {
    //        damageable.OnHit(damage);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Enemy] No damageable");
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    IDamageable damageable = collision.GetComponent<IDamageable>();

    //    if (damageable != null)
    //    {
    //        damageable.OnHit(damage);
    //    }
    //}

    public void OnHit(float damage, Vector2 knockback)
    {
        if (!Invincible)
        {
            Health -= damage;

            // Apply force to the enemy
            rb.AddForce(knockback);

            if (canTurnInvincible)
            {
                // Activate invincibility and timer
                Invincible = true;
            }
        }
    }

    public void OnHit(float damage)
    {
        if (!Invincible)
        {
            Health -= damage;

            if (canTurnInvincible)
            {
                // Activate invincibility and timer
                Invincible = true;
            }
        }
    }

    public void OnObjectDestroyed()
    {
        Destroy(gameObject);
    }

    public float GetHealth()
    {
        return Health;
    }

    public int GetExp()
    {
        return expValue;
    }
}
