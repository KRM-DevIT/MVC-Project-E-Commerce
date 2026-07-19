namespace MiniECommerce.Interfaces.Services
{
    public interface IAddressService
    {
        List<Address> GetAddresses(string UserId);

        Address? GetAddressById(string userid, int id);
        
        void UpdateAddress(Address address);

        void DeleteAddress(Address address);

        void AddNewAddress(Address address);

        bool SetAddressDefault(string UserId, int AddressId);

        void RemoveDefault(string UserId);
    }
}
