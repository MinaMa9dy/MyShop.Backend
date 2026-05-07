using FluentValidation;
using MyShop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Validators.Auth
{
    public class TokenModelDtoValidator:AbstractValidator<RefreshTokenDto>
    {
        public TokenModelDtoValidator()
        {
            
        }
    }
}
