using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpriteDepth : UISprite {

    protected override Vector3 CreateVertexPos(float x, float y, float z = 0)
    {
        return new Vector3(x, y, 1.0f/depth);
    }
}
