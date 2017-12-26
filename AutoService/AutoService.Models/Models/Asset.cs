﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoService.Models.Contracts;

namespace AutoService.Models.Models
{
    public class Asset : IAsset
    {
        public string Name { get; }
        public DateTime RegistrationDate { get; }
    }
}
