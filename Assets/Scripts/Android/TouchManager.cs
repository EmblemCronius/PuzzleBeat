﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Android.Touchmanager
{
    public delegate void OnTouchLost(int id);

    public class TouchManager : MonoBehaviour
    {
        public OnTouchLost onTouchLost;
        List<int> m_activeTouches = new List<int>();

        void Update()
        {
            UpdateTouches();
        }

        void UpdateTouches()
        {
            // Setup 
            List<int> IDOld = m_activeTouches;
            List<int> IDNew = new List<int>();
            List<int> IDRemove = new List<int>();
            List<int> IDAdd = new List<int>();

            // We work with touch finger ID's
            // Not the touch structs themselves
            Touch[] touches = Input.touches;
            for (int i = 0; i < touches.Length; i++)
            {
                IDNew.Add(touches[i].fingerId);
            }

            // If the list didn't change, don't bother.
            if (IDNew.SequenceEqual(IDOld))
                return;

            IDAdd = FilterOut(IDOld, IDNew);
            IDRemove = FilterOut(IDNew, IDOld);

            // Add whatever is new
            for (int i = 0; i < IDAdd.Count; i++)
            {
                OnNewTouch(IDAdd[i]);
                m_activeTouches.Add(IDAdd[i]);
            }

            // Add whatever is old
            for (int i = 0; i < IDRemove.Count; i++)
            {
                if (onTouchLost != null)
                    onTouchLost(IDRemove[i]);
                m_activeTouches.Remove(IDRemove[i]);
            }
        }
        private List<t> FilterOut<t>(List<t> elements, List<t> from)
        {
            List<t> E = new List<t>(from);
            for (int i = 0; i < elements.Count; i++)
            {
                E.Remove(elements[i]);
            }
            return E;
        }

        public Touch GetTouch(int ID)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == ID)
                {
                    Touch touch = Input.touches[i];
                    touch.position = new Vector3(touch.position.x, touch.position.y, 0);
                    return touch;
                }
            }
            return new Touch();
        }

        void OnNewTouch(int ID)
        {
            Touch touch = GetTouch(ID);
            Vector2 worldpos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(worldpos, Vector2.zero);

            if (hit && hit.collider)
            {
                GameObject hitObject = hit.collider.gameObject;
                TouchListener reciever = hitObject.GetComponent<TouchListener>();
                if (reciever)
                {
                    reciever.RecieveTouchID(ID);
                }
            }
        }
    }
}