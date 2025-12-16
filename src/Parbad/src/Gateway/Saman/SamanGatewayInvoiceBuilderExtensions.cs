// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

using Parbad.Abstraction;
using Parbad.Gateway.Saman.Internal;
using Parbad.InvoiceBuilder;
using System;
using System.Collections.Generic;

namespace Parbad.Gateway.Saman;

public static class SamanGatewayInvoiceBuilderExtensions
{
    /// <summary>
    /// The invoice will be sent to Saman gateway.
    /// </summary>
    /// <param name="builder"></param>
    public static IInvoiceBuilder UseSaman(this IInvoiceBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        return builder.SetGateway(SamanGateway.Name);
    }

    public static IInvoiceBuilder UseSaman(
        this IInvoiceBuilder builder,
        bool? useGetMethodForPaymentPage = null)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.SetGateway(SamanGateway.Name);

        if (useGetMethodForPaymentPage.HasValue)
        {
            builder.AddOrUpdateProperty(SamanHelper.UseGetMethodForPaymentPagePropertyKey, useGetMethodForPaymentPage.Value);
        }

        return builder;
    }

    public static IInvoiceBuilder UseSamanWithGetMethod(this IInvoiceBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        return builder.UseSaman(useGetMethodForPaymentPage: true);
    }

    public static IInvoiceBuilder UseSamanWithPostMethod(this IInvoiceBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        return builder.UseSaman(useGetMethodForPaymentPage: false);
    }

    /// <summary>
    /// Sets additional data which will be sent to Saman Gateway.
    /// </summary>
    public static IInvoiceBuilder SetSamanData(this IInvoiceBuilder builder, string cellNumber)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (!string.IsNullOrWhiteSpace(cellNumber))
        {
            builder.AddOrUpdateProperty(SamanHelper.CellNumberPropertyKey, cellNumber);
        }

        return builder;
    }

    internal static string GetSamanCellNumber(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.CellNumberPropertyKey, out var cellNumber))
        {
            return cellNumber.ToString();
        }

        return null;
    }

    internal static bool? GetSamanUseGetMethodForPaymentPage(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.UseGetMethodForPaymentPagePropertyKey, out var value) &&
            value is bool boolValue)
        {
            return boolValue;
        }

        return null;
    }

    /// <summary>
    /// Sets settlement (Tashim) information for splitting payment to multiple IBANs.
    /// Items with Amount equal to 0 will be automatically filtered out.
    /// </summary>
    /// <param name="builder">The invoice builder.</param>
    /// <param name="settlementInfos">List of settlement items containing IBAN and Amount.</param>
    public static IInvoiceBuilder SetSamanSettlementInfo(
        this IInvoiceBuilder builder,
        IEnumerable<SamanSettlementInfo> settlementInfos)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (settlementInfos == null) throw new ArgumentNullException(nameof(settlementInfos));

        builder.AddOrUpdateProperty(SamanHelper.SettlementInfoPropertyKey, new List<SamanSettlementInfo>(settlementInfos));

        return builder;
    }

    /// <summary>
    /// Adds a single settlement (Tashim) item for splitting payment. Can be called multiple times to add more items.
    /// Items with Amount equal to 0 will be automatically filtered out when sending to gateway.
    /// </summary>
    /// <param name="builder">The invoice builder.</param>
    /// <param name="iban">IBAN (Sheba) number of the beneficiary account.</param>
    /// <param name="amount">Amount to be settled to this IBAN (in Rials).</param>
    /// <param name="purchaseId">Optional purchase identifier.</param>
    public static IInvoiceBuilder AddSamanSettlement(
        this IInvoiceBuilder builder,
        string iban,
        long amount,
        string purchaseId = null)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (string.IsNullOrWhiteSpace(iban)) throw new ArgumentNullException(nameof(iban));

        builder.ChangeProperties(properties =>
        {
            List<SamanSettlementInfo> settlements;

            if (properties.TryGetValue(SamanHelper.SettlementInfoPropertyKey, out var existing) &&
                existing is List<SamanSettlementInfo> existingList)
            {
                settlements = existingList;
            }
            else
            {
                settlements = new List<SamanSettlementInfo>();
                properties[SamanHelper.SettlementInfoPropertyKey] = settlements;
            }

            settlements.Add(new SamanSettlementInfo
                            {
                                IBAN = iban,
                                Amount = amount,
                                PurchaseId = purchaseId
                            });
        });

        return builder;
    }

    /// <summary>
    /// Sets the ResNum1 reference number for Saman settlement.
    /// </summary>
    public static IInvoiceBuilder SetSamanResNum1(this IInvoiceBuilder builder, string resNum1)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (!string.IsNullOrWhiteSpace(resNum1))
        {
            builder.AddOrUpdateProperty(SamanHelper.ResNum1PropertyKey, resNum1);
        }

        return builder;
    }

    /// <summary>
    /// Sets the ResNum2 reference number for Saman settlement.
    /// </summary>
    public static IInvoiceBuilder SetSamanResNum2(this IInvoiceBuilder builder, string resNum2)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (!string.IsNullOrWhiteSpace(resNum2))
        {
            builder.AddOrUpdateProperty(SamanHelper.ResNum2PropertyKey, resNum2);
        }

        return builder;
    }

    /// <summary>
    /// Sets the ResNum3 reference number for Saman settlement.
    /// </summary>
    public static IInvoiceBuilder SetSamanResNum3(this IInvoiceBuilder builder, string resNum3)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (!string.IsNullOrWhiteSpace(resNum3))
        {
            builder.AddOrUpdateProperty(SamanHelper.ResNum3PropertyKey, resNum3);
        }

        return builder;
    }

    /// <summary>
    /// Sets the ResNum4 reference number for Saman settlement (typically used for main IBAN).
    /// </summary>
    public static IInvoiceBuilder SetSamanResNum4(this IInvoiceBuilder builder, string resNum4)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        if (!string.IsNullOrWhiteSpace(resNum4))
        {
            builder.AddOrUpdateProperty(SamanHelper.ResNum4PropertyKey, resNum4);
        }

        return builder;
    }

    internal static IEnumerable<SamanSettlementInfo> GetSamanSettlementInfo(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.SettlementInfoPropertyKey, out var settlementInfo))
        {
            return settlementInfo as IEnumerable<SamanSettlementInfo>;
        }

        return null;
    }

    internal static string GetSamanResNum1(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.ResNum1PropertyKey, out var value))
        {
            return value?.ToString();
        }

        return null;
    }

    internal static string GetSamanResNum2(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.ResNum2PropertyKey, out var value))
        {
            return value?.ToString();
        }

        return null;
    }

    internal static string GetSamanResNum3(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.ResNum3PropertyKey, out var value))
        {
            return value?.ToString();
        }

        return null;
    }

    internal static string GetSamanResNum4(this Invoice invoice)
    {
        if (invoice.Properties.TryGetValue(SamanHelper.ResNum4PropertyKey, out var value))
        {
            return value?.ToString();
        }

        return null;
    }
}
