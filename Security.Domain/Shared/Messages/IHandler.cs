using System.Threading.Tasks;

namespace Labs.Security.Domain.Shared.Messages
{
    public interface IHandler
    {
    }

    public interface IHandler<in TQuery, TResult> : IHandler
        where TQuery : IQuery<TResult>
        where TResult : IResult
    {
        Task<TResult> Execute(TQuery request);
    }
}