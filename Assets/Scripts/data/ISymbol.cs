using UnityEngine;

namespace Assets.data
{
    public interface ISymbol
    {
        string GetName();
        int GetValue();
        Sprite GetSprite();
    }
}