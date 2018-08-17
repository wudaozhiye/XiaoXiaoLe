using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorUtils
{
    public class EditorCoroutine
    {

    }

    public abstract class MetaEditor : Editor
    {
        public Object metaTarget
        {
            get
            {
                try
                {
                    return target;
                }
                catch (System.Exception)
                {
                    return FindTarget();
                }
            }
        }

        public abstract Object FindTarget();
        public System.Action onRepaint = delegate { };

        public void RepaintIt()
        {
            Repaint();
            onRepaint.Invoke();
        }
    }
}
