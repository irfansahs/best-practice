using SharedKernel.Results;

namespace Application.Abstractions.Messaging;

public delegate Task<Result<TResponse>> RequestHandlerDelegate<TResponse>();
