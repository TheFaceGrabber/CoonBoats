using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class ProceduralTest : MonoBehaviour
{
    public GameObject Land;
    public Material LandMaterial;
    public List<Region> Regions = new List<Region>();
    public int Octaves = 2;
    public float Persistance;
    public float Lacunarity;
    public float Scale;
    public int ChunkSize = 40;

    public static Vector2 ZeroOffset = new Vector2(1997,2018);

    public static Vector2 ViewerPosition;
    public static float ViewDistance = 40;

    [SerializeField] private Dictionary<Vector2,Chunk> _chunks = new Dictionary<Vector2, Chunk>();
    private List<Chunk> _chunksvisiblelastUpdate = new List<Chunk>();
    private int ChunkViewDist;
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
        Color[] cols = new Color[ChunkSize * ChunkSize];
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
                        cols[j * ChunkSize + i] = Regions[k].Colour;
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

        for (int yOffset = -ChunkViewDist; yOffset <= ChunkViewDist; yOffset++)
        {
            for (int xOffset = -ChunkViewDist; xOffset <= ChunkViewDist; xOffset++)
            {
                Vector2 viewedChunk = new Vector2(X + xOffset, Y + yOffset);
                if (_chunks.ContainsKey(viewedChunk))
                {
                    _chunks[viewedChunk].Update();
                    if(_chunks[viewedChunk].isVisible())
                        _chunksvisiblelastUpdate.Add(_chunks[viewedChunk]);
                }
                else
                {
                    _chunks.Add(viewedChunk, new Chunk(GenerateTerrain(viewedChunk*ChunkSize),ChunkSize,viewedChunk, LandMaterial));
                }
            }
        }
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
    public Vector2 Position;
    public GameObject Go;
    public Renderer ChunkRenderer;

    private Bounds _bounds;

    public Chunk(Color[] colours, int size, Vector2 posVector2, Material material)
    {
        Go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Go.transform.localScale = Vector3.one * size;
        Size = size;
        Position = posVector2 * size;
        Go.transform.position = Position;
        _bounds = new Bounds(Position, Vector2.one * size);
        Texture = new Texture2D(Size, Size) {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
        Texture.SetPixels(colours);
        Texture.Apply();
        ChunkRenderer = Go.GetComponent<Renderer>();
        ChunkRenderer.material = material;
        ChunkRenderer.material.mainTexture = Texture;
        Go.SetActive(false);
    }

    public void Update()
    {
        float viewerDist = Mathf.Sqrt(_bounds.SqrDistance(ProceduralTest.ViewerPosition));
        bool isVisible = viewerDist <= ProceduralTest.ViewDistance;
        Go.SetActive(isVisible);
    }

    public bool isVisible()
    {
        return Go.activeSelf;
    }
}
