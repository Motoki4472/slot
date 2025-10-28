using UnityEngine;

namespace Assets.data
{

    public class Combo
    {
        private int currentCombo;
        private int highCombo;

        public Combo()
        {
            currentCombo = 0;
            highCombo = PlayerPrefs.GetInt("HighCombo", 0);
        }

        public void AddCombo(int Combo)
        {
            currentCombo += Combo;
            if (currentCombo > highCombo)
            {
                highCombo = currentCombo;
                PlayerPrefs.SetInt("HighCombo", highCombo);
            }
        }

        public int GetCombo()
        {
            return currentCombo;
        }
        public int GetHighCombo()
        {
            return highCombo;
        }

        public void ResetCombo()
        {
            currentCombo = 0;
        }
    }
}