using UnityEngine;

[CreateAssetMenu(fileName = "Throwable", menuName = "ScriptableObjects/Throwable", order = 1)]
public class ThrowableSO : ScriptableObject
{
    [Range(0.01f, 7)]
    public float affectRange = 3;
}
