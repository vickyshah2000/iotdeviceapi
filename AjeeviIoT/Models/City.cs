﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AjeeviIoT.Models;

public partial class City
{
    public int CityId { get; set; }

    public string Cityname { get; set; }

    public int? StateId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual State State { get; set; }
}