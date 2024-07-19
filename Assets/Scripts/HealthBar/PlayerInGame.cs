using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInGame : MonoBehaviour
{

    public int maxHealth = 14;
    public int currentHealth;
    public int maxLuck = 5;
    public int currentLuck;

    public HealthBar healthBar;
    public LuckBar LuckBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = 10;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        currentLuck = 2;
        LuckBar.SetMaxLuck(maxLuck);
        LuckBar.SetLuck(currentLuck);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoseLife(1);
        }

        if (currentHealth == 0)
        {
            // end the adventure
        }
    }

    void GainLife(int life)
    {
        currentHealth += life;
        healthBar.SetHealth(currentHealth);
    }

    void LoseLife(int life)
    {
        currentHealth -= life;
        healthBar.SetHealth(currentHealth);
    }

    void GainLuck(int luck)
    {
        currentLuck += luck;
        LuckBar.SetLuck(currentLuck);
    }

    void LoseLuck(int luck)
    {
        currentLuck -= luck;
        LuckBar.SetLuck(currentLuck);
    }

}
