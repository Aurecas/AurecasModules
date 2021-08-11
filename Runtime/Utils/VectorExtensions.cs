using UnityEngine;
namespace AurecasLib.Utils {
    public static class VectorExtensions {
        // Gets the angle from deltas
        public static float DeltaToAngle(this Vector2 vec) {
            return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        }

        // Gets the angle between two points in space
        public static float AngleBetweenPoints(this Vector2 vec, Vector2 other) {
            return Mathf.Atan2(other.y - vec.y, other.x - vec.x) * Mathf.Rad2Deg;
        }

        // Transforms a angular coordinate (angle, magnitute) into a cartesian (X, Y) coord
        public static Vector2 AngularToCartesian(this Vector2 polar) {
            float rad = polar.x * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * polar.y;
        }

        public static float DistanceToSegment(this Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest) {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;

            // Is it a point?
            if ((dx == 0) && (dy == 0)) {
                closest = p1;
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance
            float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's end points or a point in the middle
            if (t < 0) {
                closest = new Vector2(p1.x, p1.y);
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
            }
            else if (t > 1) {
                closest = new Vector2(p2.x, p2.y);
                dx = pt.x - p2.x;
                dy = pt.y - p2.y;
            }
            else {
                closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
                dx = pt.x - closest.x;
                dy = pt.y - closest.y;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static float DistanceToSegment(this Vector2 pt, Vector2 p1, Vector2 p2) {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;

            // Is it a point?
            if ((dx == 0) && (dy == 0)) {
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance
            float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's end points or a point in the middle
            if (t < 0) {
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
            }
            else if (t > 1) {
                dx = pt.x - p2.x;
                dy = pt.y - p2.y;
            }
            else {
                dx = pt.x - p1.x + t * dx;
                dy = pt.y - p1.y + t * dy;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2 Rotate(this Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}