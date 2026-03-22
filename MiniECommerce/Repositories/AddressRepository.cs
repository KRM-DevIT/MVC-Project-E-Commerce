
namespace MiniECommerce.Repositories
{
    public class AddressRepository : Repository<Address> , IAddressRepository
    {
        private readonly ApplicationDbContext _context;
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Address? GetAddressById(string UserId, int AddressId)
        {
            return _context.Addresses
                 .FirstOrDefault(addr => addr.UserId == UserId &&
                 addr.AddressId == AddressId);
        }
        public List<Address> GetAllAddress(string userId)
        {
            return _context.Addresses.Where(addr => addr.UserId == userId).ToList();
        }
    }
}
