﻿using LightPlugin.Core.Configuration;
using LightPlugin.Core.Domain.Orders;

namespace LightPlugin.Core.Domain.Customers
{
    public class RewardPointsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value of Reward Points exchange rate
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets the minimum reward points to use
        /// </summary>
        public int MinimumRewardPointsToUse { get; set; }

        /// <summary>
        /// Gets or sets a number of points awarded for registration
        /// </summary>
        public int PointsForRegistration { get; set; }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases (amount in primary store currency)
        /// </summary>
        public decimal PointsForPurchases_Amount { get; set; }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases
        /// </summary>
        public int PointsForPurchases_Points { get; set; }
        
        /// <summary>
        /// Points are awarded when the order status is
        /// </summary>
        public OrderStatus PointsForPurchases_Awarded { get; set; }

        /// <summary>
        /// Points are canceled when the order is
        /// </summary>
        public OrderStatus PointsForPurchases_Canceled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "You will earn" message should be displayed
        /// </summary>
        public bool DisplayHowMuchWillBeEarned { get; set; }
    }
}