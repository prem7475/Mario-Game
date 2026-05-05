using MarioGame.Utils.Runtime;
using UnityEngine;

namespace MarioGame.Components.FX
{
    public static class ParticleService
    {
        public static void Burst(Vector3 position, Color color, int count = 14, float size = 0.14f)
        {
            var go = new GameObject("Particles");
            go.transform.position = position;

            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.35f;
            main.startLifetime = 0.4f;
            main.startSpeed = 3.2f;
            main.startSize = size;
            main.startColor = color;
            main.gravityModifier = 1.1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.18f;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            ps.Play();
            Object.Destroy(go, 1.2f);
        }
    }
}

