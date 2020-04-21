using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader {
    public static Level LoadLevel(TextAsset file) {
        Scanner scanner = new Scanner(file.text);
        Level level = new Level();
        int numWaves = scanner.nextInt();
        for (int waveIndex = 0; waveIndex < numWaves; waveIndex++) {
            Wave wave = new Wave();
            wave.StartTime = scanner.nextFloat();
            int numEnemyPayloads = scanner.nextInt();
            for (int payloadIndex = 0; payloadIndex < numEnemyPayloads; payloadIndex++) {
                EnemyPayload payload = new EnemyPayload();
                payload.EnemyId = scanner.nextInt();
                
                string line = scanner.nextLine();
                Debug.Log($"payload index {payloadIndex}, line {line}");
                Scanner payloadScanner = new Scanner(line);
                while (payloadScanner.hasNextNumber()) {
                    float delay = payloadScanner.nextFloat();
                    int count = payloadScanner.nextInt();
                    payload.Spawns.Add(new Vector2(delay, count));
                }
                wave.Enemies.Add(payload);
            }
            level.Waves.Add(wave);
        }
        return level;
    }
}
