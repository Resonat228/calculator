using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CreditsCalc
{
    public class PaymentScheduleAnnuitet : PaymentSchedule
    {
        public override List<DataGridViewRow> GetGraphRows(double sumCredit, int creditPeriod, double interestRateMonth,  double interestRateYear = default)
        {
            var payment = sumCredit * (interestRateMonth / (1 - Math.Pow(1 + interestRateMonth, -creditPeriod))); // Ежемесячный платеж
            var totalPlus = 0d;
            var sumCreditOperation = sumCredit;
            
            var rows = new List<DataGridViewRow>(creditPeriod);
            for (var i = 0; i < creditPeriod; ++i)
            {
                var percent = sumCredit * interestRateMonth;
                sumCreditOperation -= payment - percent;
                
                rows[i].SetValues(i + 1, payment + percent.DoubleToString(), payment.DoubleToString(),
                    percent.DoubleToString(), sumCredit.DoubleToString());
                
                totalPlus = sumCreditOperation;
            }
            
            TotalSum = payment * creditPeriod;
            TotalOverpayment = TotalSum - sumCredit + totalPlus;
            TotalPayment = payment.DoubleToString();
            
            return rows;
        } 
    }
}