using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Common
{
    public enum OrderStatus
    {
        [Display(Name="New")]
        New = 1,
        [Display(Name= "Preparing")]
        Preparing = 2,
        [Display(Name= "Prepared")]
        Prepared = 3,
        [Display(Name= "Delivering")]
        Delivering = 4,
        [Display(Name= "Delivered")]
        Delivered = 5
    }
}
