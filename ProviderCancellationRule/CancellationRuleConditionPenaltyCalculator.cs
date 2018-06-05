using System;
using ProviderCancellationRule.Entities;
using ProviderCancellationRule.Entities.Enums;

namespace ProviderCancellationRule
{
    class CancellationRuleConditionPenaltyCalculator
    {
        private readonly int _maxCancellationBeforeArrivalValue;
        private readonly ILogger _logger;

        public CancellationRuleConditionPenaltyCalculator(int maxCancellationBeforeArrivalValue, ILogger logger)
        {
            _maxCancellationBeforeArrivalValue = maxCancellationBeforeArrivalValue;
            _logger = logger;
        }

        public decimal Calculate(CancellationRuleCondition cancellationRuleCondition, Booking booking)
        {
            decimal penalty = 0;

            switch ( cancellationRuleCondition.PenaltyCalcMode )
            {
                case CancellationPenaltyCalcMode.Percent:
                    penalty = booking.AmountBeforeTax * cancellationRuleCondition.PenaltyValue / 100;
                    break;
                case CancellationPenaltyCalcMode.Fixed:
                    penalty = Math.Min( booking.AmountBeforeTax, cancellationRuleCondition.PenaltyValue );
                    break;
                case CancellationPenaltyCalcMode.FirstNightPercent:
                    penalty = ( booking.AmountBeforeTax / booking.NightsCount ) * cancellationRuleCondition.PenaltyValue / 100;
                    break;
                case CancellationPenaltyCalcMode.PrepaymentPercent:
                    penalty = booking.AmountBeforeTax * cancellationRuleCondition.PenaltyValue / 100;
                    break;
                case CancellationPenaltyCalcMode.FirstNights:
                    penalty = ( booking.AmountBeforeTax / booking.NightsCount ) * cancellationRuleCondition.PenaltyValue;
                    break;
            }

            if ( cancellationRuleCondition.CancellationBeforeArrivalMatching == CancellationBeforeArrivalMatching.NoMatter )
            {
                // Заметно увеличиваем штраф, в случае, когда срок применения правила не зависит от времени до заезда
                penalty *= 10;
            }

            _logger.Info( $"\t\tcondition: {cancellationRuleCondition}, penalty: {penalty}" );
            return penalty;
        }
    }
}
