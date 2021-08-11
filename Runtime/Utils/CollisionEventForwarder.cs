using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AurecasLib.Utils {

    public class CollisionEventForwarder : MonoBehaviour {
        public Action<Collision> CollisionEnterEvent;
        public Action<Collision> CollisionStayEvent;
        public Action<Collision> CollisionExitEvent;

        public Action<Collider> TriggerEnterEvent;
        public Action<Collider> TriggerStayEvent;
        public Action<Collider> TriggerExitEvent;

        //2D
        public Action<Collision2D> CollisionEnterEvent2D;
        public Action<Collision2D> CollisionStayEvent2D;
        public Action<Collision2D> CollisionExitEvent2D;

        public Action<Collider2D> TriggerEnterEvent2D;
        public Action<Collider2D> TriggerStayEvent2D;
        public Action<Collider2D> TriggerExitEvent2D;

        void OnCollisionEnter(Collision collision) {
            CollisionEnterEvent?.Invoke(collision);
        }

        void OnCollisionStay(Collision collision) {
            CollisionStayEvent?.Invoke(collision);
        }

        void OnCollisionExit(Collision collision) {
            CollisionExitEvent?.Invoke(collision);
        }

        private void OnTriggerEnter(Collider other) {
            TriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other) {
            TriggerStayEvent?.Invoke(other);
        }
        private void OnTriggerExit(Collider other) {
            TriggerExitEvent?.Invoke(other);
        }

        //2D

        void OnCollisionEnter2D(Collision2D collision) {
            CollisionEnterEvent2D?.Invoke(collision);
        }

        void OnCollisionStay2D(Collision2D collision) {
            CollisionStayEvent2D?.Invoke(collision);
        }

        void OnCollisionExit2D(Collision2D collision) {
            CollisionExitEvent2D?.Invoke(collision);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            TriggerEnterEvent2D?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other) {
            TriggerStayEvent2D?.Invoke(other);
        }
        private void OnTriggerExit2D(Collider2D other) {
            TriggerExitEvent2D?.Invoke(other);
        }

    }
}