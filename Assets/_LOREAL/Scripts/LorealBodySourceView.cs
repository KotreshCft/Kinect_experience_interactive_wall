
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;
using Kinect = Windows.Kinect;
public class LorealBodySourceView : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    public GameObject _handCircle;  // To store the instantiated hand circle object
    private ulong _closestBodyId;  // To track the closest body's ID

    public GameObject[] handObject;
    public Canvas _canvas;
    public RuntimeAnimatorController animatorController; 

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    void Update()
    {
        
        if (BodySourceManager == null || !_isDetectionActive)
        {
            return;
        }

        // Check for ESC key press to toggle skeleton visibility
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSkeletonView();
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        ulong closestBodyId = 0;
        float closestDistance = float.MaxValue;

        // Find the closest body
        foreach (var body in data)
        {
            if (body == null || !body.IsTracked)
            {
                continue;
            }

            trackedIds.Add(body.TrackingId);

            // Calculate distance to sensor (Z position)
            float distance = body.Joints[Kinect.JointType.SpineBase].Position.Z; // Using SpineBase to determine distance
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBodyId = body.TrackingId; // Track the closest body
            }
        }

        // Remove untracked bodies
        foreach (var trackingId in new List<ulong>(_Bodies.Keys))
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        // If there's a closest body detected, update the hand circle
        if (closestBodyId != 0)
        {
            if (!_Bodies.ContainsKey(closestBodyId))
            {
                _Bodies[closestBodyId] = CreateBodyObject(closestBodyId);
            }

            // Only update the right hand of the closest body
            UpdateRightHand(GetBodyById(closestBodyId));
            _closestBodyId = closestBodyId; // Store the closest body ID
        }
        else
        {
            _closestBodyId = 0; // Reset if no body is detected
            DestroyHandCircle(); // Optionally destroy the hand circle if no body is present
        }
    }
    public enum GameState { Idle, Running }
    public GameState currentGameState;
    private GameObject _currentHandObjectType; // Keep track of the current hand object type
    private bool _isSkeletonVisible = false; // Track whether the skeleton is visible or not

    // Method to toggle skeleton view
    private void ToggleSkeletonView()
    {
        _isSkeletonVisible = !_isSkeletonVisible; // Toggle the visibility state

        // Set the skeleton view visibility for all tracked bodies
        foreach (var body in _Bodies.Values)
        {
            body.SetActive(_isSkeletonVisible); // Show or hide the body objects
        }
    }

    /* private void UpdateRightHand(Kinect.Body body)
     {
         // Only track the right hand
         Kinect.Joint rightHand = body.Joints[Kinect.JointType.HandRight];
         Vector3 handPosition = GetVector3FromJoint(rightHand);

         // Determine which hand object to use based on the game state
         GameObject selectedHandObject = currentGameState == GameState.Running ? handObject[1] : handObject[0];

         // Check if the hand circle is instantiated
         if (_handCircle == null)
         {
             // Instantiate the hand object and set the current hand object type
             _handCircle = Instantiate(selectedHandObject);
             _handCircle.transform.SetParent(_canvas.transform, false);
             _handCircle.transform.position = handPosition;
             _currentHandObjectType = selectedHandObject; // Store the current hand object type
         }
         else if (_currentHandObjectType != selectedHandObject)
         {
             // If the hand object type has changed, destroy the old one and instantiate the new one
             Destroy(_handCircle);
             _handCircle = Instantiate(selectedHandObject);
             _handCircle.transform.SetParent(_canvas.transform, false);
             _handCircle.transform.position = handPosition;
             _currentHandObjectType = selectedHandObject; // Update the current hand object type
         }
         else
         {
             // If the correct hand object is already instantiated, just update its position
             _handCircle.transform.position = handPosition;
         }
     }*/
    private void UpdateRightHand(Kinect.Body body)
    {
        // Only track the right hand
        Kinect.Joint rightHand = body.Joints[Kinect.JointType.HandRight];
        Vector3 handPosition = GetVector3FromJoint(rightHand);

        // Determine which hand object to use based on the game state
        GameObject selectedHandObject = currentGameState == GameState.Running ? handObject[1] : handObject[0];

        // If the hand circle is not instantiated, instantiate it once
        if (_handCircle == null)
        {
            _handCircle = Instantiate(selectedHandObject);
            _handCircle.transform.SetParent(_canvas.transform, false);
            _currentHandObjectType = selectedHandObject; // Track the hand object type
        }

        // Smoothly update the position of the hand object
        if (_currentHandObjectType == selectedHandObject)
        {
            // Use Lerp to smooth the movement
            _handCircle.transform.position = Vector3.Lerp(_handCircle.transform.position, handPosition, Time.deltaTime * 10f);
        }
        else
        {
            // If switching between idle and running hand objects, only replace when necessary
            Destroy(_handCircle);
            _handCircle = Instantiate(selectedHandObject);
            _handCircle.transform.SetParent(_canvas.transform, false);
            _handCircle.transform.position = handPosition;
            _currentHandObjectType = selectedHandObject; // Update to the new hand object type
        }
    }


    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = BoneMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        body.gameObject.SetActive(false);
        return body;
    }
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.startColor = GetColorForState(sourceJoint.TrackingState);
                lr.endColor = GetColorForState(targetJoint.Value.TrackingState);
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    public GameObject[] GetBodies()
    {
        return _Bodies.Values.ToArray();
    }
    private bool _isDetectionActive = true; // Flag to control detection
    // Method to stop detection
    public void StopDetection()
    {
        _isDetectionActive = false;
    }
    // Method to restart detection (if needed)
    public void StartDetection()
    {
        _isDetectionActive = true;
    }
    private void DestroyHandCircle()
    {
        if (_handCircle != null)
        {
            Destroy(_handCircle);
            _handCircle = null; // Reset the hand circle reference
        }
    }
    // Helper method to get the vector from a Kinect joint
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
    // Helper method to get the color for a joint's tracking state
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;
            case Kinect.TrackingState.Inferred:
                return Color.red;
            default:
                return Color.black;
        }
    }
    // Method to get a body by its TrackingId
    public Kinect.Body GetBodyById(ulong trackingId)
    {
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return null;
        }
        foreach (var body in data)
        {
            if (body != null && body.TrackingId == trackingId && body.IsTracked)
            {
                return body;  // Return the body matching the provided TrackingId
            }
        }
        return null;  // No body found with the given TrackingId
    }
    public GameObject GetHandCircle()
    {
        return _handCircle;
    }
}