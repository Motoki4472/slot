using UnityEngine;

namespace Assets.Data
{
    public interface ISymbol
    {
        int GetId();
        int GetValue();
        Sprite GetSprite();
    }
}