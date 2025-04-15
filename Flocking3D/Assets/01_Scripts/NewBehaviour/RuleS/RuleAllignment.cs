using System.Collections.Generic;
using UnityEngine;

public class RuleAllignment : FlockBehaviourBase
{
    [SerializeField] private float minRotationSpeed = 1f;  // Speed when angle is small
    [SerializeField] private float maxRotationSpeed = 10f; // Speed when angle is large

    public override Vector2 CalculateVelocity(Boid boid, List<Boid> otherBoids, FlockManager flockManager)
    {
        velocity = Vector2.zero;
        Vector3 perceivedVelocity = Vector3.zero;

        foreach (Boid otherBoid in otherBoids)
        {
            if (otherBoid == boid) continue;

            perceivedVelocity += otherBoid.Velocity;

        }

        Vector2 percieved = perceivedVelocity / (otherBoids.Count - 1);
        velocity = percieved * Scalar;
        AdaptiveRotationToVelocity(boid);

        boid.RotationIterationCounter++;
        return velocity;
    }

    void AdaptiveRotationToVelocity(Boid boid)
    {
        float targetAngle = Mathf.Atan2(boid.Velocity.y, boid.Velocity.x) * Mathf.Rad2Deg;

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(boid.CurrentAngle, targetAngle));
        float dynamicSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, angleDifference / 180f);

        if (angleDifference > 1f)
        {
            boid.CurrentAngle = Mathf.SmoothDampAngle(boid.CurrentAngle, targetAngle, ref boid.angleRef, 1f / dynamicSpeed);
            boid.transform.rotation = Quaternion.Euler(0, 0, boid.CurrentAngle);
        }
    }
}




