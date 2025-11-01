using UnityEngine;

namespace Assets.Slot
{
    public class SlotSpeed
    {
        private float spinSpeed = 4.9f;
        private float randomRange = 2.0f;

        public float GetSpinSpeed()
        {
            return spinSpeed;
        }

        public float GetAdjustedSpeed(int combo)
        {
            combo = Mathf.Clamp(combo, 0, 100);

            float baseSpeed = spinSpeed + (combo / 100.0f) * spinSpeed;
            if (combo / 10 == 0 && combo != 0) baseSpeed += spinSpeed*0.7f;

            float randomOffset = Random.Range(-randomRange, randomRange*0.8f);

            return baseSpeed + randomOffset;
        }
    }
}