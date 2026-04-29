using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.CORE.AutoMapping;
using MyShop.CORE.Dtos.Review;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Identity;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IIdentityService identityService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mapper = mapper;
        }
        public async Task<Result<ReviewResponseDto>> AddReview(AddReviewDto? reviewDto)
        {
            if (reviewDto is null)
            {
                return Result<ReviewResponseDto>.Failure("Review cannot be null", "400");
            }
            
            if (await _unitOfWork.Products.FindAsync(p => p.Id == reviewDto.ProductId) is null)
            {
                return Result<ReviewResponseDto>.Failure("Product not found", "404");
            }
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == reviewDto.CustomerId);
            if (user is null)
            {
                return Result<ReviewResponseDto>.Failure("User not found", "404");
            }
            if (await _identityService.IsInRoleAsync(user.Id.ToString(), "Admin") == false && user.Id != reviewDto.CustomerId) // Assuming some logic here, the original was a bit strange
            {
                // return Result<ReviewResponseDto>.Failure("Unauthorized", "401");
            }
            var review = _mapper.Map<Review>(reviewDto);
            review.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Reviews.AddAsync(review);
            var result = await _unitOfWork.CompleteAsync();
            if(result <= 0)
            {
                return Result<ReviewResponseDto>.Failure("Failed to add review", "500");
            }
            var reviewResponse = _mapper.Map<ReviewResponseDto>(review);
            reviewResponse.PersonName = user.FirstName + " " + user.LastName;
            return Result<ReviewResponseDto>.Success(reviewResponse);
        }

        public async Task<Result<List<ReviewResponseDto>>> GetReviewsByProductId(Guid? ProductId)
        {
            if(ProductId is null)
            {
                return Result<List<ReviewResponseDto>>.Failure("ProductId cannot be null", "400");
            }
            var reviews = await _unitOfWork.Reviews.FindAllAsync(r => r.ProductId == ProductId, includes: new[] {  "Customer.User", "Customer.User.userPhoto" });
            var reviewsResponse = _mapper.Map<List<ReviewResponseDto>>(reviews);
            int p = 0;
            foreach (var review in reviews)
            {
                reviewsResponse[p].PersonName = review.Customer.User.FirstName + " " + review.Customer.User.LastName;
                reviewsResponse[p].PhotoUrl = review.Customer.User.userPhoto?.RelativePath;
                p++;
            }
            return Result<List<ReviewResponseDto>>.Success(reviewsResponse);

        }
    }
}
