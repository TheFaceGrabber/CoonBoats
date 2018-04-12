using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    public float angle;
    public AIStates curState;

    public float ViewRange;
    public float AttackRange;

    public float UpdateSpeed = 0.5f; //Time between updates in seconds

    private float _lastUpdate;

    [SerializeField]
    private ShipController _selectedTarget;
    private readonly List<ShipController> _possbleTargets = new List<ShipController>();
	private ShipController _controller;
    // Use this for initialization
    void Start () {
		_controller = GetComponent<ShipController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Time.time >= _lastUpdate + UpdateSpeed)
	    {
	        switch (curState)
	        {
	            case AIStates.Start:
	                curState = AIStates.Scan;
	                break;
	            case AIStates.Scan:
	                _possbleTargets.Clear();
                    Collider2D[] found = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), ViewRange);
                    Debug.Log(found.Length);
	                int lowestLevel = ShipController.MAXLEVEL;
	                if (found.Length > 0)
	                {
	                    for (int i = 0; i < found.Length; i++)
	                    {
	                        ShipController sc = found[i].transform.root.GetComponent<ShipController>();
                            if (sc != null && sc.gameObject != gameObject)
	                        {
	                            _possbleTargets.Add(sc);
	                            if (sc.ShipLevel < lowestLevel)
	                            {
	                                lowestLevel = sc.ShipLevel;
	                                _selectedTarget = sc;
	                            }
                            }
	                    }
                        if(_selectedTarget != null)
	                        curState = AIStates.MoveToEnemy;
                        else
                            curState = AIStates.Wander;
                    }
	                else
	                {
	                    curState = AIStates.Wander;
                    }
	                break;
	            case AIStates.Wander:
	                curState = AIStates.Scan;
                    break;
	            case AIStates.MoveToEnemy:
	                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
	                    new Vector2(_selectedTarget.transform.position.x, _selectedTarget.transform.position.y)) <= AttackRange)
	                {
	                    curState = AIStates.AttackEnemy;
                    }
	                else
	                {
						Vector3 dir = new Vector2(_selectedTarget.transform.position.x, _selectedTarget.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
						angle = Mathf.LerpAngle(angle,Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90,_controller.speedFactor);
                        _controller.SetAngle(angle);
	                    _controller.AddSpeed(_controller.GetSpeed() < 100 ? 20 * UpdateSpeed : 0);
                    }
	                break;
	            case AIStates.AttackEnemy:
	                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
	                        new Vector2(_selectedTarget.transform.position.x, _selectedTarget.transform.position.y)) > AttackRange)
	                {
	                    curState = AIStates.MoveToEnemy;
	                }
	                else
	                {
	                    Vector3 dir = new Vector2(_selectedTarget.transform.position.x, _selectedTarget.transform.position.y) - new Vector2(transform.position.x, transform.position.y);

                        Vector3 dot = _controller.Transform.InverseTransformPoint(_selectedTarget.Transform.position);
	                    if (dot.x > 0)
	                    {
	                        _controller.SetCannonSide(ShipController.ECannonSide.Right);
	                        angle = Mathf.LerpAngle(angle, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, _controller.speedFactor);
                        }
	                    else if (dot.x < 0)
	                    {
	                        _controller.SetCannonSide(ShipController.ECannonSide.Left);
	                        angle = Mathf.LerpAngle(angle, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180, _controller.speedFactor);
                        }

	                    _controller.SetAngle(angle);
                        _controller.AddSpeed(_controller.GetSpeed() > 0 ? -20 * UpdateSpeed : 0);
                        
                        _controller.FireCannons();
	                }
                    break;
	        }

	        _lastUpdate = Time.time;

	    }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ViewRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    public enum AIStates
    {
        Start,
        Scan,
        Wander,
        MoveToEnemy,
        AttackEnemy
    }
}
