﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace E_Voucher.Entities.Database.EVoucherSystem.DbModel
{
    public partial class TblRefreshToken
    {
        public string RefreshToken { get; set; }
        public long RefreshTokenId { get; }
        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}