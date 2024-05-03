using MediatR;

namespace Assistant_Kira.Requests;

internal sealed record HelloRequest(string Name) : IRequest<string>
{ }