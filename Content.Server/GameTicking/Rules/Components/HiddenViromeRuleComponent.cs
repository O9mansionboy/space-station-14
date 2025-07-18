using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent]
public sealed partial class HiddenViromeRuleComponent : Component
{
    public readonly List<EntityUid> HiddenViromeMinds = new();
    public int HiddenViromes => HiddenViromeMinds.Count;

    public readonly List<ProtoId<EntityPrototype>> Roles = new()
    {
        "HiddenViromeRoleComponent"
    };

    public readonly List<ProtoId<EntityPrototype>> Objectives = new()
    {
        "HiddenViromeSurviveObjective"
    };
}