using UnityEngine;

namespace Assets.data
{
    public class Cherry : MonoBehaviour, ISymbol
    {
        [SerializeField] private int Id = 1;
        [SerializeField] private int value = 100;
        [SerializeField] private Sprite sprite;

        public int GetId()
        {
            return Id;
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
