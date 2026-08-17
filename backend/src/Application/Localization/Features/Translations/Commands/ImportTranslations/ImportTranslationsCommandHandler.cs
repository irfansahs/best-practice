using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Localization.Features.Translations.Commands.ImportTranslations;

public sealed class ImportTranslationsCommandHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<ImportTranslationsCommand, ImportTranslationsResponse>
{
    public async Task<Result<ImportTranslationsResponse>> Handle(ImportTranslationsCommand request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0) return new ImportTranslationsResponse(0);

        var imported = 0;
        foreach (var item in request.Items)
        {
            if (!await languages.ExistsAsync(item.LanguageId, cancellationToken))
                return LocalizationErrors.LanguageNotFound;

            var existing = await db.TranslationEntries.FirstOrDefaultAsync(
                t => t.LanguageId == item.LanguageId && t.Namespace == item.Namespace.Trim() && t.Key == item.Key.Trim(),
                cancellationToken);

            if (existing is null)
            {
                var createResult = TranslationEntry.Create(Guid.NewGuid(), item.LanguageId, item.Namespace, item.Key, item.Value);
                if (createResult.IsFailure) return createResult.Error;
                db.TranslationEntries.Add(createResult.Value);
            }
            else
            {
                var updateResult = existing.UpdateValue(item.Value);
                if (updateResult.IsFailure) return updateResult.Error;
            }

            imported++;
        }

        return new ImportTranslationsResponse(imported);
    }
}
