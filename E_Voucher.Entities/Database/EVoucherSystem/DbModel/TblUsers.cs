﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace E_Voucher.Entities.Database.EVoucherSystem.DbModel
{
    public partial class TblUsers
    {
        public int UserId { get; set; }
        public string LoginId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool? MultiSessionEnable { get; set; }
        public short Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }

        public virtual TblUserPassword TblUserPassword { get; set; }
    }
}