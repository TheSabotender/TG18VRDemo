using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class Galaxy : MonoBehaviour
{
    public int NumOfStars = 8000;
    public int NumOfArms = 2;
    public float Spin = 3;
    public double ArmSpread = 0.1d;
    public double StarsAtCenterRatio = 3;
    public float scale = 1;

    private ParticleSystem.MinMaxCurve particleSizeByDistance;
    private ParticleSystem.MinMaxGradient particleColorByDistance;

    private ParticleSystem party;
    private Vector4[] points;

    void Update()
    {
        if (party == null)
        {
            party = GetComponent<ParticleSystem>();
        }
        
        if (party.particleCount == 0 || !MatchCurves(party.main.startSize, particleSizeByDistance) || !MatchGradients(party.main.startColor, particleColorByDistance))
        {
            particleColorByDistance = party.main.startColor;
            particleSizeByDistance = party.main.startSize;

            SetParticles();
        }
    }

    private bool MatchCurves(ParticleSystem.MinMaxCurve a, ParticleSystem.MinMaxCurve b)
    {
        if (a.mode != b.mode)
            return false;
        if (a.curve != b.curve)
            return false;
        if (a.constant != b.constant)
            return false;
        for (float f = 0; f <= 1; f += 0.1f)
        {
            if (Math.Abs(a.Evaluate(f) - b.Evaluate(f)) > 0.001f)
                return false;
        }

        return true;
    }
    private bool MatchGradients(ParticleSystem.MinMaxGradient a, ParticleSystem.MinMaxGradient b)
    {
        if (a.mode != b.mode)
            return false;
        if (a.gradient != b.gradient)
            return false;
        if (a.color != b.color)
            return false;
        for (float f = 0; f <= 1; f += 0.1f)
        {
            if (a.Evaluate(f) != b.Evaluate(f))
                return false;
        }

        return true;
    }

    [ExposeInEditor]
    public void Generate()
    {
        points = GenerateGalaxy(NumOfStars, NumOfArms, Spin, ArmSpread, StarsAtCenterRatio);   
    }
    
    [ExposeInEditor]
    public void SetParticles()
    {
        if (party == null)
        {
            party = GetComponent<ParticleSystem>();
        }

        if (points == null)
        {
            Generate();
        }
        
        party.Clear();
        foreach (Vector3 particle in points)
        {
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = particle;
            emitParams.velocity = Vector3.zero;
            
            party.Emit(emitParams,1);
        } 

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[party.particleCount];
        party.GetParticles(particles);
        for (var i = 0; i < particles.Length; i++)
        {
            var distance = Vector3.Distance(Vector3.zero, particles[i].position) / scale;

            particles[i].startColor = party.main.startColor.Evaluate(points[i].w);
            particles[i].startSize = party.main.startSize.Evaluate(points[i].w);
        }
        party.SetParticles(particles, particles.Length);
    }
    
    public Vector4[] GenerateGalaxy(int numOfStars, int numOfArms, float spin, double armSpread, double starsAtCenterRatio)
    {
        List<Vector4> result = new List<Vector4>(numOfStars);
        for (int i = 0; i < numOfArms; i++)
        {
            result.AddRange(GenerateArm(numOfStars/numOfArms, (float)i/numOfArms, spin, armSpread, starsAtCenterRatio));
        }
        return result.ToArray();
    }

    public Vector4[] GenerateArm(int numOfStars, float rotation, float spin, double armSpread, double starsAtCenterRatio)
    {
        Vector4[] result = new Vector4[numOfStars];
        System.Random r = new System.Random();
        
        for (int i = 0; i < numOfStars; i++)
        {
            double part = (double) i / numOfStars;
            part = Math.Pow(part, starsAtCenterRatio);

            float distanceFromCenter = (float) part;
            double position = (part * spin + rotation) * Mathf.PI * 2;

            double xFluctuation = (Pow3Constrained(r.NextDouble()) - Pow3Constrained(r.NextDouble())) * armSpread;
            double yFluctuation = (Pow3Constrained(r.NextDouble()) - Pow3Constrained(r.NextDouble())) * armSpread;

            float resultX = Mathf.Cos((float)position) * distanceFromCenter / 2 + 0.5f + (float) xFluctuation;
            float resultY = Mathf.Sin((float)position) * distanceFromCenter / 2 + 0.5f + (float) yFluctuation;

            result[i] = (new Vector3(resultX, resultY, 0) - new Vector3(0.5f, 0.5f, 0)) * scale;
            result[i].w = Mathf.PingPong((float)yFluctuation / (float)armSpread, 1) * (distanceFromCenter + 0.5f) + (distanceFromCenter * 0.25f);
        }

        return result;
    }

    public static double Pow3Constrained(double x)
    {
        float value = Mathf.Pow((float)x - 0.5f, 3) * 4 + 0.4f;
        return (Double)Mathf.Max(Mathf.Min(1, value), 0);
    }
}
