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

    [Header("Foliage")]
    public int rockChance = 10;

    [Header("Generation Settings")]
    public int chunkSize = 20;
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
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();

    private void OnValidate() {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
            ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
            ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
            ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);
    }

    private void Start() {
        seed = Random.Range(-1000000, 1000000);
        if(caveNoiseTexture == null) {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
            ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
            ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
            ores[3].spreadTexture = new Texture2D(worldSize, worldSize);
        }
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);
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
                Sprite[] tileSprites;
                if(y < height - lunarSoilLayer) {
                    tileSprites = tileAtlas.lunarRock.tileSprites;
                    if(ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpawnHeight)
                        tileSprites = tileAtlas.coal.tileSprites;
                    if(ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                        tileSprites = tileAtlas.iron.tileSprites;
                    if(ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpawnHeight)
                        tileSprites = tileAtlas.gold.tileSprites;
                    if(ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                        tileSprites = tileAtlas.diamond.tileSprites;
                } else if(y < height - 1) {
                    tileSprites = tileAtlas.lunarSoil.tileSprites;
                } else {
                    tileSprites = tileAtlas.lunarCrust.tileSprites;
                }
                if(generateCaves) {
                    if(caveNoiseTexture.GetPixel(x, y).r > 0.5f) {
                        PlaceTile(tileSprites, x, y);
                    }
                } else {
                    PlaceTile(tileSprites, x, y);
                }
                if(y >= height - 1) {
                    int t = Random.Range(0, treeChance);
                    if(t == 1) {
                        if(worldTiles.Contains(new Vector2(x, y))) {
                            GenerateTree(x, y + 1);
                        }
                    } else {
                        int i = Random.Range(0, rockChance);
                        if(i == 1) {
                            if(worldTiles.Contains(new Vector2(x, y))) {
                                PlaceTile(tileAtlas.moonRock.tileSprites, x, y + 1);
                            }
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
            PlaceTile(tileAtlas.log.tileSprites, x, y + i);
        }

        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 1);
    }

    public void PlaceTile(Sprite[] tileSprites, int x, int y) {
        GameObject newTile = new GameObject();
        float chunkCoords = Mathf.RoundToInt(x / chunkSize) * chunkSize;
        chunkCoords /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoords].transform;
        newTile.AddComponent<SpriteRenderer>();
        int spriteIndex = Random.Range(0, tileSprites.Length);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];
        newTile.name = tileSprites[0].name;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
