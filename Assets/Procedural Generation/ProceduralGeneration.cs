using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] int minRockHeight, maxRockHeight;
    [SerializeField] GameObject lunarSoil, lunarCrust, lunarRock;

    void Start() {
        Generation();
    }

    void Generation() {
        for(int x=0;x<width;x++) {
            int minHeight = height - 1;
            int maxHeight = height + 2;

            height = Random.Range(minHeight, maxHeight);
            int minRockSpawnDistance = height - minRockHeight;
            int maxRockSpawnDistance = height - maxRockHeight;
            int totalRockSpawnDistance = Random.Range(minRockSpawnDistance, maxRockSpawnDistance);
            for(int y=0;y<height;y++) {
                if(y < totalRockSpawnDistance) {
                    spawnObject(lunarRock, x, y);
                } else {
                    spawnObject(lunarSoil, x, y);
                }
            }
            if(totalRockSpawnDistance == height) {
                spawnObject(lunarRock, x, height);
            } else {
                spawnObject(lunarCrust, x, height);
            }
        }
    }

    void spawnObject(GameObject obj, int width, int height) {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}
