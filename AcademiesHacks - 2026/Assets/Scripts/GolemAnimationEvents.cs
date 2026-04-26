using UnityEngine;
using UnityEngine.AI;

public class GolemAnimationEvents : MonoBehaviour
{
    private Enemy_AI mainScript; 

    void Start() => mainScript = GetComponentInParent<Enemy_AI>();

    public void Punch_Attack() => mainScript.Punch_Attack();
    public void Stomp_Attack() => mainScript.Stomp_Attack();
    public void Throw_Attack() => mainScript.Throw_Attack();
    public void Throw_Attack_Spawn() => mainScript.Throw_Attack_Spawn();
}