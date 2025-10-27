using UnityEngine;

namespace Assets.data
{
    public class Seven : MonoBehaviour, ISymbol
    {
        [SerializeField] private string name = "Seven";
        [SerializeField] private int value = 700;
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