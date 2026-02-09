using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.PagamentoMatricula
{
    public class PagamentoMatriculaCommand : Command
    {
        public PagamentoMatriculaCommand(Guid matriculaId, Guid alunoId, decimal total, string nomeCartao, string numeroCartao, string expiracaoCartao, string cvvCartao)
        {
            MatriculaId = matriculaId;
            AlunoId = alunoId;
            Total = total;
            NomeCartao = nomeCartao;
            NumeroCartao = numeroCartao;
            ExpiracaoCartao = expiracaoCartao;
            CvvCartao = cvvCartao;
        }

        public Guid MatriculaId { get; private set; }
        public Guid AlunoId { get; private set; }
        public decimal Total { get; private set; }
        public string NomeCartao { get; private set; }
        public string NumeroCartao { get; private set; }
        public string ExpiracaoCartao { get; private set; }
        public string CvvCartao { get; private set; }
        public override bool EhValido()
        {
            ValidationResult = new PagamentoMatriculaCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
    public class PagamentoMatriculaCommandValidation : AbstractValidator<PagamentoMatriculaCommand>
    {
        public PagamentoMatriculaCommandValidation()
        {
            RuleFor(c => c.AlunoId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do aluno inválido");

            RuleFor(c => c.MatriculaId)
                .NotEqual(Guid.Empty)
                .WithMessage("Id da matricula inválido");

            RuleFor(c => c.NomeCartao)
                .NotEmpty()
                .WithMessage("O nome no cartão não foi informado");

            RuleFor(c => c.NumeroCartao)
                .CreditCard()
                .WithMessage("Número de cartão de crédito inválido");

            RuleFor(c => c.ExpiracaoCartao)
                .NotEmpty()
                .WithMessage("Data de expiração não informada")
                .Must(BeValidFutureDate)
                .WithMessage("Data de expiração inválida ou cartão expirado.");

            RuleFor(c => c.CvvCartao)
                .Length(3, 4)
                .WithMessage("O CVV não foi preenchido corretamente");
        }

        private bool BeValidFutureDate(string expiracaoCartao)
        {
            if (string.IsNullOrWhiteSpace(expiracaoCartao) || expiracaoCartao.Length != 5 || !expiracaoCartao.Contains("/"))
            {
                return false;
            }

            // Tenta fazer o Parse do string MM/AA (Ex: "12/25")
            if (DateTime.TryParseExact(
                expiracaoCartao,
                "MM/yy", // O 'yy' é para ano com 2 dígitos
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime dataExpiracao))
            {
                // 1. Obtém o último dia do mês de expiração (necessário para comparação)
                var ultimoDiaDoMes = new DateTime(dataExpiracao.Year, dataExpiracao.Month, DateTime.DaysInMonth(dataExpiracao.Year, dataExpiracao.Month));

                // 2. Compara com a data atual. 
                // O cartão é válido até o final do dia do último dia do mês de expiração.
                // Usamos Now.Date para ignorar a hora, simplificando a lógica.
                return ultimoDiaDoMes >= DateTime.Now.Date;
            }

            return false; // Falha na conversão de formato
        }
    }
}
