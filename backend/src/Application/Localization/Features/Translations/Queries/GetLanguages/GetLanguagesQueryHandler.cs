using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Localization.Features.Translations.Queries.GetLanguages;

public sealed class GetLanguagesQueryHandler(IAppDbContext db) : IRequestHandler<GetLanguagesQuery, IReadOnlyList<LanguageDto>>
{
    public async Task<Result<IReadOnlyList<LanguageDto>>> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languages = await db.Languages
            .AsNoTracking()
            .OrderBy(l => l.SortOrder)
            .Select(l => new LanguageDto(l.Id, l.Code, l.Name, l.NativeName, l.IsDefault, l.IsActive, l.SortOrder))
            .ToListAsync(cancellationToken);

        return languages;
    }
}
