using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

//inherits core features in class
public class DoorFeatures : CoreFeatures
{
    // Door Configuration - pivot information, open door state, max angle, Reverse, speed
    [Header("Door Configurations")]
    [SerializeField]
    private Transform doorPivot; //controls pivot

    [SerializeField]
    private float maxAngle = 90.0f; //Maybe < 90 will be wanted

    [SerializeField]
    private bool reverseAngleDirection = false;

    [SerializeField]
    private float doorSpeed = 1.0f;

    [SerializeField]
    private bool open = false;

    [SerializeField]
    private bool makeKinematicOnOpen = false;

    // Interaction Features for Socket Interactor, Simple Interactor 
    [Header("Interaction Configurations")]
    [SerializeField]
    private XRSocketInteractor socketInteractor;

    [SerializeField]
    private XRSimpleInteractable simpleInteractor;

    private void Start()
    {
        socketInteractor?.selectEntered.AddListener((s) =>
        {
            OpenDoor();
            PlayOnStart();
        });

        socketInteractor?.selectExited.AddListener((s) =>
        {
            PlayOnEnd();
            //When we're done xiting, we do not want to reuse the socket
            socketInteractor.socketActive = FeaturedUsage == FeaturedUsage.Once ? false : true;
        });

        simpleInteractor?.selectEntered.AddListener((s) =>
        {
            OpenDoor();
        });

        //REMOVE THIS
        //OpenDoor();
    }

    public void OpenDoor()
    {
        //open ? false : true;
        if (!open) 
        {
            PlayOnStart();
            open = true;
            StartCoroutine(ProcessMotion());
        }

    }

    private IEnumerator ProcessMotion()
    {
        while(open)
        {

        var angle = doorPivot.localEulerAngles.y < 180 ? doorPivot.localEulerAngles.y : doorPivot.localEulerAngles.y - 360;

        angle = reverseAngleDirection ? Mathf.Abs(angle) : angle;

        if (angle <= maxAngle)
        {
            doorPivot?.Rotate(Vector3.up, doorSpeed * Time.deltaTime * (reverseAngleDirection ? -1 : 1)); 
        }

        else
        {
            open = false;
            var featureRigidbody = GetComponent<Rigidbody>();
            if (featureRigidbody != null && makeKinematicOnOpen) featureRigidbody.isKinematic = true;
        }

        yield return null;
        }
    }

}
