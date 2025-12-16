// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

namespace Parbad.Gateway.Saman.Internal.Models;

/// <summary>
/// Represents a single settlement (Tashim) item for Saman Gateway.
/// </summary>
internal class SamanSettlementIbanInfo
{
    /// <summary>
    /// IBAN (Sheba) number of the beneficiary account.
    /// </summary>
    public string IBAN { get; set; }

    /// <summary>
    /// Amount to be settled to this IBAN (in Rials).
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// Optional purchase identifier.
    /// </summary>
    public string? PurchaseId { get; set; }
}
