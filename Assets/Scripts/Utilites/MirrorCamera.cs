using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MirrorCamera : MonoBehaviour
{
    private Transform MainCameratrans;
    private PositionConstraint positionConstraint;

    void Start()
    {
        MainCameratrans = GameObject.FindGameObjectWithTag("MainCamera").transform;
        positionConstraint = GetComponent<PositionConstraint>();

        if(positionConstraint != null && MainCameratrans != null)
            positionConstraint.AddSource(new ConstraintSource{sourceTransform = MainCameratrans, weight = 1});
    }

}
