using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;
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
            var valueOfPurchase = Convert.ToDouble(pricePrice.Value);
            var initialPayment = Convert.ToDouble(priceInitial.Value);
            priceCreditSum.Text = priceInitialType.SelectedIndex == 0 ? 
                (valueOfPurchase - initialPayment).ToString("N2") 
                : (valueOfPurchase - valueOfPurchase * initialPayment / 100).ToString("N2");
        }
        

        private void CalculatePaymentScheduleAnnuitet(double sumCredit, double interestRateYear, double interestRateMonth, int creditPeriod) // Метод расчета Аннуитетного платежа
        {
            var payment = sumCredit * (interestRateMonth / (1 - Math.Pow(1 + interestRateMonth, -creditPeriod))); // Ежемесячный платеж
            var totalCreditSum = payment * creditPeriod; // Итоговая сумма кредита

            itogPayment.Text = payment.ToString("N2"); // Выводим в результаты ежемесячный платёж
            itogSum.Text = (totalCreditSum).ToString("N2"); // Выводим в результаты итоговую сумму кредита

            // Заполняем график платежей
            var sumCreditOperation = sumCredit;
            var endValueCreditSumOperations = totalCreditSum;
            double ItogPlus = 0;
            for (int i = 0; i < creditPeriod; ++i)
            {
                double procent = sumCreditOperation * (interestRateYear / 100) / 12;
                sumCreditOperation -= payment - procent;
                dgvGrafik.Rows.Add();
                dgvGrafik[0, i].Value = i + 1; //номер месяца
                dgvGrafik[1, i].Value = payment.ToString("N2"); //Ежемесячный платеж
                dgvGrafik[2, i].Value = (payment - procent).ToString("N2"); //Платеж за основной долг
                dgvGrafik[3, i].Value = procent.ToString("N2"); //Платеж процента
                dgvGrafik[4, i].Value = sumCreditOperation.ToString("N2"); //Основной остаток
                endValueCreditSumOperations -= payment;
                ItogPlus = Convert.ToDouble(dgvGrafik[4, i].Value);
            }
            itogOverpayment.Text = (totalCreditSum - sumCredit + ItogPlus).ToString("N2");
        }

        private void CalculatePaymentScheduleDiffer(double sumCredit, double interestRateMonth, int creditPeriod) // Метод расчета Дифференцированного платежа
        {
            var mainPayment = sumCredit / creditPeriod; // платеж по основному долгу
                                                           // Заполняем график платежей
            var creditSum = 0f;
            var overPaymentSum = 0f;
            for (int i = 0; i < creditPeriod; ++i)
            {
                double procentPart = sumCredit * interestRateMonth; //подсчет процентной части ежемесячного платежа
                sumCredit -= mainPayment; //подсчет остатка основного долга (с каждым месяцем уменьшается)
                dgvGrafik.Rows.Add(); //добавляем строку в таблицу
                dgvGrafik[0, i].Value = i + 1; //номер месяца
                dgvGrafik[1, i].Value = (mainPayment + procentPart).ToString("N2"); //полный ежемесячный платеж
                dgvGrafik[2, i].Value = mainPayment.ToString("N2"); //платеж по основному долгу
                dgvGrafik[3, i].Value = procentPart.ToString("N2"); //процентная часть ежемесячного платежа
                dgvGrafik[4, i].Value = sumCredit.ToString("N2"); //Остаток по основному долгу
            }
            for (var i = 0; i < creditPeriod; ++i) //Подсчет итоговой стоимости и переплаты по кредиту
            {
                if (!float.TryParse(dgvGrafik[1, i].Value.ToString(), out var credit) || 
                    float.TryParse(dgvGrafik[3, i].Value.ToString(), out var overpayment))
                {
                    MessageBox.Show(Resources.ErrorText);
                }
                else
                {
                    creditSum += credit;
                    overPaymentSum += overpayment;
                }
               
                
            }
            var endValuePlus = Convert.ToDouble(dgvGrafik[4, dgvGrafik.RowCount - 1].Value);
            itogSum.Text = creditSum.ToString("N2");
            itogOverpayment.Text = (overPaymentSum + endValuePlus).ToString("N2");
            itogPayment.Text = dgvGrafik[1, 0].Value + "..." + dgvGrafik[1, dgvGrafik.RowCount - 1].Value;
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
            if (!TryValidateFields()) return;

            dgvGrafik.Rows.Clear(); // Очищаем таблицу
            var SumCredit = Convert.ToDouble(priceCreditSum.Text); // Сумма кредита
            var InterestRateYear = Convert.ToDouble(priceProcent.Value); // Процентная ставка, ГОДОВАЯ
            var InterestRateMonth = InterestRateYear / 100 / 12; // Процентная ставка, МЕСЯЧНАЯ
            var CreditPeriod =
                Convert.ToInt32(pricePeriod.Value); // Срок кредита, переводим в месяцы, если указан в годах
            if (pricePeriodCombo.SelectedIndex == 0)
                CreditPeriod *= 12;

            if (priceAnnuitet.Checked) // Аннуитетный платеж
                CalculatePaymentScheduleAnnuitet(SumCredit, InterestRateYear, InterestRateMonth, CreditPeriod);
            else if (priceDiffer.Checked) // Дифференцированный платеж
                CalculatePaymentScheduleDiffer(SumCredit, InterestRateMonth, CreditPeriod);
            
            butSaveAsCSV.Enabled = true;   
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
            itogSum.Clear();
            itogOverpayment.Clear();
            itogPayment.Clear();
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
                var x = Convert.ToInt32(dgvGrafik.Rows[i].Cells[0].Value);
                var y = Convert.ToDouble(dgvGrafik.Rows[i].Cells[4].Value);
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
            var writeString = $"Итоговая стоимость кредита;{itogSum.Text}\n" +
                              $"Сумма переплаты:;{itogOverpayment.Text}\n" +
                              $"Ежемесячный платеж:; {itogPayment.Text}\n" +
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
