using Content.Server.Objectives.Components;
using Content.Shared.Objectives.Components;

namespace Content.Server.Objectives.Systems;

public sealed partial class HiddenViromeObjectiveSystem : EntitySystem
{
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HiddenViromeConditionComponent, ObjectiveGetProgressEvent>(OnHiddenViromeGetProgress);
    }

    private void OnHiddenViromeGetProgress(EntityUid uid, HiddenViromeConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(uid);
        if (target != 0)
            args.Progress = MathF.Min(comp.HiddenViromes / target, 1f);
        else args.Progress = 1f;
    }
}

