// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Parbad.Gateway.Saman.Internal.Models;

/// <summary>
/// Token request model for Saman Gateway with Tashim Online (settlement/split) support.
/// </summary>
internal class SamanTashimTokenRequest
{
    [JsonProperty("action")]
    public string Action { get; set; }

    public string TerminalId { get; set; }

    public long Amount { get; set; }

    public string ResNum { get; set; }

    public string RedirectUrl { get; set; }

    public string CellNumber { get; set; }

    public string ResNum1 { get; set; }

    public string ResNum2 { get; set; }

    public string ResNum3 { get; set; }

    public string ResNum4 { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<SamanSettlementIbanInfo> SettlementIbanInfo { get; set; }
}
