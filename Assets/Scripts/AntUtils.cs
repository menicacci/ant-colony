using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntUtils : MonoBehaviour
{
    private static AntUtils instance;

    public static AntUtils Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AntUtils>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("AntUtils");
                    instance = obj.AddComponent<AntUtils>();
                }
            }

            return instance;
        }
    }

    private class ParticleInfo
    {
        public Vector3 position;
        public float remainingLifetime;

        public ParticleInfo(Vector3 pos, float lifetime)
        {
            position = pos;
            remainingLifetime = lifetime;
        }
    }


    public Vector3 GetTargetPosition(Collider[] ants, bool isSearching)
    {
        List<ParticleSystem> particleSystem = FilterParticles(ants, isSearching);
        List<ParticleInfo> pheromoneDetected = FindBestParticles(particleSystem);

        return pheromoneDetected.Count > 0 ? GetTargetPosition(pheromoneDetected) : Vector3.zero;
    }


    private List<ParticleInfo> FindBestParticles(List<ParticleSystem> particleSystem)
    {
        List<ParticleInfo> pheromoneDetected = new List<ParticleInfo>();

        foreach (ParticleSystem ps in particleSystem)
        {                            
              // Get the particles from the particle system
              ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];

              int numParticlesAlive = ps.GetParticles(particles);

              for (int i = 0; i < Mathf.Min(numParticlesAlive, 20); i++)
              {
                  // Accessing position information
                  Vector3 particlePosition = particles[i].position;
                  float particleLifetime = particles[i].remainingLifetime;

                  pheromoneDetected.Add(new ParticleInfo(particlePosition, particleLifetime));
              }
        }

        return pheromoneDetected;
    }


    private Vector3 GetTargetPosition(List<ParticleInfo> pheromoneDetected)
    {
        float totalWeight = 0.0f;
        Vector3 weightedSum = Vector3.zero;

        foreach (ParticleInfo particle in pheromoneDetected)
        {
            float weight = 1.0f / (particle.remainingLifetime + 1.0f); // Adding 1 to avoid division by zero
            weightedSum += particle.position * weight;
            totalWeight += weight;
        }

        return weightedSum / totalWeight;
    }

    private List<ParticleSystem> FilterParticles(Collider[] ants, bool isSearching)
    {
        List<ParticleSystem> particlesFound = new List<ParticleSystem>();
    
        int pheromoneSystem = isSearching ? 0 : 1;
        foreach (Collider ant in ants)
        {
            ParticleSystem[] particles = ant.gameObject.GetComponentsInChildren<ParticleSystem>();
    
            if (particles != null && particles.Length == 2 && particles[pheromoneSystem].isEmitting)
            {
                particlesFound.Add(particles[pheromoneSystem]);
            }
        }
    
        return particlesFound;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

}

