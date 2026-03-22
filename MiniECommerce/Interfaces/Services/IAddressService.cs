namespace MiniECommerce.Interfaces.Services
{
    public interface IAddressService
    {
        List<Address> GetAddresses(string UserId);

        Address? GetAddressById(string userid, int id);
        
        bool UpdateAddress(Address address);

        bool DeleteAddress(Address address);

        bool AddNewAddress(Address address);

        bool SetAddressDefault(string UserId, int AddressId);
    }
}
