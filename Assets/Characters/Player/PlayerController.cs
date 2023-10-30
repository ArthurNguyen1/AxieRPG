using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Interfaces;
using Spine.Unity;
using UnityEngine.SceneManagement;

// Takes and handles input and movement for a player character
public class PlayerController : MonoBehaviour, IDamageable
{
    public bool disableSimulation = false;
    public bool canTurnInvincible = false;
    public float invincibilityTime = 0.25f;
    private float invincibleTimeElapsed = 0f;

    public float hitTime = 1f;
    private float hitTimeElapsed = 0f;

    public float levelUpTime = 1f;
    private float levelUpTimeElapsed = 0f;

    Collider2D physicsCollider;
    public Stats stats;

    public float moveSpeed = 5.0f;

    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    private Rigidbody2D rb;

    private Vector2 movementInput;
    private bool canMove = true;

    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private SkeletonAnimation skeletonAnimation;
    private bool isFlipped = false;

    private bool _isHit = false;
    public bool IsHit
    {
        get { return _isHit; }
        set
        {
            if (_isHit == false && value == true)
            {
                //RunAnimationHit();
                skeletonAnimation.AnimationState.SetAnimation(0, "defense/hit-by-normal", false);
                hitTimeElapsed = 0f;
            }
            if (_isHit == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "activity/appear", true);
            }
            _isHit = value;
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
                Targetable = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            stats.TargetHealth = _health; 
            stats.CurrentHealth = _health;
        }
        get
        {
            return _health;
        }
    }
    private bool _isLevelUp = false;
    public bool IsLevelUp
    {
        get { return _isLevelUp; }
        set
        {
            if (_isLevelUp == false && value == true)
            {
                //RunAnimationHit();
                skeletonAnimation.AnimationState.SetAnimation(0, "activity/evolve", false);
                levelUpTimeElapsed = 0f;
            }
            if (_isLevelUp == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "activity/appear", true);
            }
            _isLevelUp = value;
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get a reference to the SkeletonAnimation component on your GameObject
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.SetAnimation(0, "activity/appear", true);

        physicsCollider = GetComponent<Collider2D>();
        stats = GetComponent<Stats>();
        Health = stats.health;
    }

    private void Update()
    {
        //// Control the animation playback
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    // Play an animation by its name
        //    if (skeletonAnimation.AnimationName != null)
        //        skeletonAnimation.AnimationState.SetAnimation(0, "activity/bath", true);
        //    else
        //        print("No animation name match");
        //}

        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            IDamageable comp = enemy.GetComponent<IDamageable>();
            if (comp != null && comp.GetHealth() <= 0)
            {
                stats.CurrentExp += comp.GetExp() * Time.deltaTime;
                stats.damage += 1 * Time.deltaTime;
                stats.CurrentHealth += 10 * Time.deltaTime;
                stats.TargetHealth += 10 * Time.deltaTime;
            }
        }

        if(stats.CurrentExp >= 100)
        {
            stats.CurrentExp = stats.CurrentExp % 100;
            IsLevelUp = true;
        }
    }

    private void Flip(bool flip)
    {
        // Get the local scale of the GameObject
        Vector3 localScale = transform.localScale;

        // Flip the X scale to -1 if flip is true, or 1 if flip is false
        localScale.x = flip ? -0.16f : 0.16f;

        // Apply the new local scale
        transform.localScale = localScale;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            // If movement input is not 0, try to move
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);

                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if (!success)
                {
                    TryMove(new Vector2(0, movementInput.y));
                }

                //animator.SetBool("IsMoving", success);
            }
            else
            {
                //animator.SetBool("IsMoving", false);
            } 

            // Check if the input direction is negative (left) or positive (right)
            if (movementInput.x < 0)
            {
                isFlipped = false;
            }
            else if (movementInput.x > 0)
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

        if (IsLevelUp)
        {
            levelUpTimeElapsed += Time.deltaTime;

            if (levelUpTimeElapsed > levelUpTime)
            {
                IsLevelUp = false;
                stats.level += 1;
                stats.health += 20;
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        //Can't move if there's no direction to move
        if (direction == Vector2.zero)
        {
            return false;
        }

        // Check for potential collisions
        int count = rb.Cast(
            direction, //X and Y values between -1 and 1 that represent the direction from the body to look for collisions
            movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
            castCollisions, // List of collisions to store the found collisions into after the Cast in finished
            moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

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

            stats.TakeDamage(gameObject, damage);
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

            stats.TakeDamage(gameObject, damage);
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
        throw new System.NotImplementedException();
    }
}
