using E_Voucher.Entities.Response_Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Entities.DTO
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            StatusCode = 200;
        }

        public ResponseBase(int statusCode, string errorType, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public int StatusCode;
        public string ErrorType;
        public string ErrorMessage;

        public Error GetError()
        {
            return new Error(ErrorType, ErrorMessage);
        }
    }
}
