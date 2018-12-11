using UnityEngine;

public class CameraShifter : MonoBehaviour
{
    public Camera Camera;
    public GameObject Target;
    public Bounds targetBounds;

    public void Go()
    {
        Vector3 center = GetCenter();
        center.z -= 5;
        Camera.transform.position = center;
        FitToBounds();
    }

    Vector3 GetCenter()
    {
        var rends = Target.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
            return transform.position;
        var b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
        {
            b.Encapsulate(rends[i].bounds);
        }

        targetBounds = b;
        return b.center;
    }

    void FitToBounds()
    {

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetBounds.size.x / targetBounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.orthographicSize = targetBounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.orthographicSize = targetBounds.size.y / 2 * differenceInSize;
        }

        transform.position = new Vector3(targetBounds.center.x, targetBounds.center.y, -1f);

    }
}
