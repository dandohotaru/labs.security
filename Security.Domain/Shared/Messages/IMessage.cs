namespace Infsys.Security.Auth.Core.Shared.Messages
{
    public interface IMessage
    {
    }

    public interface IRequest : IMessage
    {
    }

    public interface IResponse : IMessage
    {
    }

    public interface IQuery : IRequest
    {
    }

    public interface IQuery<TResult> : IQuery where TResult : IResult
    {
    }

    public interface IResult : IResponse
    {
    }
}