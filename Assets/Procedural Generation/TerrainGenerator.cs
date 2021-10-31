using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    public int lunarSoilLayer = 5;
    public Sprite log;
    public Sprite leaf;
    public Sprite lunarCrust;
    public Sprite lunarSoil;
    public Sprite lunarRock;

    public int treeChance = 10;
    public bool generateCaves = true;
    public float terrainAmount = 0.25f;
    public int worldSize = 100;
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;

    public float seed;
    public Texture2D noiseTexture;

    public List<GameObject> worldTiles = new List<GameObject>();

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

                    int t = Random.Range(0, treeChance);
                    if(t == 1) {
                        GenerateTree(x, y + 1);
                    }
                }
                if(generateCaves) {
                    if(noiseTexture.GetPixel(x, y).r > terrainAmount) {
                        PlaceTile(tileSprite, x, y);
                    }
                } else {
                    PlaceTile(tileSprite, x, y);
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
        PlaceTile(log, x, y);
    }

    public void PlaceTile(Sprite tileSprite, float x, float y) {
        GameObject newTile = new GameObject();
        newTile.transform.parent = this.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile);
    }
}
