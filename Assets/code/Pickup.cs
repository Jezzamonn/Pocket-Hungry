using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.code
{
    public class Pickup : MonoBehaviour
    {
        public void GetGrabbed(Transform grabber)
        {
            transform.SetParent(transform);
            foreach (Transform trans in GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = LayerMask.NameToLayer("AntBody");
            }
        }

        public void GetUngrabbed(Transform grabber)
        {
            transform.SetParent(null);
            foreach (Transform trans in GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }
}
