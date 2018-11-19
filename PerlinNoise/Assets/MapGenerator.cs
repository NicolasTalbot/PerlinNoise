using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public enum GenerationType
    {
        random, perlinNoise
    }
    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;

    public Tilemap tilemap;
    public TerrainType[] regions;
    public TerrainType[] resources;

    private TileBase FindTileFromRegion(float valeur)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (valeur <= regions[i].value)
            {
                return regions[i].tile;
            }
        }
        return resources[0].tile;
    }

    private TileBase FindTileFromResources(float valeur)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (valeur <= resources[i].value)
            {
                return resources[i].tile;
            }
        }
        return regions[0].tile;
    }

    public void SetTileMap(TileBase[] customTilemap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTilemap[y * mapWidth + x]);
            }
        }
    }

    public void GenerateMapRandom()
    {
        TileBase[] customTilemap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                float rnd = Random.Range(0f, 1f);
                customTilemap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customTilemap);
    }

    private void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }

    public void GenerateMap()
    {
        if(generationType == GenerationType.random)
        {
            GenerateMapRandom();
        }
        else if(generationType == GenerationType.perlinNoise)
        {
            GenerateMapPerlinNoise();
        }
    }

    public void GenerateResources()
    {

    }

    public void GenerateMapPerlinNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed,noiseScale,octaves,persistance,lacunarity, offset);
        float[,] noiseResources = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed+1, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] customTilemap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                float value = noiseMap[x, y];
                if(value > regions[2].value)
                {
                    float valueResources = noiseResources[x, y];
                    customTilemap[y * mapWidth + x] = FindTileFromResources(valueResources);
                    
                }
                else
                {
                    customTilemap[y * mapWidth + x] = FindTileFromRegion(value);
                }
            }
        }
        SetTileMap(customTilemap);
    }

}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float value;
    public TileBase tile;
}