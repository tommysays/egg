using System.Collections.Generic;
using UnityEngine;

public class Level {
    public List<Wave> Waves = new List<Wave>();
}

public class Wave {
    public float StartTime;
    public List<EnemyPayload> Enemies = new List<EnemyPayload>();
}

public class EnemyPayload {
    public int EnemyId;
    public List<Vector2> Spawns = new List<Vector2>();
}