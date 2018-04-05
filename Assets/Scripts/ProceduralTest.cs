using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class ProceduralTest : MonoBehaviour
{
    public static ProceduralTest Instance;

    public GameObject Land;
    public Material LandMaterial;
    public List<Region> Regions = new List<Region>();
    public int Octaves = 2;
    public float Persistance;
    public float Lacunarity;
    public float Scale;
    public int ChunkSize = 40;
    public int TextureMultiplier = 5;

    public Chunk CurrentChunk
    {
        get
        {
            if(_chunks.ContainsKey(CurrentChunkPos))
                return _chunks[CurrentChunkPos];
            else
            {
                return null;
            }
        }
    }
    
    public int ViewDistanceMultiplier = 1;

    public static Vector2 ZeroOffset = new Vector2(1997,2018);

    public static Vector2 ViewerPosition;
    public static float ViewDistance = 40;

    [SerializeField] private Dictionary<Vector2,Chunk> _chunks = new Dictionary<Vector2, Chunk>();
    private List<Chunk> _chunksvisiblelastUpdate = new List<Chunk>();
    private int ChunkViewDist;

    public Vector2 CurrentChunkPos;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        ChunkViewDist = Mathf.RoundToInt(ViewDistance / ChunkSize);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    ViewerPosition = new Vector2(transform.position.x, transform.position.y);
        UpdateVisibleChunks();
	    //DestroySurroundingChunks();
	}

    Color[] GenerateTerrain(Vector2 Pos)
    {
        Color[] cols = new Color[(ChunkSize* TextureMultiplier) * (ChunkSize* TextureMultiplier)];
        for (int i = 0; i < ChunkSize; i++)
        {
            for (int j = 0; j < ChunkSize; j++)
            {
                float amp = 1;
                float freq = 1;
                float n = 0;

                for (int k = 0; k < Octaves; k++)
                {
                    float xCoord = (i + Pos.x + ZeroOffset.x) / Scale * freq;// + Pos.x;
                    float yCoord = (j + Pos.y + ZeroOffset.y) / Scale * freq;
                    // + Pos.y;
                    float p = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    n += p * amp;

                    amp *= Persistance;
                    freq *= Lacunarity;
                }

                for (int k = 0; k < Regions.Count; k++)
                {
                    if (n > Regions[k].Height)
                        cols[j * (ChunkSize * TextureMultiplier) + i] = Regions[k].Colour;
                }

            }
        }

        return cols;
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < _chunksvisiblelastUpdate.Count; i++)
        {
            _chunksvisiblelastUpdate[i].Go.SetActive(false);
        }
        _chunksvisiblelastUpdate.Clear();

        int X = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
        int Y = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);
        CurrentChunkPos = new Vector2(X,Y);
        for (int yOffset = -ChunkViewDist * ViewDistanceMultiplier; yOffset <= ChunkViewDist * ViewDistanceMultiplier; yOffset++)
        {
            for (int xOffset = -ChunkViewDist * ViewDistanceMultiplier; xOffset <= ChunkViewDist * ViewDistanceMultiplier; xOffset++)
            {
                Vector2 viewedChunk = new Vector2(X + xOffset, Y + yOffset);
                if (_chunks.ContainsKey(viewedChunk))
                {
                    _chunks[viewedChunk].Update();
                    if(_chunks[viewedChunk].IsVisible())
                        _chunksvisiblelastUpdate.Add(_chunks[viewedChunk]);
                }
                else
                {
                    _chunks.Add(viewedChunk, new Chunk(GenerateTerrain(viewedChunk*ChunkSize),ChunkSize,TextureMultiplier,ViewDistanceMultiplier,viewedChunk, LandMaterial, Land));
                }
            }
        }
    }

    public static GameObject InstantiateChunk(GameObject chunk)
    {
        GameObject go = Instantiate(chunk);
        return go;
    }
}

[System.Serializable]
public class Region
{
    public String Name;
    public Color Colour;
    public float Height;
}

[System.Serializable]
public class Chunk
{
    public int Size;
    public Texture2D Texture;
    public Sprite Sprite;
    public Vector2 Position;
    public GameObject Go;
    public SpriteRenderer ChunkRenderer;
    private Bounds _bounds;
    private readonly int _distMultiplier;

    public Chunk(Color[] colours, int size, int textureMultipler,int distMultiplier, Vector2 posVector2, Material material, GameObject chunkObject)
    {
        _distMultiplier = distMultiplier;
        Go = ProceduralTest.InstantiateChunk(chunkObject);
        Go.transform.localScale = Vector3.one * size;
        Size = size;
        Position = posVector2 * size;
        Go.transform.position = Position;
        _bounds = new Bounds(Position, Vector2.one * size);
        Texture = new Texture2D(Size * textureMultipler, Size * textureMultipler) {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
        Texture.SetPixels(colours);
        Texture.Apply();
        Sprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), new Vector2(0.5f, 0.5f), size);
        ChunkRenderer = Go.GetComponent<SpriteRenderer>();
        ChunkRenderer.sprite = Sprite;
        Go.SetActive(false);
    }

    public void Update()
    {
        float viewerDist = Mathf.Sqrt(_bounds.SqrDistance(ProceduralTest.ViewerPosition));
        bool isVisible = viewerDist <= ProceduralTest.ViewDistance * _distMultiplier;
        Go.SetActive(isVisible);
    }

    public bool IsVisible()
    {
        return Go.activeSelf;
    }

    public bool IsNull()
    {
        return Go == null;
    }
}
