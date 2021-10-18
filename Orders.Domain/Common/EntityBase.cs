﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Common
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; protected set; }
    }
}
