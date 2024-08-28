using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zaeid.models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime AddTime { get; set; }
        public bool IsReviewed { get; set; }
        public int ImageId { get; set; }
        public DateTime PublishDate { get; set; }
        public decimal EndPrice { get; set; }
        public decimal StartPrice { get; set; }
        public string ContractFile {  get; set; }



    }
}
