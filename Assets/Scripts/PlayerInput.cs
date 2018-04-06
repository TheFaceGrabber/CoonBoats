using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public ShipController Ship;

    private bool _showingBeached;

    private Vector3 _mousePositionInWorld;
    // Update is called once per frame
    void Update()
    {
        _showingBeached = Ship.GetSpeed() <= 0 && Ship.GetIsBeached();

        if (!Ship.GetIsBeached())
        {
            _mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ship.SetAngle((Mathf.Atan2(_mousePositionInWorld.y - Ship.Transform.position.y,
                               _mousePositionInWorld.x - Ship.Transform.position.x) * Mathf.Rad2Deg - 90));

            if (UserInput.GetInput("Forwards"))
            {
                Ship.AddSpeed(Ship.GetSpeed() < 100 ? 1 : 0);
            }

            if (UserInput.GetInput("Backwards"))
            {
                Ship.AddSpeed(Ship.GetSpeed() > 0 ? -1 : 0);
            }
        }

        if (UserInput.GetInputDown("Interact"))
        {
            if (_showingBeached)
            {
                Ship.BeginUnBeaching();
            }
        }

        if (UserInput.GetInputDown("ToggleCannonSide"))
        {
            if (Ship.GetCannonSide() == ShipController.ECannonSide.Left)
            {
                Ship.SetCannonSide(ShipController.ECannonSide.Right);
            }
            else
            {
                Ship.SetCannonSide(ShipController.ECannonSide.Left);
            }
        }

        if (UserInput.GetInputDown("Fire"))
        {
            Ship.FireCannons();
        }
    }

    void OnGUI()
    {
        if (_showingBeached)
        {
            string text = "Press " + UserInput.GetKeyForButton("Interact") + " to un-beach your ship";
            float length = text.Length * 8;
            GUI.Box(new Rect(Screen.width / 2 - (length / 2), Screen.height / 2 + 60, length, 30), text);
        }
    }
}
