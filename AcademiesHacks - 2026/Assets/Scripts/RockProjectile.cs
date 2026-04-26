using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    public int damage = 3;
    private bool hasHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        Player_Controller player = collision.gameObject.GetComponent<Player_Controller>();

        if (player != null)
        {
            player.takeDamage(damage);
            hasHit = true;
            
            Destroy(gameObject, 0.1f); 
        }
    }
}