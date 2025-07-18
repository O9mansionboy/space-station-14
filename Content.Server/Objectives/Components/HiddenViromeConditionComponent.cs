using Content.Server.Objectives.Systems;

namespace Content.Server.Objectives.Components;

[RegisterComponent, Access(typeof(HiddenViromeObjectiveSystem))]
public sealed partial class HiddenViromeConditionComponent : Component
{
    [DataField]
        public int HiddenViromes { get; set; }
}
