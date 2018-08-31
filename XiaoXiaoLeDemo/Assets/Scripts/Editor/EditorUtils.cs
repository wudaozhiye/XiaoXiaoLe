using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorUtils
{
    public class EditorCoroutine
    {
        public static EditorCoroutine start(IEnumerator _routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_routine);
            coroutine.start();
            return coroutine;
        }
        readonly IEnumerator routine;
        EditorCoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }
        bool isPlaying = false;
        public void start()
        {
            EditorApplication.update += update;
            isPlaying = true;
        }
        public void stop()
        {
            EditorApplication.update -= update;
            isPlaying = false;
        }
        void update()
        {
            if (!routine.MoveNext())
            {
                stop();
            }
        }
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
