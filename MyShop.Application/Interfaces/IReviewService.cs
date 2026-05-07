using MyShop.Application.DTOs.Review;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewDto>> AddReview(AddReviewDto? reviewDto);
        Task<Result<List<ReviewDto>>> GetReviewsByProductId(Guid? ProductId);
    }
}
