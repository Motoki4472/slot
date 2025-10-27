using UnityEngine;

namespace Assets.data
{
    public class Seven : MonoBehaviour, ISymbol
    {
        [SerializeField] private int Id = 3;
        [SerializeField] private int value = 700;
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