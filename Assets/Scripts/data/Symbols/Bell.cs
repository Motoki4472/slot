using UnityEngine;

namespace Assets.data
{
    public class Bell : MonoBehaviour, ISymbol
    {
        [SerializeField] private int Id = 2;
        [SerializeField] private int value = 500;
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