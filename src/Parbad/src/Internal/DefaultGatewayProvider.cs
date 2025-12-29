// Copyright (c) Parbad. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC License, Version 3.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Parbad.Abstraction;
using Parbad.Exceptions;
using Parbad.GatewayBuilders;

namespace Parbad.Internal
{
    /// <exception cref="GatewayNotFoundException"></exception>
    /// <inheritdoc />
    public class DefaultGatewayProvider : IGatewayProvider
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes an instance of <see cref="DefaultGatewayProvider"/>.
        /// </summary>
        /// <param name="services"></param>
        public DefaultGatewayProvider(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public virtual IGateway Provide(string gatewayName)
        {
            var descriptors = _services.GetServices<GatewayDescriptor>();

            var comparedDescriptors = descriptors
                .Where(descriptor => GatewayHelper.CompareName(descriptor.GatewayType, gatewayName))
                .ToList();

            if (comparedDescriptors.Count == 0) throw new GatewayNotFoundException(gatewayName);
            if (comparedDescriptors.Count > 1) throw new InvalidOperationException($"More than one gateway with the name {gatewayName} found.");

            var gateway = _services.GetService(comparedDescriptors[0].GatewayType);

            if (gateway == null) throw new GatewayNotFoundException(gatewayName);

            return (IGateway)gateway;
        }

        /// <inheritdoc />
        public virtual IGateway ProvideByAccountName(string gatewayAccountName)
        {
            var descriptors = _services.GetServices<GatewayDescriptor>();

            foreach (var descriptor in descriptors)
            {
                var gateway = _services.GetService(descriptor.GatewayType);

                if (gateway == null) continue;

                if (HasAccountWithName(gateway, gatewayAccountName))
                {
                    return (IGateway)gateway;
                }
            }

            throw new InvalidOperationException($"No gateway found with account name \"{gatewayAccountName}\".");
        }

        private bool HasAccountWithName(object gateway, string accountName)
        {
            var gatewayType = gateway.GetType();
            var baseType = gatewayType.BaseType;

            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(GatewayBase<>))
                {
                    var accountType = baseType.GetGenericArguments()[0];
                    var accountProviderProperty = baseType.GetProperty("AccountProvider", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (accountProviderProperty != null)
                    {
                        var accountProvider = accountProviderProperty.GetValue(gateway);
                        var loadAccountsMethod = accountProviderProperty.PropertyType.GetMethod("LoadAccountsAsync");

                        if (loadAccountsMethod != null)
                        {
                            var task = (Task)loadAccountsMethod.Invoke(accountProvider, null);
                            task.Wait();

                            var resultProperty = task.GetType().GetProperty("Result");
                            var accountsCollection = resultProperty?.GetValue(task);

                            if (accountsCollection != null)
                            {
                                var getMethod = accountsCollection.GetType().GetMethod("Get", new[] { typeof(string) });
                                var account = getMethod?.Invoke(accountsCollection, new object[] { accountName });

                                return account != null;
                            }
                        }
                    }

                    break;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
