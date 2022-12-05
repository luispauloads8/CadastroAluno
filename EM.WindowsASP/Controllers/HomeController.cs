using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EM.Repository;
using EM.WindowsASP.Models;
using EM.Domain;
using System.Linq;
using System.Globalization;

namespace EM.WindowsASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly RepositorioAluno _repositorioAluno = new RepositorioAluno();

        // GET
        public IActionResult Index(string searchString, int opcao)
        {
            
            int _opcao = opcao;
            string _searchString = searchString;
            IEnumerable<Aluno> alunos = _repositorioAluno.GetAll().AsQueryable();

            if (_opcao == 1)
            {
                if (!string.IsNullOrEmpty(_searchString))
                {
                    int matricula = 0;
                    try
                    {
                         matricula = int.Parse(_searchString);
                    }
                    catch (Exception)
                    {

                        return RedirectToAction("Index");
                    }
                    
                    alunos = alunos.Where(x => x.Matricula.Equals(matricula));
                }
                else
                {
                    return View(_repositorioAluno.GetAll());
                }

            }
            else
            {
                if (_opcao == 2)
                {

                    if (!string.IsNullOrEmpty(_searchString))
                    {
                        alunos = alunos.Where(x => x.Nome.ToUpper().Contains(_searchString.ToUpper()));
                    }
                    else
                    {
                        return View(_repositorioAluno.GetAll());
                    }
                }
            }
            
            return View(alunos.ToList());

        }

        // GET
        public IActionResult CadastrarAluno()
        {
            ViewBag.DadosMatricula =  UltimaMatricula();

            return View();
        }

        [HttpPost]
        public IActionResult CadastrarAluno(int matricula, string cpf, [Bind("Matricula,Nome,Sexo,Nascimento,CPF")] Aluno aluno)
        {

            if (Cpf.Check(cpf) != true)
            {
                TempData["cpfinvalido"] = "CPF não e valido!";
            }
            else
            {

                if (ModelState.IsValid)
                {
                    if (PedidoExists(aluno))
                    {
                        TempData["matriculaobrigatoria"] = "Matícula ja utlizada! Utilizar a matrícula " + UltimaMatricula() + " para realizar o cadastro!";
                    }
                    else
                    {
                        _repositorioAluno.Add(aluno);
                        return RedirectToAction(nameof(Index));
                    }

                }
            }
            return View(aluno);
        }

        // GET
        public IActionResult EditarAluno(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alunoatualizado = _repositorioAluno.GetByMatricula(id.Value);
            if (alunoatualizado == null)
            {
                return NotFound();
            }

            return View(alunoatualizado);
        }

        [HttpPost]
        public IActionResult EditarAluno(int matricula, string cpf, [Bind("Matricula,Nome,Sexo,Nascimento,CPF")] Aluno aluno)
        {
            if (Cpf.Check(cpf) != true)
            {
                TempData["cpfinvalido"] = "CPF não e valido!";
            }
            else
            {

                try
                {
                    if (ModelState.IsValid)
                    {
                        _repositorioAluno.Update(aluno);
                        TempData["Sucesso"] = "Aluno Alterada com Sucesso!";
                    }

                }
                catch (Exception)
                {

                    TempData["Erro"] = "Aluno não pode ser alterada!";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aluno);
        }
        //GET
        public IActionResult DeletarAluno(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var alunoexcluir = _repositorioAluno.GetByMatricula(id.Value);
            if (alunoexcluir == null)
            {
                return NotFound();
            }
            return View(alunoexcluir);
        }

        [HttpPost]
        public IActionResult DeletarAluno(Aluno aluno)
        {
          
                _repositorioAluno.Remove(aluno);
                return RedirectToAction(nameof(Index));
                      
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private bool PedidoExists(Aluno aluno)
        {
            return _repositorioAluno.GetAll().Contains(aluno);
        }


        public static partial class Cpf
        {
            /// <summary>
            /// Performs a digit check on the given number.
            /// </summary>
            /// <param name="number">Number to check</param>
            /// <returns>True if it's a valid CPF, false otherwise</returns>
            public static bool Check(string number)
            {
                // if it's empty or the length is not 11 (number without mask) or 14 (number with mask) it's invalid
                if (string.IsNullOrWhiteSpace(number) || (number.Length != 11 && number.Length != 14))
                    return false;

                // if the length is 14 (number with mask) and the mask is incorrect it's invalid
                if (number.Length == 14 && (number[3] != '.' || number[7] != '.' || number[11] != '-'))
                    return false;

                // striping off the mask
                var digits = new string(number.Where(char.IsDigit).ToArray());

                // if the number of digits is different than 11 it's invalid
                if (digits.Length != 11)
                    return false;

                // getting the verifies (last two digits)
                var verifiers = digits.Substring(9, 2);

                // getting the "actual" number to be verified (first nine digits)
                var actualNumber = digits.Substring(0, 9);

                // doing a mod11 check on the 9 length number
                var verifier1 = Mod11(actualNumber);

                // if doesn't match the first verifies it's invalid
                if (verifier1 != verifiers[0])
                    return false;

                // now it's a 10 length number
                actualNumber += verifier1;

                // doing a mod11 check on the 10 length number
                var verifier2 = Mod11(actualNumber);

                // at this point if matches it means that is valid
                return verifier2 == verifiers[1];
            }

            private static char Mod11(string number)
            {
                var sum = 0;

                for (int i = number.Length - 1, multiplier = 2; i >= 0; --i, ++multiplier)
                    sum += int.Parse(number[i].ToString(), CultureInfo.InvariantCulture) * multiplier;

                var mod11 = sum % 11;
                return mod11 < 2 ? '0' : (11 - mod11).ToString(CultureInfo.InvariantCulture)[0];
            }
        }

        public int UltimaMatricula()
        {
            var dadosmatricula = 1;

            IEnumerable<Aluno> alunos = _repositorioAluno.GetAll().AsQueryable();
            try
            {
                dadosmatricula = alunos.OrderByDescending(x => x.Matricula).First().Matricula + 1;
                ViewBag.DadosMatricula = dadosmatricula;
            }
            catch (Exception)
            {

                ViewBag.DadosMatricula = dadosmatricula + 1;
            }
            return dadosmatricula;
        }

    }
}