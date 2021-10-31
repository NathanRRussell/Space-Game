using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    public Sprite tile;
    public float terrainAmount;
    public int worldSize = 100;
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;
    public float seed;
    public Texture2D noiseTexture;

    private void Start() {
        seed = Random.Range(-1000000, 1000000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateTerrain() {
        for(int x=0;x<worldSize;x++) {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            for(int y=0;y<height;y++) {
                if(noiseTexture.GetPixel(x, y).r > 0.25f) {
                    GameObject newTile = new GameObject(name = "tile");
                    newTile.transform.parent = this.transform;
                    newTile.AddComponent<SpriteRenderer>();
                    newTile.GetComponent<SpriteRenderer>().sprite = tile;
                    newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
                }
            }
        }
    }

    public void GenerateNoiseTexture() {
        noiseTexture = new Texture2D(worldSize, worldSize);
        for(int x=0;x<noiseTexture.width;x++) {
            for(int y=0;y<noiseTexture.height;y++) {
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }
}
