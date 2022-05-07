using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreditsCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pricePeriodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

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

        private void CreditSumFunc() // Метод подсчета суммы кредита
        {
            double ValueOfPurchase = Convert.ToDouble(pricePrice.Value);
            double InitialPayment = Convert.ToDouble(priceInitial.Value);
            if (priceInitialType.SelectedIndex == 0)
            {
                priceCreditSum.Text = (ValueOfPurchase - InitialPayment).ToString("N2");
            }
            else
            {
                priceCreditSum.Text = (ValueOfPurchase - ((ValueOfPurchase * InitialPayment) / 100)).ToString("N2");
            }
        }

        private void priceCreditSum_TextChanged(object sender, EventArgs e)
        {
    
        }

        private void PaymentScheduleAnnuitet(double SumCredit, double InterestRateYear, double InterestRateMonth, int CreditPeriod) // Метод расчета Аннуитетного платежа
        {
            double Payment = SumCredit * (InterestRateMonth / (1 - Math.Pow(1 + InterestRateMonth, -CreditPeriod))); // Ежемесячный платеж
            double ItogCreditSum = Payment * CreditPeriod; // Итоговая сумма кредита

            itogPayment.Text = Payment.ToString("N2"); // Выводим в результаты ежемесячный платёж
            itogSum.Text = (ItogCreditSum).ToString("N2"); // Выводим в результаты итоговую сумму кредита

            // Заполняем график платежей
            double SumCreditOperation = SumCredit;
            double ItogCreditSumOperation = ItogCreditSum;
            double ItogPlus = 0;
            for (int i = 0; i < CreditPeriod; ++i)
            {
                double procent = SumCreditOperation * (InterestRateYear / 100) / 12;
                SumCreditOperation -= Payment - procent;
                dgvGrafik.Rows.Add();
                dgvGrafik[0, i].Value = i + 1; //номер месяца
                dgvGrafik[1, i].Value = Payment.ToString("N2"); //Ежемесячный платеж
                dgvGrafik[2, i].Value = (Payment - procent).ToString("N2"); //Платеж за основной долг
                dgvGrafik[3, i].Value = procent.ToString("N2"); //Платеж процента
                dgvGrafik[4, i].Value = SumCreditOperation.ToString("N2"); //Основной остаток
                ItogCreditSumOperation -= Payment;
                ItogPlus = Convert.ToDouble(dgvGrafik[4, i].Value);
            }
            itogOverpayment.Text = (ItogCreditSum - SumCredit + ItogPlus).ToString("N2");
        }

        private void PaymentScheduleDiffer(double SumCredit, double InterestRateMonth, int CreditPeriod) // Метод расчета Дифференцированного платежа
        {
            double MainPayment = SumCredit / CreditPeriod; // платеж по основному долгу
                                                           // Заполняем график платежей
            double ItogCreditSum = 0;
            double OverPaymentSum = 0;
            for (int i = 0; i < CreditPeriod; ++i)
            {
                double procentPart = SumCredit * InterestRateMonth; //подсчет процентной части ежемесячного платежа
                SumCredit -= MainPayment; //подсчет остатка основного долга (с каждым месяцем уменьшается)
                dgvGrafik.Rows.Add(); //добавляем строку в таблицу
                dgvGrafik[0, i].Value = i + 1; //номер месяца
                dgvGrafik[1, i].Value = (MainPayment + procentPart).ToString("N2"); //полный ежемесячный платеж
                dgvGrafik[2, i].Value = MainPayment.ToString("N2"); //платеж по основному долгу
                dgvGrafik[3, i].Value = procentPart.ToString("N2"); //процентная часть ежемесячного платежа
                dgvGrafik[4, i].Value = SumCredit.ToString("N2"); //Остаток по основному долгу
            }
            for (int i = 0; i < CreditPeriod; ++i) //Подсчет итоговой стоимости и переплаты по кредиту
            {
                ItogCreditSum += Convert.ToDouble(dgvGrafik[1, i].Value);
                OverPaymentSum += Convert.ToDouble(dgvGrafik[3, i].Value);
            }
            double ItogPlus = Convert.ToDouble(dgvGrafik[4, dgvGrafik.RowCount - 1].Value);
            itogSum.Text = ItogCreditSum.ToString("N2");
            itogOverpayment.Text = (OverPaymentSum + ItogPlus).ToString("N2");
            itogPayment.Text = Convert.ToString(dgvGrafik[1, 0].Value) + "..." + Convert.ToString(dgvGrafik[1, dgvGrafik.RowCount - 1].Value);
        }

        private void butPriceGo_Click(object sender, EventArgs e)
        {
            if ((Convert.ToInt32(priceInitial.Value) > Convert.ToInt32(pricePrice.Value)) || (priceInitialType.SelectedIndex == 1 && Convert.ToInt32(priceInitial.Value) > 99))
            {
                MessageBox.Show("Сумма кредита не может быть отрицательной или равной нулю.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                dgvGrafik.Rows.Clear(); // Очищаем таблицу
                double SumCredit = Convert.ToDouble(priceCreditSum.Text); // Сумма кредита
                double InterestRateYear = Convert.ToDouble(priceProcent.Value); // Процентная ставка, ГОДОВАЯ
                double InterestRateMonth = InterestRateYear / 100 / 12; // Процентная ставка, МЕСЯЧНАЯ
                int CreditPeriod = Convert.ToInt32(pricePeriod.Value); // Срок кредита, переводим в месяцы, если указан в годах
                if (pricePeriodCombo.SelectedIndex == 0)
                    CreditPeriod *= 12;

                if (priceAnnuitet.Checked == true) // Аннуитетный платеж
                {
                    PaymentScheduleAnnuitet(SumCredit, InterestRateYear, InterestRateMonth, CreditPeriod);
                }
                else if (priceDiffer.Checked == true) // Дифференцированный платеж
                {
                    PaymentScheduleDiffer(SumCredit, InterestRateMonth, CreditPeriod);
                }
                butSaveAsCSV.Enabled = true;
            }
        }

        private void butSumGo_Click(object sender, EventArgs e)
        {
            if ((Convert.ToInt32(priceInitial.Value) > Convert.ToInt32(pricePrice.Value)) || (priceInitialType.SelectedIndex == 1 && Convert.ToInt32(priceInitial.Value) > 99))
            {
                MessageBox.Show("Сумма кредита не может быть отрицательной или равной нулю.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                dgvGrafik.Rows.Clear(); // Очищаем таблицу
                double SumCredit = Convert.ToDouble(priceCreditSum.Text); // Сумма кредита
                double InterestRateYear = Convert.ToDouble(priceProcent.Value); // Процентная ставка, ГОДОВАЯ
                double InterestRateMonth = InterestRateYear / 100 / 12; // Процентная ставка, МЕСЯЧНАЯ
                int CreditPeriod = Convert.ToInt32(pricePeriod.Value); // Срок кредита, переводим в месяцы, если указан в годах
                if (pricePeriodCombo.SelectedIndex == 0)
                    CreditPeriod *= 12;

                if (priceAnnuitet.Checked == true) // Аннуитетный платеж
                {
                    PaymentScheduleAnnuitet(SumCredit, InterestRateYear, InterestRateMonth, CreditPeriod);
                }
                else if (priceDiffer.Checked == true) // Дифференцированный платеж
                {
                    PaymentScheduleDiffer(SumCredit, InterestRateMonth, CreditPeriod);
                }
                butSaveAsCSV.Enabled = true;
            }
        }

        private void ClearFunc() // Метод очистки расчетов
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

        private void butSumClear_Click(object sender, EventArgs e)
        {
            ClearFunc();
        }

        private void butPriceClear_Click(object sender, EventArgs e)
        {
            ClearFunc();
        }


        private void pricePrice_ValueChanged(object sender, EventArgs e)
        {
            CreditSumFunc();
        }

        private void priceInitial_ValueChanged(object sender, EventArgs e)
        {
            CreditSumFunc();
        }

        private void экспортРасчетовToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvGrafik.Rows.Count; i++)
            {
                int x = Convert.ToInt32(dgvGrafik.Rows[i].Cells[0].Value);
                var y = Convert.ToDouble(dgvGrafik.Rows[i].Cells[4].Value);
                chart1.Series[0].Points.AddXY(x, y);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveTableAsCSV = new SaveFileDialog();
            saveTableAsCSV.Filter = "Документ CSV (*.csv) |*.csv";
            saveTableAsCSV.Title = "Сохранить результат расчетов";
            if (saveTableAsCSV.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream file = new FileStream(saveTableAsCSV.FileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(file, Encoding.Default);
                    sw.Write("Итоговая стоимость кредита:" + ";" + itogSum.Text);
                    sw.WriteLine();
                    sw.Write("Сумма переплаты:" + ";" + itogOverpayment.Text);
                    sw.WriteLine();
                    sw.Write("Ежемесячный платеж:" + ";" + itogPayment.Text);
                    sw.WriteLine();
                    sw.Write("Месяц:" + ";" + "Сумма платежа" + ";" + "Платеж по основному долгу" + ";" + "Платеж по процентам" + ";" + "Остаток основного долга" + ";");
                    sw.WriteLine();
                    for (int i = 0; i < dgvGrafik.RowCount; i++)
                    {
                        for (int j = 0; j < dgvGrafik.ColumnCount; j++)
                        {
                            sw.Write(Convert.ToDouble(dgvGrafik.Rows[i].Cells[j].Value));
                            if (j < dgvGrafik.ColumnCount - 1)
                                sw.Write(";");
                        }
                        sw.WriteLine();
                    }
                    sw.Close();
                }
                catch
                {
                    MessageBox.Show("Перезаписываемый файл уже используется другой программой.\nЗакройте его и повторите попытку, либо сохраните как новый файл.", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
