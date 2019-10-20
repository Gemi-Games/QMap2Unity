﻿using System;
using UnityEngine;

[Serializable]
public class SingleLayerMask
{
    [SerializeField]
    private int m_LayerIndex = 0;

    public SingleLayerMask(int lLayerIndex)
    {
        m_LayerIndex = lLayerIndex;
    }

    public int LayerIndex
    {
        get { return m_LayerIndex; }
    }

    public void Set(int lLayerIndex)
    {
        if (lLayerIndex > 0 && lLayerIndex < 32)
        {
            m_LayerIndex = lLayerIndex;
        }
    }

    public int Mask
    {
        get { return 1 << m_LayerIndex; }
    }
}
