using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dubi.Circuit;

public class AssignCircuitEventTest : CircuitEventComponentBool
{
    [SerializeField] Transform testObject = null;
    float angleSpeed = 5.0f;

    Vector3 startDir = Vector3.up;
    Vector3 goalDir = Vector3.down;

    Coroutine routine = null;

    protected override void OnRegister(bool circuitEventValue)
    {
        this.testObject.up = circuitEventValue ? this.goalDir : this.startDir;
    }

    protected override void OnValueChanged(bool circuitEventValue)
    {
        if (this.routine != null)
            StopCoroutine(this.routine);

        this.routine = StartCoroutine(RotateTo(circuitEventValue ? this.goalDir : this.startDir));
    }

    IEnumerator RotateTo(Vector3 goalDir)
    {
        while(Vector3.Angle(this.testObject.up, goalDir) > 0.1f)
        {
            Debug.Log(Vector3.Angle(this.testObject.up, goalDir));

            this.testObject.up = Vector3.RotateTowards(this.testObject.up, goalDir, Time.deltaTime * this.angleSpeed, 0.0f);
            yield return null;
        }

        this.testObject.up = goalDir;
    }
}
