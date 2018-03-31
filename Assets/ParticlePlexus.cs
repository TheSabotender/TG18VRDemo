using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour
{
    public float maxDistance = 1f;
    public int maxConnections = 5;
    public int maxLineRenderers = 100;
    [Range(0,1)]public float UseParticleColors = 0;
    [Range(0,1)]public float UseParticleWidth = 0;

    new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem.MainModule particleSystemMainModule;

    public LineRenderer lineRendererTemplate;
    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    private Transform _transform;

	// Update is called once per frame
	void Update ()
	{
	    if (particleSystem == null || _transform == null)
	    {
            particleSystem = GetComponent<ParticleSystem>();
            particleSystemMainModule = particleSystem.main;
        }

	    if (lineRenderers.Count > maxLineRenderers)
	    {
	        for (int i = maxLineRenderers; i < lineRenderers.Count; i++)
	        {
	            Destroy(lineRenderers[i].gameObject);
	        }
            lineRenderers.RemoveRange(maxLineRenderers, lineRenderers.Count - maxLineRenderers);
	    }

        int lrIndex = 0;
        int lineRendererCount = lineRenderers.Count;

        if (maxConnections <= 0 || maxLineRenderers <= 0)
	    {
	        return;
	    }
        
	    int maxParticles = particleSystemMainModule.maxParticles;

	    if (particles == null || particles.Length < maxParticles)
	    {
	        particles = new ParticleSystem.Particle[maxParticles];
	    }

	    particleSystem.GetParticles(particles);
	    int particleCount = particleSystem.particleCount;

	    float maxDistanceSqr = maxDistance*maxDistance;


	    ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;

	    switch (simulationSpace)
	    {
	            case ParticleSystemSimulationSpace.Local:
                _transform = transform;
                break;
                case ParticleSystemSimulationSpace.Custom:
                _transform = particleSystemMainModule.customSimulationSpace;
                break;
                case ParticleSystemSimulationSpace.World:
                _transform = transform;
                break;
            default:
                throw new System.NotSupportedException(string.Format("Unsupported simulations space '{0}'.", System.Enum.GetName(typeof(ParticleSystemSimulationSpace), particleSystemMainModule.simulationSpace)));
	            break;
	    }

	    for (int i = 0; i < particleCount; i++)
	    {
	        Vector3 p1_position = particles[i].position;
	        Color p1_color = particles[i].GetCurrentColor(particleSystem);
	        float p1_size = particles[i].GetCurrentSize(particleSystem);

            int connections = 0;

            for (int j = i+1; j < particleCount; j++)
            {
                if (lrIndex == maxLineRenderers)
                {
                    break;
                }

                Vector3 p2_position = particles[j].position;
                Color p2_color = particles[j].GetCurrentColor(particleSystem);
                float p2_size = particles[j].GetCurrentSize(particleSystem);

                float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);

                if (distanceSqr <= maxDistanceSqr)
                {
                    LineRenderer lr;

                    if (lrIndex == lineRendererCount)
                    {
                        lr = Instantiate(lineRendererTemplate, transform, false);
                        lineRenderers.Add(lr);

                        lineRendererCount++;
                    }
                    lr = lineRenderers[lrIndex];

                    lr.enabled = true;
                    lr.useWorldSpace = simulationSpace == ParticleSystemSimulationSpace.World;

                    lr.positionCount = 2;
                    lr.SetPosition(0, p1_position);
                    lr.SetPosition(1, p2_position);

                    lr.startColor = Color.Lerp(lineRendererTemplate.startColor, p1_color, UseParticleColors);
                    lr.endColor = Color.Lerp(lineRendererTemplate.startColor, p2_color, UseParticleColors);

                    lr.startWidth = Mathf.Lerp(lineRendererTemplate.startWidth, p1_size, UseParticleWidth);
                    lr.endWidth = Mathf.Lerp(lineRendererTemplate.endWidth, p2_size, UseParticleWidth);

                    lrIndex++;
                    connections++;

                    if (connections == maxConnections || lrIndex == maxLineRenderers)
                    {
                        break;
                    }
                }
            }
        }

	    for (int i = lrIndex; i < lineRendererCount; i++)
	    {
            lineRenderers[i].enabled = false;
	    }
	}
}
