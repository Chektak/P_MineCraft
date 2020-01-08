using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Particle_Idle : Particle
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(particleDestroy(part));
    }
    IEnumerator particleDestroy(ParticleSystem particle)
    {
        while (particle.isPlaying == true)
        {
            yield return null;
        }
        Destroy(particle.gameObject);
        yield break;
    }
}
