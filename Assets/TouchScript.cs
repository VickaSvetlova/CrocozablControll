﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum state
{
    move = 0,
    rotation = 1,
    setpoint = 2,
    inside = 3,
    scale = 4
}

public class TouchScript : MonoBehaviour
{
    public LayerMask touchInputMask;
    private Transform target = null;
    private objectScript targetScript;
    private Vector3 trim;
    private float dist;
    private state Stat;
    private Vector3 startPos;

    public float SensevityScale = 0.001f;
    public float rotationSpeed = 540f; // скорость вращения

    // ------------------------------------------
    private bool _inside;

    public Text text;
    public bool setPos;

    //----------------------------------------------
    //moveInside
    [Header("Слой/слои пола")]
    public LayerMask layer_floor;

    private void Start()
    {
        Stat = state.move;
        text.text = Stat.ToString();
    }

    public void statesSwich(string str)
    {
        setPos = false;
        if (str == "move")
        {
            Stat = state.move;
        }
        if (str == "rotate")
        {
            Stat = state.rotation;
        }
        if (str == "setpoint")
        {
            Stat = state.setpoint;
            setPos = true;
        }
        if (str == "inside")
        {
            Stat = state.inside;
            _inside = true;
        }
        if (str == "scale")
        {
            Stat = state.scale;
        }
        text.text = Stat.ToString();
    }

    public void changeSens(float sens)
    {
        rotationSpeed = sens;
        SensevityScale = sens;
        text.text = "" + rotationSpeed;
    }

    void OnMoveStart(Vector3 _pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(_pos);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);

        if (Physics.Raycast(ray, out hit, touchInputMask))
        {
            target = hit.transform;
            dist = hit.distance;
            trim = hit.collider.transform.position - hit.point;
        }
    }

    void OnMoveStay(Vector3 _pos)
    {
        if (!target) return;
        Ray ray = Camera.main.ScreenPointToRay(_pos);
        Vector3 pos = ray.origin + ray.direction * dist;
        pos += trim;
        target.position = pos;
    }

    void OnMoveEnd(Vector3 _pos)
    {
        target = null;
    }

    void OnRotateStart(Vector3 _pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(_pos);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);

        if (Physics.Raycast(ray, out hit, touchInputMask))
        {
            target = hit.transform;
            startPos = _pos;
        }
    }

    void OnRotateStay(Vector3 _pos)
    {
        if (!target) return;
        float dX = (_pos.x - startPos.x) / Screen.width;
        float dY = (_pos.y - startPos.y) / Screen.height;
        target.Rotate(Vector3.up, -dX * rotationSpeed, Space.World);
        target.Rotate(Vector3.up, -dY * rotationSpeed, Space.World);
        startPos = _pos;


    }

    void OnRotateEnd(Vector3 _pos)
    {
        target = null;
    }

    void OnZoomStart(Vector3 _pos1, Vector3 _pos2)
    {
        targetScript = (objectScript)FindObjectOfType(typeof(objectScript));
        dist = (_pos1 - _pos2).magnitude;
    }

    void OnZoomStay(Vector3 _pos1, Vector3 _pos2)
    {
        if (!targetScript) return;
        float delta = dist - (_pos1 - _pos2).magnitude;
        targetScript.Scale(delta * SensevityScale);
    }

    void OnZoomEnd(Vector3 _pos1, Vector3 _pos2)
    {
        target = null;
    }

    void Update()
    {
#if UNITY_EDITOR

        switch (Stat)
        {
            case state.move:

                if (Input.GetMouseButtonDown(0))
                    OnMoveStart(Input.mousePosition);
                if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
                    OnMoveStay(Input.mousePosition);
                if (!Input.GetMouseButton(0))
                    OnMoveEnd(Vector3.zero);
                break;

            //поворот
            case state.rotation:

                if (Input.GetMouseButtonDown(0))
                    OnRotateStart(Input.mousePosition);
                if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
                    OnRotateStay(Input.mousePosition);
                if (!Input.GetMouseButton(0))
                    OnRotateEnd(Vector3.zero);
                break;

            case state.scale:

                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    OnZoomStart(Vector3.left, Vector3.right);
                    OnZoomStay(Vector3.left, Vector3.right * (1 + Input.GetAxis("Mouse ScrollWheel"))); // типа двигаем правый тач
                }
                else
                {
                    OnZoomEnd(Vector3.zero, Vector3.zero);
                }
                break;
        }
#endif

        switch (Stat)
        {

            case state.move:

                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                        OnMoveStart(touch.position);
                    if (touch.phase == TouchPhase.Moved)
                        OnMoveStay(touch.position);
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        OnMoveEnd(touch.position);
                }
                break;
            case state.rotation:

                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                        OnRotateStart(touch.position);
                    if (touch.phase == TouchPhase.Moved)
                        OnRotateStay(touch.position);
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        OnRotateEnd(touch.position);
                }
                break;
            case state.scale:
                if (Input.touchCount == 2)
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);
                    if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                        OnZoomStart(touch1.position, touch2.position);
                    if ((touch1.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Stationary) && // тач1 стоит или двигается
                       (touch2.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Stationary) && // тач2  стоит или двигается
                       (touch1.phase != TouchPhase.Stationary && touch2.phase != TouchPhase.Stationary)) // тач1 не стоит && тач2 не стоит
                        OnZoomStay(touch1.position, touch2.position);
                    if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended ||
                       touch1.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
                        OnZoomEnd(touch1.position, touch2.position);
                }
                break;
        }
    }
    public void Inside()
    {
        objectScript objTemp = (objectScript)FindObjectOfType(typeof(objectScript));
        if (objTemp != null)
        {
            objTemp.Inside();
        }
    }
    #region move_control_Inside
    private Vector3 MoveTo(Ray _ray)
    {
        bool toUp = _ray.direction.y > 0f ? true : false; // вверх или вниз?
        RaycastHit[] rhs = Physics.RaycastAll(_ray, 100f, layer_floor); // рейкастим насквозь
        int min1 = 0; // индекс ближайшего
        int min2 = -1; // индекс следующего после ближайшего
        if (rhs.Length > 1) // если больше одного попадания - ищем очередность...
        {
            for (int i = 1; i < rhs.Length; i++)
            {
                if (rhs[i].distance < rhs[min1].distance)
                {
                    min1 = i;
                }
                else
                {
                    if (min2 != -1)
                    {
                        if (rhs[i].distance < rhs[min2].distance)
                            min2 = i;
                    }
                    else min2 = i;
                }
            }
        }
        else // ...иначе очередность не нужна
        {
            min1 = 0;
            min2 = 0;
        }

        switch (toUp)
        {
            case (true): // если вверх, то надо поднять точку над полом
                {
                    Collider col = rhs[min1].collider; // коллайдер
                    Renderer rend = col.gameObject.GetComponent<Renderer>(); // рендер коллайдера
                    float height = rend.bounds.size.y + 0.01f; // высота пола + чуть-чуть
                    Vector3 v = rhs[min1].point; // точка коллизии
                    v.y += height; // поднимаем на высоту
                    Ray ray = new Ray(v, Vector3.down); // луч вниз для поиска верхней грани
                    RaycastHit rh; // ага-ага
                    Physics.Raycast(ray, out rh); // рейкастим, проверять не надо, ведь мы ТОЧНО над коллайдером
                    print("To up:" + rh.collider.name); // дебаг ))
                    return rh.point; // отдаём точку
                    
                }

            case (false): // если вниз, то просто отдаём вторую точку
                {
                    print("To down:" + rhs[min2].collider.name); // дебаг ))
                    return rhs[min2].point; // отдаём точку
                   
                }

        }
        return Vector3.zero;
    }
}