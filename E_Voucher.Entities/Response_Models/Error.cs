using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.Response_Models
{
    public sealed class Error
    {
        public string Code { get; }
        public string Message { get; }

        public Error(string code, string description)
        {
            Code = code;
            Message = description;
        }
    }
}
