using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int minRockHeight, maxRockHeight;
    [SerializeField] Tilemap lunarSoilTilemap, lunarCrustTilemap, lunarRockTilemap;
    [SerializeField] Tile lunarSoil, lunarCrust, lunarRock;
    [Range(0, 100)]
    [SerializeField] float heightValue, smoothness;
    [SerializeField] float seed;

    void Start() {
        seed = Random.Range(-1000000,1000000);
        Generation();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            seed = Random.Range(-1000000,1000000);
            Generation();
        } else if (Input.GetKeyDown(KeyCode.D)) {
            lunarRockTilemap.ClearAllTiles();
            lunarSoilTilemap.ClearAllTiles();
            lunarCrustTilemap.ClearAllTiles();
        }
    }

    void Generation() {
        for(int x=0;x<width;x++) {
            int height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise(x/smoothness, seed));
            int minRockSpawnDistance = height - minRockHeight;
            int maxRockSpawnDistance = height - maxRockHeight;
            int totalRockSpawnDistance = Random.Range(minRockSpawnDistance, maxRockSpawnDistance);
            for(int y=0;y<height;y++) {
                if(y < totalRockSpawnDistance) {
                    lunarRockTilemap.SetTile(new Vector3Int(x, y, 0), lunarRock);
                } else {
                    lunarSoilTilemap.SetTile(new Vector3Int(x, y, 0), lunarSoil);
                }
            }
            if(totalRockSpawnDistance == height) {
                lunarRockTilemap.SetTile(new Vector3Int(x, height, 0), lunarRock);
            } else {
                lunarCrustTilemap.SetTile(new Vector3Int(x, height, 0), lunarCrust);
            }
        }
    }
}
