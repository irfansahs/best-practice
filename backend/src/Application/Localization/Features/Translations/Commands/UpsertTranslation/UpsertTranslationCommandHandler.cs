using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Localization.Features.Translations.Commands.UpsertTranslation;

public sealed class UpsertTranslationCommandHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<UpsertTranslationCommand, UpsertTranslationResponse>
{
    public async Task<Result<UpsertTranslationResponse>> Handle(UpsertTranslationCommand request, CancellationToken cancellationToken)
    {
        if (!await languages.ExistsAsync(request.LanguageId, cancellationToken))
            return LocalizationErrors.LanguageNotFound;

        var existing = await db.TranslationEntries.FirstOrDefaultAsync(
            t => t.LanguageId == request.LanguageId && t.Namespace == request.Namespace.Trim() && t.Key == request.Key.Trim(),
            cancellationToken);

        if (existing is null)
        {
            var createResult = TranslationEntry.Create(Guid.NewGuid(), request.LanguageId, request.Namespace, request.Key, request.Value);
            if (createResult.IsFailure) return createResult.Error;
            db.TranslationEntries.Add(createResult.Value);
            return new UpsertTranslationResponse(createResult.Value.Id);
        }

        var updateResult = existing.UpdateValue(request.Value);
        if (updateResult.IsFailure) return updateResult.Error;
        return new UpsertTranslationResponse(existing.Id);
    }
}
