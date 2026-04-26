using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;


public class Enemy_AI : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource enemyWalk;
    public AudioSource RoarAudio;
    public AudioSource JumpSFX;
    public AudioSource PunchSFX;
    public AudioSource ThrowSFX;
    public AudioSource EnemyDmgSFX;
    public AudioSource EnemyDeathSFX;

    private Animator animator;
    private Player_Controller player_Controller;
    private GameObject player;
    private NavMeshAgent Agent;
    private GameObject enemySight;

    [Header("Health & UI Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Image healthFillImage;      
    public TextMeshProUGUI healthText; 

    [Header("Split Settings")]
    public int generation = 0; 
    public int maxGenerations = 2; 
    public float splitOffset = 1.5f; 
    public GameObject deathSplitEffect; 
    private bool isDead = false; 

    [Header("Timing & Immunity")]
    public float startupDelay = 3f;
    private float startupTimer;

    [Header("State Machine Settings")]
    public float stomp_Distance = 3f; 
    public float Punch_Distance = 3f; 
    public float Throw_Distance = 3f; 
    public float visionRange = 20f;   

    [Header("Cooldown & Timing")]
    public float attackCooldown = 2f;    
    public float animationDuration = 1.5f; 
    
    private float cooldownTimer = 0f;
    private float lockoutTimer = 0f;

    [Header("Attack Effects")]
    public float DestroyTime = 1f;
    public GameObject stompEffect;
    private GameObject stompPOS;
    public GameObject punchEffect;
    private GameObject punchPOS;
    public GameObject throwProjectile;
    public GameObject rockSpawnPos;
    private GameObject x;
    private GameObject y;
    private GameObject z;

    [Header("Rock Throw Settings")]
    public float rockThrowSpeed = 17f; 
    private GameObject currentRock; 

    void Start()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateUI();

        startupTimer = startupDelay;

        stompPOS = gameObject.transform.Find("DustExplosionPos").gameObject;
        punchPOS = gameObject.transform.Find("EarthShatterPos").gameObject;
        enemySight = gameObject.transform.Find("Golem_Model").transform.Find("Enemy_Vision").gameObject;
        player = GameObject.Find("Player");
        player_Controller = player.GetComponent<Player_Controller>();
        Agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.transform.Find("Golem_Model").GetComponent<Animator>();

        if (!RoarAudio.isPlaying)
        {
            RoarAudio.Play();
        }

        if (generation > 0)
        {
            lockoutTimer = 0f; 
            cooldownTimer = attackCooldown;
        }
    }

    void Update()
    {

        if (Agent.velocity.magnitude > 0.1f && !Agent.isStopped)
        {
            if (!enemyWalk.isPlaying) 
            {
                enemyWalk.Play();
            }
        }
        else
        {
            enemyWalk.Stop();
        }


        if (startupTimer > 0)
        {
            startupTimer -= Time.deltaTime;
            Agent.isStopped = true;
            animator.SetBool("Moving", false);
            return; 
        }



        if (Input.GetKeyUp(KeyCode.P))
        {
            TakeDamage(50);     
        }

        if (currentHealth <= 0 || isDead) return;

        gameObject.transform.LookAt(player.transform);
        
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (lockoutTimer > 0)
        {
            lockoutTimer -= Time.deltaTime;
            Agent.isStopped = true;
            animator.SetBool("Moving", false);
            return; 
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= Punch_Distance && CanSeePlayer())
        {
            TriggerAttack("Punch", animationDuration, false);
        }
        else if (distanceToPlayer <= stomp_Distance && CanSeePlayer() && cooldownTimer <= 0)
        {
            TriggerAttack("Stomp_Attack", animationDuration, true);
        }
        else if (distanceToPlayer <= Throw_Distance && CanSeePlayer() && cooldownTimer <= 0)
        {
            TriggerAttack("Throw_Rock", animationDuration, true);
        }
        else
        {
            Agent.isStopped = false;
            animator.SetBool("Moving", true);
            Agent.SetDestination(player.transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0 || isDead) return;


        if (startupTimer > 0) return;

        currentHealth -= damage;
        if (!EnemyDmgSFX.isPlaying)
        {
            EnemyDmgSFX.Play();
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = ((currentHealth / maxHealth) * 100).ToString("F0") + "%";
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (Agent != null && Agent.isActiveAndEnabled)
        {
            Agent.isStopped = true;
        }
        
        animator.SetBool("Moving", false);
        animator.SetTrigger("Die");
        
        if (EnemyDeathSFX != null) EnemyDeathSFX.Play();

        if (deathSplitEffect != null)
        {
            GameObject fx = Instantiate(deathSplitEffect, transform.position, transform.rotation);
            Destroy(fx, 3f);
        }

        if (generation < maxGenerations)
        {
            SpawnSplits();
        }
        else
        {
            
            Invoke("CheckForRemainingEnemies", 3.1f); 
        }

        Destroy(gameObject, 3f);
    }

    private void CheckForRemainingEnemies()
    {
        
        Enemy_AI[] remainingEnemies = GameObject.FindObjectsByType<Enemy_AI>(FindObjectsInactive.Exclude);
        
        if (remainingEnemies.Length <= 1)
        {
            CompleteProtocol();
        }
    }

    public void CompleteProtocol()
    {
        SceneManager.LoadScene("The End 1");
    }
    private void SpawnSplits()
    {
        Vector3 spawnLeft = transform.position + (transform.right * -splitOffset);
        Vector3 spawnRight = transform.position + (transform.right * splitOffset);

        CreateSplitClone(spawnLeft);
        CreateSplitClone(spawnRight);
    }

    private void CreateSplitClone(Vector3 spawnPos)
    {
        GameObject clone = Instantiate(gameObject, spawnPos, transform.rotation);
        Enemy_AI cloneScript = clone.GetComponent<Enemy_AI>();

        cloneScript.generation = this.generation + 1;
        cloneScript.maxHealth = this.maxHealth * 0.5f;
        clone.transform.localScale = transform.localScale * 0.5f;

        Animator cloneAnimator = clone.transform.Find("Golem_Model").GetComponent<Animator>();
        if (cloneAnimator != null)
        {
            cloneAnimator.Rebind();
            cloneAnimator.Update(0f);
        }

        NavMeshAgent cloneAgent = clone.GetComponent<NavMeshAgent>();
        if (cloneAgent != null)
        {
            cloneAgent.radius *= 0.5f;
            cloneAgent.height *= 0.5f;
        }
    }

    void TriggerAttack(string triggerName, float duration, bool applyCooldown)
    {
        Agent.isStopped = true;
        gameObject.transform.LookAt(player.transform);
        animator.SetBool("Moving", false);
        animator.SetTrigger(triggerName);

        lockoutTimer = duration; 
        if (applyCooldown)
        {
            cooldownTimer = attackCooldown; 
        }
    }

    bool CanSeePlayer()
    {
        if (enemySight == null || player == null) return false;

        Vector3 directionToPlayer = (player.transform.position - enemySight.transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(enemySight.transform.position, directionToPlayer, out hit, visionRange))
        {
            if (hit.collider.gameObject == player)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Punch_Distance);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, stomp_Distance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Throw_Distance);

        if (enemySight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(enemySight.transform.position, enemySight.transform.forward * visionRange);
        }
    }

    public void Punch_Attack()
    {
        x = GameObject.Instantiate(punchEffect, punchPOS.transform.position, punchPOS.transform.rotation);
        x.transform.SetParent(punchPOS.transform);
        if (PunchSFX != null) PunchSFX.Play();
        Destroy(x, DestroyTime);
        player_Controller.takeDamage(5);
    }

    public void Stomp_Attack()
    {
        y = GameObject.Instantiate(stompEffect, stompPOS.transform.position, stompPOS.transform.rotation);
        y.transform.SetParent(stompPOS.transform);
        if (JumpSFX != null) JumpSFX.Play();
        Destroy(y, DestroyTime);
        player_Controller.takeDamage(3);
    }

    public void Throw_Attack_Spawn()
    {
        if (ThrowSFX != null) ThrowSFX.Play();
        currentRock = GameObject.Instantiate(throwProjectile, rockSpawnPos.transform.position, rockSpawnPos.transform.rotation);
        currentRock.transform.SetParent(rockSpawnPos.transform);
    }

    public void Throw_Attack()
    {
        if (currentRock == null) return;

        currentRock.transform.SetParent(null);
        Rigidbody rb = currentRock.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        RockProjectile projectileScript = currentRock.GetComponent<RockProjectile>();
        if (projectileScript == null) 
        {
            projectileScript = currentRock.AddComponent<RockProjectile>();
        }
        projectileScript.damage = 10; 

        Vector3 targetPos = player.transform.position + Vector3.up * 1.5f;
        Vector3 throwDirection = (targetPos - currentRock.transform.position).normalized;

        rb.linearVelocity = throwDirection * rockThrowSpeed;

        Destroy(currentRock, 5f);
    }
}