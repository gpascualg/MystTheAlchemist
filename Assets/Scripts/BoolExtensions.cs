using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoolExtensions
{
    public static int ToInt(this bool self)
    {
        return self ? 1 : 0;
    }
}
