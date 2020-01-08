using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class Particle : MonoBehaviour
{
    public ParticleSystem part;
    public Renderer rend;
    
    public void ChangeMaterial(Material newMtr) {
        rend.material = newMtr;
    }
}
