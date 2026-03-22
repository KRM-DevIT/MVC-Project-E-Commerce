
using Azure.Core;

namespace MiniECommerce.Interfaces.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;
        public AddressService(IAddressRepository repository)
        {
            _repository = repository;
        }
        public bool AddNewAddress(Address address)
        {
            try
            {
                _repository.Insert(address);
                _repository.Save();
                return true;
            }

            catch
            {
                return false;
            }
        }

        public bool DeleteAddress(Address address)
        {
            try
            {
                _repository.Delete(address);
                _repository.Save();
                return true;
            }

            catch {
                return false;
            }
        }

        public Address? GetAddressById(string UserId, int id)
        {
            return _repository.GetAddressById(UserId, id);
        }

        public List<Address> GetAddresses(string UserId)
        {
            return _repository.GetAllAddress(UserId);
        }

        public bool SetAddressDefault(string UserId, int AddressId)
        {
            try
            {
              var address = _repository.GetAddressById(UserId, AddressId);
                if (address == null) return false;
                address.IsDefault = true;
                _repository.Update(address); 
                return true;
          
            }

            catch
            {
                return false;
            }
        }

        public bool UpdateAddress(Address address)
        {
            try
            {
                _repository.Update(address);
                _repository.Save();

                return true;
            }

            catch { return false; }
        }
    }
}
