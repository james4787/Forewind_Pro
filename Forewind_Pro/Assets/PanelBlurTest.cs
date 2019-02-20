using GaussianBlur_Mobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Forewind
{
	public class PanelBlurTest : MonoBehaviour {

        public float Iterations = 8;
        public float DownRes = 2;

        public Gradient TintGradient;

        public BlurRenderer BRM;
        public Material BlurMaterial;

        void Start()
        {
            BRM = BlurRenderer.Create();
        }

        // Update is called once per frame
        void Update()
        {
            Color thisColor = TintGradient.Evaluate(0f);

            BRM.Iterations = (int) Iterations;
            BRM.DownRes = (int) DownRes;
            BRM.UpdateBlur = true;
            BRM.UpdateRate = 0.001f;

            BlurMaterial.SetFloat("_Saturation", 1.0f);
            BlurMaterial.SetFloat("_Lightness", 0.5f);
            BlurMaterial.SetColor("_TintColor", thisColor);

        }
    }
}