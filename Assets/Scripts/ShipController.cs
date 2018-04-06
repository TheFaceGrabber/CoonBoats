using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    Vector3 mousePositionInWorld;
    float angle;
    [SerializeField]
    private float speedFector = 2f;
    [SerializeField]
    private float speed;
    
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
    private bool _showBeached;
    private bool _isbeached;

    private Transform _transform;

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
        _transform = transform;
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
	        GetInput();
	        if (!_isbeached)
	        {
	            mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	            angle = (Mathf.Atan2(mousePositionInWorld.y - _transform.position.y,
	                         mousePositionInWorld.x - _transform.position.x) * Mathf.Rad2Deg - 90);
	        }

            _transform.rotation =
	                Quaternion.Lerp(_transform.rotation, Quaternion.Euler(0, 0, angle), speedFector * Time.deltaTime);
	            rb.AddRelativeForce(Vector2.up * (speed / 10));
	    }
	    else
	    {
	        Vector3 Dir = _lastUnBeachedPosMod - new Vector2(_transform.position.x, _transform.position.y);
	        float angle = (Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg) - 90;
            Quaternion q = Quaternion.AngleAxis(angle,Vector3.forward);
            _transform.rotation = Quaternion.Slerp(_transform.rotation,q, _Unbeachspeed/rb.mass);
            _transform.position = Vector3.Lerp(_transform.position, _lastUnBeachedPosMod, _Unbeachspeed / rb.mass);
	        if (Vector3.Distance(_transform.position, _lastUnBeachedPosMod) < 0.2f)
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
                    _isbeached = true;
            }
        }

        _isbeached = false;
    }

    private void GetInput()
    {
        if (!_isbeached)
        {
            _showBeached = false;
            _lastUnBeachedPos = _transform.position;
            if (UserInput.GetInput("Forwards"))
            {
                speed += speed < 100 ? 1 : 0;
            }

            if (UserInput.GetInput("Backwards"))
            {
                speed -= speed > 0 ? 1 : 0;
            }
        }
        else
        {

            Vector2 dir = _lastUnBeachedPos - new Vector2(_transform.position.x, _transform.position.y);
            Vector2 pos = _lastUnBeachedPos + dir * 3;
            _lastUnBeachedPosMod = pos;
            speed -= speed > 0 ? 5 : 0;
            if (speed <= 0)
                _showBeached = true;
            else
                _showBeached = false;
        }

        if (UserInput.GetInputDown("ToggleCannonSide"))
        {
            if (RightSideCannon.isActived)
            {
                RightSideCannon.isActived = false;
                LeftSideCannon.isActived = true;
            }
            else if(LeftSideCannon.isActived)
            {
                RightSideCannon.isActived = true;
                LeftSideCannon.isActived = false;
            }
        }

        if (UserInput.GetInputDown("Interact"))
        {
            if (_showBeached)
            {
                _isUnbeaching = true;
            }
        }

        if (UserInput.GetInputDown("Fire"))
        {
            LeftSideCannon.Fire();
            RightSideCannon.Fire();
        }
    }

    void OnGUI()
    {
        if (_showBeached)
        {
            string text = "Press " + UserInput.GetKeyForButton("Interact") + " to un-beach your ship";
            float length = text.Length * 8;
            GUI.Box(new Rect(Screen.width / 2 - (length / 2), Screen.height / 2 + 60, length, 30), text);
        }
    }
}
