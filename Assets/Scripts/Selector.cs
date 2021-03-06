﻿using UnityEngine;

public class Selector : MonoBehaviour
{
    private Transform element;
    public ElementGrid Grid;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if (hitInfo.transform.gameObject.tag == "Element")
                {
                    if (element == null)
                    {
                        element = hitInfo.transform;
                        element.GetComponent<Renderer>().material.shader=Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
                    }
                    else if (hitInfo.transform == element)
                    {
                        element.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                        element = null;
                    }
                    else
                    {
                        var first = element.gameObject.GetComponent<Element>();
                        var second = hitInfo.transform.gameObject.GetComponent<Element>();

                        int xdiff = Mathf.Abs(first.X - second.X);
                        int ydiff = Mathf.Abs(first.Y - second.Y);
                        if (xdiff < 2 && ydiff < 2 && (ydiff+xdiff<2))
                        {
                            Grid.TryChange(first,second);
                        }

                        element.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                        element = null;
                    }
                }
            }
        }

    }
}
