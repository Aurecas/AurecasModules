using UnityEngine;

namespace AurecasLib.Particle {
    public class ParticleActions : MonoBehaviour {
        public ParticleSystem[] psTarget;
        public GameObject particlesParent;

        public void StartParticles(int id) {
            if(particlesParent != null)
            {
                particlesParent.SetActive(true);
            }

            psTarget[id].Play(true);
        }

        public void StopParticles(int id) {
            if (particlesParent != null)
            {
                particlesParent.SetActive(false);
            }

            psTarget[id].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}