using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using LooterShooter;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player", story: "[Agent] chases player within [ChaseRange]", category: "Action/Movement", id: "c6686d9d9ddb4733a8343a54772124cc")]
public partial class ChasePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> ChaseRange;

    private ILocomotion _locomotion;
    private Transform _agentTransform;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        _locomotion = Agent.Value.GetComponent<ILocomotion>();
        if (_locomotion == null) return Status.Failure;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return Status.Failure;

        _agentTransform = Agent.Value.transform;
        _locomotion.SetTarget(playerRef.Transform);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_locomotion == null || _agentTransform == null) return Status.Failure;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return Status.Failure;

        float distance = Vector3.Distance(_agentTransform.position, playerRef.Transform.position);

        if (distance > ChaseRange.Value)
        {
            return Status.Failure;
        }

        if (_locomotion.HasArrived)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        _locomotion?.Stop();
    }
}
