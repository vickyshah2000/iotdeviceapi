﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AjeeviIoT.Models;

public partial class Organisation
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public string LongName { get; set; }

    public string TradeName { get; set; }

    public int? OrgTypeId { get; set; }

    public int? OrgRoleId { get; set; }

    public string Remarks { get; set; }

    public int? Entityid { get; set; }
}