using Dapper;
using FreeCource.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Service.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;
        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));

        }
        public async Task<Response<NoContent>> Delete(int id)
        {
            var discountResult = await GetById(id);
            if (discountResult.Data == null)
                return Response<NoContent>.Fail("No discount found", 404);

            var status = await _dbConnection.ExecuteAsync("delete from discount where id=@Id", new { Id = id });
            if (status > 0)
                return Response<NoContent>.Success(204);
            return Response<NoContent>.Fail("An error occure", 500);
        }

        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>("Select * from discount");
            return Response<List<Models.Discount>>.Success(discounts.ToList(), 200);
        }

        public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where code=@Code and userid=@UserId", new { Code = code, UserId = userId })).SingleOrDefault();
            if (discount == null)
                return Response<Models.Discount>.Fail("Discount not found", 404);
            return Response<Models.Discount>.Success(discount, 200);
        }

        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("Select * from discount where id=@Id", new { Id = id })).SingleOrDefault();
            if (discount == null)
                return Response<Models.Discount>.Fail("Discount not found", 404);
            return Response<Models.Discount>.Success(discount, 200);
        }

        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var status = await _dbConnection.ExecuteAsync("Insert into discount(userid,rate,code) values(@UserId,@Rate,@Code)", discount);
            if (status > 0)
                return Response<NoContent>.Success(200);
            return Response<NoContent>.Fail("An error occure", 500);
        }

        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var discountResult = await GetById(discount.Id);
            if(discountResult.Data == null)
                return Response<NoContent>.Fail("No discount found", 404);

            var status = await _dbConnection.ExecuteAsync("Update discount set userid=@UserId,code=@Code,rate=@Rate where id=@Id", discount);
            if (status > 0)
                return Response<NoContent>.Success(204);
            return Response<NoContent>.Fail("An error occure", 500);
        }
    }
}
