using System;
using System.Windows.Forms;
using CreditsCalc.Properties;

namespace CreditsCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            priceInitialType.SelectedIndex = 0;
            pricePeriodCombo.SelectedIndex = 0;
            priceAnnuitet.Checked = true;
            sumPeriodCombo.SelectedIndex = 0;
            sumAnnuitet.Checked = true;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            pricePrice.Focus();
        }
        private void pricePrice_Enter(object sender, EventArgs e)
        {
            pricePrice.Select(0, pricePrice.Text.Length);
        }

        private void priceInitial_Enter(object sender, EventArgs e)
        {
            priceInitial.Select(0, priceInitial.Text.Length);
        }

        private void priceProcent_Enter(object sender, EventArgs e)
        {
            priceProcent.Select(0, priceProcent.Text.Length);
        }

        private void pricePeriod_Enter(object sender, EventArgs e)
        {
            pricePeriod.Select(0, pricePeriod.Text.Length);
        }

        private void sumCreditSum_Enter(object sender, EventArgs e)
        {
            sumCreditSum.Select(0, sumCreditSum.Text.Length);
        }

        private void sumProcent_Enter(object sender, EventArgs e)
        {
            sumProcent.Select(0, sumProcent.Text.Length);
        }

        private void sumPeriod_Enter(object sender, EventArgs e)
        {
            sumPeriod.Select(0, sumPeriod.Text.Length);
        }

        private void CalculateCreditSum() // Метод подсчета суммы кредита
        {
            var valueOfPurchase = pricePrice.Value.ToDouble();
            var initialPayment = priceInitial.Value.ToDouble();
            priceCreditSum.Text = priceInitialType.SelectedIndex == 0 ? 
                (valueOfPurchase - initialPayment).DoubleToString() 
                : (valueOfPurchase - valueOfPurchase * initialPayment / 100).DoubleToString();
        }

        private bool TryValidateFields()
        {
            if (Convert.ToInt32(priceInitial.Value) > Convert.ToInt32(pricePrice.Value) 
                || priceInitialType.SelectedIndex == 1 && Convert.ToInt32(priceInitial.Value) > 99)
            {
                MessageBox.Show(Resources.CreditSumError, Resources.ErrorText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void CalculateSchedule(object sender, EventArgs eventArgs)
        {
            if (!TryValidateFields())
            {
                return;
            }

            dgvGrafik.Rows.Clear(); // Очищаем таблицу
            var sumCredit = priceCreditSum.Text.ToDouble(); // Сумма кредита
            var interestRateYear = priceProcent.Value.ToDouble(); // Процентная ставка, ГОДОВАЯ
            var interestRateMonth = interestRateYear / 100 / 12; // Процентная ставка, МЕСЯЧНАЯ
            var creditPeriod = pricePeriod.Value.ToInt32(); 
            
            if (pricePeriodCombo.SelectedIndex == 0)
            {
                creditPeriod *= 12;
            }
            butSaveAsCSV.Enabled = true;
            PaymentSchedule paymentSchedule = default;

            //ToDO: связать priceAnnuited и PriceDiffer чтобы одна из них была всегда актвина
            if (priceAnnuitet.Checked)
            {
                paymentSchedule = new PaymentScheduleAnnuitet();
            }
            else if (priceDiffer.Checked)
            {
                paymentSchedule = new PaymentScheduleDiffer();
            }

            if (paymentSchedule == default)
            {
                MessageBox.Show(Resources.ErrorText);
                return;
            }
            
            dgvGrafik.Rows.Add(paymentSchedule.GetGraphRows(sumCredit, creditPeriod, interestRateMonth, interestRateYear));
            
            TotalSum.Text = paymentSchedule.TotalSum.DoubleToString();
            TotalOverpayment.Text = paymentSchedule.TotalOverpayment.DoubleToString();
            TotalPayment.Text = paymentSchedule.TotalPayment;
        }

        private void Clear() // Метод очистки расчетов
        {
            pricePrice.Value = pricePrice.Minimum;
            priceInitial.Value = priceInitial.Minimum;
            priceInitialType.SelectedIndex = 0;
            priceProcent.Value = priceProcent.Minimum;
            pricePeriod.Value = pricePeriod.Minimum;
            pricePeriodCombo.SelectedIndex = 0;
            sumCreditSum.Value = sumCreditSum.Minimum;
            sumProcent.Value = sumProcent.Minimum;
            sumPeriod.Value = sumPeriod.Minimum;
            sumPeriodCombo.SelectedIndex = 0;
            TotalSum.Clear();
            TotalOverpayment.Clear();
            TotalPayment.Clear();
            dgvGrafik.Rows.Clear();
            butSaveAsCSV.Enabled = false;
        }

        private void ClearFields(object sender, EventArgs e)
        {
            Clear();
        }

        private void FieldValueChanged(object sender, EventArgs e)
        {
            CalculateCreditSum();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < dgvGrafik.Rows.Count; i++)
            {
                var x = dgvGrafik.Rows[i].Cells[0].Value.ToDouble();
                var y = dgvGrafik.Rows[i].Cells[4].Value.ToDouble();
                chart1.Series[0].Points.AddXY(x, y);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var saveTableAsCSV = new SaveFileDialog();
            saveTableAsCSV.Filter = "Документ CSV (*.csv) |*.csv";
            saveTableAsCSV.Title = "Сохранить результат расчетов";
            if (saveTableAsCSV.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CSVManager.TryWriteCSV(saveTableAsCSV.FileName, GetSaveString());
        }

        private string GetSaveString()
        {
            var writeString = $"Итоговая стоимость кредита;{TotalSum.Text}\n" +
                              $"Сумма переплаты:;{TotalOverpayment.Text}\n" +
                              $"Ежемесячный платеж:; {TotalPayment.Text}\n" +
                              "Месяц:;Сумма платежа;Платеж по основному долгу;Платеж по процентам;Остаток основного долга;";

            for (var i = 0; i < dgvGrafik.RowCount; i++)
            {
                for (var j = 0; j < dgvGrafik.ColumnCount; j++)
                {
                    writeString += Convert.ToDouble(dgvGrafik.Rows[i].Cells[j].Value);
                    if (j < dgvGrafik.ColumnCount - 1)
                        writeString += ";";
                }

                writeString += '\n';
            }

            return writeString;
        }
    }
}
