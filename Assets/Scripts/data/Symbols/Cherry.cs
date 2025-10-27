using UnityEngine;

namespace Assets.data
{
    public class Cherry : MonoBehaviour, ISymbol
    {
        [SerializeField] private string name = "Cherry";
        [SerializeField] private int value = 100;
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
