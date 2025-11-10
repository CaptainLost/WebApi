namespace Core.Application.Abstractions.Messaging.Commands;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResonse> : IBaseCommand
{

}