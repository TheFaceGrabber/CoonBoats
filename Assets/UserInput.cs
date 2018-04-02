using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

public class UserInput : MonoBehaviour {

    public static List<InputKey> AllKeys = new List<InputKey>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (File.Exists("Inputs.xml"))
        {
            LoadXml();
            Debug.Log("Loaded Keys with " + AllKeys.Count.ToString() + " keys");
        }
        else
        {
            ResetKeys();    
            SaveToXml();
            Debug.Log("Saved keys");
        }
    }

    public static void ResetKeys()
    {
        AllKeys.Clear();
        AllKeys.Add(new InputKey("LeftSide", KeyCode.Q));
        AllKeys.Add(new InputKey("RightSide", KeyCode.E));
        AllKeys.Add(new InputKey("Forwards",KeyCode.W));
        AllKeys.Add(new InputKey("Backwards", KeyCode.S));
        AllKeys.Add(new InputKey("Fire", KeyCode.Space));
    }

    static InputKey GetInputKey(string name)
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            if (AllKeys[i].InputName == name)
                return AllKeys[i];
        }

        return null;
    }

    public static bool GetInputDown(string input)
    {
        InputKey inp = GetInputKey(input);
        if (!inp.IsNull())
        {
            return Input.GetKeyDown(inp.InputKeyCode);
        }

        return false;
    }

    public static bool GetInput(string input)
    {
        InputKey inp = GetInputKey(input);
        if (!inp.IsNull())
        {
            return Input.GetKey(inp.InputKeyCode);
        }

        return false;
    }

    public static bool GetInputUp(string input)
    {
        InputKey inp = GetInputKey(input);
        if (!inp.IsNull())
        {
            return Input.GetKeyUp(inp.InputKeyCode);
        }

        return false;
    }

    public static bool SaveToXml()
    {
        XmlDocument doc = new XmlDocument();
        var xs = new XmlSerializer(typeof(List<InputKey>));
        using (MemoryStream stream = new MemoryStream())
        {
            xs.Serialize(stream,AllKeys);
            stream.Position = 0;
            doc.Load(stream);
            doc.Save("Inputs.xml");
            stream.Close();
        }
        return true;
    }

    public static bool LoadXml()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("Inputs.xml");
        string xmStr = doc.OuterXml;
        using (StringReader reader = new StringReader(xmStr))
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<InputKey>));
            using (XmlReader r = new XmlTextReader(reader))
            {
                AllKeys = (List<InputKey>) ser.Deserialize(r);
                r.Close();
            }

            reader.Close();
        }

        return true;
    }
}

[System.Serializable]
public class InputKey
{
    public string InputName;
    public KeyCode InputKeyCode;

    public InputKey(string name, KeyCode code)
    {
        InputName = name;
        InputKeyCode = code;
    }

    public InputKey()
    {

    }

    public bool IsNull()
    {
        return string.IsNullOrEmpty((InputName));
    }
}