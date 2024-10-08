using Managers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using ModelView;
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


    }
}
