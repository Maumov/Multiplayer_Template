using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConstants
{
    public static ushort broadcastPort = 47777; // Puerto UDP para LAN discovery
    public static float broadcastInterval = 1f; // Cada cuánto se envía broadcast (seg)
    public static string broadcastMessage = "UNITY_SERVER";
}