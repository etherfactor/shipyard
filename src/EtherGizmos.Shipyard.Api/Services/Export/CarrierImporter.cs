using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Abstractions;
using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Api.Services.Export;

internal class CarrierImporter : IExportDocumentImporter
{
    private readonly IModelValidatorFactory _modelValidatorFactory;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public string Kind => "carrier";

    public SecurableType SecurableType => SecurableType.Carrier;

    public CarrierImporter(
        IModelValidatorFactory modelValidatorFactory,
        IUnitOfWorkFactory uowFactory)
    {
        _modelValidatorFactory = modelValidatorFactory;
        _uowFactory = uowFactory;
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new ObjectToInferredTypesConverter());
    }

    public Task<ImporterResult> VerifyAsync(
        ExportDocument document,
        CancellationToken cancellationToken = default)
    {
        return ImportCoreAsync(document, saveChanges: false, cancellationToken);
    }

    public Task<ImporterResult> ImportAsync(
        ExportDocument document,
        CancellationToken cancellationToken = default)
    {
        return ImportCoreAsync(document, saveChanges: true, cancellationToken);
    }

    private async Task<ImporterResult> ImportCoreAsync(
        ExportDocument document,
        bool saveChanges,
        CancellationToken cancellationToken = default)
    {
        if (!Kind.Equals(document.Kind, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Only supports {Kind}");

        try
        {
            using var uow = _uowFactory.AsUnfiltered().Create();
            var carrierRepo = uow.Repository<Carrier>();

            var node = JsonSerializer.SerializeToNode(document.Data, _jsonOptions)!.ToJsonString();
            var carrierData = JsonSerializer.Deserialize<CarrierExport>(node, JsonSerializerOptions.Export)!;

            var carrier = await carrierRepo.Data
                .SingleOrDefaultAsync(e => e.Slug == carrierData.Slug, cancellationToken: cancellationToken);

            var isNew = false;
            if (carrier is null)
            {
                isNew = true;

                carrier = new();
                carrierRepo.Create(carrier);
            }

            carrierData.Apply(carrier);

            var validator = _modelValidatorFactory.GetValidator<Carrier>();
            await validator.ValidateAsync(carrier, cancellationToken);

            if (saveChanges)
                await uow.SaveChangesAsync(cancellationToken);

            return new(
                document.Kind,
                document.SchemaVersion,
                saveChanges ? carrier.Id : isNew ? null : carrier.Id,
                carrier.Slug,
                isNew ? ImporterResultStatusType.Created : ImporterResultStatusType.Updated);
        }
        catch (JsonException ex)
        {
            return new(
                document.Kind,
                document.SchemaVersion,
                null,
                null,
                ImporterResultStatusType.Error,
                ex.Message);
        }
    }
}
