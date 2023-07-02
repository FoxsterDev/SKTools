using UnityEngine;

namespace SKTools.EditorExtensions
{
    public static class TransformExt
    {
        public static void SetLocalX(this Transform transform, float x)
        {
            if (transform != null)
            {
                var position = transform.localPosition;
                if (!Mathf.Approximately(position.x, x))
                {
                    position.x = x;
                    transform.localPosition = position;
                }
            }
        }

        public static void SetLocalY(this Transform transform, float y)
        {
            if (transform != null)
            {
                var position = transform.localPosition;
                if (!Mathf.Approximately(position.y, y))
                {
                    position.y = y;
                    transform.localPosition = position;
                }
            }
        }

        public static void SetLocalZ(this Transform transform, float z)
        {
            if (transform != null)
            {
                var position = transform.localPosition;
                if (!Mathf.Approximately(position.z, z))
                {
                    position.z = z;
                    transform.localPosition = position;
                }
            }
        }

        public static void SetX(this Transform transform, float x)
        {
            if (transform != null)
            {
                var position = transform.localPosition;
                if (!Mathf.Approximately(position.x, x))
                {
                    position.x = x;
                    transform.position = position;
                }
            }
        }

        public static void SetY(this Transform transform, float y)
        {
            if (transform != null)
            {
                var position = transform.localPosition;
                if (!Mathf.Approximately(position.y, y))
                {
                    position.y = y;
                    transform.position = position;
                }
            }
        }

        public static void SetZ(this Transform transform, float z)
        {
            if (transform != null)
            {
                var position = transform.localPosition;
                if (!Mathf.Approximately(position.z, z))
                {
                    position.z = z;
                    transform.position = position;
                }
            }
        }

        public static void SetAngleX(this Transform transform, float x)
        {
            if (transform != null)
            {
                var angles = transform.localEulerAngles;
                if (!Mathf.Approximately(angles.x, x))
                {
                    angles.x = x;
                    transform.localEulerAngles = angles;
                }
            }
        }

        public static void SetAngleY(this Transform transform, float y)
        {
            if (transform != null)
            {
                var angles = transform.localEulerAngles;
                if (!Mathf.Approximately(angles.y, y))
                {
                    angles.y = y;
                    transform.localEulerAngles = angles;
                }
            }
        }

        public static void SetAngleZ(this Transform transform, float z)
        {
            if (transform != null)
            {
                var angles = transform.localEulerAngles;
                if (!Mathf.Approximately(angles.z, z))
                {
                    angles.z = z;
                    transform.localEulerAngles = angles;
                }
            }
        }
    }
}