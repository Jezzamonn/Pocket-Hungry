using UnityEngine;

namespace Assets.code
{
    public static class Helpers
    {
        public static void SetLayer(this Transform tranform, string layer)
        {
            foreach (Transform trans in tranform.gameObject.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
    }
}
