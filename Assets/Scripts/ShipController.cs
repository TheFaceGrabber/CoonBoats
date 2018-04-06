using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    float Angle;
    [SerializeField]
    private float speedFactor = 2f;
    [SerializeField]
    private float Speed;
    
    private Rigidbody2D rb;

    [SerializeField]
    public CannonSide LeftSideCannon;
    [SerializeField]
    public CannonSide RightSideCannon;

    public List<GameObject> CannonPointsLeft = new List<GameObject>();
    public List<GameObject> CannonPointsRight = new List<GameObject>();

    public List<Transform> CollisionsTransforms = new List<Transform>();

    private Vector2 _lastUnBeachedPos;
    private Vector2 _lastUnBeachedPosMod;
    private bool _isUnbeaching;
    private float _Unbeachspeed = 0.05f;

    //Temporary
    private bool _isbeached;

    public Transform Transform { get; set; }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        var left = new GameObject();
        left.transform.parent = LeftSideCannon.transform;
        left.transform.localEulerAngles = Vector2.up;
        left.transform.localPosition = Vector2.zero;
        CannonPointsLeft.Add(left);
        var right = new GameObject();
        right.transform.parent = RightSideCannon.transform;
        right.transform.localEulerAngles = Vector2.up;
        right.transform.localPosition = Vector2.zero;
        CannonPointsRight.Add(right);
        Transform = transform;
        LeftSideCannon.CannonPoints = CannonPointsLeft;
        RightSideCannon.CannonPoints = CannonPointsRight;
		RightSideCannon.isActived = true;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    CheckBeached();
        
        if (!_isUnbeaching)
	    {
	        Transform.rotation =
	                Quaternion.Lerp(Transform.rotation, Quaternion.Euler(0, 0, Angle), speedFactor * Time.deltaTime);
	            rb.AddRelativeForce(Vector2.up * (Speed / 10));

	        if (!_isbeached)
	        {
	            _lastUnBeachedPos = Transform.position;
	        }
	        else
	        {

	            Vector2 dir = _lastUnBeachedPos - new Vector2(Transform.position.x, Transform.position.y);
	            Vector2 pos = _lastUnBeachedPos + dir * 3;
	            _lastUnBeachedPosMod = pos;
	            Speed -= Speed > 0 ? 5 : 0;
	        }
        }
	    else
	    {
	        Vector3 Dir = _lastUnBeachedPosMod - new Vector2(Transform.position.x, Transform.position.y);
	        float angle = (Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg) - 90;
            Quaternion q = Quaternion.AngleAxis(angle,Vector3.forward);
	        Transform.rotation = Quaternion.Slerp(Transform.rotation,q, _Unbeachspeed/rb.mass);
	        Transform.position = Vector3.Lerp(Transform.position, _lastUnBeachedPosMod, _Unbeachspeed / rb.mass);
	        if (Vector3.Distance(Transform.position, _lastUnBeachedPosMod) < 0.2f)
	            _isUnbeaching = false;
	    }

    }

    public void CheckBeached()
    {
        for (int i = 0; i < CollisionsTransforms.Count; i++)
        {
            if (ProceduralTest.Instance.CurrentChunk != null)
            {
                float xDifference = Mathf.Abs(ProceduralTest.Instance.CurrentChunk.Position.x) - Mathf.Abs(CollisionsTransforms[i].position.x);
                float yDifference = Mathf.Abs(ProceduralTest.Instance.CurrentChunk.Position.y) - Mathf.Abs(CollisionsTransforms[i].position.y);
                int xPos = Mathf.RoundToInt(xDifference + (float)ProceduralTest.Instance.ChunkSize / 2);
                int yPos = Mathf.RoundToInt(yDifference + (float)ProceduralTest.Instance.ChunkSize / 2);
                if (ProceduralTest.Instance.CurrentChunk.Texture.GetPixel(xPos, yPos) != Color.clear)
                {
                    _isbeached = true;
                    return;
                }
            }
        }

        _isbeached = false;
    }
    
    public void SetCannonSide(ECannonSide side)
    {
        switch (side)
        {
            case ECannonSide.Left:
                RightSideCannon.isActived = false;
                LeftSideCannon.isActived = true;
                break;
            case ECannonSide.Right:
                RightSideCannon.isActived = true;
                LeftSideCannon.isActived = false;
                break;
        }
    }

    public void FireCannons()
    {
        LeftSideCannon.Fire();
        RightSideCannon.Fire();
    }

    public ECannonSide GetCannonSide()
    {
        if (RightSideCannon.isActived)
            return ECannonSide.Right;
        else
            return ECannonSide.Left;
    }

    public void BeginUnBeaching()
    {
        _isUnbeaching = true;
    }

    public float GetSpeed()
    {
        return Speed;
    }

    public void SetSpeed(float speed)
    {
        this.Speed = speed;
    }

    public void AddSpeed(float add)
    {
        this.Speed += add;
    }

    public bool GetIsBeached()
    {
        return _isbeached;
    }

    public void SetAngle(float angle)
    {
        this.Angle = angle;
    }

    public enum ECannonSide
    {
        Left,
        Right
    }
}
