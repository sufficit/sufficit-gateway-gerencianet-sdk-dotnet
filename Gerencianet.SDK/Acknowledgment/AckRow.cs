using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GerencianetSDK.Acknowledgment
{
    /// <summary>
    /// Each line of a confirmations file <br /><br />
    /// (pt_BR) Cada linha do Arquivo de Confirmações .RET
    /// </summary>
    public class AckRow
    {
        public static implicit operator AckRow(string text)
        {
            return text.ToAckRow();
        }

        public static implicit operator string (AckRow row)
        {
            return row.ToAckRowString();
        }


        /// <summary>
        /// Número sequencial do registro
        /// </summary>
        [Range(1, 9999999999)]
        public int Index { get; set; }

        /// <summary>
        /// Número da conta de pagamentos da empresa
        /// </summary>
        [Range(typeof(Int64), "1", "99999999999999999999")]
        public Int64 Account { get; set; }

        /// <summary>
        /// Código das ocorrências retornos
        /// </summary>
        [Range(1, 99)]
        public int Code { get; set; }

        /// <summary>
        /// Data da ocorrência
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Número da cobrança
        /// </summary>
        [Range(typeof(Int64), "1", "99999999999999999999")]
        public Int64 Charge { get; set; }

        /// <summary>
        /// Valor da cobrança
        /// </summary>
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 99999999999.99)]
        public decimal Total { get; set; }

        /// <summary>
        /// Valor pago da cobrança
        /// </summary>
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 99999999999.99)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Tipo de cobrança (1=Boleto, 2=Lâmina{carnê})
        /// </summary>
        [Range(0, 9)]
        public int Type { get; set; }

        /// <summary>
        /// Código próprio do sistema do integrador
        /// </summary>
        [MaxLength(20)]
        public string Custom { get; set; }

        /// <summary>
        /// Data do vencimento da cobrança
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        public DateTime Expire { get; set; }

        /// <summary>
        /// Data do pagamento informada pelo banco
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        public DateTime Payed { get; set; }

        /// <summary>
        /// Valor da tarifa de intermediação do pagamento
        /// </summary>
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 99999999999.99)]
        public decimal Tariff { get; set; }
    }
}
