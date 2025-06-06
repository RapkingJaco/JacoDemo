﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EfRepository.DbEntity;

public partial class CustomerTransInfo
{
    public int TransId { get; set; }

    public int CustomerId { get; set; }

    public int BankInfoId { get; set; }

    public string TransType { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAfter { get; set; }

    public DateTime TransDate { get; set; }
    public string? ReceiverBankName { get; set; }
    public string? ReceiverAccount { get; set; }

    public string Note { get; set; }

    public virtual CustomerBankInfo BankInfo { get; set; }

    public virtual CustomerInfo Customer { get; set; }
}