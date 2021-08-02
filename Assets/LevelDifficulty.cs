using UnityEngine;

/// <summary>
/// A static class for setting and getting the value of level difficulty across various scripts.
/// </summary>
public static class LevelDifficulty
{
    [Tooltip("Value used in modifying the difficulty of a level.")]
    [Range(1, 50)]
    public static int levelDifficulty = 6;
    public static int maxLevelReached = 6;
}
