using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class for setting and getting the value of level difficulty across various scripts.
/// </summary>
public static class LevelDifficulty
{
    [Tooltip("Value used in modifying the difficulty of a level.")]
    [Range(1, 50)]
    public static int levelDifficulty=10;
    public static int maxLevelReached=10;
    public static List<GameObject> objectsToTeleport = new List<GameObject>();
}
    