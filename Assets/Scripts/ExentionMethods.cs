using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExentionMethods {
    
    public static bool isEmpty(this Texture2D tex)
    {
        bool isEmtpy = true;
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                if (tex.GetPixel(x, y) != Color.clear)
                {
                    isEmtpy = false;
                    return isEmtpy;
                }
            }
        }

        return isEmtpy;
    }
}
