using FluentValidation;
using MyShop.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Auth
{
    public class TokenModelDtoValidator:AbstractValidator<RefreshTokenDto>
    {
        public TokenModelDtoValidator()
        {
            
        }
    }
}
