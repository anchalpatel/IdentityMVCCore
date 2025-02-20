﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityMVCCore.Models;

public partial class Employee
{
    public int Id { get; set; }
    [NotMapped]
    public string EncryptedId { get; set; }
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public decimal? Salary { get; set; }

    public string? Address { get; set; }
}
