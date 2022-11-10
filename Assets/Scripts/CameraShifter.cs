using UnityEngine;

public class CameraShifter : MonoBehaviour
{
    public Camera Camera;
    public GameObject Target;
    public Bounds targetBounds;

    //я вспомнил что эта штука делает
    //она центрирует камеру, чтобы любая сетка ровно помещалась в экран
    public void RecenterCamera()
    {
        Vector3 center = GetCenter();
        center.z -= 5;//магическая переменная
        Camera.transform.position = center;
        FitToBounds();
    }

    Vector3 GetCenter()
    {
        //берем ВСЕ рендереры элементов, ахаха
        var rends = Target.GetComponentsInChildren<Renderer>();
        
        //если их нет то просто берем центр
        if (rends.Length == 0)
            return transform.position;
        var bounds = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
        {
            //в целом ненужная, но прикольная тема, мы из многих рендеров делаем большую рамку
            //получается в итоге общая зона всех рендеров
            bounds.Encapsulate(rends[i].bounds);
        }

        //я так сделал из-за того, что у меня сетка (фон) тоже состоит из динамичных элементов
        //поэтому я не могу просто взять ее баундс. Но в целом можно было просто растянуть плашку
        targetBounds = bounds;
        return bounds.center;
    }

    void FitToBounds()
    {
        //подстраиваем наши баундс под размер экрана чтоб сетка четко влезала
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

        //даже ниче тут не переделывал
        transform.position = new Vector3(targetBounds.center.x, targetBounds.center.y, -1f);
    }
}
