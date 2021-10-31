using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Tile Sprites")]
    public Sprite log;
    public Sprite leaf;
    public Sprite lunarCrust;
    public Sprite lunarSoil;
    public Sprite lunarRock;

    [Header("Trees")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 7;

    [Header("Generation Settings")]
    public int lunarSoilLayer = 5;
    public bool generateCaves = true;
    public float terrainAmount = 0.25f;
    public int worldSize = 100;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float seed;
    public Texture2D noiseTexture;

    private List<Vector2> worldTiles = new List<Vector2>();

    private void Start() {
        seed = Random.Range(-1000000, 1000000);
        GenerateNoiseTexture();
        GenerateTerrain();
    }

    public void GenerateTerrain() {
        for(int x=0;x<worldSize;x++) {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            for(int y=0;y<height;y++) {
                Sprite tileSprite;
                if(y < height - lunarSoilLayer) {
                    tileSprite = lunarRock;
                } else if(y < height - 1) {
                    tileSprite = lunarSoil;
                } else {
                    tileSprite = lunarCrust;
                }
                if(generateCaves) {
                    if(noiseTexture.GetPixel(x, y).r > terrainAmount) {
                        PlaceTile(tileSprite, x, y);
                    }
                } else {
                    PlaceTile(tileSprite, x, y);
                }
                if(y >= height - 1) {
                    int t = Random.Range(0, treeChance);
                    if(t == 1) {
                        if(worldTiles.Contains(new Vector2(x, y))) {
                            GenerateTree(x, y + 1);
                        }
                    }
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

    void GenerateTree(float x, float y) {
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for(int i=0;i<treeHeight;i++) {
            PlaceTile(log, x, y + i);
        }

        PlaceTile(leaf, x, y + treeHeight);
        PlaceTile(leaf, x, y + treeHeight + 1);
        PlaceTile(leaf, x, y + treeHeight + 2);

        PlaceTile(leaf, x - 1, y + treeHeight);
        PlaceTile(leaf, x - 1, y + treeHeight + 1);

        PlaceTile(leaf, x + 1, y + treeHeight);
        PlaceTile(leaf, x + 1, y + treeHeight + 1);
    }

    public void PlaceTile(Sprite tileSprite, float x, float y) {
        GameObject newTile = new GameObject();
        newTile.transform.parent = this.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
