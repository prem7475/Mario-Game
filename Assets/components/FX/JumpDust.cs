using UnityEngine;

namespace MarioGame.Components.FX
{
    public sealed class JumpDust : MonoBehaviour
    {
        [SerializeField] private Color dustColor = new Color(1f, 1f, 1f, 0.75f);
        [SerializeField] private int count = 10;
        [SerializeField] private float size = 0.14f;

        public void Puff(Vector3 position)
        {
            ParticleService.Burst(position, dustColor, count: count, size: size);
        }
    }
}

