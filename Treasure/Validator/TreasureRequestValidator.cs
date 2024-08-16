using FluentValidation;
using Treasure.Models.Treasuare;

namespace Treasure.Validator
{
    public class TreasureRequestValidator : AbstractValidator<MatrixRequest>
    {
        public TreasureRequestValidator()
        {
            RuleFor(x => x.M).NotEmpty();
            RuleFor(x => x.M).GreaterThanOrEqualTo(0);
            RuleFor(x => x.M).LessThanOrEqualTo(500);
            RuleFor(x => x.N).NotEmpty();
            RuleFor(x => x.N).GreaterThanOrEqualTo(1);
            RuleFor(x => x.P).NotEmpty();
            RuleFor(x => x.P).GreaterThanOrEqualTo(0);
        }
    }
}
