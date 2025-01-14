using Fusion;
using UnityEngine;

/**
 * This component represents a ball moving at a constant speed.
 */
public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer lifeTimer { get; set; }

    [SerializeField] private float lifeTime = 5.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private int damagePerHit = 1;

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        // Debug.Log(lifeTimer);
        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += speed * transform.forward * Runner.DeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter " + other.gameObject.name + " " + other.gameObject.tag);
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.DealDamageRpc(damagePerHit);
        }
    }
}
