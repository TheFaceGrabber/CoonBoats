using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class ProceduralTest : MonoBehaviour
{
    public GameObject Land;

    public int w = 100;
    public int h = 100;

    public int LoadDistance = 12;

    public List<Region> Regions = new List<Region>();

    [SerializeField] private List<Chunk> _chunks = new List<Chunk>();

    public float Scale;

    public int Octaves = 2;

    public float Persistance;

    public float Lacunarity;

    public Vector2 Location;

    [SerializeField]
    private bool _generate = true;

    private Vector2 _lastPos;

    private float _lastScale;

    private Vector2 _nextChunkPos = new Vector2(0,-0.5f);
    // Use this for initialization
    void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	    GenerateSurroundingChunks();
	    DestroySurroundingChunks();
	}

    IEnumerator GenerateChunk()
    {
        Chunk c = new Chunk();
        c.ChunkPos = _nextChunkPos;
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                float amp = 1;
                float freq = 1;
                float n = 0;

                for (int k = 0; k < Octaves; k++)
                {
                    float xCoord = (i + _nextChunkPos.x) / Scale * freq;
                    float yCoord = (j + _nextChunkPos.y) / Scale * freq;
                    float p = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    n += p * amp;

                    amp *= Persistance;
                    freq *= Lacunarity;

                }

                if (n > 0f)
                {
                    GameObject go = Instantiate(Land, new Vector2(i - (w / 2) + _nextChunkPos.x, j - (h / 2) + _nextChunkPos.y), Quaternion.identity);
                    c.Blocks.Add(go);
                    //if(n < 1f)
                    for (int k = 0; k < Regions.Count; k++)
                    {
                        if (n > Regions[k].Height)
                            go.GetComponent<SpriteRenderer>().color = Regions[k].Colour;
                    }
                }
            }
        }

        _chunks.Add(c);
        yield return new WaitForEndOfFrame();
    }

    void GenerateSurroundingChunks()
    {
        int X = (int)transform.position.x;
        int Y = (int)transform.position.y;

        for (int i = X - w; i < X + w; i+= w)
        {
            for (int j = Y - w; j < Y + w; j += w)
            {
                CreateChunk(i, j);
            }
        }
    }

    void CreateChunk(int x, int y)
    {
        float xPos = Mathf.FloorToInt((float)x / (float)w) * w;
        float yPos = Mathf.FloorToInt((float)y / (float)w) * w;

        Vector2 nextPos = new Vector2(x, y);

        if (!HasChunkAtPos(nextPos))
        {
            _nextChunkPos = nextPos;
            StartCoroutine(GenerateChunk());
        }
    }

    void DestroySurroundingChunks()
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            float dist = Vector2.Distance(_chunks[i].ChunkPos, transform.position);
            if (dist > LoadDistance)
            {
                DestroyChunk(_chunks[i]);
            }
        }
    }

    void DestroyChunk(Chunk c)
    {
        for (int i = 0; i < c.Blocks.Count; i++)
        {
            Destroy(c.Blocks[i]);
        }

        _chunks.Remove(c);
    }

    bool HasChunkAtPos(Vector2 pos)
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            if (_chunks[i].ChunkPos == pos)
                return true;
        }
        return false;
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
    public Vector2 ChunkPos;
    public List<GameObject> Blocks = new List<GameObject>();
}
