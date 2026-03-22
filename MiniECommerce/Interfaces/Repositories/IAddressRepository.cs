namespace MiniECommerce.Interfaces.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        List<Address> GetAllAddress(string userId);
        Address? GetAddressById(string UserId, int AddressId);
    }
}
