using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Domain.Enums
{
    public enum RoleEnum
    {
        [Display(Name = "Admin")]
        Admin,
        [Display(Name = "Customer")]
        Customer,
        [Display(Name = "Seller")]
        Seller 
    }
    public enum StatusEnum
    {
        Inactive = 0,
        Active = 1,
        Pending = 2,
        
    }
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        public const string Seller = "Seller";
    }
}
