using Final.Enums;
using Final.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zaeid.models
{
    public class Payment
    {
        public int Id { get; set; }
        public Enums.PaymentMetod Method {  get; set; }
        public bool IsDone { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int AuctionID {  get; set; }
        public virtual Auction Auction { get; set; }

    }
    //add three object auction,event and admin with edit the relations and add them to configuration

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {

        }
    }
}
