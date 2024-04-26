using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CatigerStudio.Utils
{
    public class AspectRatioTracker : MonoBehaviour
    {
        // Start is called before the first frame update
        private Camera main_camera;

        void Start()
        {
            main_camera = Camera.main;
        }

        public UnityEvent onAspectRatioChanged;

        [SerializeField]
        private float currentAspect;

        // Update is called once per frame
        void Update()
        {
            float new_aspect = main_camera.aspect;
            if (new_aspect != currentAspect)
            {
                currentAspect = new_aspect;
                onAspectRatioChanged?.Invoke();
            }
        }
    }
}