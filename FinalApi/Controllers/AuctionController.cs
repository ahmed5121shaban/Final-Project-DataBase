﻿using Managers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using ModelView;
using System.Security.Claims;
namespace FinalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController:ControllerBase
    {
        AuctionManager auctionManager;
        public AuctionController(AuctionManager _auctionManager)
        {
            this.auctionManager= _auctionManager;   
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var auctions =  auctionManager.GetAll();
            return Ok(auctions);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuction(AddAuctionModel _item)
        {

            var result =await auctionManager.Add(_item.toAuctionModel());
            if (result == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetAllActive()
        {
            var auctions = auctionManager.GetAll();
            var ActiveAuctions = auctions.Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now).ToList();
            return Ok(ActiveAuctions);
        }

        [HttpGet("Ended")]
        public async Task<IActionResult> GetAllEnded()
        {
            var auctions = auctionManager.GetAll();
            var EndedAuctions = auctions.Where(a => a.EndDate < DateTime.Now).ToList();
            return Ok(EndedAuctions);
        }
        [HttpGet("SellerLive")]
        public async Task<IActionResult> SellerLiveAuction()
        {
            var SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var LiveAuctions = auctionManager.GetAll().Where(i => i.Item.SellerID == SellerId && i.StartDate <= DateTime.Now && i.EndDate >= DateTime.Now).ToList();
            return Ok(LiveAuctions);
        }
    }
}
