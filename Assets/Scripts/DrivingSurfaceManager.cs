using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DrivingSurfaceManager : MonoBehaviour
{
    public ARPlaneManager PlaneManager;
    public ARRaycastManager RaycastManager;
    public ARPlane LockedPlane;

    public void LockPlane(ARPlane keepPlane)
    {
        if (LockedPlane == keepPlane) return;

        foreach (var plane in PlaneManager.trackables)
        {
            if (plane != keepPlane)
            {
                plane.gameObject.SetActive(false);
            }
        }

        LockedPlane = keepPlane;

        PlaneManager.planesChanged -= DisableNewPlanes;
        PlaneManager.planesChanged += DisableNewPlanes;
    }

    private void Start()
    {
        PlaneManager = GetComponent<ARPlaneManager>();
    }

    private void Update()
    {
        // Follow plane merges so the platform keeps growing (infinite surface)
        if (LockedPlane?.subsumedBy != null)
        {
            LockedPlane = LockedPlane.subsumedBy;
        }
    }

    private void DisableNewPlanes(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            if (plane != LockedPlane)
                plane.gameObject.SetActive(false);
        }
    }
}
