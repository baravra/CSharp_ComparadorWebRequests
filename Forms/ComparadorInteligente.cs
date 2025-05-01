using ComparadorWebRequests.Forms;
using ComparadorWebRequests.Logic.Comparison;
using ComparadorWebRequests.Logic.Models;
using System.Net;

namespace ComparadorWebRequests
{
    public partial class ComparadorInteligente : Form
    {
        public ComparadorInteligente()
        {
            InitializeComponent();
        }

        private void btnComparar_Click(object sender, EventArgs e)
        {
            try
            {
                string portalText = txtPortal.Text;
                string roboText = txtRobo.Text;

                var tipoComparacao = rbdRequest.Checked
                    ? ContentComparerFactory.ComparisonType.Request
                    : ContentComparerFactory.ComparisonType.Response;

                IHttpContent portalContent = tipoComparacao == ContentComparerFactory.ComparisonType.Request
                    ? new HttpRequestContent(portalText)
                    : new HttpResponseContent(portalText);

                IHttpContent roboContent = tipoComparacao == ContentComparerFactory.ComparisonType.Request
                    ? new HttpRequestContent(roboText)
                    : new HttpResponseContent(roboText);

                dynamic comparer = ContentComparerFactory.CreateComparer(tipoComparacao);

                var resultado = comparer.Compare(portalContent, roboContent);

                var resultForm = new ResultForm(resultado);
                resultForm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Um erro ocorreu: {ex.Message}", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
