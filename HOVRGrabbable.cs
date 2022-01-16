using UnityEngine;

/*
 * Author: Jeroen Ceyssens (https://github.com/Jecey)
 * See LICENSE file to see availability of this script
 */
public class HOVRGrabbable : OVRGrabbable
{
    private Transform _snapTransform;
    private bool _shouldSnapPos;
    private bool _shouldSnapRot;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Set own custom values equal to the inspector values of OVRGrabbable
        _snapTransform = m_snapOffset;
        _shouldSnapPos = snapPosition;
        _shouldSnapRot = snapOrientation;

        // Disable the editor values after copying to stop the base class from carrying out
        // the procedures
        m_snapOffset = null;
        m_snapOrientation = false;
        m_snapPosition = false;
    }

    // Override the GrabBegin call of OVRGrabbable and utilize the custom snapping mechanic.
    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        if (_snapTransform)
        {
            if (_shouldSnapRot)
            {
                Quaternion handRotation = hand.transform.rotation;
                if (hand.name.ToLower().Contains("left")) // Flip the Z axis to give the same results as the right hand
                    handRotation.eulerAngles = new Vector3(handRotation.eulerAngles.x, handRotation.eulerAngles.y, 180 + handRotation.eulerAngles.z);
                this.transform.rotation = handRotation * Quaternion.Inverse(Quaternion.Inverse(this.transform.rotation) * _snapTransform.rotation);
            }
            if (_shouldSnapPos)
            {
                this.transform.position = hand.transform.position;
                Vector3 snapPoint = _snapTransform.position;
                if (hand.name.ToLower().Contains("left")) // Flip the Y value due to flipped Z axis
                   snapPoint = this.transform.TransformPoint(new Vector3(_snapTransform.localPosition.x, -_snapTransform.localPosition.y, _snapTransform.localPosition.z));
                Vector3 diff = hand.transform.position - snapPoint;
                this.transform.position += diff;
            }

        }
        base.GrabBegin(hand, grabPoint);
    }
}
