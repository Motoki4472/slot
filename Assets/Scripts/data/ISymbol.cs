using UnityEngine;

namespace Assets.data
{
    public interface ISymbol
    {
        int GetId();
        int GetValue();
        Sprite GetSprite();
    }
}