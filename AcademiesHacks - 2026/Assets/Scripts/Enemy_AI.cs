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
    public float stomp_Distance = 3f; // The threshold to stop and stomp
    public float Punch_Distance = 3f; // The threshold to stop and stomp
    public float Throw_Distance = 3f; // The threshold to stop and stomp
    public float visionRange = 20f;   // Maximum distance the raycast checks

    void Start()
    {
        enemySight = gameObject.transform.Find("Golem_Model").transform.Find("Enemy_Vision").gameObject;
        player = GameObject.Find("Player");
        player_Controller = player.GetComponent<Player_Controller>();
        Agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.transform.Find("Golem_Model").GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // State Machine Logic: Are we close enough AND can we see the player?
        if (distanceToPlayer <= Punch_Distance && CanSeePlayer())
        {
            // IN RANGE STATE: Stop moving. You will add your attack triggers here.
            Agent.isStopped = true;
            animator.SetBool("Moving", false);
            animator.SetTrigger("Punch");
        }

        else if(distanceToPlayer <= stomp_Distance && CanSeePlayer())
        {
            
            Agent.isStopped = true;
            animator.SetBool("Moving", false);
            animator.SetTrigger("Stomp_Attack");

        }

        else if(distanceToPlayer <= Throw_Distance && CanSeePlayer())
        {
            
            Agent.isStopped = true;
            animator.SetBool("Moving", false);
            animator.SetTrigger("Throw_Rock");

        }

        else
        {
            // CHASE STATE: Keep following the player
            Agent.isStopped = false;
            animator.SetBool("Moving", true);
            Agent.SetDestination(player.transform.position);
        }
    }

    // Shoots a raycast from the Enemy_Vision object to the Player
    bool CanSeePlayer()
    {
        if (enemySight == null || player == null) return false;

        Vector3 directionToPlayer = (player.transform.position - enemySight.transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(enemySight.transform.position, directionToPlayer, out hit, visionRange))
        {
            // If the first thing the ray hits is the player, we have line of sight
            if (hit.collider.gameObject == player)
            {
                return true;
            }
        }
        return false; // Hits a wall or nothing
    }

    // Draws the visual spheres and lines in the Editor
    private void OnDrawGizmosSelected()
    {
        // The red sphere is your attack distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Punch_Distance);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, stomp_Distance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Throw_Distance);

        // A yellow line showing where the enemy is looking
        if (enemySight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(enemySight.transform.position, enemySight.transform.forward * visionRange);
        }
    }
}