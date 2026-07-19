using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Areas.Customer.ViewModels;
using MiniECommerce.Models;
using System.Security.Claims;

namespace MiniECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IUnitOfWork _unitOfWork;
        public AddressController(
             IAddressService addressService,
             IUnitOfWork unitOfWork)
        {
            _addressService = addressService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var addresses = _addressService.GetAddresses(userId);

            return View(addresses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(addressVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var address = new Address
            {
                Street = vm.Street,
                City = vm.City,
                Country = vm.Country,
                Zip = vm.Zip,
                IsDefault = vm.IsDefault,
                UserId = userId
            };

            if (vm.IsDefault)
                _addressService.RemoveDefault(userId);

            _addressService.AddNewAddress(address);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var address = _addressService.GetAddressById(userId, id);

            if (address == null)
                return NotFound();

            var vm = new addressVM
            {
                AddressId = address.AddressId,
                Street = address.Street,
                City = address.City,
                Country = address.Country,
                Zip = address.Zip,
                IsDefault = address.IsDefault
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(addressVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var address = _addressService.GetAddressById(userId, vm.AddressId);

            if (address == null)
                return NotFound();

            address.Street = vm.Street;
            address.City = vm.City;
            address.Country = vm.Country;
            address.Zip = vm.Zip;

            if (vm.IsDefault)
                _addressService.SetAddressDefault(userId, vm.AddressId);
            else
                address.IsDefault = vm.IsDefault;
           
            _addressService.UpdateAddress(address);

            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var address = _addressService.GetAddressById(userId, id);

            if (address == null)
                return NotFound();

            _addressService.DeleteAddress(address);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = _addressService.SetAddressDefault(userId, id);

            if (!result)
                return NotFound();
            
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}