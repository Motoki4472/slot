using UnityEngine;

namespace Assets.Slot
{
    public class SlotSpeed
    {
        [SerializeField] private float spinSpeed = 5.0f;
        [SerializeField] private float randomRange = 2.0f;

        public float GetSpinSpeed()
        {
            return spinSpeed;
        }

        public float GetAdjustedSpeed(int combo)
        {
            combo = Mathf.Clamp(combo, 0, 100);

            float baseSpeed = spinSpeed + (combo / 100.0f) * spinSpeed;

            float randomOffset = Random.Range(-randomRange, randomRange);

            return baseSpeed + randomOffset;
        }
    }
}