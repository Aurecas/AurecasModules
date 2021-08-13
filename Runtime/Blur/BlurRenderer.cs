using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Blur {
    [RequireComponent(typeof(Camera))]
    public class BlurRenderer : MonoBehaviour {
        // Types
        [System.Serializable]
        public struct ShaderParams {
            [Range(1, 20)] public int iterations;
            [Range(0, 4)] public int downRes;
            [Range(0, 2)] public float brightness;
            [Range(0, 2)] public float saturation;

            public static ShaderParams standard {
                get {
                    return new ShaderParams {
                        iterations = 8,
                        downRes = 1,
                        brightness = 0.3f,
                        saturation = 0.5f
                    };
                }
            }
        }

        // Static
        public static BlurRenderer instance;

        // Components
        Camera myCamera;

        // Internal
        Material shaderPassMaterial;

        public Material GetBlur(ShaderParams param) {
            // Create texture
			int w = 512;
			int h = 512;
            if(myCamera.aspect < 1) {
                w = 512;
                h = 1024;
            }
            else if(myCamera.aspect > 1) {
                w = 1024;
                h = 5012;
            }
            RenderTexture tex = new RenderTexture(w >> param.downRes, h >> param.downRes, 24, RenderTextureFormat.ARGB32);
            tex.Create();

            // Render
            RenderFromScreen(tex, param);
            
            // Create material and assign texture
            Material mat = new Material(Shader.Find("AurecasLib/Blur/GaussianBlur"));
            mat.SetTexture("_MobileBlur", tex);
            mat.SetFloat("_Lightness", param.brightness);
            mat.SetFloat("_Saturation", param.saturation);

            // Result
            return mat;
        }

        public Material GetBlurFromTexture(Texture source, ShaderParams param) {
            // Create texture
            RenderTexture tex = new RenderTexture(1024 >> param.downRes, 512 >> param.downRes, 24, RenderTextureFormat.ARGB32);
            tex.Create();

            // Render
            RenderFromTexture(source, tex, param);

            // Create material and assign texture
            Material mat = new Material(Shader.Find("AurecasLib/Blur/GaussianBlur"));
            mat.SetTexture("_MobileBlur", tex);
            mat.SetFloat("_Lightness", param.brightness);
            mat.SetFloat("_Saturation", param.saturation);

            // Result
            return mat;
        }

        public void FreeBlur(Material mat) {
            // Dispose blur assets
            if (mat != null) {
                var tex = mat.GetTexture("_MobileBlur");
                Destroy(tex);
                Destroy(mat);
            }
        }

        void RenderFromScreen(RenderTexture target, ShaderParams param) {
            // Render the camera to a texture of the same size as the screen (canvases should remain the same size)
            RenderTexture screen = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            myCamera.targetTexture = screen;
            myCamera.Render();
            myCamera.targetTexture = null;

            // Define texture width and height
            int width = target.width >> param.downRes; //Screen.width >> shaderDownRes;
            int height = (int)(width / (float)Screen.width * Screen.height); //Screen.height >> shaderDownRes;

            // Copy screen texture to temp texture
            RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            Graphics.Blit(screen, rt);
            RenderTexture.ReleaseTemporary(screen);

            // Loop to add the blur to temp texture
            for (int i = 0; i < param.iterations; i++) {
                RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(rt, rt2, shaderPassMaterial);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            // Copy temp texture to the target
            Graphics.Blit(rt, target);

            // Free temp texture
            RenderTexture.ReleaseTemporary(rt);
        }

        void RenderFromTexture(Texture source, RenderTexture target, ShaderParams param) {

            if(source == null) {
                source = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
                myCamera.targetTexture = source as RenderTexture;
                myCamera.Render();
                myCamera.targetTexture = null;
            }

            // Set the width and height
            int width = source.width >> param.downRes;
            int height = (int)(width / (float)source.width * source.height);

            // Create a temp texture
            RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);

            // Move the provided texture to our temp render texture
            Graphics.Blit(source, rt);

            // Loop to add the blur to our temp texture
            for (int i = 0; i < param.iterations; i++) {
                RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(rt, rt2, shaderPassMaterial);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            // Store our texture in target
            Graphics.Blit(rt, target);

            // Free the temp texture
            RenderTexture.ReleaseTemporary(rt);
        }

        void OnDestroy() {
            // Free assets from memory
            if (shaderPassMaterial == null) {
                Destroy(shaderPassMaterial);
            }
        }

        void Awake() {
            // Components
            myCamera = GetComponent<Camera>();

            // Create our material if we have not already
            if (shaderPassMaterial == null) {
                shaderPassMaterial = new Material(Shader.Find("Hidden/GaussianBlur"));
            }
        }

        /// <summary>
        /// Creates a BlurRenderer on the main camera
        /// </summary>
        static public BlurRenderer Create() {
            BlurRenderer BRM = Camera.main.gameObject.GetComponent<BlurRenderer>();

            if (BRM == null) {
                BRM = Camera.main.gameObject.AddComponent<BlurRenderer>();
            }

            instance = BRM;
            return BRM;
        }
        
        static public BlurRenderer Create(Camera ThisCamera) {
            BlurRenderer BRM = ThisCamera.gameObject.GetComponent<BlurRenderer>();

            if (BRM == null) {
                BRM = ThisCamera.gameObject.AddComponent<BlurRenderer>();
            }

            instance = BRM;
            return BRM;
        }
    }
}
