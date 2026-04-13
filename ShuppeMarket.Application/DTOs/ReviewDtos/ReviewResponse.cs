using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.DTOs.ReviewDtos
{
    public class ReviewResponse
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreateAt { get; set; }
        public string ProductId { get; set; }
        public string AccountId { get; set; }
        public string FullName { get; set; }

    }
}
