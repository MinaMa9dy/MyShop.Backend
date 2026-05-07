using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.Application.Mapping;
using MyShop.Application.DTOs.Review;
using MyShop.Domain.Entities;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyShop.Application.Services
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
        public async Task<Result<ReviewDto>> AddReview(AddReviewDto? reviewDto)
        {
            if (reviewDto is null)
            {
                return Result<ReviewDto>.Failure("Review cannot be null", ErrorCode.VALIDATION_ERROR);
            }
            
            if (await _unitOfWork.Products.Query().FirstOrDefaultAsync(p => p.Id == reviewDto.ProductId) is null)
            {
                return Result<ReviewDto>.Failure("Product not found", ErrorCode.NOT_FOUND);
            }
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == reviewDto.CustomerId);
            if (user is null)
            {
                return Result<ReviewDto>.Failure("User not found", ErrorCode.NOT_FOUND);
            }
            if (await _identityService.IsInRoleAsync(user.Id.ToString(), "Admin") == false && user.Id != reviewDto.CustomerId)
            {
                // return Result<ReviewResponseDto>.Failure("Unauthorized", ErrorCode.UNAUTHENTICATED);
            }
            var review = _mapper.Map<Review>(reviewDto);
            review.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Reviews.AddAsync(review);
            var result = await _unitOfWork.CompleteAsync();
            if(result <= 0)
            {
                return Result<ReviewDto>.Failure("Failed to add review", ErrorCode.SERVER_ERROR);
            }
            var reviewResponse = _mapper.Map<ReviewDto>(review);
            reviewResponse.PersonName = user.FirstName + " " + user.LastName;
            return Result<ReviewDto>.Created(reviewResponse);
        }

        public async Task<Result<List<ReviewDto>>> GetReviewsByProductId(Guid? ProductId)
        {
            if(ProductId is null)
            {
                return Result<List<ReviewDto>>.Failure("ProductId cannot be null", ErrorCode.VALIDATION_ERROR);
            }
            var reviews = await _unitOfWork.Reviews.Query()
                .Where(r => r.ProductId == ProductId)
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.userPhoto)
                .ToListAsync();

            var reviewsResponse = _mapper.Map<List<ReviewDto>>(reviews);
            for (int i = 0; i < reviews.Count; i++)
            {
                var review = reviews[i];
                reviewsResponse[i].PersonName = review.Customer.User.FirstName + " " + review.Customer.User.LastName;
                reviewsResponse[i].PhotoUrl = review.Customer.User.userPhoto?.RelativePath;
            }
            return Result<List<ReviewDto>>.Ok(reviewsResponse);

        }
    }
}
