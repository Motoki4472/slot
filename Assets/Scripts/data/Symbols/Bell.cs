using UnityEngine;

namespace Assets.data
{
    public class Bell : MonoBehaviour, ISymbol
    {
        [SerializeField] private string name = "Bell";
        [SerializeField] private int value = 250;
        [SerializeField] private Sprite sprite;

        public string GetName()
        {
            return name;
        }

        public int GetValue()
        {
            return value;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}