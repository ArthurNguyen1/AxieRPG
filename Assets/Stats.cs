using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Base Stats")]
    public float health;
    public float damage;
    public int level;
    public int maxExp;

    //Health Slider Variables
    public float damageLerpDuration;
    private float currentHealth;
    private float targetHealth;
    private int currentExp;
    private Coroutine damageCoroutine;

    HealthUI healthUI;

    private void Awake()
    {
        healthUI = GetComponent<HealthUI>();

        currentHealth = health;
        targetHealth = health;

        healthUI.Start3DSlider(health);
        healthUI.Update2DSlider(health, currentHealth);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            TakeDamage(gameObject, damage);
        }
    }

    public void TakeDamage(GameObject target, float damageAmount)
    {
        Stats targetStats = target.GetComponent<Stats>();

        targetStats.targetHealth -= damageAmount;

        if(targetStats.targetHealth <= 0)
        {
            //Destroy(target.gameObject);
            CheckIfPlayerDead();
        }
        else if(targetStats.damageCoroutine == null)
        {
            targetStats.StartLerpHealth();
        }
    }

    private void CheckIfPlayerDead()
    {
        healthUI.Update2DSlider(health, 0);
    }

    private void StartLerpHealth()
    {
        if(damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(LerpHealth());
        }
    }

    private IEnumerator LerpHealth()
    {
        float elapsedTime = 0;
        float initialHealth = currentHealth;
        float target = targetHealth;

        while(elapsedTime < damageLerpDuration)
        {
            currentHealth = Mathf.Lerp(initialHealth, target, elapsedTime / damageLerpDuration);
            UpdateHealthUI();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentHealth = target;
        UpdateHealthUI();

        damageCoroutine = null;
    }

    private void UpdateHealthUI()
    {
        healthUI.Update2DSlider(health, currentHealth);
        healthUI.Update3DSlider(currentHealth);
    }
}
