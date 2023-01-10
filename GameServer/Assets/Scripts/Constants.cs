using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This is file ensures the server's tickrate stays constant in unity
//It can be found in this video https://www.youtube.com/watch?v=4uHTSknGJaY&feature=youtu.be made by Tom Weiland
public class Constants
{
    public const int TICKS_PER_SEC = 30; // How many ticks per second
    public const float MS_PER_TICK = 1000f / TICKS_PER_SEC; // How many milliseconds per tick
}