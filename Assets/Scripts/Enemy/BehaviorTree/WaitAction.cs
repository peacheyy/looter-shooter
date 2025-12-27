using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait", story: "Wait for [Duration] seconds", category: "Action/Time", id: "d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9")]
public partial class WaitAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Duration;

    private float _timer;

    protected override Status OnStart()
    {
        _timer = Duration.Value;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}
