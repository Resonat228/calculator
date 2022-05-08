using System.Collections.Generic;
using System.Windows.Forms;

namespace CreditsCalc
{
    public abstract class PaymentSchedule
    {
         public double TotalSum {  get;  protected set; }
         public double TotalOverpayment { get; protected set; }
         public string TotalPayment { get;  protected set; }
         public abstract List<DataGridViewRow> GetGraphRows(double sumCredit, int creditPeriod, double interestRateMonth,  double interestRateYear = default);
    }
}