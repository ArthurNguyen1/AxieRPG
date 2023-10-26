using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class Abilities : MonoBehaviour
{
    private Stats _stats;

    private SkeletonAnimation skeletonAnimation;
    private PlayerController playerController;

    [Header("Ability 1")]
    public Image abilityImage1;
    public Text abilityText1;
    public KeyCode ability1Key;
    public float ability1Cooldown = 5;
    public TriggeredAttack ability1HitBox;
    private Collider2D ability1Collider;

    [Header("Ability 2")]
    public Image abilityImage2;
    public Text abilityText2;
    public KeyCode ability2Key;
    public float ability2Cooldown = 7;
    public TriggeredAttack ability2HitBox;
    private Collider2D ability2Collider;

    [Header("Ability 3")]
    public Image abilityImage3;
    public Text abilityText3;
    public KeyCode ability3Key;
    public float ability3Cooldown = 7;
    public TriggeredAttack ability3HitBox;
    private Collider2D ability3Collider;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;


    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;

    // Property ability 1
    private bool _isHornAttack = false;
    public bool IsHornAttack
    {
        get { return _isHornAttack; }
        set
        {
            if (_isHornAttack == false && value == true)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "attack/melee/horn-gore", false);
            }
            if (_isHornAttack == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "activity/appear", true);
            }
            _isHornAttack = value;
        }
    }

    // Property ability 2
    private bool _isMouthAttack = false;
    public bool IsMouthAttack
    {
        get { return _isMouthAttack; }
        set
        {
            if (_isMouthAttack == false && value == true)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "attack/melee/mouth-bite", false);
            }
            if (_isMouthAttack == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "activity/appear", true);
            }
            _isMouthAttack = value;
        }
    }

    // Property ability 3
    private bool _isTailAttack = false;
    public bool IsTailAttack
    {
        get { return _isTailAttack; }
        set
        {
            if (_isTailAttack == false && value == true)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "attack/melee/tail-thrash", false);
            }
            if (_isTailAttack == true && value == false)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "activity/appear", true);
            }
            _isTailAttack = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;


        abilityText1.text = "";
        abilityText2.text = "";
        abilityText3.text = "";

        _stats = GetComponent<Stats>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        playerController = GetComponent<PlayerController>();

        ability1Collider = ability1HitBox.GetComponent<Collider2D>();
        ability1HitBox.enabled = false;

        ability2Collider = ability2HitBox.GetComponent<Collider2D>();
        ability2HitBox.enabled = false;

        ability3Collider = ability3HitBox.GetComponent<Collider2D>();
        ability3HitBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ability1Input();
        Ability2Input();
        Ability3Input();

        AbilityCooldown(ref currentAbility1Cooldown, ability1Cooldown, ref isAbility1Cooldown, abilityImage1, abilityText1);
        AbilityCooldown(ref currentAbility2Cooldown, ability2Cooldown, ref isAbility2Cooldown, abilityImage2, abilityText2);
        AbilityCooldown(ref currentAbility3Cooldown, ability3Cooldown, ref isAbility3Cooldown, abilityImage3, abilityText3);

        // ability 1
        if(ability1Cooldown - currentAbility1Cooldown >= 0.5)
        {
            ability1HitBox.StopAttack();
        }
        if (ability1Cooldown - currentAbility1Cooldown >= 1 && IsHornAttack == true)
        {
            playerController.UnlockMovement();
            IsHornAttack = false;
        }

        // ability 2
        if(ability2Cooldown - currentAbility2Cooldown >= 0.5)
        {
            ability2HitBox.StopAttack();
        }
        if (ability2Cooldown - currentAbility2Cooldown >= 1 && IsMouthAttack == true)
        {
            playerController.UnlockMovement();
            IsMouthAttack = false;
        }

        // ability 3
        if(ability3Cooldown - currentAbility3Cooldown >= 0.5)
        {
            ability3HitBox.StopAttack();
        }
        if (ability3Cooldown - currentAbility3Cooldown >= 1 && IsTailAttack == true)
        {
            playerController.UnlockMovement();
            IsTailAttack = false;
        }

        //Debug.Log("[Ability] Animation:" + skeletonAnimation.AnimationName);

    }

    private void Ability1Input()
    {
        if(Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            isAbility1Cooldown = true;
            currentAbility1Cooldown = ability1Cooldown;

            playerController.LockMovement();
            ability1HitBox.AttackDamage = _stats.damage * 20 / 100;

            Vector3 localScale = transform.localScale;
            if (localScale.x < 0)
                ability1HitBox.AttackLeft();
            else if (localScale.x > 0)
                ability1HitBox.AttackRight();
            IsHornAttack = true;
        }
    }

    private void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown)
        {
            isAbility2Cooldown = true;
            currentAbility2Cooldown = ability2Cooldown;

            playerController.LockMovement();
            ability2HitBox.AttackDamage = _stats.damage * 40 / 100;

            Vector3 localScale = transform.localScale;
            if (localScale.x < 0)
                ability2HitBox.AttackLeft();
            else if (localScale.x > 0)
                ability2HitBox.AttackRight();
            IsMouthAttack = true;
        }
    }

    private void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown)
        {
            isAbility3Cooldown = true;
            currentAbility3Cooldown = ability3Cooldown;

            playerController.LockMovement();
            ability3HitBox.AttackDamage = _stats.damage * 80 / 100;

            Vector3 localScale = transform.localScale;
            if (localScale.x < 0)
                ability3HitBox.AttackLeft();
            else if (localScale.x > 0)
                ability3HitBox.AttackRight();
            IsTailAttack = true;
        }
    }

    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, Text skillText)
    {
        if(isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            //Debug.Log("[AbilityCooldown] currentCooldown: " + currentCooldown);

            if (skillImage != null)
            {
                skillImage.fillAmount = currentCooldown / maxCooldown;
            }
            if (skillText != null)
            {
                skillText.text = Mathf.Ceil(currentCooldown).ToString();
            }

            if (currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0f;
            }
        }
        else
        {
            if (skillImage != null)
            {
                skillImage.fillAmount = 0f;
            }
            if (skillText != null)
            {
                skillText.text = "";
            }
        }
    }
}
