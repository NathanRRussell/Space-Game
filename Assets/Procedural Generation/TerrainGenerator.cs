using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Trees")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 7;

    [Header("Generation Settings")]
    public int chunkSize = 16;
    public int lunarSoilLayer = 5;
    public bool generateCaves = true;
    public float surfaceValue = 0.25f;
    public int worldSize = 100;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public float coalRarity, coalSize;
    public float ironRarity, ironSize;
    public float goldRarity, goldSize;
    public float diamondRarity, diamondSize;
    public Texture2D coalSpread;
    public Texture2D ironSpread;
    public Texture2D goldSpread;
    public Texture2D diamondSpread;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();

    private void OnValidate() {
        if(caveNoiseTexture == null) {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            goldSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);
        }
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(coalRarity, coalSize, coalSpread);
        GenerateNoiseTexture(ironRarity, ironSize, ironSpread);
        GenerateNoiseTexture(goldRarity, goldSize, goldSpread);
        GenerateNoiseTexture(diamondRarity, diamondSize, diamondSpread);
    }

    private void Start() {
        seed = Random.Range(-1000000, 1000000);
        if(caveNoiseTexture == null) {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            coalSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            goldSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);
        }
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(coalRarity, coalSize, coalSpread);
        GenerateNoiseTexture(ironRarity, ironSize, ironSpread);
        GenerateNoiseTexture(goldRarity, goldSize, goldSpread);
        GenerateNoiseTexture(diamondRarity, diamondSize, diamondSpread);
        CreateChunks();
        GenerateTerrain();
    }

    public void CreateChunks() {
        int numChunks = worldSize/chunkSize;
        worldChunks = new GameObject[numChunks];
        for(int i=0;i<numChunks;i++) {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }

    public void GenerateTerrain() {
        for(int x=0;x<worldSize;x++) {
            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            for(int y=0;y<height;y++) {
                Sprite tileSprite;
                if(y < height - lunarSoilLayer) {
                    if(coalSpread.GetPixel(x, y).r > 0.5f) {
                        tileSprite = tileAtlas.coal.tileSprite;
                    } else if(ironSpread.GetPixel(x, y).r > 0.5f) {
                        tileSprite = tileAtlas.iron.tileSprite;
                    } else if(goldSpread.GetPixel(x, y).r > 0.5f) {
                        tileSprite = tileAtlas.gold.tileSprite;
                    } else if(diamondSpread.GetPixel(x, y).r > 0.5f) {
                        tileSprite = tileAtlas.diamond.tileSprite;
                    } else {
                        tileSprite = tileAtlas.lunarRock.tileSprite;
                    }
                } else if(y < height - 1) {
                    tileSprite = tileAtlas.lunarSoil.tileSprite;
                } else {
                    tileSprite = tileAtlas.lunarCrust.tileSprite;
                }
                if(generateCaves) {
                    if(caveNoiseTexture.GetPixel(x, y).r > 0.5f) {
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

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture) {
        for(int x=0;x<noiseTexture.width;x++) {
            for(int y=0;y<noiseTexture.height;y++) {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if(v > limit) {
                    noiseTexture.SetPixel(x, y, Color.white);
                } else {
                    noiseTexture.SetPixel(x, y, Color.black);
                }
            }
        }
        noiseTexture.Apply();
    }

    void GenerateTree(int x, int y) {
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for(int i=0;i<treeHeight;i++) {
            PlaceTile(tileAtlas.log.tileSprite, x, y + i);
        }

        PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprite, x, y + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, x - 1, y + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, x + 1, y + treeHeight + 1);
    }

    public void PlaceTile(Sprite tileSprite, int x, int y) {
        GameObject newTile = new GameObject();
        float chunkCoords = Mathf.RoundToInt(x / chunkSize) * chunkSize;
        chunkCoords /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoords].transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
