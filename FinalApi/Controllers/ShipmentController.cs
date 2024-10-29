using FinalApi;
using Managers;
using Microsoft.AspNetCore.Mvc;
using ModelView.Account;
using static FinalApi.Enums;

namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController : ControllerBase
    {
        AuctionManager auctionManager;
        private readonly AccountManager accountManager;
        ItemManager itemManager;
        public ShipmentController(AuctionManager _auctionManager, AccountManager _accountManager , ItemManager _itemManager)
        {
            this.auctionManager = _auctionManager;
            this.accountManager = _accountManager;
            this.itemManager = _itemManager;

        }

        [HttpGet("GetShipmentDetails/{id}")]
        public async Task<IActionResult> GetShipmentDetails(int id)
            {
                if (id == 0)
                {
                    return BadRequest("Auction ID is required.");
                }

                // Fetch auction
                var auction = auctionManager.GetAll().FirstOrDefault(i => i.ID == id);
                if (auction == null)
                {
                    return NotFound("Auction not found.");
                }

                // Fetch buyer
                var buyer = accountManager.GetAll().FirstOrDefault(i => i.Id == auction.BuyerID);
                if (buyer == null)
                {
                    return NotFound("Buyer not found.");
                }

                // Fetch item
                var item = itemManager.GetAll().FirstOrDefault(i => i.ID == auction.ItemID);
                if (item == null)
                {
                    return NotFound("Item not found.");
                }

                // Fetch seller
                var seller = accountManager.GetAll().FirstOrDefault(i => i.Id == item.SellerID);
                if (seller == null)
                {
                    return NotFound("Seller not found.");
                }

                // Create shipment details view model
                var shipmentDetails = new ShipmentDetailsViewModel
                {
                    ShipmentId = auction.ID.ToString(),
                    EstimatedDelivery = auction.EndDate.AddDays(5),
                    FromAddress = new ShippingAddressViewModel
                    {
                        Street = seller.Street,
                        City = seller.City,
                        PostalCode = seller.PostalCode,
                        Country = seller.Country,
                    },
                    ToAddress = new ShippingAddressViewModel
                    {
                        Street = buyer.Street,
                        City = buyer.City,
                        PostalCode = buyer.PostalCode,
                        Country = buyer.Country,
                    },
                    Status = auction.ShippingStatus
                };

                return Ok(shipmentDetails);
            }

        [HttpPost("UpdateShipmentStatus")]
        public async Task<IActionResult> UpdateShipmentStatus(int id, AuctionShippingStatus newStatus)
        {
         
            var auction = await auctionManager.GetOne(id);
            if (auction == null)
            {
                return NotFound(new { message = "Auction not found." });
            }

            auction.ShippingStatus = newStatus;
            var isUpdated = await auctionManager.Update(auction);

            if (!isUpdated)
            {
                Console.WriteLine("Failed to update the auction.");
                return StatusCode(500, new { message = "An error occurred while updating the shipment status." });
            }

            return Ok(new { message = "Shipment status updated successfully." });
        }

    }
}