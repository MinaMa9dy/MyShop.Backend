using MyShop.CORE.Dtos.Review;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewResponseDto>> AddReview(AddReviewDto? reviewDto);
        Task<Result<List<ReviewResponseDto>>> GetReviewsByProductId(Guid? ProductId);
    }
}
