using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Roles;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using System.Text;

namespace Content.Server.GameTicking.Rules;

public sealed partial class HiddenViromeRuleSystem : GameRuleSystem<HiddenViromeRuleComponent>
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly ObjectivesSystem _objectives = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;

    public readonly ProtoId<AntagPrototype> HiddenViromePrototypeId = "HiddenVirome";
    public readonly ProtoId<NpcFactionPrototype> NanotrasenFactionId = "NanoTrasen";
    public readonly ProtoId<NpcFactionPrototype> HiddenViromeFactionId = "HiddenVirome";

    public readonly SoundSpecifier BriefingSound = new SoundPathSpecifier("/Audio/Ambience/Antag/changeling_start.ogg");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HiddenViromeRuleComponent, AfterAntagEntitySelectedEvent>(OnSelectAntag);
        SubscribeLocalEvent<HiddenViromeRuleComponent, ObjectivesTextPrependEvent>(OnTextPrepend);
    }

    private void OnSelectAntag(EntityUid uid, HiddenViromeRuleComponent comp, ref AfterAntagEntitySelectedEvent args)
    {
        MakeHiddenVirome(args.EntityUid, comp);
    }

    public bool MakeHiddenVirome(EntityUid target, HiddenViromeRuleComponent comp)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return false;

        // briefing
        if (TryComp<MetaDataComponent>(target, out var metaData))
        {
            var briefing = Loc.GetString("hiddenvirome-role-greeting", ("name", metaData?.EntityName ?? "Unknown"));
            var briefingShort = Loc.GetString("hiddenvirome-role-greeting-short", ("name", metaData?.EntityName ?? "Unknown"));

            _antag.SendBriefing(target, briefing, Color.LightBlue, BriefingSound);
            _role.MindHasRole<HiddenViromeRoleComponent>(mindId, out var HiddenViromeRole);
            _role.MindHasRole<RoleBriefingComponent>(mindId, out var briefingComp);
            if (HiddenViromeRole is not null && briefingComp is null)
            {
                AddComp<RoleBriefingComponent>(HiddenViromeRole.Value.Owner);
                Comp<RoleBriefingComponent>(HiddenViromeRole.Value.Owner).Briefing = briefing;
            }
        }

        // Add hidden virome role :3
        EnsureComp<HiddenViromeRoleComponent>(target);

        // hivemind stuff
        _npcFaction.RemoveFaction(target, NanotrasenFactionId, false);
        _npcFaction.AddFaction(target, HiddenViromeFactionId);

        //try add objectives
        comp.HiddenViromeMinds.Add(mindId);

        foreach (var objective in comp.Objectives)
            _mind.TryAddObjective(mindId, mind, objective);

        return true;
    }

    private void OnTextPrepend(EntityUid uid, HiddenViromeRuleComponent comp, ref ObjectivesTextPrependEvent args)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Loc.GetString("hidden-virome-objective-text"));
        sb.AppendLine(Loc.GetString("hidden-virome-objective-progress", ("progress", comp.HiddenViromes)));
        args.Text = sb.ToString();

    }
}
