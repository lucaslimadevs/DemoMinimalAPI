using System.ComponentModel.DataAnnotations;

namespace DemoMinimalAPI.Models
{
    public class Fornecedor
    {
        public Guid Id { get; set; }
        [Required, MinLength(4)]
        public string? Nome { get; set; }
        [Required, MinLength(11)]
        public string? Documento { get; set; }
        public bool Ativo { get; set; }
    }
}
