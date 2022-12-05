using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Domain
{
    public class Aluno : IEntidade
    {
        [Required(ErrorMessage = "Informe a matricula")]
        public int Matricula { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o CPF")]
        [StringLength(14)]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Informe a data de nascimento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public DateTime  Nascimento { get; set; }

        public EnumeradorSexo Sexo { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Aluno aluno &&
                   Matricula.Equals(aluno.Matricula);
        }

        public override int GetHashCode()
        {
            return Matricula.GetHashCode();
        }
    }
}
