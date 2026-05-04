using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Database;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Api.Models;

//[YamlObject]
public partial class CarrierExport
{
    //[YamlConstructor]
    public CarrierExport() { }

    public CarrierExport(
        Carrier carrier)
    {
        Name = carrier.Name;
        Slug = carrier.Slug;
        Steps = [.. carrier.Steps.Select(e => new CarrierStepExportV1(e))];
        Rules = [.. carrier.Rules.Select(e => new CarrierRuleExportV1(e))];
    }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Slug { get; set; } = null!;

    [Required]
    public List<CarrierStepExportV1> Steps { get; set; } = [];

    [Required]
    public List<CarrierRuleExportV1> Rules { get; set; } = [];

    public Carrier Apply(
        Carrier carrier)
    {
        carrier.Name = Name;
        carrier.Slug = Slug;

        carrier.Steps = [.. carrier.Steps.Take(Steps.Count)];
        for (var i = 0; i < Steps.Count; i++)
        {
            var current = carrier.Steps.Skip(i).FirstOrDefault();
            if (current is null)
            {
                current = new();
                carrier.Steps.Add(current);
            }

            var apply = Steps[i];
            apply.Apply(current);
        }

        carrier.Rules = [.. carrier.Rules.Take(Rules.Count)];
        for (var i = 0; i < Rules.Count; i++)
        {
            var current = carrier.Rules.Skip(i).FirstOrDefault();
            if (current is null)
            {
                current = new();
                carrier.Rules.Add(current);
            }

            var apply = Rules[i];
            apply.Apply(current);
        }

        return carrier;
    }
}

//[YamlObject]
public partial class CarrierStepExportV1
{
    //[YamlConstructor]
    public CarrierStepExportV1() { }

    public CarrierStepExportV1(
        CarrierRunbookStep step)
    {
        StepType = step.StepType switch
        {
            Database.Enums.StepType.Click => StepTypeDTO.Click,
            Database.Enums.StepType.Extract => StepTypeDTO.Extract,
            Database.Enums.StepType.Navigate => StepTypeDTO.Navigate,
            Database.Enums.StepType.Replace => StepTypeDTO.Replace,
            Database.Enums.StepType.Script => StepTypeDTO.Script,
            Database.Enums.StepType.Send => StepTypeDTO.Send,
            Database.Enums.StepType.Set => StepTypeDTO.Set,
            Database.Enums.StepType.WaitFor => StepTypeDTO.WaitFor,
            _ => 0
        };
        From = step.Payload.TryGetValue("from", out var from) ? from as string : null;
        IsRegex = step.Payload.TryGetValue("isRegex", out var isRegex) ? isRegex as bool? : null;
        Name = step.Payload.TryGetValue("name", out var name) ? name as string : null;
        Script = step.Payload.TryGetValue("script", out var script) ? script as string : null;
        Selector = step.Payload.TryGetValue("selector", out var selector) ? selector as string : null;
        To = step.Payload.TryGetValue("to", out var to) ? to as string : null;
        Trim = step.Payload.TryGetValue("trim", out var trim) ? trim as string : null;
        Url = step.Payload.TryGetValue("url", out var url) ? url as string : null;
        Value = step.Payload.TryGetValue("value", out var value) ? value as string : null;
        Var = step.Payload.TryGetValue("var", out var var) ? var as string : null;
    }

    [Required]
    public StepTypeDTO StepType { get; set; }

    public string? From { get; set; }

    public bool? IsRegex { get; set; }

    public string? Name { get; set; }

    public string? Script { get; set; }

    public string? Selector { get; set; }

    public string? To { get; set; }

    public string? Trim { get; set; }

    public string? Url { get; set; }

    public string? Value { get; set; }

    public string? Var { get; set; }

    public CarrierRunbookStep Apply(
        CarrierRunbookStep step)
    {
        step.StepType = StepType switch
        {
            StepTypeDTO.Click => Database.Enums.StepType.Click,
            StepTypeDTO.Extract => Database.Enums.StepType.Extract,
            StepTypeDTO.Navigate => Database.Enums.StepType.Navigate,
            StepTypeDTO.Replace => Database.Enums.StepType.Replace,
            StepTypeDTO.Script => Database.Enums.StepType.Script,
            StepTypeDTO.Send => Database.Enums.StepType.Send,
            StepTypeDTO.Set => Database.Enums.StepType.Set,
            StepTypeDTO.WaitFor => Database.Enums.StepType.WaitFor,
            _ => 0
        };
        if (From is not null) step.Payload["from"] = From;
        if (IsRegex is not null) step.Payload["isRegex"] = IsRegex;
        if (Name is not null) step.Payload["name"] = Name;
        if (Script is not null) step.Payload["script"] = Script;
        if (Selector is not null) step.Payload["selector"] = Selector;
        if (To is not null) step.Payload["to"] = To;
        if (Trim is not null) step.Payload["trim"] = Trim;
        if (Url is not null) step.Payload["url"] = Url;
        if (Value is not null) step.Payload["value"] = Value;
        if (Var is not null) step.Payload["var"] = Var;

        return step;
    }
}

//[YamlObject]
public partial class CarrierRuleExportV1
{
    //[YamlConstructor]
    public CarrierRuleExportV1() { }

    public CarrierRuleExportV1(
        CarrierStatusRule rule)
    {
        Pattern = rule.Pattern;
        StatusType = rule.StatusTypeId switch
        {
            1 => StatusTypeDTO.Waiting,
            10 => StatusTypeDTO.InTransit,
            20 => StatusTypeDTO.OutForDelivery,
            100 => StatusTypeDTO.Delivered,
            -10 => StatusTypeDTO.FailedAttempt,
            -100 => StatusTypeDTO.Returned,
            -200 => StatusTypeDTO.Expired,
            _ => StatusTypeDTO.Unknown
        };
        Priority = rule.Priority;
        IsActive = rule.IsActive;
    }

    [Required]
    public string Pattern { get; set; } = null!;

    [Required]
    public StatusTypeDTO StatusType { get; set; }

    public int Priority { get; set; } = 999999;

    public bool IsActive { get; set; } = true;

    public CarrierStatusRule Apply(
        CarrierStatusRule rule)
    {
        rule.Pattern = Pattern;
        rule.StatusTypeId = (int)StatusType;
        rule.Priority = Priority;
        rule.IsActive = IsActive;

        return rule;
    }
}
