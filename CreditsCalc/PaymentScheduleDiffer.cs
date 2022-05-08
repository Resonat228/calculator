using System.Collections.Generic;
using System.Windows.Forms;

namespace CreditsCalc
{
    public class PaymentScheduleDiffer : PaymentSchedule
    {
        public override List<DataGridViewRow> GetGraphRows(double sumCredit, int creditPeriod, double interestRateMonth,  double interestRateYear = default)
        {
            var payment = sumCredit / creditPeriod; 
            var rows = new List<DataGridViewRow>(creditPeriod);
            
            for (var i = 0; i < creditPeriod; ++i)
            {
                var percentPart = sumCredit * interestRateMonth;
                sumCredit -= payment;
                rows[i].SetValues(i + 1, (payment + percentPart).DoubleToString(), payment.DoubleToString(),
                    percentPart.DoubleToString(), sumCredit.DoubleToString());
            }
            
            var overPaymentSum = 0d;
            var creditSum = 0d;

            for (var i = 0; i < creditPeriod; ++i) 
            {
                creditSum += rows[1].Cells[i].Value.ToDouble();
                overPaymentSum += rows[3].Cells[i].Value.ToDouble();
            }
            
            var endValuePlus = rows[4].Cells[rows.Count].Value.ToDouble();
            TotalSum = creditSum;
            TotalOverpayment = overPaymentSum + endValuePlus;
            TotalPayment = string.Join("...", rows[1].Cells[0].Value, rows[1].Cells[rows.Count - 1].Value);
            return rows;
        }
    }
}