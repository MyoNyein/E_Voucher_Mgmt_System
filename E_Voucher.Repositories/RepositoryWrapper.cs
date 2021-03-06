﻿using E_Voucher.Entities.Database.EVoucherSystem;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private EVoucher_System_DBContex db_Evoucher;
        private IConfiguration configuration;

        private IRefreshTokenRepository _refresh;
        private IUserRepository _user;
        private IEVoucherRepository _evoucher;
        private IPromoCodeRepository _promoCode;
        private IEStoreRepository _estore;
        public RepositoryWrapper(EVoucher_System_DBContex _db_Evoucher, IConfiguration _configuration)
        {
            db_Evoucher = _db_Evoucher;
            configuration = _configuration;
        }

        public IRefreshTokenRepository RefreshToken
        {
            get
            {
                if (_refresh == null)
                {
                    _refresh = new RefreshTokenRepository(db_Evoucher, configuration);
                }
                return _refresh;
            }
        }
        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(db_Evoucher,configuration);
                }
                return _user;
            }
        }
        public IEVoucherRepository EVoucher
        {
            get
            {
                if (_evoucher == null)
                {
                    _evoucher = new EVoucherRepository(db_Evoucher,configuration);
                }
                return _evoucher;
            }
        }
        public IPromoCodeRepository PromoCode
        {
            get
            {
                if (_promoCode == null)
                {
                    _promoCode = new PromoCodeRepository(db_Evoucher, configuration);
                }
                return _promoCode;
            }
        }

        public IEStoreRepository Estore
        {
            get
            {
                if (_estore == null)
                {
                    _estore = new EStoreRepository(db_Evoucher, configuration);
                }
                return _estore;
            }
        }
    }
}
