﻿using AutoService.Models.Enums;

namespace AutoService.Models.Contracts
{
    public interface IPart
    {
        string Name { get; set; }

        //type string because some numbers may start with 0
        string Number { get; set; }

        decimal PurchasePrice { get; set; }

        //comma separated
        string OENumbers { get; set; }

        string Producer { get; set; }

        string Vendor { get; set; }

        //below two are not part of the Interface because they are decided when parts are put in warehouse, not when they are "interfaced" between Vendor and AutoService
        //PartMainCategory partMainCategory { get; }
        //PartSubCategory partSubCategory { get; }


    }
}