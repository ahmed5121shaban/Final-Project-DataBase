using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Models.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using ModelView;
namespace FinalApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class FavAuctionsController : ControllerBase
    {
        private FavAuctionManager favAuctionManager;
        private AuctionManager auctionManager;
        private BuyerManager buyerManager;

        public FavAuctionsController(FavAuctionManager _favAuctionManager,AuctionManager _auctionManager,BuyerManager _buyerManager) { 
        
        this.favAuctionManager= _favAuctionManager;
            this.auctionManager = _auctionManager;
            this.buyerManager = _buyerManager;
        }

        [HttpGet]
        [Route("add/{auctionId}")]

        public async Task<IActionResult> AddToFav(int auctionId)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var buyer = await buyerManager.GetOne(Id);
            var auction=await auctionManager.GetOne(auctionId);
            var favAuction=  favAuctionManager.GetAll().Where(f => f.AuctionID == auction.ID && f.BuyerID == Id).FirstOrDefault();

            if (favAuction != null)
            {
               var result=await favAuctionManager.Delete(favAuction);
                if(result)
                return new JsonResult("remove");
            }
            else
            {
                if (auction != null)
                {
                    var result = await favAuctionManager.Add(new FavAuctions
                    {
                        Buyer = buyer,
                        BuyerID = Id,
                        AuctionID = auction.ID,
                        Auction = auction
                    });
                    if (result == true)
                    {
                        return new JsonResult("added");

                    }
                }
            }
            return new JsonResult("failed to add to fav");


        }


        [HttpGet]
        [Route("getfavauctionids")]
    public async Task<IActionResult> GetFavAuctionIds()
        {
            var currentUtcTime = DateTime.UtcNow;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auctions = auctionManager.GetAll().ToList();
            var favAuctionIds = favAuctionManager.GetAll().Where(f => f.BuyerID == userId&&f.Auction.Ended==false).Select(f=>f.AuctionID).ToList();
            
            return new JsonResult(new { favAuctionIds });
        }
        [HttpGet]
        [Route("getallactivefav")]
        public async Task<IActionResult> GetAllActive()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auctions = auctionManager.GetAll().ToList();
            var favAuctions = favAuctionManager.GetAll().Where(f => f.BuyerID == userId&&f.Auction.Ended==false && f.Auction.StartDate <= DateTime.Now && f.Auction.EndDate >= DateTime.Now).Select(a => new {auction=a.Auction.SeeDetails()}).ToList();

            return new JsonResult(favAuctions);
        }


        [HttpGet]
        [Route("getallupcomingfav")]
        public async Task<IActionResult> GetAllUpcoming()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favAuctions = favAuctionManager.GetAll().Where(f => f.BuyerID == userId && f.Auction.Ended == false && f.Auction.StartDate > DateTime.Now).Select(a => new {auction = a.Auction.SeeDetails()}).ToList();

            return new JsonResult(favAuctions);
        }

        [HttpGet]
        [Route("getallendedfav")]
        public async Task<IActionResult> GetAllEnded()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auctions = auctionManager.GetAll().ToList();
            var favAuctions = favAuctionManager.GetAll().Where(f => f.BuyerID == userId && f.Auction.Ended == true).Select(a => new { auction = a.Auction.SeeDetails() }).ToList();

            return new JsonResult(favAuctions);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favAuction = favAuctionManager.GetAll().Where(f => f.BuyerID == userId && f.AuctionID == id).FirstOrDefault();
            var result = await favAuctionManager.Delete(favAuction);
            if (result)
                return new JsonResult("favauction deleted successfully");
            else
                return new JsonResult("failed to delete");
        }


        [HttpGet("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favAuctions = favAuctionManager.GetAll().Where(f => f.BuyerID == userId).ToList();
           
            foreach(var favAuction in favAuctions)
            {
                await favAuctionManager.Delete(favAuction);
            }

            
                return new JsonResult("favauctions deleted successfully");
            
        }

    }
}
