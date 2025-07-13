namespace UserManagementAPI.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventData) where T : class;
    }
}
