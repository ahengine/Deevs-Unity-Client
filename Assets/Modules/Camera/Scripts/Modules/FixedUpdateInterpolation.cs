using UnityEngine;
using System.Collections;

/// <summary>
/// Interpolates a GameObject's position and rotation while being updated in FixedUpdate.
/// </summary>
[DefaultExecutionOrder(-100)]
public class FixedUpdateInterpolation : MonoBehaviour
{
    private Vector3 pos0;
    private Vector3 pos1;
    private Quaternion rot0;
    private Quaternion rot1;

    private Vector3 lastUpdatePos;
    private Quaternion lastUpdateRot;

    private readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void OnEnable()
    {
        lastUpdatePos = pos0 = pos1 = transform.localPosition;
        lastUpdateRot = rot0 = rot1 = transform.localRotation;
        StartCoroutine(LateFixedUpdate());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void FixedUpdate()
    {
        var pos = transform.localPosition;
        if (pos == lastUpdatePos)
        {
            transform.localPosition = pos1;
            lastUpdatePos = pos1;
        }
        else
        {
            AcceptUpdatedPosition(pos);
        }

        var rot = transform.localRotation;
        if (rot == lastUpdateRot)
        {
            transform.localRotation = rot1;
            lastUpdateRot = rot1;
        }
        else
        {
            AcceptUpdatedRotation(rot);
        }
    }

    private IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return waitForFixedUpdate;
            pos0 = pos1;
            pos1 = transform.localPosition;
            lastUpdatePos = pos1;

            rot0 = rot1;
            rot1 = transform.localRotation;
            lastUpdateRot = rot1;
        }
    }

    private void Update()
    {
        var t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;

        var pos = transform.localPosition;
        if (pos == lastUpdatePos)
        {
            var newPosition = Vector3.Lerp(pos0, pos1, t);
            transform.localPosition = newPosition;
            lastUpdatePos = newPosition;
        }
        else
        {
            AcceptUpdatedPosition(pos);
        }

        var rot = transform.localRotation;
        if (rot == lastUpdateRot)
        {
            var newRotation = Quaternion.Lerp(rot0, rot1, t);
            transform.localRotation = newRotation;
            lastUpdateRot = newRotation;
        }
        else
        {
            AcceptUpdatedRotation(rot);
        }
    }

    private void AcceptUpdatedPosition(Vector3 pos)
    {
        pos0 = pos1 = pos;
        lastUpdatePos = pos;
    }

    private void AcceptUpdatedRotation(Quaternion rot)
    {
        rot0 = rot1 = rot;
        lastUpdateRot = rot;
    }
}