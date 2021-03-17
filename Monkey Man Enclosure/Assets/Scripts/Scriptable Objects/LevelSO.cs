using UnityEngine;

[CreateAssetMenu(fileName = "LevelProperties", menuName = "ScriptableObjects/LevelProperties", order = 1)]
public class LevelSO : ScriptableObject
{
    [Range(0, 5)]
    public int monkeyAmount = 2;
    [Range(0, 1)]
    public float disruptionFrequency = 0.5f;
}
