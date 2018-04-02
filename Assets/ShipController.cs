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
	    GetInput();
        mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	    angle = (Mathf.Atan2(mousePositionInWorld.y - transform.position.y, mousePositionInWorld.x - transform.position.x) * Mathf.Rad2Deg - 90);

	    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), speedFector * Time.deltaTime);
	    rb.AddRelativeForce(Vector2.up * (speed/10));
    }

    private void GetInput()
    {
        if (UserInput.GetInput("Forwards"))
        {
            speed += speed < 100 ? 1 : 0;
        }

        if (UserInput.GetInput("Backwards"))
        {
            speed -= speed > 0 ? 1 : 0;
        }

        if (UserInput.GetInputDown("LeftSide"))
        {
            RightSideCannon.isActived = false;
            LeftSideCannon.isActived = true;
        }

        if (UserInput.GetInputDown("RightSide"))
        {
            LeftSideCannon.isActived = false;
            RightSideCannon.isActived = true;
        }

        if (UserInput.GetInputDown("Fire"))
        {
            LeftSideCannon.Fire();
            RightSideCannon.Fire();
        }
    }
}
