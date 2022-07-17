using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeRoll : MonoBehaviour
{
    ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        DiceMovement.Instance.onDiceMovementEnded.AddListener(OnDiceMovement);

        particles = GetComponent<ParticleSystem>();
    }

    private void OnDiceMovement(Direction dir, Vector3Int playerGridPos)
    {
        transform.position = playerGridPos;
        particles.Play();
    }
}
