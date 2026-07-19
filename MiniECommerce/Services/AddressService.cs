

namespace MiniECommerce.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;
        public AddressService(IAddressRepository repository)
        {
            _repository = repository;
        }
        public void AddNewAddress(Address address)
        {
                _repository.Insert(address);  
        }

        public void DeleteAddress(Address address)
        {
                _repository.Delete(address);           
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

                var addresses = _repository.GetAllAddress(UserId);
                var selectedaddress = addresses.FirstOrDefault(id => id.AddressId == AddressId);

                if (selectedaddress == null) return false;

                foreach (var address in addresses)
                {
                    if (address.IsDefault)
                    {
                        address.IsDefault = false;
                        _repository.Update(address);
                    }
                }

                // Default the address after cleaning the Defaults from current one 
                selectedaddress.IsDefault = true;
                _repository.Update(selectedaddress); 
                return true;
          
            }

            catch
            {
                return false;
            }
        }

        public void RemoveDefault(string UserId)
        {
            var addresses = _repository.GetAllAddress(UserId);

            foreach (var address in addresses)
            {
                if (address.IsDefault)
                {
                    address.IsDefault = false;
                    _repository.Update(address);
                }
            }
        }
        public void UpdateAddress(Address address)
        {
                _repository.Update(address);
        }
    }
}
