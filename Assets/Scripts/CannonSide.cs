using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSide : MonoBehaviour
{
    public bool isActived
    {
        get
        {
            return _isActived;
        }
        set
        {
            _isActived = value;
            TempVisuals();
        }
    }

    bool _isActived;

    public Rigidbody2D Projectile;
    public float ProjectileForce;

    public Rigidbody2D ShipRigidbody;

    public List<GameObject> CannonPoints = new List<GameObject>();

    public AudioClip soundEffect;
    public float minSoundDelay;
    public float maxSoundDelay;

    public Material SideMat;

    AudioSource source;

	// Use this for initialization
	void Start ()
    {
        source = GetComponent<AudioSource>();
        source.clip = soundEffect;
    }

    void TempVisuals()
    {
        if(isActived)
        {
            SideMat.color = Color.white;
        }
        else
        {
            SideMat.color = Color.gray;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void Fire()
    {
        if (!isActived)
            return;

        StartCoroutine("e_Fire");
    }

    IEnumerator e_Fire()
    {
        for (int i = 0; i < CannonPoints.Count; i++)
        {
            Rigidbody2D r = Instantiate<Rigidbody2D>(Projectile, CannonPoints[i].transform.position, CannonPoints[i].transform.rotation);
            r.velocity = ShipRigidbody.velocity;
            r.AddForce(r.transform.up * ProjectileForce, ForceMode2D.Impulse);
            source.Play();
            yield return new WaitForSeconds(Random.Range(minSoundDelay, maxSoundDelay));
        }
    }
}
