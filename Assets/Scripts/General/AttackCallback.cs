using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCallback : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            player.ctx.DownAttackBounced = true;
    }

}
