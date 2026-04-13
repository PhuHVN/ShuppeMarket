using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.DTOs.ReviewDtos
{
    public class ReviewRequest
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
