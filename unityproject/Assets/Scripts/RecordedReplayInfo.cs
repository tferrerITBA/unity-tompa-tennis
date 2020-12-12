﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordedReplayInfo
{
    public Vector3 position;
    public Quaternion rotation;

    public RecordedReplayInfo(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
