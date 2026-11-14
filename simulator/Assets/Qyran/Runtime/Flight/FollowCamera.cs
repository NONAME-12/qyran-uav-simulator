using UnityEngine;

namespace Qyran.Flight
{
    public sealed class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public bool FirstPerson { get; private set; }
        Vector3 smoothVelocity;
        bool snap = true;
        public string ModeName => FirstPerson ? "FPV" : "Chase";

        public void Toggle()
        {
            FirstPerson = !FirstPerson;
            Snap();
        }

        public void Snap()
        {
            snap = true;
            smoothVelocity = Vector3.zero;
        }

        void LateUpdate()
        {
            if (target == null) return;
            if (FirstPerson)
            {
                transform.SetPositionAndRotation(target.TransformPoint(new Vector3(0, 0.08f, 0.22f)), target.rotation);
                return;
            }
            Quaternion heading = Quaternion.Euler(0, target.eulerAngles.y, 0);
            Vector3 wanted = target.position + heading * new Vector3(0, 1.5f, -3.5f);
            wanted.y = Mathf.Max(1.1f, wanted.y);
            Vector3 lookAt = target.position + heading * new Vector3(0, 0.15f, 1.7f);
            if (Physics.Linecast(lookAt, wanted, out RaycastHit hit) && hit.distance > 0.7f)
                wanted = hit.point + hit.normal * 0.3f;
            transform.position = snap ? wanted : Vector3.SmoothDamp(transform.position, wanted, ref smoothVelocity, 0.16f);
            transform.rotation = Quaternion.LookRotation(lookAt - transform.position);
            snap = false;
        }
    }
}
