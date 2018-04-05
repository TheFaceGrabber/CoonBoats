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

    private Vector2 lastUnBeachedPos;
    private Vector2 lastUnBeachedPosMod;
    private bool _isUnbeaching;
    private float _Unbeachspeed = 0.1f;

    //Temporary
    private bool showBeached;


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

        LeftSideCannon.CannonPoints = CannonPointsLeft;
        RightSideCannon.CannonPoints = CannonPointsRight;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (!_isUnbeaching)
	    {
	        GetInput();
	        mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	        angle = (Mathf.Atan2(mousePositionInWorld.y - transform.position.y,
	                     mousePositionInWorld.x - transform.position.x) * Mathf.Rad2Deg - 90);

	        transform.rotation =
	            Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), speedFector * Time.deltaTime);
	        rb.AddRelativeForce(Vector2.up * (speed / 10));
	    }
	    else
	    {
	        Vector3 Dir = lastUnBeachedPosMod - new Vector2(transform.position.x, transform.position.y);
	        float angle = (Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg) - 90;
            Quaternion q = Quaternion.AngleAxis(angle,Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation,q, _Unbeachspeed/rb.mass);
            transform.position = Vector3.Lerp(transform.position, lastUnBeachedPosMod, _Unbeachspeed / rb.mass);
	    }
	}

    public bool IsBeached()
    {
        for (int i = 0; i < CollisionsTransforms.Count; i++)
        {
            if (ProceduralTest.Instance.CurrentChunk != null)
            {
                float xDifference = Mathf.Abs(ProceduralTest.Instance.CurrentChunk.Position.x) - Mathf.Abs(transform.position.x);
                float yDifference = Mathf.Abs(ProceduralTest.Instance.CurrentChunk.Position.y) - Mathf.Abs(transform.position.y);
                int xPos = Mathf.RoundToInt(xDifference + (float)ProceduralTest.Instance.ChunkSize / 2);
                int yPos = Mathf.RoundToInt(yDifference + (float)ProceduralTest.Instance.ChunkSize / 2);
                if (ProceduralTest.Instance.CurrentChunk.Texture.GetPixel(xPos, yPos) != Color.clear)
                    return true;
            }
        }

        return false;
    }

    private void GetInput()
    {
        if (!IsBeached())
        {
            showBeached = false;
            lastUnBeachedPos = transform.position;
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

            Vector2 dir = lastUnBeachedPos - new Vector2(transform.position.x, transform.position.y);
            Vector2 pos = lastUnBeachedPos + dir * 2;
            lastUnBeachedPosMod = pos;
            speed -= speed > 0 ? 5 : 0;
            if (speed <= 0)
                showBeached = true;
            else
                showBeached = false;
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
            if (showBeached)
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
        if (showBeached)
        {
            string text = "Press " + UserInput.GetKeyForButton("Interact") + " to un-beach your ship";
            float length = text.Length * 8;
            GUI.Box(new Rect(Screen.width / 2 - (length / 2), Screen.height / 2 + 60, length, 30), text);
        }
    }
}
