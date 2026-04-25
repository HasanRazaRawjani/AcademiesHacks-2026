using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{

    private Animator animator;
    private Player_Controller player_Controller;
    private GameObject player;
    private NavMeshAgent Agent;
    private GameObject enemySight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemySight = gameObject.transform.Find("Golem_Model").transform.Find("Enemy_Vision").gameObject;
        player = GameObject.Find("Player");
        player_Controller = player.GetComponent<Player_Controller>();
        Agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.transform.Find("Golem_Model").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    { 
        animator.SetBool("Moving", true);
        Agent.SetDestination(player.transform.position);
    }
}
