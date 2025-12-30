using UnityEngine;

namespace LooterShooter
{
    public interface ILocomotion
    {
        bool HasArrived { get; }
        bool IsMoving { get; }
        float Speed { get; set; }
        float StoppingDistance { get; set; }

        void SetDestination(Vector3 destination);
        void SetTarget(Transform target);
        void Stop();
    }
}
