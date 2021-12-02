using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubbles : MonoBehaviour
{

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        int numParticlesAlive = ps.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (particles[i].position.y >= 0)
                particles[i].remainingLifetime = 0f;
        }

        ps.SetParticles(particles, numParticlesAlive);
    }
}
