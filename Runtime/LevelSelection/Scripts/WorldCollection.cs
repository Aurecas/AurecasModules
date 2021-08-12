using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World Collection", menuName = "Scriptable Objects/World Collection")]
public class WorldCollection : ScriptableObject
{
    public WorldSO[] worlds;
}
