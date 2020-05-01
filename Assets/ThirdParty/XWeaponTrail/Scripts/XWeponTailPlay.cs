using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class XWeponTailPlay : MonoBehaviour
{
    private XWeaponTrail[] trails;
    void Awake()
    {
        var trail = GetComponent<XWeaponTrail>();
        if (trail != null)
        {
            Debug.LogError("XWeponTailPlay 只能是XWeaponTrail的父节点");
        }

        trails = GetComponentsInChildren<XWeaponTrail>();
    }

    void OnEnable()
    {
        if (trails != null)
        {
            foreach (var trail in trails)
            {
                trail.Activate();
            }
        }
    }

    void OnDisable()
    {
        if (trails != null)
        {
            foreach (var trail in trails)
            {
                trail.Deactivate();
            }
        }
    }
}
