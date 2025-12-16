// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

namespace Parbad.Gateway.Saman;

/// <summary>
/// Represents settlement (Tashim) information for splitting payment to multiple IBANs.
/// </summary>
public class SamanSettlementInfo
{
    /// <summary>
    /// IBAN (Sheba) number of the beneficiary account.
    /// </summary>
    public string IBAN { get; set; }

    /// <summary>
    /// Amount to be settled to this IBAN (in Rials).
    /// Note: Items with Amount equal to 0 will be automatically filtered out.
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// Optional purchase identifier.
    /// </summary>
    public string PurchaseId { get; set; }
}
