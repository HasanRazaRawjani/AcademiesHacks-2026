using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    private Animator animator;
    private Player_Controller player_Controller;
    private GameObject player;
    private NavMeshAgent Agent;
    private GameObject enemySight;

    [Header("State Machine Settings")]
    public float stomp_Distance = 3f; 
    public float Punch_Distance = 3f; 
    public float Throw_Distance = 3f; 
    public float visionRange = 20f;   

    [Header("Cooldown & Timing")]
    public float attackCooldown = 2f;    // Time spent walking before next attack
    public float animationDuration = 1.5f; // Duration to let the animation play (freeze movement)
    
    private float cooldownTimer = 0f;
    private float lockoutTimer = 0f;

    [Header("Attack Effects")]
    public GameObject stompEffect;
    private GameObject stompPOS;
    public GameObject punchEffect;
    private GameObject punchPOS;
    public GameObject throwProjectile;

    void Start()
    {
        stompPOS = gameObject.transform.Find("DustExplosionPos").gameObject;
        punchPOS = gameObject.transform.Find("EarthShatterPos").gameObject;
        enemySight = gameObject.transform.Find("Golem_Model").transform.Find("Enemy_Vision").gameObject;
        player = GameObject.Find("Player");
        player_Controller = player.GetComponent<Player_Controller>();
        Agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.transform.Find("Golem_Model").GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Handle Cooldown (The walking delay)
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // 2. Handle Lockout (The animation freeze)
        if (lockoutTimer > 0)
        {
            lockoutTimer -= Time.deltaTime;
            Agent.isStopped = true;
            animator.SetBool("Moving", false);
            // We return here so NO movement logic runs until the animation is "done"
            return; 
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 3. Attack Logic
        // Punch - Spam allowed, but we still lock the animation for its duration
        if (distanceToPlayer <= Punch_Distance && CanSeePlayer())
        {
            TriggerAttack("Punch", animationDuration, false);
        }
        // Stomp
        else if (distanceToPlayer <= stomp_Distance && CanSeePlayer() && cooldownTimer <= 0)
        {
            TriggerAttack("Stomp_Attack", animationDuration, true);
        }
        // Throw
        else if (distanceToPlayer <= Throw_Distance && CanSeePlayer() && cooldownTimer <= 0)
        {
            TriggerAttack("Throw_Rock", animationDuration, true);
        }
        // 4. Chase State
        else
        {
            Agent.isStopped = false;
            animator.SetBool("Moving", true);
            Agent.SetDestination(player.transform.position);
        }
    }

    // Helper to keep the code clean
    void TriggerAttack(string triggerName, float duration, bool applyCooldown)
    {
        Agent.isStopped = true;
        gameObject.transform.LookAt(player.transform);
        animator.SetBool("Moving", false);
        animator.SetTrigger(triggerName);

        lockoutTimer = duration; // Freeze the AI for the duration of the clip
        if (applyCooldown)
        {
            cooldownTimer = attackCooldown; // Start the walk-back-at-player delay
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
        Debug.Log("Punch");

    }

    public void Stomp_Attack()
    {
        Debug.Log("Stomp");
    }

    public void Throw_Attack()
    {
        Debug.Log("Throw");
    }

    public void Throw_Attack_Spawn()
    {
        
    }

}